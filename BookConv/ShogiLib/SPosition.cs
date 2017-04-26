using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ShogiLib
{
    public enum Handicap
    {
        HIRATE,    // 平手
        KYO,       // 香落ち
        RIGHT_KYO, // 右香車落ち
        KAKU,      // 角落ち
        HISYA,     // 飛車落ち
        HIKYO,     // 飛車香落ち
        H2,     // 2枚落ち
        H3,     // 3枚落ち
        H4,     // ４枚落ち
        H5,
        LEFT5, // 左５枚落ち
        H6,
        H8,
        H10,
        OTHER
    }

    public static class HandicapExtention
    {
        // 手合割
        public static readonly Hashtable HandicapHash = new Hashtable()
        {
            { "平手",     Handicap.HIRATE },
            { "香落ち",   Handicap.KYO },
            { "右香落ち", Handicap.RIGHT_KYO },
            { "角落ち",   Handicap.KAKU },
            { "飛車落ち", Handicap.HISYA },
            { "飛香落ち", Handicap.HIKYO },
            { "二枚落ち", Handicap.H2 },
            { "三枚落ち", Handicap.H3 },
            { "四枚落ち", Handicap.H4 },
            { "五枚落ち", Handicap.H5 },
            { "左五枚落ち", Handicap.LEFT5 },
            { "六枚落ち", Handicap.H6 },
            { "八枚落ち", Handicap.H8 },
            { "十枚落ち", Handicap.H10 },
            { "その他",   Handicap.OTHER }
        };

        /// <summary>
        /// 先後の判定
        /// </summary>
        /// <param name="handicap"></param>
        /// <returns></returns>
        public static bool IsSenGo(this Handicap handicap)
        {
            bool ret = false;

            if (handicap == Handicap.HIRATE || handicap == Handicap.OTHER)
            {
                ret = true;
            }

            return ret;
        }

        public static string ToKifuString(this Handicap handicap)
        {
            foreach (DictionaryEntry de in HandicapHash)
            {
                if ((Handicap)de.Value == handicap)
                {
                    return (string)de.Key;
                }
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// 盤
    /// </summary>
    public class SPosition : ICloneable
    {
        public const int HandMax = 9;  // 持ち駒

        private PlayerColor turn;   // 手番

        private int[][] hand;
        private int[] blackHand; // 先手持ち駒
        private int[] whiteHand; // 後手持ち駒

        private Piece[] board;

        private MoveData moveLast;  // 最新の指し手

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SPosition()
        {
            this.blackHand = new int[HandMax];
            this.whiteHand = new int[HandMax];

            this.hand = new int[][] { this.blackHand, this.whiteHand };

            this.board = new Piece[Square.NSQUARE];
            this.moveLast = new MoveData();

            this.Init();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            this.ResetBoard();
        }

        /// <summary>
        /// 　盤情報を初期局面にする
        /// </summary>
        private void ResetBoard()
        {
            this.turn = PlayerColor.Black;

            for (int i = 0; i < HandMax; i++)
            {
                this.blackHand[i] = 0;
                this.whiteHand[i] = 0;
            }

            // 全部空白で埋める
            for (int i = 0; i < Square.NSQUARE; i++)
            {
                this.board[i] = Piece.NoPiece;
            }

            this.board[Square.SQ91] = Piece.WKYO;
            this.board[Square.SQ81] = Piece.WKEI;
            this.board[Square.SQ71] = Piece.WGIN;
            this.board[Square.SQ61] = Piece.WKIN;
            this.board[Square.SQ51] = Piece.WOU;
            this.board[Square.SQ41] = Piece.WKIN;
            this.board[Square.SQ31] = Piece.WGIN;
            this.board[Square.SQ21] = Piece.WKEI;
            this.board[Square.SQ11] = Piece.WKYO;

            this.board[Square.SQ82] = Piece.WHI;
            this.board[Square.SQ22] = Piece.WKAK;

            for (int sq = Square.SQ93; sq <= Square.SQ13; sq++)
            {
                this.board[sq] = Piece.WFU;
            }

            for (int sq = Square.SQ97; sq <= Square.SQ17; sq++)
            {
                this.board[sq] = Piece.BFU;
            }

            this.board[Square.SQ28] = Piece.BHI;
            this.board[Square.SQ88] = Piece.BKAK;

            this.board[Square.SQ99] = Piece.BKYO;
            this.board[Square.SQ89] = Piece.BKEI;
            this.board[Square.SQ79] = Piece.BGIN;
            this.board[Square.SQ69] = Piece.BKIN;
            this.board[Square.SQ59] = Piece.BOU;
            this.board[Square.SQ49] = Piece.BKIN;
            this.board[Square.SQ39] = Piece.BGIN;
            this.board[Square.SQ29] = Piece.BKEI;
            this.board[Square.SQ19] = Piece.BKYO;

            this.moveLast.Initialize(); // 初期化
        }

        /// <summary>
        /// 反転する
        /// </summary>
        public void Flip()
        {
            int sqe = Square.NSQUARE - 1;

            for (int sq = 0; sq < Square.SQ55; sq++, sqe--)
            {
                Piece tmpPiece = this.board[sq];
                this.board[sq] = this.board[sqe].Opp();
                this.board[sqe] = tmpPiece.Opp();
            }

            int[] tmp = new int[HandMax];
            Array.Copy(this.blackHand, tmp, HandMax);
            Array.Copy(this.whiteHand, this.blackHand, HandMax);
            Array.Copy(tmp, this.whiteHand, HandMax);
        }

        /// <summary>
        /// 駒の移動
        /// </summary>
        /// <param name="moveData"></param>
        /// <returns></returns>
        public bool Move(MoveData moveData)
        {
            bool ret = true;

            Debug.Assert(moveData.ToSquare >= 0 && moveData.ToSquare < Square.NSQUARE, "引数エラー");
            Debug.Assert(moveData.MoveType.IsMove(), "引数エラー");

            if (moveData.MoveType.HasFlag(MoveType.DropFlag))
            {
                Debug.Assert(moveData.Piece != Piece.NoPiece, "引数エラー");
                ret = this.MoveDrop(moveData);
            }
            else if (moveData.MoveType == MoveType.Pass) 
            {
                // pass
            }
            else 
            {
                Debug.Assert(moveData.Piece != Piece.NoPiece, "引数エラー");
                ret = this.MoveNormal(moveData);
            }

            if (!ret)
            {
                // 指し手登録エラー
                return false;
            }

            MoveData.Copy(this.moveLast, moveData);

            this.turn = this.turn.Opp(); // 手番変更

            return ret;
        }

        /// <summary>
        /// 通常の駒の移動
        /// </summary>
        /// <param name="moveData"></param>
        /// <returns></returns>
        private bool MoveNormal(MoveData moveData)
        {
            Piece piece;

            // 一応最初に簡易なチェックk

            Debug.Assert(moveData.FromSquare >= 0 && moveData.FromSquare < Square.NSQUARE, "引数エラー");

            // from側に駒がある
            if (!(this.board[moveData.FromSquare] != Piece.NoPiece))
            {
                Debug.Assert(false, "引数エラー");
                return false;
            }

            // 色が一致 2手差し対応のため色チェックは外す
            //            Debug.Assert(this.board[move_data.from_square].ColorOf() == turn);

            // 移動先が空白か相手の駒
            if (!((this.board[moveData.ToSquare] == Piece.NoPiece) || (this.board[moveData.ToSquare].ColorOf() == this.turn.Opp())))
            {
                Debug.Assert(false, "引数エラー");
                return false;
            }

            piece = moveData.Piece | PieceExtensions.PieceFlagFromColor(this.turn);  // 棋譜からの指し手だと色フラグが付いていないので現在のターンで先後を決める

            Debug.Assert(this.board[moveData.FromSquare] == piece, "引数エラー"); // from位置の駒が違う

            // 成りなり判定
            if (moveData.MoveType.HasFlag(MoveType.Promotion))
            {
                // 成りのチェック成っていない駒->成るのみOk
                Debug.Assert(!piece.IsPromoted(), "すでになっている駒を成ろうとしている");

                piece |= Piece.PromotionFlag;
            }

            this.board[moveData.FromSquare] = Piece.NoPiece;
            this.board[moveData.ToSquare] = piece;

            if (moveData.CapturePiece != Piece.NoPiece)
            {
                if (this.turn == PlayerColor.White)
                {
                    this.whiteHand[(int)moveData.CapturePiece.ToHnadIndex()] += 1;
                }
                else
                {
                    this.blackHand[(int)moveData.CapturePiece.ToHnadIndex()] += 1;
                }
            }

            return true;
        }

        /// <summary>
        /// 持ち駒を打つ
        /// </summary>
        /// <param name="moveData"></param>
        /// <returns></returns>
        private bool MoveDrop(MoveData moveData)
        {
            Piece piece;

            // 一応最初に簡易なチェックk

            piece = moveData.Piece | PieceExtensions.PieceFlagFromColor(this.turn);

            // 持ってない
            if (!this.IsHand(this.turn, moveData.Piece.ToHnadIndex()))
            {
                Debug.Assert(false, "持っていない駒は打てない");
                return false;
            }

            Debug.Assert(this.board[moveData.ToSquare] == Piece.NoPiece, "移動先が空白でない");
            Debug.Assert(!piece.IsPromoted(), "成っている状態で打てない");

            this.hand[(int)this.turn][(int)moveData.Piece.ToHnadIndex()] -= 1;

            this.board[moveData.ToSquare] = piece;

            return true;
        }

        /// <summary>
        /// 1手戻す
        /// </summary>
        /// <param name="moveData"></param>
        /// <returns></returns>
        public bool UnMove(MoveData moveData, MoveData curent)
        {
            bool ret = true;

            Debug.Assert(moveData.ToSquare >= 0 && moveData.ToSquare < Square.NSQUARE, "引数エラー");
            Debug.Assert(moveData.MoveType.IsMove(), "引数エラー");

            if (moveData.MoveType.HasFlag(MoveType.DropFlag))
            {
                Debug.Assert(moveData.Piece != Piece.NoPiece, "引数エラー");
                ret = this.UnMoveDrop(moveData);
            }
            else if (moveData.MoveType == MoveType.Pass)
            {
                // パス
            }
            else
            {
                Debug.Assert(moveData.Piece != Piece.NoPiece, "引数エラー");
                ret = this.UnMoveNormal(moveData);
            }

            if (!ret)
            {
                // 指し手登録エラー
                return false;
            }

            if (curent != null)
            {
                MoveData.Copy(this.moveLast, curent);
            }

            this.turn = this.turn.Opp(); // 手番変更

            return ret;
        }

        /// <summary>
        /// 1手戻す
        /// </summary>
        /// <param name="moveData"></param>
        /// <returns></returns>
        private bool UnMoveNormal(MoveData moveData)
        {
            Piece piece;

            // 一応最初に簡易なチェックk

            Debug.Assert(moveData.FromSquare >= 0 && moveData.FromSquare < Square.NSQUARE, "引数エラー");

            // from側は駒がないはず
            Debug.Assert(this.board[moveData.FromSquare] == Piece.NoPiece, "引数エラー");

            // to側は駒があるかつ自分の駒
            Debug.Assert((this.board[moveData.ToSquare] != Piece.NoPiece) && (this.board[moveData.ToSquare].ColorOf() == this.turn.Opp()), "引数エラー");

            piece = moveData.Piece;

            // 成りなり判定
            if (moveData.MoveType.HasFlag(MoveType.Promotion))
            {
                // 一応成りを落とす
                piece &= ~Piece.PromotionFlag;
            }

            this.board[moveData.FromSquare] = piece;
            this.board[moveData.ToSquare] = moveData.CapturePiece;

            if (moveData.CapturePiece != Piece.NoPiece)
            {
                if (this.turn.Opp() == PlayerColor.White)
                {
                    this.whiteHand[(int)moveData.CapturePiece.ToHnadIndex()] -= 1;
                }
                else
                {
                    this.blackHand[(int)moveData.CapturePiece.ToHnadIndex()] -= 1;
                }
            }

            return true;
        }

        /// <summary>
        /// 持ち駒を打つ
        /// </summary>
        /// <param name="moveData"></param>
        /// <returns></returns>
        private bool UnMoveDrop(MoveData moveData)
        {
            Piece piece;
            PlayerColor unmoveTurn = this.Turn.Opp(); // unmoveするときは現在のターンと反対側

            // 一応最初に簡易なチェックk

            piece = moveData.Piece;

            Debug.Assert(this.board[moveData.ToSquare] == piece, "駒が違う？");
            Debug.Assert(!piece.IsPromoted(), "成っている状態で打てない");

            this.hand[(int)unmoveTurn][(int)moveData.Piece.ToHnadIndex()] += 1;

            this.board[moveData.ToSquare] = Piece.NoPiece;

            return true;
        }

        // -------------------------------------------------
        // Clone

        public object Clone()
        {
            SPosition pos;

            pos = (SPosition)MemberwiseClone();

            if (this.blackHand != null)
            {
                pos.blackHand = (int[])this.blackHand.Clone();
            }

            if (this.whiteHand != null)
            {
                pos.whiteHand = (int[])this.whiteHand.Clone();
            }

            pos.hand = new int[][] { pos.blackHand, pos.whiteHand };

            if (this.board != null)
            {
                pos.board = (Piece[])this.board.Clone();
            }

            if (this.moveLast != null)
            {
                pos.moveLast = new MoveData(this.moveLast);
            }


            return pos;
        }

        // -------------------------------------------------

        /// <summary>
        /// 指定した駒の検索
        /// </summary>
        /// <param name="piece"> 探したい駒 </param>
        /// <returns>  Square.NSQUARE寄り小さい 見つかった位置
        ///            == Square.NSQUARE みつからなかった
        /// </returns>
        public int SearchPiece(Piece piece)
        {
            int sq;

            for (sq = 0; sq < Square.NSQUARE; sq++)
            {
                if (this.board[sq] == piece)
                {
                    break;
                }
            }

            return sq;
        }

        // 指定した場所の駒
        public bool IsBlack(int rank, int file)
        {
            return this.board[(rank * 9) + file].ColorOf() == PlayerColor.Black;
        }

        public bool IsWhite(int rank, int file)
        {
            return this.board[(rank * 9) + file].ColorOf() == PlayerColor.White;
        }

        public bool IsEmpty(int rank, int file)
        {
            return this.board[(rank * 9) + file] == Piece.NoPiece;
        }

        // 駒を持ってる？
        public bool IsHand(PlayerColor color, PieceType pieceType)
        {
            if (pieceType >= PieceType.FU && pieceType <= PieceType.HI)
            {
                // ok
            }
            else
            {
                return false;
            }

            if (this.hand[(int)color][(int)pieceType] == 0)
            {
                return false;
            }

            return true;
        }

        // -------------------------------------------------
        // アクセサ

        /// <summary>
        /// 手番
        /// </summary>
        public PlayerColor Turn
        {
            get
            {
                return this.turn; 
            }

            set 
            {
                if (this.turn != value)
                {
                    this.turn = value;
                }
            }
        }

        /// <summary>
        /// 持ち駒の枚数取得
        /// </summary>
        /// <param name="color"></param>
        /// <param name="pieceType"></param>
        /// <returns></returns>
        public int GetHand(PlayerColor color, PieceType pieceType)
        {
            return this.hand[(int)color][(int)pieceType];
        }

        /// <summary>
        /// 先手持ち駒の枚数取得
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public int GetBlackHand(PieceType piece)
        {
            return this.blackHand[(int)piece];
        }

        /// <summary>
        /// 先手持ち駒枚数設定
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="num"></param>
        public void SetBlackHand(PieceType piece, int num)
        {
            this.blackHand[(int)piece] = num;
        }

        /// <summary>
        /// 後手持ち駒枚数取得
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public int GetWhiteHand(PieceType piece)
        {
            return this.whiteHand[(int)piece];
        }

        /// <summary>
        /// 後手持ち駒枚数設定
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="num"></param>
        public void SetWhiteHand(PieceType piece, int num)
        {
            this.whiteHand[(int)piece] = num;
        }

        /// <summary>
        /// 指定した座標の盤上の駒を取得
        /// </summary>
        /// <param name="sq">座標</param>
        /// <returns>コマ番号</returns>
        public Piece GetPiece(int sq)
        {
            return this.board[sq];
        }

        /// <summary>
        /// 指定した座標の盤上の駒を取得
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        public Piece GetPiece(int file, int rank)
        {
            return this.board[(rank * 9) + file];
        }

        /// <summary>
        /// 指定した座標に駒を設定
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rank"></param>
        /// <param name="piece"></param>
        public void SetPiece(int file, int rank, Piece piece)
        {
            this.board[(rank * 9) + file] = piece;
        }

        public void SetPiece(int sq, Piece piece)
        {
            this.board[sq] = piece;
        }

        /// <summary>
        /// 盤面取得
        /// </summary>
        public IEnumerable<Piece> Board
        {
            get
            {
                foreach (var item in this.board)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// 先手持ち駒の配列取得
        /// </summary>
        public int[] BlackHand
        {
            get { return this.blackHand; }
        }

        /// <summary>
        /// 後手持ち駒配列取得
        /// </summary>
        public int[] WhiteHand
        {
            get { return this.whiteHand; }
        }

        /// <summary>
        /// 最後の指し手取得
        /// </summary>
        public MoveData MoveLast
        {
            get { return this.moveLast; }
        }

        // -------------------------------------------------
        // ハンディキャップ設定

        // 香落ち
        public void SetHandicapKyo()
        {
            this.ResetBoard();

            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ11] = Piece.NoPiece;
        }

        // 右香落ち
        public void SetHandicapRightKyo()
        {
            this.ResetBoard();
            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ91] = Piece.NoPiece;
        }

        // 角落ち
        public void SetHandicapKaku()
        {
            this.ResetBoard();

            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ22] = Piece.NoPiece;
        }

        // 飛車落ち
        public void SetHandicapHisya()
        {
            this.ResetBoard();
            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ82] = Piece.NoPiece;
        }

        // 飛香落ち
        public void SetHandicapHiKyo()
        {
            this.ResetBoard();
            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ11] = Piece.NoPiece;
            this.board[Square.SQ82] = Piece.NoPiece;
        }

        // 2枚落ち
        public void SetHandicap2()
        {
            this.ResetBoard();
            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ22] = Piece.NoPiece;
            this.board[Square.SQ82] = Piece.NoPiece;
        }

        // 3枚落ち
        public void SetHandicap3()
        {
            this.ResetBoard();
            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ22] = Piece.NoPiece;
            this.board[Square.SQ82] = Piece.NoPiece;

            this.board[Square.SQ91] = Piece.NoPiece; // 右香
        }

        // 4枚落ち
        public void SetHandicap4()
        {
            this.ResetBoard();
            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ22] = Piece.NoPiece; // 角
            this.board[Square.SQ82] = Piece.NoPiece; // 飛車
            this.board[Square.SQ11] = Piece.NoPiece; // 左香
            this.board[Square.SQ91] = Piece.NoPiece; // 右香
        }

        // 5枚落ち
        public void SetHandicap5()
        {
            this.ResetBoard();
            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ22] = Piece.NoPiece; // 角
            this.board[Square.SQ82] = Piece.NoPiece; // 飛車
            this.board[Square.SQ11] = Piece.NoPiece; // 左香
            this.board[Square.SQ81] = Piece.NoPiece; // 右桂
            this.board[Square.SQ91] = Piece.NoPiece; // 左香
        }

        // 左5枚落ち
        public void SetHandicapLeft5()
        {
            this.ResetBoard();
            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ22] = Piece.NoPiece; // 角
            this.board[Square.SQ82] = Piece.NoPiece; // 飛車
            this.board[Square.SQ11] = Piece.NoPiece; // 左香
            this.board[Square.SQ21] = Piece.NoPiece; // 左桂
            this.board[Square.SQ91] = Piece.NoPiece; // 右香
        }

        // 6枚落ち
        public void SetHandicap6()
        {
            this.ResetBoard();
            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ22] = Piece.NoPiece; // 角
            this.board[Square.SQ82] = Piece.NoPiece; // 飛車
            this.board[Square.SQ11] = Piece.NoPiece; // 左香
            this.board[Square.SQ21] = Piece.NoPiece; // 左桂
            this.board[Square.SQ81] = Piece.NoPiece; // 右桂
            this.board[Square.SQ91] = Piece.NoPiece; // 右香
        }

        // 8枚落ち
        public void SetHandicap8()
        {
            this.ResetBoard();
            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ22] = Piece.NoPiece; // 角
            this.board[Square.SQ82] = Piece.NoPiece; // 飛車
            this.board[Square.SQ11] = Piece.NoPiece; // 左香
            this.board[Square.SQ21] = Piece.NoPiece; // 左桂
            this.board[Square.SQ31] = Piece.NoPiece; // 銀
            this.board[Square.SQ71] = Piece.NoPiece; // 銀
            this.board[Square.SQ81] = Piece.NoPiece; // 右桂
            this.board[Square.SQ91] = Piece.NoPiece; // 右香
        }

        // 10枚落ち
        public void SetHandicap10()
        {
            this.ResetBoard();
            this.turn = PlayerColor.White; // 上手から指す

            this.board[Square.SQ22] = Piece.NoPiece; // 角
            this.board[Square.SQ82] = Piece.NoPiece; // 飛車
            this.board[Square.SQ11] = Piece.NoPiece; // 左香
            this.board[Square.SQ21] = Piece.NoPiece; // 左桂
            this.board[Square.SQ31] = Piece.NoPiece; // 銀
            this.board[Square.SQ41] = Piece.NoPiece; // 金
            this.board[Square.SQ61] = Piece.NoPiece; // 金
            this.board[Square.SQ71] = Piece.NoPiece; // 銀
            this.board[Square.SQ81] = Piece.NoPiece; // 右桂
            this.board[Square.SQ91] = Piece.NoPiece; // 右香
        }

        /// <summary>
        /// 反転
        /// </summary>
        public void Reverse()
        {
            for (int i = 0; i < HandMax; i++)
            {
                int num = this.blackHand[i];

                this.blackHand[i] = this.whiteHand[i];
                this.whiteHand[i] = num;
            }

            // 反転
            for (int i = 0, j = Square.NSQUARE - 1; i <= (Square.NSQUARE / 2); i++, j--)
            {
                Piece piece = this.board[i];

                this.board[i] = this.board[j].Opp();

                this.board[j] = piece.Opp();
            }
        }

        // 詰将棋配置
        public void InitMatePosition()
        {
            this.turn = PlayerColor.Black;

            for (int i = 0; i < HandMax; i++)
            {
                this.blackHand[i] = 0;
                this.whiteHand[i] = 0;
            }

            // 全部空白で埋める
            for (int i = 0; i < Square.NSQUARE; i++)
            {
                this.board[i] = Piece.NoPiece;
            }

            this.board[4] = Piece.WOU;

            this.WhiteHand[(int)PieceType.FU] = 18;
            this.WhiteHand[(int)PieceType.KYO] = 4;
            this.WhiteHand[(int)PieceType.KEI] = 4;
            this.WhiteHand[(int)PieceType.GIN] = 4;
            this.WhiteHand[(int)PieceType.KIN] = 4;
            this.WhiteHand[(int)PieceType.KAK] = 2;
            this.WhiteHand[(int)PieceType.HI] = 2;
        }

        // ボードクリア(CSA用
        public void BoardClear()
        {
            for (int i = 0; i < Square.NSQUARE; i++)
            {
                this.board[i] = Piece.NoPiece;
            }

            for (int i = 0; i < HandMax; i++)
            {
                this.blackHand[i] = 0;
                this.whiteHand[i] = 0;
            }
        }

        /// <summary>
        /// 指定した座標が盤内にあるかどうか
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        public static bool InBoard(int file, int rank)
        {
            if ((file >= 0 && file < Square.NFILE)
                && (rank >= 0 && rank < Square.NRANK))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 盤面比較
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Equals(SPosition pos)
        {
            for (int i = 0; i < Square.NSQUARE; i++)
            {
                if (this.board[i] != pos.board[i])
                {
                    return false;
                }
            }

            for (int i = 0; i < this.blackHand.Length; i++)
            {
                if (this.blackHand[i] != pos.blackHand[i])
                {
                    return false;
                }
            }

            for (int i = 0; i < this.whiteHand.Length; i++)
            {
                if (this.whiteHand[i] != pos.whiteHand[i])
                {
                    return false;
                }
            }

            if (this.turn != pos.turn)
            {
                return false;
            }

            return true;
        }
    }
}
