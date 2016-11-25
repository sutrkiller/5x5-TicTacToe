using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class Grid
    {
        private const int Computer = 1;
        private const int Player = -1;

        private readonly int _size;
        private bool _nextMoveComputer;
        private readonly int[][] _grid;
        private int _moves;

        public Grid(int size, bool computerFirst)
        {
            _size = size;
            _nextMoveComputer = computerFirst;
            _grid = new int[_size][];
            for (var i = 0; i < _size; i++)
            {
                _grid[i] = new int[_size];
            }
        }

        public Grid Add(int x, int y)
        {
            _grid[y][x] = _nextMoveComputer ? Computer : Player;
            ++_moves;
            _nextMoveComputer = !_nextMoveComputer;
            return this;
        }

        public Grid Clone()
        {
            var grid = new Grid(_size, _nextMoveComputer);
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    grid._grid[y][x] = _grid[y][x];
                }
            }
            grid._moves = _moves;
            return grid;
        }

        public List<Node> GetPossibleMoves()
        {
            if (_moves == 0)
            {
                return new List<Node> { new Node { X = _size / 2, Y = _size / 2 } };
            }

            var result = new List<Node>();

            for (var y = 0; y < _size; y++)
            {
                for (var x = 0; x < _size; x++)
                {
                    if (_grid[y][x] != 0) continue;
                    var computerNeighbours = 0;
                    var playerNeighbours = 0;
                    var neighbour = false;

                    for (var y1 = Math.Max(y - 1, 0); y1 <= y + 1 && y1 < _size; y1++)
                    {
                        for (var x1 = Math.Max(x - 1, 0); x1 <= x + 1 && x1 < _size; x1++)
                        {
                            if (_grid[y1][x1] == 0) continue;
                            neighbour = true;
                            if (_grid[y1][x1] == Computer)
                            {
                                ++computerNeighbours;
                            }
                            else
                            {
                                ++playerNeighbours;
                            }
                            //break;
                        }
                        //if (neighbour) break;
                    }

                    if (neighbour) result.Add(new Node { X = x, Y = y,ComputerNeighbours = computerNeighbours, PlayerNeighbours = playerNeighbours});
                }
            }

            return result;
        }

        public int RateGrid(Node node)
        {
            Stopwatch watch = Stopwatch.StartNew();
            var horizontal = FindHorizontalSequences();
            var vertical = FindVerticalSequences();
            var upDownDiagonal = FindUpDownDiagonalSequences();
            var downUpDiagonal = FindDownUpDiagonalSequences();

            var all = new List<List<Sequence>> { horizontal, vertical, upDownDiagonal, downUpDiagonal };

            var result = 0;
            foreach (var seqs in all)
            {
                foreach (var seq in seqs)
                {
                    //result += (int)Math.Pow(10, seq.Length) * seq.AroundFreeCells * seq.Number;

                    if (seq.Length < 3) continue;
                    if (seq.Length + seq.AroundFreeCells < 5) continue;
//                    if (seq.Length == 5)
//                    {
//                        ++result;
//                    }
                    result += (int)Math.Pow(10, seq.Length - 2) * seq.Number;
                }
            }
            Debug.WriteLine("Rate: "+watch.ElapsedTicks);
            return result;
        }

        public int RateLocal(Node node)
        {
            Stopwatch watch = Stopwatch.StartNew();
            var sequences = FindAllSequences();
            watch.Stop();

            Debug.WriteLine("Sequences: "+watch.ElapsedTicks);

            watch.Restart();
            var left = GetLeft(node.X, node.Y, true);
            var left2 = GetLeft(node.X, node.Y, false);
            watch.Stop();

            Debug.WriteLine("Other: " + watch.ElapsedTicks *8);
            return 0;
        }


        public List<Sequence> FindAllSequences()
        {
            var horizontal = FindHorizontalSequences();
            var vertical = FindVerticalSequences();
            var upDownDiagonal = FindUpDownDiagonalSequences();
            var downUpDiagonal = FindDownUpDiagonalSequences();

            return new List<List<Sequence>> {horizontal, vertical, upDownDiagonal, downUpDiagonal}.SelectMany(x=>x).ToList();
        }

        private List<Sequence> FindVerticalSequences()
        {
            var sequences = new List<Sequence>();
            for (int x = 0; x < _size; x++)
            {
                int last = 0;
                Sequence seq = null;
                for (int y = 0; y < _size; y++)
                {
                    if (_grid[y][x] != last)
                    {
                        if (_grid[y][x] == 0)
                        {
                            seq.X2 = x;
                            seq.Y2 = y-1;
                            seq.AroundFreeCells++;
                            sequences.Add(seq);
                            seq = null;
                        }
                        else
                        {
                            if (seq != null)
                            {
                                seq.X2 = x;
                                seq.Y2 = y-1;
                                sequences.Add(seq);
                            }
                            seq = new Sequence()
                            {
                                AroundFreeCells = last == 0 && y > 0 ? 1 : 0,
                                X1 = x,
                                Y1 = y,
                                Length = 1,
                                Number = _grid[y][x]
                            };
                        }
                        last = _grid[y][x];
                    }
                    else
                    {
                        if (seq == null)
                        {
                        }
                        else
                        {
                            seq.Length++;
                        }
                    }
                }
                if (seq != null)
                {
                    seq.X2 = x;
                    seq.Y2 = _size-1;
                    sequences.Add(seq);
                }
            }

            return sequences;
        }

        private List<Sequence> FindHorizontalSequences()
        {
            var sequences = new List<Sequence>();
            for (int y = 0; y < _size; y++)
            {
                int last = 0;
                Sequence seq = null;
                for (int x = 0; x < _size; x++)
                {
                    if (_grid[y][x] != last)
                    {
                        if (_grid[y][x] == 0)
                        {
                            seq.X2 = x - 1;
                            seq.Y2 = y;
                            seq.AroundFreeCells++;
                            sequences.Add(seq);
                            seq = null;
                        }
                        else
                        {
                            if (seq != null)
                            {
                                seq.X2 = x - 1;
                                seq.Y2 = y;
                                sequences.Add(seq);
                            }
                            seq = new Sequence()
                            {
                                AroundFreeCells = last == 0 && x > 0 ? 1 : 0,
                                X1 = x,
                                Y1 = y,
                                Length = 1,
                                Number = _grid[y][x]
                            };
                        }
                        last = _grid[y][x];
                    }
                    else
                    {
                        if (seq == null)
                        {
                        }
                        else
                        {
                            seq.Length++;
                        }
                    }
                }
                if (seq != null)
                {
                    seq.X2 = _size -1;
                    seq.Y2 = y;
                    sequences.Add(seq);
                }
            }

            return sequences;
        }

        private List<Sequence> FindUpDownDiagonalSequences()
        {
            var sequences = new List<Sequence>();
            for (int i = -_size; i < _size; i++)
            {
                int last = 0;
                Sequence seq = null;
                for (int y = 0,x =i; y < _size && x<_size; y++,x++)
                {
                    if (x<0) continue;

                    if (_grid[y][x] != last)
                    {
                        if (_grid[y][x] == 0)
                        {
                            seq.X2 = x - 1;
                            seq.Y2 = y -1;
                            seq.AroundFreeCells++;
                            sequences.Add(seq);
                            seq = null;
                        }
                        else
                        {
                            if (seq != null)
                            {
                                seq.X2 = x - 1;
                                seq.Y2 = y -1;
                                sequences.Add(seq);
                            }
                            seq = new Sequence()
                            {
                                AroundFreeCells = last == 0 && x > 0 && y >0 ? 1 : 0,
                                X1 = x,
                                Y1 = y,
                                Length = 1,
                                Number = _grid[y][x]
                            };
                        }
                        last = _grid[y][x];
                    }
                    else
                    {
                        if (seq == null)
                        {
                        }
                        else
                        {
                            seq.Length++;
                        }
                    }
                }
                if (seq != null)
                {
                    seq.X2 = i + _size -1;
                    seq.Y2 = _size -1;
                    sequences.Add(seq);
                }

            }
            return sequences;
        }

        private List<Sequence> FindDownUpDiagonalSequences()
        {
            var sequences = new List<Sequence>();
            for (int i = 2*_size; i >= 0; i--)
            {
                int last = 0;
                Sequence seq = null;
                for (int y = 0,x =i; y < _size && x>=0; y++,x--)
                {
                    if (x>=_size) continue;

                    if (_grid[y][x] != last)
                    {
                        if (_grid[y][x] == 0)
                        {
                            seq.X2 = x + 1;
                            seq.Y2 = y -1;
                            seq.AroundFreeCells++;
                            sequences.Add(seq);
                            seq = null;
                        }
                        else
                        {
                            if (seq != null)
                            {
                                seq.X2 = x + 1;
                                seq.Y2 = y -1;
                                sequences.Add(seq);
                            }
                            seq = new Sequence()
                            {
                                AroundFreeCells = last == 0 && x < _size-1 && y >0 ? 1 : 0,
                                X1 = x,
                                Y1 = y,
                                Length = 1,
                                Number = _grid[y][x]
                            };
                        }
                        last = _grid[y][x];
                    }
                    else
                    {
                        if (seq == null)
                        {
                        }
                        else
                        {
                            seq.Length++;
                        }
                    }
                }
                if (seq != null)
                {
                    seq.X2 = i - _size +1;
                    seq.Y2 = _size -1;
                    sequences.Add(seq);
                }

            }
            return sequences;
        }

        #region Local

        private int GetLeft(int x, int y, bool me)
        {
            int finding = me ? Computer : Player;
            int result = 0;
            for (int i = x - 1; i >= 0 && _grid[y][i] == finding; --i)
            {
                result++;
            }
            return result;
        }
        private  int GetRight(int x, int y, bool me)
        {
            int finding = me ? Computer : Player;
            int result = 0;
            for (int i = x + 1; i < _size && _grid[y][i] == finding; ++i)
            {
                result++;
            }
            return result;
        }

        private int GetUp(int x, int y, bool me)
        {
            int finding = me ? Computer : Player;
            int result = 0;
            for (int i = x - 1; i >= 0 && _grid[i][x] == finding; --i)
            {
                result++;
            }
            return result;
        }

        private int GetDown(int x, int y, bool me)
        {
            int finding = me ? Computer : Player;
            int result = 0;
            for (int i = x + 1; i < _size && _grid[i][x] == finding; ++i)
            {
                result++;
            }
            return result;
        }

        private int GetLeftUp(int x, int y, bool me)
        {
            int finding = me ? Computer : Player;
            int result = 0;
            for (int i = x - 1, j = y - 1; i >= 0 && j >= 0 && _grid[j][i] == finding; --i, --j)
            {
                result++;
            }
            return result;
        }

        private int GetLeftDown(int x, int y, bool me)
        {
            int finding = me ? Computer : Player;
            int result = 0;
            for (int i = x - 1, j = y + 1; i >= 0 && j < _size && _grid[j][i] == finding; --i, ++j)
            {
                result++;
            }
            return result;
        }

        private int GetRightUp(int x, int y, bool me)
        {
            int finding = me ? Computer : Player;
            int result = 0;
            for (int i = y - 1, j = x + 1; i >= 0 && j < _size && _grid[i][j] == finding; --i, ++j)
            {
                result++;
            }
            return result;
        }

        private int GetRightDown(int x, int y, bool me)
        {
            int finding = me ? Computer : Player;
            int result = 0;
            for (int i = x + 1, j = y + 1; i < _size && j < _size && _grid[j][i] == finding; ++i, ++j)
            {
                result++;
            }
            return result;
        }

        #endregion

        #region Debug

        public void DrawGrid()
        {
            var comp = (_moves % 2 == 0 && _nextMoveComputer) || (_moves %2 == 1 && !_nextMoveComputer) ? "X" : "O";
            var player = comp == "X" ? "O" : "X";

            Console.Write("|");
            for (int x = 0; x < _size; x++)
            {
                Console.Write("-");
            }
            Console.WriteLine("|");

            for (int y = 0; y < _size; y++)
            {
                Console.Write("|");
                for (int x = 0; x < _size; x++)
                {
                    if (_grid[y][x] == 0) Console.Write(" ");
                    if (_grid[y][x] == Computer) Console.Write(comp);
                    if (_grid[y][x] == Player) Console.Write(player);
                }
                Console.WriteLine("|");
            }
            Console.Write("|");
            for (int x = 0; x < _size; x++)
            {
                Console.Write("-");
            }
            Console.WriteLine("|");
        }

        #endregion
    }
}
