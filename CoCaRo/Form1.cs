using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoCaRo
{
    public partial class Form1 : Form
    {
        ChessBoardManager ChessBoard;
        SocketManager socket;
        bool myTurn = true;

        public Form1()
        {
            InitializeComponent();
            ChessBoard = new ChessBoardManager(pnlChessBoard,txtPlayerName,pctbMark);
            ChessBoard.EndedGame += ChessBoard_EndGame;
            ChessBoard.PlayerMarked += ChessBoard_PlayerMarked;

            prgbCountDown.Step = Cons.COOL_DOWN_STEP;
            prgbCountDown.Maximum = Cons.COOL_DOWN_TIME;
            prgbCountDown.Value = Cons.COOL_DOWN_TIME;
            //prgbCountDown.Value = 0;


            tmCoolDown.Interval = Cons.COOL_DOWN_INTERVAL;

            NewGame();
            socket = new SocketManager();
            //tmCoolDown.Start();
        }


        private void ChessBoard_PlayerMarked(object sender, ButtonClickEvent e)
        {
            DisableChessBoard();
            StartTimer();
            ResetProgessBar(Cons.COOL_DOWN_TIME);
            
            try
            {
                socket.Send(new SocketData((int)SocketCommand.SEND_POINT, "Hihihi", e.ClickedPoint));
            }
            catch
            {

            }
            NotMyTurn();
            Listen();
        }

        private void ChessBoard_EndGame(object sender, EventArgs e)
        {
            EndGame();
        }

        private void tmCoolDown_Tick(object sender, EventArgs e)
        {
            prgbCountDown.PerformStep();
            if (prgbCountDown.Value <= 0)
            {
                //tmCoolDown.Stop();
                TimeOut();
            }

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        #region Menustrip_Menu
        private void TimeOut()
        {
            //if(ChessBoard.CurrentPlayer)
            StopTimer();
            DisableChessBoard();
            if (myTurn)
            {
                MessageBox.Show("Ban da het gio");
            }
            else
            {
                MessageBox.Show("Doi thu da het gio");
            }
        }
        private void EndGame()
        {
            StopTimer();
            DisableChessBoard();
            if (myTurn)
            {
                MessageBox.Show("You Win");
            }
            else
            {
                MessageBox.Show("You Lose");
            }
        }
        void NewGame()
        {
            StopTimer();
            ResetProgessBar(Cons.COOL_DOWN_TIME);
            ChessBoard.DrawChessBoard();
        }
        void Quit()
        {
            Application.Exit();
        }
        void Undo()
        {
            ChessBoard.undo();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            NewGame();
            EnableChessBoard();
            try
            {
                socket.Send(new SocketData((int)SocketCommand.NEW_GAME, "Hihihi", new Point()));
            }
            catch { }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }
        #endregion


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Really quit?", "Infomation", MessageBoxButtons.OKCancel) != DialogResult.OK)
                e.Cancel = true;
            else
            {
                try
                {
                    socket.Send(new SocketData((int)SocketCommand.QUIT, "", new Point()));
                }
                catch
                {

                }
            }
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            txtIp.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            if (string.IsNullOrEmpty(txtIp.Text))
            {
                txtIp.Text = socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }
        }
        private void btnLan_Click(object sender, EventArgs e)
        {
            socket.IP = txtIp.Text;
            if (!socket.ConnectServer())
            {
                socket.IsServer = true;
                EnableChessBoard();
                socket.CreateServer();
            }
            else
            {
                socket.IsServer = false;
                DisableChessBoard();
                Listen();
                //socket.Send(new SocketData((int)SocketCommand.NOTYFI,"Da ket noi",null));
            }
        }


        private void Listen()
        {
            try
            {
                Thread listenThread = new Thread(() =>
                {
                    try
                    {
                        SocketData data = (SocketData)socket.Receive();
                        ProcessData(data);
                    }
                    catch {  }
                });
                listenThread.IsBackground = true;
                listenThread.Start();
            }
            catch
            {

            }
        }

        private void ProcessData(SocketData data)
        {
            switch (data.Command)
            {
                case (int)SocketCommand.NOTYFI:
                    MessageBox.Show(data.Message);
                    break;
                case (int)SocketCommand.NEW_GAME:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        NewGame();
                    }));
                    DisableChessBoard();

                    break;
                case (int)SocketCommand.END_GAME:
                    break;
                case (int)SocketCommand.TIME_OUT:
                    break;
                case (int)SocketCommand.QUIT:
                    StopTimer();
                    MessageBox.Show("Nguoi choi da thoat");
                    break;
                case (int)SocketCommand.SEND_POINT:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        StartTimer();
                        EnableChessBoard();
                        ChessBoard.OtherPlayerMark(data.Point);
                        ResetProgessBar(Cons.COOL_DOWN_TIME);
                        ItsMyTurn();
                    }));
                    
                    break;
                case (int)SocketCommand.UNDO:
                    break;
                default:
                    break;
            }
            Listen();
        }

        private void ItsMyTurn()
        {
            myTurn = true;
        }
        private void NotMyTurn()
        {
            myTurn = false;
        }
        private void StartTimer()
        {
            tmCoolDown.Start();
        }
        private void StopTimer()
        {
            tmCoolDown.Stop();
        }
        private void EnableChessBoard()
        {
            pnlChessBoard.Enabled = true;
        }
        private void DisableChessBoard()
        {
            pnlChessBoard.Enabled = false;
        }
        private void ResetProgessBar(int value)
        {
            prgbCountDown.Value = value;
        }
    }
}
