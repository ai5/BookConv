using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ShogiLib
{
    /// <summary>
    /// プレイヤーの色
    /// </summary>
    public enum PlayerColor
    {
        Black,  // 先手
        White,  // 後手
        NoColor,  // 色なし
        NCOLOR = 2
    }

    /// <summary>
    /// PlayerColorの拡張メソッド
    /// </summary>
    public static class PlayerColorExtentions
    {
        public static PlayerColor Opp(this PlayerColor color)
        {
            return color ^ PlayerColor.White;
        }

        public static char ToChar(this PlayerColor color)
        {
            return color == PlayerColor.Black ? '▲' : '△';
        }
    }

    /// <summary>
    /// ピースタイプ
    /// </summary>
    public enum PieceType : byte
    {
        NoPieceType,  // 駒なし
        FU,    // 歩
        KYO,   // 香
        KEI,   // 桂馬
        GIN,   // 銀
        KIN,   // 金
        KAK,   // 角
        HI,    // 飛車
        OU,   // 王

        Pawn = 1, // 歩
        Lance,    // 香車
        Knight,   // 桂馬
        Silver,   // 銀
        Gold,     // 金
        Bishop,   // 角
        Rook,     // 飛車
        King,     // 王
    }

    /// <summary>
    /// 駒
    /// </summary>
    public enum Piece : byte
    {
        NoPiece,

        BFU = PieceType.FU, // 先手歩
        BKYO,  // 香車
        BKEI,  // 桂
        BGIN,  // 銀
        BKIN,  // 金
        BKAK,  // 角
        BHI,   // 飛車
        BOU,   // 王

        // 先手成り駒
        BTO = BFU | NARI,
        BNKYO = BKYO | NARI,
        BNKEI = BKEI | NARI,
        BNGIN = BGIN | NARI,
        BUMA = BKAK | NARI,
        BRYU = BHI | NARI,

        // 後手の駒
        WFU = PieceType.FU | WhiteFlag,
        WKYO,
        WKEI,
        WGIN,
        WKIN,
        WKAK,
        WHI,
        WOU,

        // 後手成り駒
        WTO = WFU | NARI,
        WNKYO = WKYO | NARI,
        WNKEI = WKEI | NARI,
        WNGIN = WGIN | NARI,
        WUMA = WKAK | NARI,
        WRYU = WHI | NARI,

        WhiteFlag = 0x10,  // 後手
        PromotionFlag = 0x08, // 成りフラグ
        NARI = 0x08,         // 成りフラグ NARI==PromotionFlag

        TypeMask = 0x07, // PieceType部分のマスク用
        PromotionMask  = 0x0f, // promotion判定用のフラグ

        // 英語用定義は面倒なのでなし
    }

    /// <summary>
    /// Pieceの拡張メソッドとユーティリティ
    /// </summary>
    public static class PieceExtensions
    {
        private const int ColorShift = 4;

        /// <summary>
        /// PieceTypeを返す
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static PieceType TypeOf(this Piece piece)
        {
            PieceType pt = (PieceType)(piece & Piece.TypeMask);

            if (pt == PieceType.NoPieceType)
            {
                pt = (PieceType)(piece & Piece.PromotionFlag);
            }

            return pt;
        }

        /// <summary>
        /// 持ち駒にした時のINDEX
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static PieceType ToHnadIndex(this Piece piece)
        {
            return (PieceType)(piece & Piece.TypeMask);
        }

        /// <summary>
        /// 駒の色を返す
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static PlayerColor ColorOf(this Piece piece)
        {
            if (piece == Piece.NoPiece)
            {
                return PlayerColor.NoColor;
            }

            return (PlayerColor)((int)(piece & Piece.WhiteFlag) >> ColorShift);
        }

        /// <summary>
        /// 駒のタイプと色から駒をさくせい　
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="pt"></param>
        /// <param name="color"></param>
        public static Piece MakePiece(PieceType pt, PlayerColor color)
        {
            return (Piece)((int)pt | ((int)color << ColorShift));
        }

        /// <summary>
        /// 先後を駒の色フラグに変換
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Piece PieceFlagFromColor(PlayerColor color)
        {
            return (Piece)((int)color << ColorShift);
        }

        /// <summary>
        /// 成駒判定
        /// </summary>
        /// <param name="piece"></param>
        /// <returns>true 成り駒</returns>
        public static bool IsPromoted(this Piece piece)
        {
            return (piece & Piece.PromotionMask) > Piece.PromotionFlag;
        }

        /// <summary>
        /// 駒の反転
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static Piece Opp(this Piece piece)
        {
            if (piece != Piece.NoPiece)
            {
                if (piece.ColorOf() == PlayerColor.Black)
                {
                    piece |= Piece.WhiteFlag;
                }
                else
                {
                    piece &= ~Piece.WhiteFlag;
                }
            }

            return piece;
        }
    }

    public static class DeepCopyHelper
    {
        public static T DeepCopy<T>(T target)
        {
            T result;
            BinaryFormatter b = new BinaryFormatter();

            MemoryStream mem = new MemoryStream();

            try
            {
                b.Serialize(mem, target);
                mem.Position = 0;
                result = (T)b.Deserialize(mem);
            }
            finally
            {
                mem.Close();
            }

            return result;
        }
    }
}
