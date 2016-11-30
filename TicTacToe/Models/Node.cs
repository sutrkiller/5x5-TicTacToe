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
        public int Max { get; set; } = int.MaxValue; //Beta
        public int Min { get; set; } = int.MinValue; //Alpha
        public bool IsMaxNode { get; set; }
        public int ComputerNeighbours { get; set; }
        public int PlayerNeighbours { get; set; }
        public IList<Node> Children { get; set; } = new List<Node>();

        public RatingResult ValueN { get; set; }
        public RatingResult MinN { get; set; }
        public RatingResult MaxN { get; set; }
    }
}
