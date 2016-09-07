using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShogiLib
{
    public static class ImportYaneuraOuBook
    {
        public static SBook ImportYaneuraOu(string filename)
        {
            SBook book = new SBook();

            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string line;

                    SPosition pos = new SPosition();

                    book.Add(pos, null, 0, 0, 0); // 平手初期局面をいれる

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("sfen") || line.StartsWith("startpos") || line.StartsWith("position"))
                        {
                            Sfen.ReadNotation(pos, line);
                        }
                        else if (line.StartsWith("#") || line.StartsWith("//"))
                        {
                            // コメント
                        }
                        else
                        {
                            string[] str_array = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            if (str_array.Length >= 5)
                            {
                                MoveData move = Sfen.ParseMove(pos, str_array[0]);

                                if (move.Piece == Piece.NoPiece || pos.GetPiece(move.ToSquare).ColorOf() == move.Piece.ColorOf())
                                {
                                    Debug.WriteLine("bb");
                                }

                                if (move != null && move.MoveType.IsMoveWithoutPass())
                                {
                                    int weight;
                                    int depth = 0;
                                    int value = 0;

                                    int.TryParse(str_array[2], out value);
                                    int.TryParse(str_array[3], out depth);

                                    if (int.TryParse(str_array[4], out weight))
                                    {
                                        // 指し手の追加
                                        book.Add(pos, move, weight, value, depth);
                                    }
                                }
                            }
                        }
                    }
                }

                // idの付け直し
                book.SetIds();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return book;
        }
    }
}
