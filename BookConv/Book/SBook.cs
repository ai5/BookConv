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
        private readonly Dictionary<string, SBookState> books = new Dictionary<string, SBookState>();

        public Dictionary<string, SBookState> Books
        {
            get { return this.books; }
        }

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

        /// <summary>
        /// stateの取得
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public SBookState GetBookState(string key)
        {
            SBookState state;

            if (!this.books.TryGetValue(key, out state))
            {
                state = null;
            }

            return state;
        }

        /// <summary>
        /// 局面追加
        /// </summary>
        public void Add(SPosition pos, MoveData moveData, int weight, int value, int depth)
        {
            string key = pos.PositionToString(1); 

            SBookState state = this.GetBookState(key);

            if (state == null)
            {
                // 現在の局面がない場合
                state = new SBookState();
                state.Games = 1;
                state.WonBlack = 0;
                state.WonWhite = 0;

                state.Position = key;

                this.books.Add(key, state);
                this.BookStates.Add(state);
            }

            if (moveData != null)
            {
                pos.Move(moveData);
                string next_key = pos.PositionToString(1); 
                SBookState next_state = this.GetBookState(next_key);
                pos.UnMove(moveData, null);

                if (next_state == null)
                {
                    next_state = new SBookState();
                    next_state.Games = 1;
                    next_state.WonBlack = 0;
                    next_state.WonWhite = 0;

                    next_state.Position = next_key;

                    this.books.Add(next_key, next_state);
                    this.BookStates.Add(next_state);
                }

                SBookMove m = new SBookMove(moveData, next_state);
                SBookMove move = state.GetMove(m);
            
                if (move == null)
                {
                    // 指し手が無い場合は追加
                    m.Weight = weight;
                    m.Value = value;
                    m.Depth = depth;
                    state.AddMove(m);

                    // next_stateのPositionをクリアする
                    next_state.Position = string.Empty;
                    move = m;
                }
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            // idの付け直し
            this.SetIds();

            // 書き込み
            using (Stream wr = new FileStream(filename, FileMode.Create))
            {
                Serializer.Serialize(wr, this);
            }
        }

        /// <summary>
        /// IDの再設定
        /// </summary>
        public void SetIds()
        {
            int id = 0;
            foreach (SBookState state in this.BookStates)
            {
                state.Id = id++;
            }

            // 指し手のIDをポインタに変換する
            foreach (SBookState state in this.BookStates)
            {
                foreach (SBookMove move in state.Moves)
                {
                    if (move.NextState != null)
                    {
                        move.NextStateId = move.NextState.Id;
                    }
                    else
                    {
                        move.NextStateId = -1;
                    }
                }
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

        public int Value = 0;

        public int Depth = 0;

        public SBookMove(MoveData movedata, SBookState state)
        {
            this.NextState = state;

            this.Move = SBookMove.MoveFromMoveData(movedata);
            this.Evalution = SBookMoveEvalution.None;
            this.Weight = 1;
        }

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