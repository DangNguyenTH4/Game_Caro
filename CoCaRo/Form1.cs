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
            pnlChessBoard.Enabled = false;

            tmCoolDown.Start();
            prgbCountDown.Value = Cons.COOL_DOWN_TIME;

            socket.Send(new SocketData((int)SocketCommand.SEND_POINT,"Hihihi",e.ClickedPoint));
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
                EndGame();
            }

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        #region Menustrip_Menu

        private void EndGame()
        {
            tmCoolDown.Stop();
            pnlChessBoard.Enabled = false;
            MessageBox.Show("End");
        }
        void NewGame()
        {
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
            tmCoolDown.Stop();
            prgbCountDown.Value = Cons.COOL_DOWN_TIME;
            NewGame();
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
                pnlChessBoard.Enabled = true;
                socket.CreateServer();
            }
            else
            {
                socket.IsServer = false;
                pnlChessBoard.Enabled = false;
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
                    break;
                case (int)SocketCommand.END_GAME:
                    break;
                case (int)SocketCommand.QUIT:
                    break;
                case (int)SocketCommand.SEND_POINT:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        tmCoolDown.Start();
                        pnlChessBoard.Enabled = true;
                        ChessBoard.OtherPlayerMark(data.Point);
                        prgbCountDown.Value = Cons.COOL_DOWN_TIME;
                    }));
                    
                    break;
                case (int)SocketCommand.UNDO:
                    break;
                default:
                    break;
            }
            Listen();
        }
    }
}
