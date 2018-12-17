using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CoCaRo
{
    class ChessBoardManager
    {
        #region Properties
        private Panel chessBoard;
        private List<Player> players;
        private int currentPlayer;
        private TextBox playerName;
        private PictureBox playerMark;
        private List<List<Button>> matrix;
        private event EventHandler<ButtonClickEvent> playerMarked;
        private event EventHandler endedGame;

        public event EventHandler<ButtonClickEvent> PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }
        public Panel ChessBoard { get => chessBoard; set => chessBoard = value; }
        internal List<Player> Players { get => players; set => players = value; }
        public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public PictureBox PlayerMark { get => playerMark; set => playerMark = value; }
        public TextBox PlayerName { get => playerName; set => playerName = value; }
        public List<List<Button>> Matrix { get => matrix; set => matrix = value; }

        #endregion
        #region Initialize
        public ChessBoardManager(Panel chessBoard,TextBox playerName,PictureBox mark)
        {
            this.chessBoard = chessBoard;
            this.playerName = playerName;
            this.playerMark = mark;
            this.players = new List<Player>()
            {
                new Player("Dang",Image.FromFile(Application.StartupPath + "\\Resources\\dau-o.png")),
                new Player("Mai", Image.FromFile(Application.StartupPath + "\\Resources\\dau-x.png"))
            };

           
        }
        #endregion
        #region Methods
        public void DrawChessBoard()
        {
            chessBoard.Enabled = true;
            chessBoard.Controls.Clear();
            CurrentPlayer = 0;
            ChangePlayer();

            Matrix = new List<List<Button>>();
            Button oldBtn = new Button() { Width = 0, Location = new Point(0, 0) };
            for(int i = 0; i< Cons.CHESS_BOARD_SIZE; i++)
            {
                Matrix.Add(new List<Button>());
                for(int j = 0; j < Cons.CHESS_BOARD_SIZE; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Cons.CHESS_WIDTH,
                        Height = Cons.CHESS_HEIGHT,
                        Location = new Point(oldBtn.Location.X + oldBtn.Width, oldBtn.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString()
                    };
                    //btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.Click += btn_Click;
                    chessBoard.Controls.Add(btn);
                    oldBtn = btn;
                    Matrix[i].Add(btn);
                }
                oldBtn.Location = new Point(0, oldBtn.Location.Y + Cons.CHESS_HEIGHT);
                oldBtn.Width = 0;
                oldBtn.Height = 0;
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackgroundImage != null)
                return;
            Mark(btn);
            ChangePlayer();

            if (playerMarked != null)
                playerMarked(this, new ButtonClickEvent(GetChessPoint(btn)));

            if (isEndGame(btn))
            {
                EndGame();
            }


        }

        public void OtherPlayerMark(Point point)
        {
            Button btn = Matrix[point.X][point.Y];
            if (btn.BackgroundImage != null)
                return;
            //ChessBoard.Enabled = true;

            Mark(btn);
            ChangePlayer();
            if (isEndGame(btn))
            {
                EndGame();
            }
        }
        private bool isEndGame(Button btn)
        {
            return (isEndHorizontal(btn) || isEndVertical(btn) || isEndPrimaryDiagonal(btn) || isEndSubDiagonal(btn));
        }

        //x la hang
        //y la cot
        private Point GetChessPoint(Button btn)
        {
            Point point;
            int horizontal = Convert.ToInt32(btn.Tag);
            int vertical = Matrix[ horizontal].IndexOf(btn);
            point = new Point(horizontal,vertical);
            return point;
        }
        private bool isEndHorizontal(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countLeft = 0;
            for(int i=point.Y; i >= 0; i--)
            {

                if (Matrix[point.X][i].BackgroundImage == btn.BackgroundImage)
                    countLeft++;
                else break;
            }
            int countRight = 0;

            for (int i = point.Y+1; i < Cons.CHESS_BOARD_SIZE; i++)
            {

                if (Matrix[point.X][i].BackgroundImage == btn.BackgroundImage)
                    countRight++;
                else break;
            }
            return countLeft + countRight >= Cons.POINT_TO_WIN;
        }
        private bool isEndVertical(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countTop= 0;
            for (int i = point.X; i >= 0; i--)
            {

                if (Matrix[i][point.Y].BackgroundImage == btn.BackgroundImage)
                    countTop++;
                else break;
            }
            int countBottom = 0;

            for (int i = point.Y + 1; i < Cons.CHESS_BOARD_SIZE; i++)
            {

                if (Matrix[point.X][i].BackgroundImage == btn.BackgroundImage)
                    countBottom++;
                else break;
            }
            return countTop + countBottom >= Cons.POINT_TO_WIN;
        }
        private bool isEndPrimaryDiagonal(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X - i < 0 || point.Y - i < 0)
                    break;
                if (Matrix[point.X - i][point.Y - i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else break;
            }
            int countBottom = 0;
            for (int i = 1; i <= Cons.CHESS_BOARD_SIZE-point.X; i++)
            {
                if(point.X+i>=Cons.CHESS_BOARD_SIZE || point.Y + i >= Cons.CHESS_BOARD_SIZE)
                {
                    break;
                }
                if (Matrix[point.X + i][point.Y + i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else break;
            }
            return countBottom + countTop >= Cons.POINT_TO_WIN;

        }
        private bool isEndSubDiagonal(Button btn)
        {

            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X - i < 0 || point.Y + i >= Cons.CHESS_BOARD_SIZE)
                    break;
                if (Matrix[point.X - i][point.Y + i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else break;
            }
            int countBottom = 0;
            for (int i = 1; i <= Cons.CHESS_BOARD_SIZE - point.X; i++)
            {
                if (point.X + i >= Cons.CHESS_BOARD_SIZE || point.Y - i < 0)
                {
                    break;
                }
                if (Matrix[point.X + i][point.Y - i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else break;
            }
            return countBottom + countTop >= Cons.POINT_TO_WIN;
        }
        public void EndGame()
        {
            //MessageBox.Show("win");
            if (endedGame != null)
                endedGame(this, new EventArgs());
        }
        void Mark(Button btn)
        {
            btn.BackgroundImage = Players[currentPlayer].Mark;
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
        }
        void ChangePlayer()
        {
            PlayerName.Text = Players[CurrentPlayer].Name;
            PlayerMark.Image = Players[CurrentPlayer].Mark;
            
        }

        public bool undo()
        {
            return false;
        }

        #endregion

    }
    public class ButtonClickEvent : EventArgs
    {
        private Point clickedPoint;

        public Point ClickedPoint { get => clickedPoint; set => clickedPoint = value; }

        public ButtonClickEvent(Point point)
        {
            this.clickedPoint = point;
        }
    }
}
