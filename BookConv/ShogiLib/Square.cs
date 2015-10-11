using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShogiLib
{
    /*
board[0] = ９一
board[80] = １九
FILE1                    FILE90  FILEは値が逆なのに注意
９ ８ ７ ６ ５ ４ ３ ２ １
+---------------------------+
| と 杏 圭 全 金 馬 龍 玉 ・|一 RANK_1
| ・ ・ ・ ・ ・ ・ ・ ・ ・|二 RANK_2
|v玉v歩 ・ ・ ・ 杏 ・ ・v歩|三 RANK_3
|v桂 ・v銀 ・ ・ ・ ・v金 ・|四 RANK_3
| 歩 ・ ・v歩 歩 歩 ・ ・ 歩|五 .
| ・ ・ ・ ・ ・ ・ 歩 ・ ・|六 .
| ・ 歩 ・ 歩v銀 ・ ・ 歩 ・|七 .
| ・ 銀 ・ ・ ・ ・ 桂 ・ ・|八 .
| 香 桂 ・ 金 ・ 角 ・ ・ 香|九 RANK_9
+---------------------------+
     */

    public enum File
    {
        FILE_1, FILE_2, FILE_3, FILE_4, FILE_5, FILE_6, FILE_7, FILE_8, FILE_9
    }

    public enum Rank
    {
        RANK_1, RANK_2, RANK_3, RANK_4, RANK_5, RANK_6, RANK_7, RANK_8, RANK_9
    }

    // 指し手の座標 file, rank形式 
    public struct MoveCoord
    {
        public int Rank; // ファイル
        public int File; // ランク

        public MoveCoord(int rank, int file)
        {
            this.Rank = rank;
            this.File = file;
        }
    }

    /// <summary>
    /// 将棋盤を１次元配列にしたときの定義や変換メソッド
    /// </summary>
    public static class Square
    {
        public const int SQ91 = 0, SQ81 = 1, SQ71 = 2, SQ61 = 3, SQ51 = 4, SQ41 = 5, SQ31 = 6, SQ21 = 7, SQ11 = 8;
        public const int SQ92 = 9, SQ82 = 10, SQ72 = 11, SQ62 = 12, SQ52 = 13, SQ42 = 14, SQ32 = 15, SQ22 = 16, SQ12 = 17;
        public const int SQ93 = 18, SQ83 = 19, SQ73 = 20, SQ63 = 21, SQ53 = 22, SQ43 = 23, SQ33 = 24, SQ23 = 25, SQ13 = 26;
        public const int SQ94 = 27, SQ84 = 28, SQ74 = 29, SQ64 = 30, SQ54 = 31, SQ44 = 32, SQ34 = 33, SQ24 = 34, SQ14 = 35;
        public const int SQ95 = 36, SQ85 = 37, SQ75 = 38, SQ65 = 39, SQ55 = 40, SQ45 = 41, SQ35 = 42, SQ25 = 43, SQ15 = 44;
        public const int SQ96 = 45, SQ86 = 46, SQ76 = 47, SQ66 = 48, SQ56 = 49, SQ46 = 50, SQ36 = 51, SQ26 = 52, SQ16 = 53;
        public const int SQ97 = 54, SQ87 = 55, SQ77 = 56, SQ67 = 57, SQ57 = 58, SQ47 = 59, SQ37 = 60, SQ27 = 61, SQ17 = 62;
        public const int SQ98 = 63, SQ88 = 64, SQ78 = 65, SQ68 = 66, SQ58 = 67, SQ48 = 68, SQ38 = 69, SQ28 = 70, SQ18 = 71;
        public const int SQ99 = 72;
        public const int SQ89 = 73;
        public const int SQ79 = 74;
        public const int SQ69 = 75, SQ59 = 76, SQ49 = 77, SQ39 = 78, SQ29 = 79, SQ19 = 80;

        public const int NFILE = 9;
        public const int NRANK = 9;

        public const int NSQUARE = 81;

        /// <summary>
        /// squareからfileを返す
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public static int FileOf(this int sq)
        {
            return sq % NFILE;
        }
        
        /// <summary>
        /// squareからRankを返す
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public static int RankOf(this int sq)
        {
            return sq / NFILE;
        }

        /// <summary>
        /// Squareから筋(将棋盤の筋表記を数値にしたもの1~9）を返す
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public static int SujiOf(this int sq)
        {
            return (int)Square.NFILE - (sq % NFILE);
        }

        /// <summary>
        /// squareから段を返す
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public static int DanOf(this int sq)
        {
            return (sq / NFILE) + 1;
        }

        /// <summary>
        /// file,rankからsquareを作る
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        public static int Make(Rank file, File rank)
        {
            return ((int)rank * NFILE) + (int)file;
        }

        /// <summary>
        /// file,rankからsquqreを作る
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        public static int Make(int file, int rank)
        {
            return ((int)rank * NFILE) + (int)file;
        }

        /// <summary>
        /// ボード内に値が入っているかどうか
        /// </summary>
        /// <param name="sq"></param>
        /// <returns></returns>
        public static bool InBoard(int sq)
        {
            if (sq >= 0 && sq < Square.NSQUARE)
            {
                return true;
            }

            return false;
        }

        // FILE/RANK関連

        /// <summary>
        /// fileを筋い変換
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static int ToSuji(this int file)
        {
            return (int)(Square.NFILE - file);
        }

        /// <summary>
        /// 筋をfileに変換
        /// </summary>
        /// <param name="suji"></param>
        /// <returns></returns>
        public static int ToFile(this int suji)
        {
            return (int)(Square.NFILE - suji);
        }

        /// <summary>
        /// rankを段に変換
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        public static int ToDan(this int rank)
        {
            return rank + 1;
        }

        /// <summary>
        /// 段をrankに変換
        /// </summary>
        /// <param name="dan"></param>
        /// <returns></returns>
        public static int ToRank(this int dan)
        {
            return dan - 1;
        }
    }
}
