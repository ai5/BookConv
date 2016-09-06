using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Apery;

namespace ShogiLib
{
    public static class ExportAperyBook
    {
        private static readonly Dictionary<PieceType, AperyPieceType> PieceTypeTable = new Dictionary<PieceType, AperyPieceType>()
        {
            { PieceType.Pawn, AperyPieceType.Pawn },
            { PieceType.Lance, AperyPieceType.Lance },
            { PieceType.Knight, AperyPieceType.Knight },
            { PieceType.Silver, AperyPieceType.Silver },
            { PieceType.Gold, AperyPieceType.Gold },
            { PieceType.Bishop, AperyPieceType.Bishop },
            { PieceType.Rook, AperyPieceType.Rook },
        };

        private static readonly Dictionary<Piece, AperyPiece> PieceTable = new Dictionary<Piece, AperyPiece>()
        {
            // 全部書くのは無駄っぽいが・・・
            { Piece.BFU, AperyPiece.BPawn },
            { Piece.BKYO, AperyPiece.BLance },
            { Piece.BKEI, AperyPiece.BKnight },
            { Piece.BGIN, AperyPiece.BSilver },
            { Piece.BKIN, AperyPiece.BGold },
            { Piece.BKAK, AperyPiece.BBishop },
            { Piece.BHI, AperyPiece.BRook },
            { Piece.BOU, AperyPiece.BKing },
            { Piece.BTO, AperyPiece.BProPawn },
            { Piece.BNKYO, AperyPiece.BProLance },
            { Piece.BNKEI, AperyPiece.BProKnight },
            { Piece.BNGIN, AperyPiece.BProSilver },
            { Piece.BUMA, AperyPiece.BHorse },
            { Piece.BRYU, AperyPiece.BDragon },

            { Piece.WFU, AperyPiece.WPawn },
            { Piece.WKYO, AperyPiece.WLance },
            { Piece.WKEI, AperyPiece.WKnight },
            { Piece.WGIN, AperyPiece.WSilver },
            { Piece.WKIN, AperyPiece.WGold },
            { Piece.WKAK, AperyPiece.WBishop },
            { Piece.WHI, AperyPiece.WRook },
            { Piece.WOU, AperyPiece.WKing },
            { Piece.WTO, AperyPiece.WProPawn },
            { Piece.WNKYO, AperyPiece.WProLance },
            { Piece.WNKEI, AperyPiece.WProKnight },
            { Piece.WNGIN, AperyPiece.WProSilver },
            { Piece.WUMA, AperyPiece.WHorse },
            { Piece.WRYU, AperyPiece.WDragon },
        };

        /// <summary>
        /// SBookをAperyBookに変換して保存する
        /// </summary>
        /// <param name="book"></param>
        /// <param name="filename"></param>
        public static void ExportApery(this SBook book, string filename)
        {
            // 初期局面の出力
            AperyBook aperyBook = new AperyBook();
            SPosition position = new SPosition();
            book.ClearCount();
            int cnt = 0;

            foreach (SBookState state in book.BookStates)
            {
                if (state.Position != string.Empty)
                {
                    // 局面が入っている場合
                    Sfen.PositionFromString(position, state.Position);
                }

                // 指し手出力
                if (state.Count == 0 && ((state.Id == 0) || (state.Position != string.Empty)))
                {
                    WriteMoves(state, position, aperyBook);
                }

                cnt++;
            }

            aperyBook.Save(filename);
        }

        /// <summary>
        /// 指し手の出力
        /// </summary>
        /// <param name="bookstate"></param>
        /// <param name="position"></param>
        /// <param name="aperyBook"></param>
        private static void WriteMoves(SBookState bookstate, SPosition position, AperyBook aperyBook)
        {
            if (bookstate == null)
            {
                return;
            }

            if (bookstate.Count != 0)
            {
                return; // 既に出力した
            }

            bookstate.Count++;

            foreach (SBookMove move in bookstate.Moves)
            {
                if (move.Weight != 0)
                {
                    aperyBook.Add(GetKey(position), move.ConvFromToPro(), move.Weight);
                }

                MoveData moveData = move.GetMoveData();

                position.Move(moveData);

                // 再帰呼び出し
                WriteMoves(move.NextState, position, aperyBook);

                position.UnMove(moveData, null);
            }
        }

        /// <summary>
        /// Aperyの指し手に変換する
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        private static ushort ConvFromToPro(this SBookMove move)
        {
            int fromToPro = 0;

            fromToPro = ((int)MakeSquare(move.To) << AperyBook.ToShift);

            if (move.MoveType.HasFlag(MoveType.DropFlag))
            {
                fromToPro |= (AperyBook.DropOfs + (int)move.Piece.TypeOf().ConvAperyPieceType()) << AperyBook.FromShift;
            }
            else
            {
                fromToPro |= (int)MakeSquare(move.From) << AperyBook.FromShift;
                if (move.MoveType.HasFlag(MoveType.Promotion))
                {
                    fromToPro |= AperyBook.PromoBit;
                }
            }

            return (ushort)fromToPro;
        }

        /// <summary>
        /// PieceTypeの変換
        /// </summary>
        /// <param name="pieceType"></param>
        /// <returns></returns>
        private static AperyPieceType ConvAperyPieceType(this PieceType pieceType)
        {
            AperyPieceType aperyPiece = AperyPieceType.Occupied;

            if (PieceTypeTable.ContainsKey(pieceType))
            {
                aperyPiece = PieceTypeTable[pieceType];
            }

            return aperyPiece;
        }

        /// <summary>
        /// Pieceの変換
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        private static AperyPiece ConvAperyPiece(this Piece piece)
        {
            AperyPiece aperyPiece = AperyPiece.Empty;

            if (PieceTable.ContainsKey(piece))
            {
                aperyPiece = PieceTable[piece];
            }

            return aperyPiece;
        }

        /// <summary>
        /// Aperyの
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        private static AperySquare MakeSquare(int sq)
        {
            int rank = sq.RankOf();
            int file = 8 - sq.FileOf(); // fileは左右逆

            return (AperySquare)((file * 9) + rank);
        }

        private static int RankOf(this AperySquare sq)
        {
            return (int)sq % 9;
        }

        private static int FileOf(this AperySquare sq)
        {
            return 8 - ((int)sq / 9);
        }

        /// <summary>
        /// SPositionからApery用のハッシュキーを取得
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private static ulong GetKey(SPosition pos)
        {
            ulong key = 0;

            // 盤上の駒
            for (AperySquare ap = AperySquare.I9; ap < AperySquare.SquareNum; ap++)
            {
                int sq = Square.Make(ap.FileOf(), ap.RankOf());

                Piece piece = pos.GetPiece(sq);

                if (piece != Piece.NoPiece)
                {
                    int index = (int)piece.ConvAperyPiece();

                    key ^= AperyBook.ZobPiece[index][(int)ap];
                }
            }

            // 持ち駒
            for (PieceType pt = PieceType.FU; pt < PieceType.King; pt++)
            {
                int num = pos.GetHand(pos.Turn, pt);

                key ^= AperyBook.ZobHand[(int)pt - 1][num];
            }

            if (pos.Turn == PlayerColor.White)
            {
                key ^= AperyBook.ZobTurn;
            }

            return key;
        }
    }
}
