using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class Program
    {
        private const int _me = -1;
        private const int _other = -2;
        private static int[][] _grid;
        private static int _moves=0;
        private static int _length;


        public static void Main(string[] args)
        {
            _length = Convert.ToInt32(Console.ReadLine());
            var myTurn = Console.ReadLine().ToUpper() == "X";

            _grid = new int[_length][];
            for(int i=0;i< _length; ++i)
            {
                _grid[i] = new int[_length];
            }

            for(;;)
            {
                if (myTurn)
                {
                    var output = _moves == 0 ? new Tuple<int, int>(_length/2,_length/2) : FindNextTurn();
                    _grid[output.Item2][output.Item1] = _me;
                    Console.WriteLine($"{output.Item1} {output.Item2}");
                } else
                {
                    var input = Console.ReadLine();
                    var split = input.Split(' ');
                    var x = Convert.ToInt32(split[0]);
                    var y = Convert.ToInt32(split[1]);
                    if (x < 0 || y < 0) return;
                    _grid[y][x] = _other;
                }
                myTurn = !myTurn;
                _moves++;
            }

        }

        private static Tuple<int,int> FindNextTurn()
        {
            var me = RateGrid(_grid, true);
            var other = RateGrid(_grid, false);

            if (me == null)
            {
                _instant = false;
                return new Tuple<int, int>(_instantX, _instantY);
            }

            int maxMe = 0;
            int xMe = 0;
            int yMe = 0;
            int maxO = 0;
            int xO = 0;
            int yO = 0;
            int max = 0;
            var xy = new List<Tuple<int, int>>();
            //int x = 0;
            //int y = 0;
            for(int i=0;i<_grid.Length;++i)
            {
                for(int j=0;j<_grid.Length;++j)
                {
                    if (me[i][j] > maxMe)
                    {
                        maxMe = me[i][j];
                        xMe = j;
                        yMe = i;
                    }
                    if (other[i][j] > maxO)
                    {
                        maxO = other[i][j];
                        xO = j;
                        yO = i;
                    }
                    int sum = other[i][j] + me[i][j];
                    if (sum > max)
                    {
                        max = sum;
                        xy.Clear();
                    }
                    if (sum == max)
                    {
                        xy.Add(new Tuple<int, int>(j, i));
                    }
                }
            }
            if (maxMe > maxO) return new Tuple<int, int>(xMe, yMe);
            if (maxMe < maxO) return new Tuple<int, int>(xO, yO);
            var maxRes = xy.Select((t,i)=>new { I = i, M = me[t.Item2][t.Item1] });
            return xy.ElementAt( maxRes.First(t => t.M == maxRes.Max(x => x.M)).I);

        }


        private static bool _instant = false;
        private static int _instantX = -1;
        private static int _instantY = -1;
        private static int[][] RateGrid(int[][] grid,bool me)
        {
            int length = grid.Length;
            var gridNew = new int[length][];
            for(int i=0;i<length;++i)
            {
                gridNew[i] = new int[length];
            }

            for(int y=0;y<length;++y)
            {
                for(int x=0;x<length;++x)
                {
                    if (_grid[y][x] < 0) continue;
                    int row = GetLeft(x, y, me) + GetRight(x, y, me);
                    int col = GetUp(x, y, me) + GetDown(x, y, me);
                    int upDown = GetLeftUp(x, y, me) + GetRightDown(x, y, me);
                    int downUp = GetLeftDown(x, y, me) + GetRightUp(x, y, me);

                    var sums = new int[] { GetLeft(x, y, me) + GetRight(x, y, me), GetUp(x, y, me) + GetDown(x, y, me) , GetLeftUp(x, y, me) + GetRightDown(x, y, me) , GetLeftDown(x, y, me) + GetRightUp(x, y, me) };

                    if (me && sums.Any(a=>a >= 4)) {
                        _instant = true;
                        _instantX = x;
                        _instantY = y;
                        return null;
                    }
                    //if (!me && sums.Any(a=>a>=3))
                    //{
                    //    _instant = true;
                    //    _instantX = x;
                    //    _instantY = y;
                    //}

                        //gridNew[y][x] = row + col + upDown + downUp;
                        gridNew[y][x] = sums.Sum();
                }
            }
            return gridNew;
        }

        private static int GetLeft(int x, int y, bool me)
        {
            int finding = me ? _me : _other;
            int result = 0;
            for(int i = x-1;i>=0 && _grid[y][i] == finding;--i)
            {
                result++;
            }
            return result;
        }
        private static int GetRight(int x, int y, bool me)
        {
            int finding = me ? _me : _other;
            int result = 0;
            for (int i = x +1; i < _length && _grid[y][i] == finding; ++i)
            {
                result++;
            }
            return result;
        }

        private static int GetUp(int x, int y, bool me)
        {
            int finding = me ? _me : _other;
            int result = 0;
            for (int i = x - 1; i >= 0 && _grid[i][x] == finding; --i)
            {
                result++;
            }
            return result;
        }

        private static int GetDown(int x, int y, bool me)
        {
            int finding = me ? _me : _other;
            int result = 0;
            for (int i = x + 1; i < _length && _grid[i][x] == finding; ++i)
            {
                result++;
            }
            return result;
        }

        private static int GetLeftUp(int x, int y, bool me)
        {
            int finding = me ? _me : _other;
            int result = 0;
            for (int i = x - 1,j=y-1; i >= 0 && j >=0 && _grid[j][i] == finding; --i,--j)
            {
                result++;
            }
            return result;
        }

        private static int GetLeftDown(int x, int y, bool me)
        {
            int finding = me ? _me : _other;
            int result = 0;
            for (int i = x - 1, j = y + 1; i >= 0 && j < _length && _grid[j][i] == finding; --i, ++j)
            {
                result++;
            }
            return result;
        }

        private static int GetRightUp(int x, int y, bool me)
        {
            int finding = me ? _me : _other;
            int result = 0;
            for (int i = y - 1, j = x + 1; i >= 0 && j < _length && _grid[i][j] == finding; --i, ++j)
            {
                result++;
            }
            return result;
        }

        private static int GetRightDown(int x, int y, bool me)
        {
            int finding = me ? _me : _other;
            int result = 0;
            for (int i = x + 1, j = y + 1; i < _length && j < _length && _grid[j][i] == finding; ++i, ++j)
            {
                result++;
            }
            return result;
        }
    }
}
