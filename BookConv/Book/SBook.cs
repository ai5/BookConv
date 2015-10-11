using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Xml;

using System.Runtime.Serialization;

using ProtoBuf;
using ShogiLib;

namespace ShogiLib
{
    // オンメモリのBook
    public partial class SBook
    {
        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="filename"></param>
        public static SBook Load(string filename)
        {
            SBook book = null;

            using (Stream sr = new FileStream(filename, FileMode.Open))
            {
                 book = Serializer.Deserialize<SBook>(sr);
            }

            foreach (SBookState state in book.BookStates)
            {
                foreach (SBookMove move in state.Moves)
                {
                    // ID->ポインタ変換
                    if (move.NextStateId >= 0 && move.NextStateId < book.BookStates.Count)
                    {
                        move.NextState = book.BookStates[move.NextStateId];
                    }
                }
            }

            return book;
        }

        /// <summary>
        /// カウントのクリア
        /// </summary>
        public void ClearCount()
        {
            foreach (SBookState state in this.BookStates)
            {
                state.Count = 0;
            }
        }
    }

    /// <summary>
    /// 局面
    /// </summary>   
    public partial class SBookState
    {
        // 参照カウント
        public int Count;

        // 指し手があるかどうか
        public bool IsContainsMoves(SBookMove move)
        {
            return this.Moves.Exists(x => x.Move == move.Move);
        }

       public SBookMove GetMove(SBookMove move)
       {
           return this.Moves.Find(x => x.Move == move.Move);
       }

        public void AddMove(SBookMove move)
        {
            this.Moves.Add(move);
        }
    }

    /// <summary>
    /// 指し手
    /// </summary>
    public partial class SBookMove
    {
        private const int FromDanOfs = 0;
        private const int FromSujiOfs = 4;
        private const int ToDanOfs = 8;
        private const int ToSujiOfs = 12;
        private const int PromotionOfs = 19;
        private const int CapturePieceOfs = 20;
        private const int PieceOfs = 24;
        private const int WhiteOfs = 31;

        private const int PromotionFlag = 1 << PromotionOfs;
        private const int WhiteFlag = 1 << WhiteOfs;
        private const int DanMask = 0xf;
        private const int SujiMask = 0xf;
        private const int FromMaxk = 0xff;
        private const int CapturePieceMask = 0xf;
        private const int PieceMask = 0x1f;

        public SBookState NextState;
      
        public int From
        {
            get
            {
                int dan = (this.Move >> FromDanOfs) & DanMask;
                int suji = (this.Move >> FromSujiOfs) & SujiMask;

                return Square.Make(suji.ToFile(), dan.ToRank());
            }
        }

        public int To
        {
            get
            {
                int dan = (this.Move >> ToDanOfs) & DanMask;
                int suji = (this.Move >> ToSujiOfs) & SujiMask;
                return Square.Make(suji.ToFile(), dan.ToRank());
            }
        }

        public Piece CapturePiece
        {
            get
            {
                int index = (this.Move >> CapturePieceOfs) & CapturePieceMask;
                Piece piece = (Piece)index;

                if (piece != Piece.NoPiece && this.Move > 0)
                {
                    // 相手の持ち駒にする
                    piece |= Piece.WhiteFlag;
                }

                return piece;
            }
        }

        public MoveType MoveType
        {
            get
            {
                MoveType type = MoveType.Normal;

                if ((this.Move & FromMaxk) == 0)
                {
                    type = MoveType.Drop;
                }
                else
                {
                    if (this.Promotion)
                    {
                        type |= MoveType.Promotion;
                    }

                    if (((this.Move >> CapturePieceOfs) & CapturePieceMask) != 0)
                    {
                        type |= MoveType.Capture;
                    }
                }

                return type;
            }
        }

        public bool Promotion
        {
            get
            {
                return (this.Move & PromotionFlag) != 0;
            }
        }

        public Piece Piece
        {
            get
            {
                int index = (this.Move >> PieceOfs) & PieceMask;

                Piece piece = (Piece)index;

                return piece;
            }
        }

        public PlayerColor Turn
        {
            get
            {
                return (this.Move > 0) ? PlayerColor.Black : PlayerColor.White;
            }
        }

        /// <summary>
        /// 指し手の値に変更
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static int MoveFromMoveData(MoveData movedata)
        {
            return MoveFromMoveData(movedata.FromSquare, movedata.ToSquare, movedata.MoveType, movedata.Piece, movedata.CapturePiece, movedata.Turn);
        }

        public static int MoveFromMoveData(int fromSquare, int toSquare, MoveType moveType, Piece piece, Piece capturePiece, PlayerColor turn)
        {
            int move = 0;

            if (!moveType.HasFlag(MoveType.DropFlag))
            {
                move |= fromSquare.DanOf() << FromDanOfs;
                move |= fromSquare.SujiOf() << FromSujiOfs;
            }

            move |= toSquare.DanOf() << ToDanOfs;
            move |= toSquare.SujiOf() << ToSujiOfs;
            move |= (int)(capturePiece & Piece.PromotionMask) << CapturePieceOfs;

            if (moveType.HasFlag(MoveType.Promotion))
            {
                move |= PromotionFlag;
            }

            move |= (int)piece << PieceOfs;

            if (turn == PlayerColor.White)
            {
                move |= WhiteFlag;
            }

            return move;
        }

        /// <summary>
        /// MoveDataを取得
        /// </summary>
        /// <returns></returns>
        public MoveData GetMoveData()
        {
            MoveData moveData = new MoveData();

            moveData.ToSquare = this.To;
            moveData.FromSquare = this.From;
            moveData.Piece = this.Piece;
            moveData.MoveType = this.MoveType;
            moveData.CapturePiece = this.CapturePiece;

            return moveData;
        }
    }

    // move
    // 0-3 from dan  (1-9)  打つ手の場合は0
    // 4-7 form suji (1-9)  打つ手の場合は0
    // 8-11 to dan (1-9)
    // 12-15 to suji (1-9)
    // 19    promotion flag
    // 20-23 capture piece 取った駒 (Promotionフラグも含む
    // 24-29 piece
    // 31   0:black 1:white
}