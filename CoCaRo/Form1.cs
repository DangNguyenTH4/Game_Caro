using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoCaRo
{
    public partial class Form1 : Form
    {
        ChessBoardManager ChessBoard;
        public Form1()
        {
            InitializeComponent();
            ChessBoard = new ChessBoardManager(pnlChessBoard,txtPlayerName,pctbMark);

            prgbCountDown.Step = Cons.COOL_DOWN_STEP;
            prgbCountDown.Maximum = Cons.COOL_DOWN_TIME;
            prgbCountDown.Value = 0;

            tmCoolDown.Interval = Cons.COOL_DOWN_INTERVAL;

            ChessBoard.DrawChessBoard();

            //tmCoolDown.Start();
        }

        private void tmCoolDown_Tick(object sender, EventArgs e)
        {
            prgbCountDown.PerformStep();
        }
    }
}
