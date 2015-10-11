using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShogiLib
{
    // 指し手のタイプ
    public enum MoveType : byte
    {
        NoMove,
        MoveFlag = 0x20,    // 通常の指し手
        DropFlag = 0x40,    // 打つ手
        ResultFlag = 0x80,    // 結果
        MoveMask = MoveFlag | Promotion,

        // 指し手はビット表現
        Normal = MoveFlag,
        Promotion = MoveFlag | 0x01, // 成る手
        Unpromotion = MoveFlag | 0x02, // 不成り
        Capture = MoveFlag | 0x04, // 取る手
        Same = MoveFlag | 0x08, // 同ほげ
        Pass = MoveFlag | 0x10, // パス

        // 打つ手
        Drop = DropFlag,

        // 結果は通常の値
        Resign = ResultFlag | 0,  // 投了
        Stop = ResultFlag | 1,  // 中断
        Repetition = ResultFlag | 2,  // 千日手
        Draw = ResultFlag | 3,  // 持将棋
        Timeout = ResultFlag | 4,  // 切れ負け

        Mate = ResultFlag | 5,  // 詰み
        NonMate = ResultFlag | 6,  // 不詰み
        LoseFoul = ResultFlag | 7,  // 反則負け
        WinFoul = ResultFlag | 8,  // 反則勝ち

        // エンジンからくる入玉勝ち宣言
        WinNyugyoku = ResultFlag | 9, // 入玉宣言で勝ち
        LoseNyugyoku = ResultFlag | 10, // 入玉宣言で負け
    }

    /// <summary>
    /// MoveTypeのExtention
    /// </summary>
    public static class MoveTypeExtentions
    {
        /// <summary>
        /// 結果の移動タイプの場合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsResult(this MoveType type)
        {
            bool ret = false;
            if (type.HasFlag(MoveType.ResultFlag))
            {
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// 盤面が動く指し手
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsMove(this MoveType type)
        {
            bool ret = false;

            if (type.HasFlag(MoveType.MoveFlag) || type.HasFlag(MoveType.DropFlag))
            {
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// 盤面が動く指し手
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsMoveWithoutPass(this MoveType type)
        {
            bool ret = false;

            if (type == MoveType.Pass)
            {
                // pass
            }
            else if (type.HasFlag(MoveType.MoveFlag) || type.HasFlag(MoveType.DropFlag))
            {
                ret = true;
            }

            return ret;
        }
    }

    /// <summary>
    /// 指し手データ
    /// 盤面を動かすのに必要な情報
    /// </summary>
    [Serializable]
    public class MoveData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MoveData()
        {
            this.Init();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="moveData"></param>
        public MoveData(MoveData moveData)
        {
            Copy(this, moveData);
        }

        /// <summary>
        /// タイプ指定
        /// </summary>
        /// <param name="moveType"></param>
        public MoveData(MoveType moveType)
        {
            MoveType = moveType;

            this.ToSquare = 0;
            this.FromSquare = 0;
            this.Piece = Piece.NoPiece;
            this.CapturePiece = Piece.NoPiece;
        }

        public int ToSquare { get; set; }      // 移動先

        public int FromSquare { get; set; }    // 移動元

        public MoveType MoveType { get; set; } // 指し手のタイプ

        public Piece Piece { get; set; } // 駒

        public Piece CapturePiece { get; set; } // 取った駒

        // 手番
        public PlayerColor Turn 
        {
            get
            {
                return (Piece & Piece.WhiteFlag) != 0 ? PlayerColor.White : PlayerColor.Black;
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Init()
        {
            this.ToSquare = 0;
            this.FromSquare = 0;
            MoveType = MoveType.NoMove;
            Piece = Piece.NoPiece;
            this.CapturePiece = Piece.NoPiece;
        }

        public virtual void Initialize()
        {
            this.Init();
        }

        public static void Copy(MoveData dest, MoveData src)
        {
            dest.ToSquare = src.ToSquare;
            dest.FromSquare = src.FromSquare;
            dest.MoveType = src.MoveType;
            dest.Piece = src.Piece;
            dest.CapturePiece = src.CapturePiece;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="movedata"></param>
        /// <returns></returns>
        public bool Equals(MoveData movedata)
        {
            bool ret = false;

            if (movedata == null)
            {
                return false;
            }

            if (this.MoveType.HasFlag(MoveType.DropFlag) && movedata.MoveType.HasFlag(MoveType.DropFlag))
            {
                if (this.ToSquare == movedata.ToSquare
                    && this.Piece == movedata.Piece)
                {
                    ret = true;
                }
            }
            else if (this.MoveType.HasFlag(MoveType.MoveFlag) && movedata.MoveType.HasFlag(MoveType.MoveFlag))
            {
                if (this.FromSquare == movedata.FromSquare
                    && this.ToSquare == movedata.ToSquare
                    && (this.MoveType & MoveType.MoveMask) == (movedata.MoveType & MoveType.MoveMask))
                {
                    ret = true;
                }
            }
            else
            {
                if (this.MoveType == movedata.MoveType)
                {
                    ret = true;
                }
            }

            return ret;
        }
    }
}
