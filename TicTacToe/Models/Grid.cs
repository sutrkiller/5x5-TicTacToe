using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class Grid
    {
        private readonly int _size;
        private readonly int[][] _grid;
        public Grid(int size)
        {
            _size = size;
            _grid = new int[_size][];
            for (var i = 0; i < _size; i++)
            {
                _grid[i] = new int[_size];
            }
        }
    }
}
