using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class Sequence
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int AroundFreeCells { get; set; }
        public int Number { get; set; }
        public int Length { get; set; }

        public IEnumerable<Tuple<int, int>> EnumerateCells()
        {
            var xChange = (X2 - X1) / Length;
            var yChange = (Y2 - Y1) / Length;

            for (int x = X1, y=Y1; x < X2 && y < Y2; x+=xChange,y+=yChange)
            {
                yield return new Tuple<int, int>(x, y);
            }
        }
    }
}
