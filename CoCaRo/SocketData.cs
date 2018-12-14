using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoCaRo
{
    [Serializable]
    public class SocketData
    {
        private int command;
        private Point point;
        private string message;

        public int Command { get => command; set => command = value; }
        public Point Point { get => point; set => point = value; }
        public string Message { get => message; set => message = value; }

        public SocketData(int command,string message,Point? point = null)
        {
            this.command = command;
            this.point = (Point)point;
            this.message = message;
        }


    }
    public enum SocketCommand
    {
        SEND_POINT,
        NOTYFI,
        NEW_GAME,
        END_GAME,
        UNDO,
        QUIT
    }
}
