using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ShogiLib
{
    /// <summary>
    /// SFEN形式の盤面とUSI形式の指し手生成
    /// </summary>
    public static class Sfen
    {
        private static readonly Dictionary<char, PieceType> CharToPieceHashtable = new Dictionary<char, PieceType>()
        {
            { 'K', PieceType.OU }, // 玉 King
            { 'R', PieceType.HI }, // 飛車  Rook
            { 'B', PieceType.KAK },  // 角  Bishop
            { 'G', PieceType.KIN },  // 金 Gold
            { 'S', PieceType.GIN },  // 銀 Silver
            { 'N', PieceType.KEI }, // 桂馬 kNight
            { 'L', PieceType.KYO }, // 香車 Lance
            { 'P', PieceType.FU },  // 歩  Pawn
        };

        public static bool IsSfen(string str)
        {
            bool ret = false;

            if (str.StartsWith("position") || str.StartsWith("sfen") || str.StartsWith("startpos"))
            {
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// 盤面情報読み込み
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="sfen"></param>
        public static void PositionFromString(SPosition position, string sfen)
        {
            position.Init();
            ReadPosition(position, sfen);
        }

        /// <summary>
        /// 盤面出力
        /// </summary>
        /// <param name="notation"></param>
        /// <returns></returns>
        public static string PositionToString(this SPosition position, int num)
        {
            string sfen = string.Empty;

            using (StringWriter wr = new StringWriter())
            {
                WritePosition(position, wr, num);

                sfen = wr.ToString();
            }

            return sfen;
        }

        /// <summary>
        /// 指し手文字列を返す
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public static string MoveToString(MoveData move)
        {
            string sfen = string.Empty;

            using (StringWriter wr = new StringWriter())
            {
                WriteMove(move, wr);

                sfen = wr.ToString();
            }

            return sfen;
        }

        ///////////////////////////////////////////////////////////////////// 

        /// <summary>
        /// sfen形式の盤情報を読み込み
        /// </summary>
        /// <param name="notation"></param>
        /// <param name="sr"></param>
        private static int ReadPosition(SPosition position, string sfen, int index = 0)
        {
            int file, rank;
            int ch;
            Piece piece;
            int piece_num;

            SPosition pos = position;

            file = 0;
            rank = 0;

            if (sfen != string.Empty)
            {
                pos.BoardClear();
            }

            // 局面データ
            while (index < sfen.Length)
            {
                ch = sfen[index];
                index++;

                if (ch == ' ')
                {
                    break;
                }

                if (ch == '/')
                {
                    // 段の区切り
                    file = 0;
                    rank += 1;
                    if (rank >= Square.NRANK)
                    {
                        break;
                    }
                }
                else if (ch >= '0' && ch <= '9')
                {
                    // 数字は駒がないところ
                    file += ch - '0';
                }
                else
                {
                    piece = Piece.NoPiece;

                    // なり
                    if (ch == '+')
                    {
                        piece |= Piece.PromotionFlag;
                        if (index < sfen.Length)
                        {
                            ch = sfen[index];
                            index++;
                            if (ch == ' ')
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    // 後手
                    if (ch >= 'a' && ch <= 'z')
                    {
                        piece |= Piece.WhiteFlag;
                        ch = char.ToUpper((char)ch);
                    }

                    // charをPiece変換
                    PieceType pieceType;
                    if (CharToPieceHashtable.TryGetValue((char)ch, out pieceType))
                    {
                        piece |= (Piece)pieceType;
                    }
                    else
                    {
                        // 不明な文字列
                        Debug.Print("parse error");
                        piece = Piece.NoPiece;
                    }

                    if (piece.TypeOf() != PieceType.NoPieceType)
                    {
                        // 駒がある
                        if (file < Square.NFILE)
                        {
                            pos.SetPiece(file, rank, piece);
                            file++;
                        }
                    }
                }
            }

            // 手番
            while (index < sfen.Length)
            {
                ch = sfen[index];
                index++;

                if (ch == ' ')
                {
                    break;
                }

                if (ch == 'w')
                {
                    pos.Turn = PlayerColor.White;
                }
                else if (ch == 'b')
                {
                    pos.Turn = PlayerColor.Black;
                }
            }

            // 持ち駒
            while (index < sfen.Length)
            {
                ch = sfen[index];
                index++;
                if (ch == ' ')
                {
                    break;
                }

                piece_num = 1;

                // 枚数
                if (ch >= '0' && ch <= '9')
                {
                    piece_num = ch - '0';
                    if (index < sfen.Length)
                    {
                        ch = sfen[index];
                        index++;
                    }
                    else
                    {
                        break;
                    }

                    if (ch == ' ')
                    {
                        break;
                    }
                    else if (ch >= '0' && ch <= '9')
                    {
                        piece_num = (piece_num * 10) + (ch - '0');
                        if (index < sfen.Length)
                        {
                            ch = sfen[index];
                            index++;
                        }
                        else
                        {
                            break;
                        }

                        if (ch == ' ')
                        {
                            break;
                        }
                    }
                }

                PieceType piece_type;

                if (CharToPieceHashtable.TryGetValue(char.ToUpper((char)ch), out piece_type))
                {
                    if (char.IsUpper((char)ch))
                    {
                        // 大文字は先手
                        pos.SetBlackHand(piece_type, piece_num); 
                    }
                    else
                    {
                        // 後手
                        pos.SetWhiteHand(piece_type, piece_num);
                    }
                }
            }

            // n手目 (読み飛ばす
            while (index < sfen.Length)
            {
                ch = sfen[index];
                index++;
                if (ch == ' ')
                {
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// fileを返す
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private static int FileFromChar(char ch)
        {
            int file = -1;

            if (ch >= '1' && ch <= '9')
            {
                int suji = ch - '0';

                file = suji.ToFile();
            }

            return file;
        }

        /// <summary>
        /// rankを返す
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private static int RankFromChar(char ch)
        {
            int rank = -1;

            if (ch >= 'a' && ch <= 'i')
            {
                int dan = ch - 'a' + 1;

                rank = dan.ToRank();
            }

            return rank;
        }

        private static char CharFromPieceType(PieceType pt)
        {
            char ch;

            ch = Sfen.CharToPieceHashtable.FirstOrDefault(x => x.Value == pt).Key;
            if (ch == 0)
            {
                ch = ' ';
            }

            return ch;
        }

        public static PieceType PieceTypeFromChar(char ch)
        {
            PieceType type = PieceType.NoPieceType;

            if (Sfen.CharToPieceHashtable.ContainsKey(ch))
            {
                type = Sfen.CharToPieceHashtable[ch];
            }

            return type;
        }

        /// <summary>
        /// 局面の出力
        /// </summary>
        /// <param name="position"></param>
        /// <param name="sr"></param>
        private static void WritePosition(SPosition position, TextWriter wr, int movenumber)
        {
            int sq = 0;
            int space = 0;

            // 盤面出力

            for (int rank = 0; rank < Square.NRANK; rank++)
            {
                // 段の切り替わりで/を出力
                if (rank != 0)
                {
                    wr.Write('/');
                }

                for (int file = 0; file < Square.NFILE; file++, sq++)
                {
                    Piece piece = position.GetPiece(sq);
                    char ch;

                    if (piece == Piece.NoPiece)
                    {
                        space++;
                    }
                    else
                    {
                        if (space != 0)
                        {
                            wr.Write(space);
                            space = 0;
                        }

                        if (piece.IsPromoted())
                        {
                            // 成り
                            wr.Write('+');
                        }

                        ch = CharFromPieceType(piece.TypeOf());

                        if (piece.HasFlag(Piece.WhiteFlag))
                        {
                            ch = char.ToLower(ch);
                        }

                        wr.Write(ch);
                    }
                }

                if (space != 0)
                {
                    wr.Write(space);
                    space = 0;
                }
            }

            // 手番の出力
            if (position.Turn == PlayerColor.White)
            {
                wr.Write(" w ");
            }
            else
            {
                wr.Write(" b ");
            }

            // 持ち駒の出力
            int hand_cnt = 0;
            for (PieceType pt = PieceType.HI; pt > PieceType.NoPieceType; pt--)
            {
                int num = position.GetBlackHand(pt);
                if (num != 0)
                {
                    if (num > 1)
                    {
                        wr.Write(num);
                    }

                    wr.Write(CharFromPieceType(pt));

                    hand_cnt++;
                }
            }

            for (PieceType pt = PieceType.HI; pt > PieceType.NoPieceType; pt--)
            {
                int num = position.GetWhiteHand(pt);
                if (num != 0)
                {
                    if (num > 1)
                    {
                        wr.Write(num);
                    }

                    char ch = CharFromPieceType(pt);
                    ch = char.ToLower(ch); // 後手は小文字
                    wr.Write(ch);

                    hand_cnt++;
                }
            }

            if (hand_cnt == 0)
            {
                wr.Write("-");
            }

            if (movenumber != 0)
            {
                wr.Write(" {0}", movenumber); // 手数 実際には次が何手目か
            }
        }

         /// <summary>
        /// 指し手の出力
        /// </summary>
        /// <param name="position"></param>
        /// <param name="sr"></param>
        private static void WriteMove(MoveData move_data, TextWriter wr)
        {
            // 以下のような漢字
            // ７六歩(77)    7g7f
            // １一1銀成(22) 1a2b+
            // ６五金打      G*6e

            if (move_data.MoveType.IsResult())
            {
                // 結果の場合
                switch (move_data.MoveType)
                {
                    case MoveType.Resign: // 投了
                    case MoveType.Timeout: // 切れ負け
                    case MoveType.LoseFoul: // 反則負け
                    case MoveType.LoseNyugyoku: // 入玉負け
                        wr.Write("resign"); 
                        break;
                    case MoveType.Repetition: // 千日手
                    case MoveType.Draw:　　// 持将棋
                        wr.Write("draw");
                        break;
                    case MoveType.WinFoul: // 反則勝ち
                    case MoveType.WinNyugyoku: // 入玉勝ち
                        wr.Write("win");
                        break;
                    default:
                        // 上記以外は出力しない
                        break;
                }
            }
            else if (move_data.MoveType == MoveType.Pass)
            {
                wr.Write("pass"); // gps合わせで0000ではなくpassにする
            }
            else if (move_data.MoveType.HasFlag(MoveType.DropFlag))
            {
                wr.Write(
                    "{0}*{1}{2}",
                    CharFromPieceType(move_data.Piece.TypeOf()),
                    (char)('1' + move_data.ToSquare.SujiOf() - 1),
                    (char)('a' + move_data.ToSquare.DanOf() - 1));
            }
            else if (move_data.MoveType.HasFlag(MoveType.MoveFlag))
            {
                wr.Write(
                    "{0}{1}{2}{3}",
                    (char)('1' + move_data.FromSquare.SujiOf() - 1),
                    (char)('a' + move_data.FromSquare.DanOf() - 1),
                    (char)('1' + move_data.ToSquare.SujiOf() - 1),
                    (char)('a' + move_data.ToSquare.DanOf() - 1));

                if (move_data.MoveType.HasFlag(MoveType.Promotion))
                {
                    // 成り
                    wr.Write("+");
                }
            }
        }     
    }

    /// <summary>
    /// エンジンから受け取った情報のToken分割
    /// </summary>
    internal class Tokenizer
    {
        private int index;
        private string str;
        private string temp;

        public Tokenizer(string str)
        {
            this.str = str;

            this.index = 0;
            this.temp = null;
        }

        public bool IsSeparator(char ch)
        {
            return ch == ' ' || ch == '\n' || ch == '\r';
        }

        /// <summary>
        /// スペース区切りの単語抜き出し
        /// </summary>
        /// <returns></returns>
        public string Token()
        {
            if (this.temp != null)
            {
                string str = this.temp;

                this.temp = null;

                return str;
            }

            string subStr = string.Empty;
            int startPos = -1;

            for (; this.index < this.str.Length; this.index++)
            {
                if (!this.IsSeparator(this.str[this.index]))
                {
                    if (startPos == -1)
                    {
                        startPos = this.index;
                    }
                }
                else if (this.IsSeparator(this.str[this.index]))
                {
                    if (startPos != -1)
                    {
                        break;
                    }
                }
                else
                {
                    // 何もしない
                }
            }

            if (startPos != -1)
            {
                subStr = this.str.Substring(startPos, this.index - startPos);
            }

            return subStr;
        }

        /// <summary>
        /// USI optionの名前を取り出す特殊処理
        /// </summary>
        public string TokenName()
        {
            int pos;
            string subStr;

            // スペースのスキップ
            for (; this.index < this.str.Length; this.index++)
            {
                if (!this.IsSeparator(this.str[this.index]))
                {
                    break;
                }
            }

            if (this.index >= this.str.Length)
            {
                subStr = string.Empty;
            }
            else
            {
                pos = this.str.IndexOf(" type", this.index);
                if (pos <= this.index)
                {
                    subStr = string.Empty;
                }
                else
                {
                    subStr = this.str.Substring(this.index, pos - this.index);
                    this.index = pos + 1;
                }
            }

            return subStr;
        }

        /// <summary>
        /// ポジションを切り出す
        /// </summary>
        /// <returns></returns>
        public string TokenPosition()
        {
            int pos;
            string subStr;

            // スペースのスキップ
            for (; this.index < this.str.Length; this.index++)
            {
                if (!this.IsSeparator(this.str[this.index]))
                {
                    break;
                }
            }

            if (this.index >= this.str.Length)
            {
                subStr = string.Empty;
            }
            else
            {
                pos = this.str.IndexOf("moves", this.index);
                if (pos <= this.index)
                {
                    subStr = this.str.Substring(this.index, this.str.Length - this.index);
                    this.index = this.str.Length;
                }
                else
                {
                    subStr = this.str.Substring(this.index, pos - this.index);
                    this.index = pos;
                }
            }

            return subStr;
        }

        // 残りを全部取り出す
        public string Last()
        {
            string subStr = string.Empty;

            if ((this.index + 1) < this.str.Length)
            {
                subStr = this.str.Substring(this.index + 1, this.str.Length - this.index - 1);
            }

            return subStr;
        }

        /// <summary>
        /// 数値のパース
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long ParseNum(string str, out int cnt)
        {
            long num = 0;
            int index = 0;
            bool minus = false;

            if (str.Length >= 1)
            {
                if (str[0] == '-')
                {
                    // マイナス
                    minus = true;
                    index++;
                }
            }

            for (; index < str.Length; index++)
            {
                char c = str[index];

                if (c >= '0' && c <= '9')
                {
                    num = num * 10;
                    num = num + (c - '0');
                }
                else if (c == 'K' || c == 'k')
                {
                    num = num * 1000;
                    break;
                }
                else if (c == 'M' || c == 'm')
                {
                    num = num * 1000 * 1000;
                    break;
                }
                else
                {
                    break;
                }
            }

            cnt = index;

            if (minus)
            {
                num = -num;
            }

            return num;
        }

        public static long ParseNum(string str)
        {
            int cnt;

            return ParseNum(str, out cnt);
        }

        public void Push(string str)
        {
            this.temp = str;
        }
    }
}
