using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace ShogiLib
{
    public static class ExportYaneuraOUBook
    {
        /// <summary>
        /// SBookをAperyBookに変換して保存する
        /// </summary>
        /// <param name="book"></param>
        /// <param name="filename"></param>
        public static void ExportYaneuraOUbook(this SBook book, string filename)
        {
            //
            using (StreamWriter wr = new StreamWriter(filename, false, Encoding.UTF8))
            {
                wr.WriteLine("#YANEURAOU-DB2016 1.00");

                SPosition position = new SPosition();
                book.ClearCount();
                int cnt = 0;

                foreach (SBookState state in book.BookStates)
                {
                    if (state.Count == 0 && ((state.Id == 0) || (state.Position != string.Empty)))
                    {
                        if (state.Position != string.Empty)
                        {
                            Sfen.PositionFromString(position, state.Position);
                        }

                        // 指し手の出力 ルートからの局面以外はやねうら王2016には正しく認識されない
                        WriteMoves(state, position, wr, 1);
                    }

                    cnt++;
                }
            }
        }

        /// <summary>
        /// 指し手の出力
        /// </summary>
        /// <param name="bookstate"></param>
        /// <param name="position"></param>
        /// <param name="aperyBook"></param>
        private static void WriteMoves(SBookState bookstate, SPosition position, StreamWriter wr, int depth)
        {
            if (bookstate == null)
            {
                return;
            }

            if (bookstate.Count != 0)
            {
                return; // 既に出力した
            }

            int count = 0;
            foreach (SBookMove move in bookstate.Moves)
            {
                if (move.Weight != 0)
                {
                    count++;
                }
            }

            if (count != 0)
            {
                // 局面の出力
                wr.WriteLine("sfen " + position.PositionToString(depth));

                bookstate.Count++;

                foreach (SBookMove move in bookstate.Moves)
                {
                    if (move.Weight != 0)
                    {
                        // 指し手の出力
                        MoveData moveData = move.GetMoveData();
                        string next_str = "none";

                        SBookMove next_move = GetNextMove(move.NextState);
                        if (next_move != null)
                        {
                            MoveData nextMoveData = next_move.GetMoveData();
                            next_str = Sfen.MoveToString(nextMoveData);
                        }

                        wr.WriteLine("{0} {1} 0 32 {2}", Sfen.MoveToString(moveData), next_str, move.Weight);
                    }
                }
            }

            foreach (SBookMove move in bookstate.Moves)
            {
                // 指し手の出力
                MoveData moveData = move.GetMoveData();

                position.Move(moveData);

                // 再帰呼び出し
                WriteMoves(move.NextState, position, wr, depth + 1);

                position.UnMove(moveData, null);
            }
        }


        private static SBookMove GetNextMove(SBookState bookstate)
        {
            if (bookstate == null)
            {
                return null;
            }

            int weight = 0;
            SBookMove next = null;

            foreach (SBookMove move in bookstate.Moves)
            {
                if (move.Weight > weight)
                {
                    next = move;
                    weight = move.Weight;
                }
            }

            return next;
        }
    }
}
