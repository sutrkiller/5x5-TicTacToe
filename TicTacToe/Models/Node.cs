using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Value { get; set; }
        public int Max { get; set; } //Beta
        public int Min { get; set; } //Alpha
        public bool IsMaxNode { get; set; }
        public IList<Node> Children { get; set; }
    }
}
