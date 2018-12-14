using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoCaRo
{
    class Player
    {
        private string name;
        private int playerEr;
        private Image mark;
        public string Name { get => name; set => name = value; }
        public Image Mark { get => mark; set => mark = value; }
        public int PlayerEr { get => playerEr; set => playerEr = value; }

        public Player(string name,Image mark)
        {
            this.name = name;
            this.mark = mark;
        }
    }
}
