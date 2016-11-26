using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShogiLib
{
    public static class ExportGikouBook
    {
        private static int[] squareTable =
        {
            Square.SQ11, Square.SQ12, Square.SQ13, Square.SQ14, Square.SQ15, Square.SQ16, Square.SQ17, Square.SQ18, Square.SQ19,
            Square.SQ21, Square.SQ22, Square.SQ23, Square.SQ24, Square.SQ25, Square.SQ26, Square.SQ27, Square.SQ28, Square.SQ29,
            Square.SQ31, Square.SQ32, Square.SQ33, Square.SQ34, Square.SQ35, Square.SQ36, Square.SQ37, Square.SQ38, Square.SQ39,
            Square.SQ41, Square.SQ42, Square.SQ43, Square.SQ44, Square.SQ45, Square.SQ46, Square.SQ47, Square.SQ48, Square.SQ49,
            Square.SQ51, Square.SQ52, Square.SQ53, Square.SQ54, Square.SQ55, Square.SQ56, Square.SQ57, Square.SQ58, Square.SQ59,
            Square.SQ61, Square.SQ62, Square.SQ63, Square.SQ64, Square.SQ65, Square.SQ66, Square.SQ67, Square.SQ68, Square.SQ69,
            Square.SQ71, Square.SQ72, Square.SQ73, Square.SQ74, Square.SQ75, Square.SQ76, Square.SQ77, Square.SQ78, Square.SQ79,
            Square.SQ81, Square.SQ82, Square.SQ83, Square.SQ84, Square.SQ85, Square.SQ86, Square.SQ87, Square.SQ88, Square.SQ89,
            Square.SQ91, Square.SQ92, Square.SQ93, Square.SQ94, Square.SQ95, Square.SQ96, Square.SQ97, Square.SQ98, Square.SQ99,
        };

        /// <summary>
        /// SBookをGikou形式に変換して保存する
        /// </summary>
        /// <param name="book"></param>
        /// <param name="filename"></param>
        public static void ExportGikou(this SBook book, string filename)
        {
            //
            GikouBook gikouBook = new GikouBook();

            {
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
                        WriteMoves(state, position, gikouBook);
                    }

                    cnt++;
                }
            }

            gikouBook.Save(filename);
        }

        /// <summary>
        /// 指し手の出力
        /// </summary>
        /// <param name="bookstate"></param>
        /// <param name="position"></param>
        /// <param name="aperyBook"></param>
        private static void WriteMoves(SBookState bookstate, SPosition position, GikouBook gikouBook)
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
                long key = ComputeKey(position, gikouBook);

                foreach (SBookMove move in bookstate.Moves)
                {
                    if (move.Weight != 0)
                    {
                        gikouBook.Add(key, (uint)move.Weight, (uint)move.Weight, move.Value, move.ConvertGikouMove());
                    }
                }
            }

            foreach (SBookMove move in bookstate.Moves)
            {
                // 指し手の出力
                MoveData moveData = move.GetMoveData();

                position.Move(moveData);

                // 再帰呼び出し
                WriteMoves(move.NextState, position, gikouBook);

                position.UnMove(moveData, null);
            }
        }

        /// <summary>
        /// 局面のハッシュキーを取得
        /// </summary>
        /// <param name="position"></param>
        /// <param name="gikouBook"></param>
        /// <returns></returns>
        private static long ComputeKey(SPosition position, GikouBook gikouBook)
        {
            long key = 0;

            SPosition pos = (SPosition)position.Clone();

            // 後手番であれば、将棋盤を１８０度反転して、先手番として扱う
            if (pos.Turn == PlayerColor.White)
            {
                pos.Flip();
            }

            // 盤上の駒
            int gikout_sq = 0;
            foreach (var sq in squareTable)
            {
                key += gikouBook.HashSeeds.GetPsq((int)pos.GetPiece(sq), gikout_sq);
                gikout_sq += 1;
            }

            // 持ち駒
            foreach (PlayerColor color in new PlayerColor[] {PlayerColor.Black, PlayerColor.White})
            {
                for (int pt = 1; pt < SPosition.HandMax; pt++)
                {
                    for (int n = pos.GetHand(color, (PieceType)pt); n > 0; n--)
                    {
                        key += gikouBook.HashSeeds.GetHand((int)color, pt);
                    }
                }
            }

            return key;
        }

        /// <summary>
        /// Gikouの指し手に変換する
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        private static GikouMove ConvertGikouMove(this SBookMove move)
        {
            GikouMove gikouMove = new GikouMove();

            if (move.Turn == PlayerColor.White)
            {
                gikouMove.to = (uint)((move.To.FileOf() * 9) + (8 - move.To.RankOf()));
                gikouMove.from = (uint)((move.From.FileOf() * 9) + (8 - move.From.RankOf()));
                gikouMove.promotion = move.Promotion ? 1U : 0;
                gikouMove.drop = move.MoveType.HasFlag(MoveType.DropFlag) ? 1U : 0;
                gikouMove.piece = (uint)move.Piece.Opp();
                gikouMove.capture = (uint)move.CapturePiece.Opp();
            }
            else
            {
                gikouMove.to = (uint)((8 - move.To.FileOf()) * 9 + move.To.RankOf());
                gikouMove.from = (uint)((8 - move.From.FileOf()) * 9 + move.From.RankOf());
                gikouMove.promotion = move.Promotion ? 1U : 0;
                gikouMove.drop = move.MoveType.HasFlag(MoveType.DropFlag) ? 1U : 0;
                gikouMove.piece = (uint)move.Piece;
                gikouMove.capture = (uint)move.CapturePiece;
            }

            return gikouMove;
        }
    }

    public class GikouBook
    {
        private SortedList<long, List<GikouBookEntry>> entries;
        private HashSeeds hashseeds;

        public GikouBook()
        {
            this.entries = new SortedList<long, List<GikouBookEntry>>();
            this.hashseeds = new HashSeeds();
        }

        public HashSeeds HashSeeds
        {
            get { return this.hashseeds; }
        }

        /// <summary>
        /// 追加
        /// </summary>
        public void Add(long key, uint frequency, uint win_count, int score, GikouMove move)
        {
            GikouBookEntry entry = new GikouBookEntry()
            {
                Key = key,
                Move = move,
                Frequency = frequency,
                WinCount = win_count,
                Score = score,
            };

            if (!this.entries.ContainsKey(key))
            {
                // ない場合
                this.entries.Add(key, new List<GikouBookEntry>());
            }

            this.entries[key].Add(entry);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                // hashseedを書き込む
                fs.Write(this.hashseeds.GetBytes(), 0, HashSeeds.HashSeedSize);

                // 指し手を書き込む
                foreach (var kvp in this.entries)
                {
                    kvp.Value.Sort((a, b) => ((int)a.Frequency - (int)b.Frequency));

                    foreach (var entry in kvp.Value)
                    {
                        fs.Write(entry.GetBytes(), 0, GikouBookEntry.EntrySize);
                    }
                }
            }
        }

    }

    public class HashSeeds
    {
        private long[,] psq = new Int64[GikouPiece.Number, GikouSquare.Number];
        private long[,] hands = new Int64[GikouColor.Number, GikouPieceType.Number];

        private const int PsqSize  = (8 * GikouPiece.Number * GikouSquare.Number);
        private const int HandsSize= (8 * GikouColor.Number * GikouPieceType.Number);
        public const int HashSeedSize = PsqSize + HandsSize;

        public HashSeeds()
        {
            Random rand = new Random(0);

            for (int pc = GikouPiece.Min; pc <= GikouPiece.Max; pc++)
            {
                for (int sq = 0; sq < GikouSquare.Number; sq++)
                {
                    psq[pc, sq] = randNext(rand);
                }
            }

            for (int color = 0; color < GikouColor.Number; color++)
            {
                for (int pt = GikouPieceType.Min; pt <= GikouPieceType.Max; pt++)
                {
                    hands[color, pt] = randNext(rand);
                }
            }
        }

        public long GetPsq(int piece, int sq)
        {
            return this.psq[piece, sq];
        } 

        public long GetHand(int color, int pieceType)
        {
            return this.hands[color, pieceType];
        } 

        private long randNext(Random rand)
        {
            byte[] buf = new byte[8];

            rand.NextBytes(buf);

            long longRand = BitConverter.ToInt64(buf, 0);

            return longRand;
        }

        public byte[] GetBytes()
        {
            byte[] bin = new byte[HashSeedSize];
            int ofs = 0;

            foreach (var val in this.psq)
            {
                Array.Copy(BitConverter.GetBytes(val), 0, bin, ofs, 8);
                ofs += 8;
            }

            foreach (var val in this.hands)
            {
                Array.Copy(BitConverter.GetBytes(val), 0, bin, ofs, 8);
                ofs += 8;
            }

            return bin;
        }
    }

    /*
      *    9  8  7  6  5  4  3  2  1
     * +----------------------------+
     * | 72 63 54 45 36 27 18  9  0 | 一
     * | 73 64 55 46 37 28 19 10  1 | 二
     * | 74 65 56 47 38 29 20 11  2 | 三
     * | 75 66 57 48 39 30 21 12  3 | 四
     * | 76 67 58 49 40 31 22 13  4 | 五
     * | 77 68 59 50 41 32 23 14  5 | 六
     * | 78 69 60 51 42 33 24 15  6 | 七
     * | 79 70 61 52 43 34 25 16  7 | 八
     * | 80 71 62 53 44 35 26 17  8 | 九
     * +----------------------------+
     */
    public class GikouSquare
	{
        public const int Number = 81;
	}

    public class GikouPiece
    {
        public const int Min = 1;
        public const int Max = 31;
        public const int Number = 32;
    }

    public class GikouPieceType
    {
        public const int Min = 1;
        public const int Max = 7;
        public const int Number = 16;
    }

    public class GikouColor
    {
        public const int Number = 2;
    }

    public enum GikouFile
	{
		File1, File2, File3, File4, File5, File6, File7, File8, File9
	}
	
	public enum GikouRank
	{
		Rank1, Rank2, Rank3, Rank4, Rank5, Rank6, Rank7, Rank8, Rank9
	}

    /*
 * Moveオブジェクトでは、32ビットの領域を以下のように割り当てています。
 *
 * <pre>
 * xxxxxxxx xxxxxxxx xxxxxxxx x1111111 移動先のマス
 * xxxxxxxx xxxxxxxx xxxxxxxx 1xxxxxxx 成る手のフラグ
 * xxxxxxxx xxxxxxxx x1111111 xxxxxxxx 移動元のマス
 * xxxxxxxx xxxxxxxx 1xxxxxxx xxxxxxxx 打つ手のフラグ
 * xxxxxxxx xxx11111 xxxxxxxx xxxxxxxx 動かす駒
 * xxxxxx11 111xxxxx xxxxxxxx xxxxxxxx 取る駒
 * </pre>
 */
    public struct GikouMove
    {
        [BitfieldLength(7)]
        public uint to;  // 移動先

        [BitfieldLength(1)]
        public uint promotion; // 成

        [BitfieldLength(7)]
        public uint from; // 移動元

        [BitfieldLength(1)]
        public uint drop; // 打

        [BitfieldLength(5)]
        public uint piece; // 動かす駒

        [BitfieldLength(5)]
        public uint capture; // 取る駒

        public uint ToUint()
        {
            return PrimitiveConversion.ToUint(this);
        }
    }

    public class GikouBookEntry
    {
        public long Key;
        public GikouMove Move;
        public uint Frequency = 0;
        public uint WinCount = 0;
        public uint Opening = 1u << 31; // その他
        public int Score = 32601; // 不明

        public const int EntrySize = 32;

        /// <summary>
        /// バイナリ取得
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] bin = new byte[EntrySize];

            Array.Copy(BitConverter.GetBytes(this.Key), 0, bin, 0, 8);
            Array.Copy(BitConverter.GetBytes(this.Move.ToUint()), 0, bin, 8, 4);
            Array.Copy(BitConverter.GetBytes(this.Frequency), 0, bin, 12, 4);
            Array.Copy(BitConverter.GetBytes(this.WinCount), 0, bin, 16, 4);

            Array.Copy(BitConverter.GetBytes(this.Opening), 0, bin, 20, 4);
            Array.Copy(BitConverter.GetBytes(this.Score), 0, bin, 24, 4);

            return bin;
        }
    }

    /// <summary>
    /// ビットフィールドアトリビュート
    /// http://stackoverflow.com/questions/14464/bit-fields-in-c-sharp
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    sealed class BitfieldLengthAttribute : Attribute
    {
        uint length;

        public BitfieldLengthAttribute(uint length)
        {
            this.length = length;
        }

        public uint Length { get { return length; } }
    }

    static class PrimitiveConversion
    {
        public static uint ToUint<T>(T t) where T : struct
        {
            uint r = 0;
            int offset = 0;

            // For every field suitably attributed with a BitfieldLength
            foreach (System.Reflection.FieldInfo f in t.GetType().GetFields())
            {
                object[] attrs = f.GetCustomAttributes(typeof(BitfieldLengthAttribute), false);
                if (attrs.Length == 1)
                {
                    uint fieldLength = ((BitfieldLengthAttribute)attrs[0]).Length;

                    // Calculate a bitmask of the desired length
                    uint mask = 0;
                    for (int i = 0; i < fieldLength; i++)
                        mask |= 1u << i;

                    r |= ((UInt32)f.GetValue(t) & mask) << offset;

                    offset += (int)fieldLength;
                }
            }

            return r;
        }
    }
}
