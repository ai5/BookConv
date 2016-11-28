using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShogiLib
{
    public class ImportGikouBook
    {
        public static SBook Import(string filename)
        {
            SBook book = new SBook();

            SPosition pos = new SPosition();

            try
            {
                GikouBook gbook = new GikouBook(filename);

                ReadGikouBook(book, gbook, pos);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return book;
        }

        private static void ReadGikouBook(SBook book, GikouBook gbook ,SPosition pos)
        {
            long key = ExportGikouBook.ComputeKey(pos, gbook);

            SBookState state = book.GetBookState(pos.PositionToString(1));
            if (state != null && state.Moves.Count != 0)
            {
                // すでに登録されてる？
                return;
            }

            // 局面登録
            book.Add(pos, null, 0, 0, 0);

            List<GikouBookEntry> entrys = gbook.GetEntry(key);

            if (entrys != null)
            {
                foreach (GikouBookEntry en in entrys)
                {
                    MoveData move = ConvertMove(pos.Turn, en.Move);

                    book.Add(pos, move, (int)en.Frequency, en.Score, 1);
                }

                foreach (GikouBookEntry en in entrys)
                {
                    MoveData move = ConvertMove(pos.Turn, en.Move);
                    pos.Move(move);
                    ReadGikouBook(book, gbook, pos);
                    pos.UnMove(move, null);
                }
            }
        }

        /// <summary>
        /// 内部指し手に変換
        /// </summary>
        /// <param name="gmove"></param>
        /// <returns></returns>
        private static MoveData ConvertMove(PlayerColor color, GikouMove gmove)
        {
            MoveData move = new MoveData();

            if (color == PlayerColor.White)
            {
                move.ToSquare = Square.Make((int)(gmove.to / 9), (int)(8 - (gmove.to % 9)));
                move.FromSquare = Square.Make((int)(gmove.from / 9), (int)(8 - (gmove.from % 9)));

                move.MoveType = gmove.promotion == 1 ? MoveType.Promotion : MoveType.Normal;
                if (gmove.drop == 1)
                {
                    move.MoveType |= MoveType.DropFlag;
                }

                move.Piece = (Piece)gmove.piece | Piece.WhiteFlag;
                move.CapturePiece = ((Piece)gmove.capture).Opp();
            }
            else
            {
                move.ToSquare = Square.Make((int)(8 - (gmove.to / 9)), (int)gmove.to % 9);
                move.FromSquare = Square.Make((int)(8 - (gmove.from / 9)), (int)gmove.from % 9);

                move.MoveType = gmove.promotion == 1 ? MoveType.Promotion : MoveType.Normal;
                if (gmove.drop == 1)
                {
                    move.MoveType |= MoveType.DropFlag;
                }

                move.Piece = (Piece)gmove.piece;
                move.CapturePiece = (Piece)gmove.capture;
            }
                if (move.ToSquare < 0 || move.FromSquare < 0)
                {
                Console.WriteLine("???");
                }

            return move;
        }
    }
}
