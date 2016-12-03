using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class Grid
    {
        public const int Computer = 1;
        public const int Player = -1;

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

        public Grid Remove(int x, int y)
        {
            _grid[y][x] = 0;
            --_moves;
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

                    if (neighbour) result.Add(new Node { X = x, Y = y, ComputerNeighbours = computerNeighbours, PlayerNeighbours = playerNeighbours });
                }
            }

            return result;
        }


        private int[] _dirX = {-1,-1,-1,0,1,1,1,0};
        private int[] _dirY = {1,0,-1,-1,-1,0,1,1};

        private List<string> GetDirs(int x, int y,bool computer)
        {

            int number = computer ? Computer : Player;

            List<string> dirs = new List<string>();
            for (int i = 0; i < 8; i++)
            {
                StringBuilder builder = new StringBuilder();
                for (int j = 1; j < 5; j++)
                {
                    int xc = _dirX[i] * j + x;
                    int yc = _dirY[i] * j + y;

                    if (xc >= _size || yc >= _size || xc < 0 || yc < 0) break;
                    if (_grid[yc][xc] == -number) break;
                    builder.Append(_grid[yc][xc] == 0 ? " " : "x");
                }
                dirs.Add(builder.ToString());
            }

            return Enumerable.Range(0, 4).Select(i => string.Join("",dirs[i].Reverse()) + "x" + dirs[i + 4]).ToList();
        }

        public double RateString(string str, bool computer, double add = 0)
        {
            if (Regex.IsMatch(str, "xxxxx")) return 6 + add;

            if (Regex.IsMatch(str, " xxxx ")) return 5 + add;

            if (Regex.IsMatch(str, " xxxx")) return 4 + add;
            if (Regex.IsMatch(str, "x xxx")) return 4 + add;
            if (Regex.IsMatch(str, "xx xx")) return 4 + add;
            if (Regex.IsMatch(str, "xxx x")) return 4 + add;
            if (Regex.IsMatch(str, "xxxx ")) return 4 + add;

            if (Regex.IsMatch(str, "  xxx ")) return 4 + add;
            if (Regex.IsMatch(str, " xxx  ")) return 4 + add;
            if (Regex.IsMatch(str, " xx x ")) return 4 + add;
            if (Regex.IsMatch(str, " x xx ")) return 4 + add;

            if (Regex.IsMatch(str, "xxx  ")) return 3 + add;
            if (Regex.IsMatch(str, " xxx ")) return 3 + add;
            if (Regex.IsMatch(str, "  xxx")) return 3 + add;
            if (Regex.IsMatch(str, " x xx")) return 3 + add;
            if (Regex.IsMatch(str, " xx x")) return 3 + add;
            if (Regex.IsMatch(str, "x  xx")) return 3 + add;
            if (Regex.IsMatch(str, "x x x")) return 3 + add;
            if (Regex.IsMatch(str, "x xx ")) return 3 + add;
            if (Regex.IsMatch(str, "xx  x")) return 3 + add;
            if (Regex.IsMatch(str, "xx x ")) return 3 + add;

            if (Regex.IsMatch(str, "   xx ")) return 3 + add;
            if (Regex.IsMatch(str, "  x x ")) return 3 + add;
            if (Regex.IsMatch(str, "  xx  ")) return 3 + add;
            if (Regex.IsMatch(str, " x  x ")) return 3 + add;
            if (Regex.IsMatch(str, " x x  ")) return 3 + add;
            if (Regex.IsMatch(str, " xx   ")) return 3 + add;

            if (Regex.IsMatch(str, "   xx")) return 2 + add;
            if (Regex.IsMatch(str, "  x x")) return 2 + add;
            if (Regex.IsMatch(str, "  xx ")) return 2 + add;
            if (Regex.IsMatch(str, " x  x")) return 2 + add;
            if (Regex.IsMatch(str, " x x ")) return 2 + add;
            if (Regex.IsMatch(str, " xx  ")) return 2 + add;
            if (Regex.IsMatch(str, "x   x")) return 2 + add;
            if (Regex.IsMatch(str, "x  x ")) return 2 + add;
            if (Regex.IsMatch(str, "x x  ")) return 2 + add;
            if (Regex.IsMatch(str, "xx   ")) return 2 + add;



            return 0;
        }

        public Tuple<int,int> RateGrid(bool computer)
        {
            List<Tuple<List<double>,int,int>> lists = new List<Tuple<List<double>, int, int>>();
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    if (_grid[y][x] != 0) continue;
                    var dirs = GetDirs(x, y, computer);
                    var resultC = dirs.Select(((s, i) => RateString(s, true, 0.1))).OrderByDescending(s => s);

                    //if (_grid[y][x] != 0) return 0;
                    var dirsp = GetDirs(x, y, !computer);
                    var resultP = dirsp.Select(((s, i) => RateString(s, true))).OrderByDescending(s => s);

                    lists.Add(new Tuple<List<double>, int, int>(resultC.Concat(resultP).OrderByDescending(s => s).ToList(),x,y));
                }
            }

            var conv = lists.Select(x => string.Join("", x.Item1.Select(c => (int) c))).ToList();
            var min = conv.Max();
            var mins = conv.Select((x,i)=>new {i,x}).Where(x => x.x == min).Select(x=>lists[x.i]).ToList();

            for (int i = 0; i < 8; i++)
            {
                var max = mins.Max(x => x.Item1[i]);
                mins = mins.Where(x => Math.Abs(x.Item1[i] - max) < 0.01).ToList();
                if (mins.Count() == 1) break;
            }

            if (mins.Count==0) return new Tuple<int, int>(_size/2,_size/2);
            return new Tuple<int, int>(mins[0].Item2,mins[0].Item3);
        }

        public double RateLocal(int x, int y, IReadOnlyCollection<Sequence> sequences)
        {
            var all = sequences.Where(s => s.AroundFreeCells.Any(c => c.Item1 == x && c.Item2 == y)).ToList();
            int number = _nextMoveComputer ? Computer : Player;
            var allC = all.Where(s => s.Number == number).ToList();
            var allO = all.Where(s => s.Number != number).ToList();
            double result = 0;
            double result2 = 0;

            foreach (var sequence in allC)
            {
                var rating = sequence.Length * (sequence.AroundFreeCells.Count + (sequence.Length/5)*3 + sequence.Length /4 ) * 100 * (FreeCellsInDirection(x, y, sequence.Type, 5 - sequence.Length).Take(5 - sequence.Length).Count() + sequence.Length == 5 ? 1 : 0);

//                rating += sequence.AroundFreeCells.Take(5 - sequence.Length).Count() * 10;
//                rating += FreeCellsInDirection(x, y, sequence.Type, 5 - sequence.Length).Take(5 - sequence.Length - sequence.AroundFreeCells.Count).Count();
                result += rating;
                //result += sequence.Number * 10;

//                result += Math.Pow(10,sequence.Length) + sequence.Number +
//                          FreeCellsInDirection(x, y, sequence.Type, 5 - sequence.Length).Count() / 10.0;
            }

            foreach (var sequence in allO)
            {
                var rating = sequence.Length * (sequence.AroundFreeCells.Count + (sequence.Length / 5) * 3 + sequence.Length / 4) * 100 * (FreeCellsInDirection(x, y, sequence.Type, 5 - sequence.Length).Take(5 - sequence.Length).Count() + sequence.Length == 5 ? 1 : 0);
                //                rating += sequence.AroundFreeCells.Take(5 - sequence.Length).Count() * 10;
                //                rating += FreeCellsInDirection(x, y, sequence.Type, 5 - sequence.Length).Take(5 - sequence.Length - sequence.AroundFreeCells.Count).Count();
                result += rating;
                //result += sequence.Number * 10;

                //                result += Math.Pow(10,sequence.Length) + sequence.Number +
                //                          FreeCellsInDirection(x, y, sequence.Type, 5 - sequence.Length).Count() / 10.0;
            }
            return result;
        }

        //Rate only for comp/player and for one cell == how good this move will be
        public double Rate(int x, int y, IReadOnlyCollection<Sequence> sequences)
        {
            double resultC = 100;
            double resultO = -100;
            var all = sequences.Where(s => s.AroundFreeCells.Any(c => c.Item1 == x && c.Item2 == y)).ToList();
            if (all.Count == 0) return 1;
            int number = _nextMoveComputer ? Computer : Player; //computer moves
            var allC = all.Where(s => s.Number == number).ToList();
            var allO = all.Where(s => s.Number != number).ToList();

            //P already won
            if (allO.Any(s => s.Length >= 5)) return resultO;
            ++resultO;

            //C can win...some of the sequences has 4
            if (allC.Any(s => s.Length >= 4)) return resultC;
            //resultC--;

            //C can win...seqs with same type with length >= 4
            if (allC.GroupBy(s => s.Type).Any(g => g.Sum(s => s.Length) >= 4)) return resultC;
            resultC--;

            //P's 4 cannot be blocked
            if (allO.Any(s => s.Length == 4 && s.AroundFreeCells.Count == 2)) return resultO;
            resultO++;

            //P's 4 can be blocked
            if (allO.Any(s => s.Length >= 4 && s.AroundFreeCells.Count == 1)) return resultC;
            //resultC--;

            //P's seqs of same type and length >= 4 can be blocked
            if (allO.GroupBy(s => s.Type).Any(g => g.Sum(s => s.Length) >= 4)) return resultC;
            resultC--;

            //C can extend to four...seqs with same type with length >= 3 + 4f
            if (allC.GroupBy(s => s.Type).Any(g => g.Sum(s => s.Length) >= 3 && g.Sum(s=>s.AroundFreeCells.Count)==4)) return resultC;
            //resultC--;

            //C can extend to four...n* 3 + 2f
            if (allC.Count(s => s.Length == 3 && s.AroundFreeCells.Count == 2) > 0) return resultC + allO.Sum(s => s.Length) / 10.0;
            //resultC--;

            //C can extend to four...2* 3 + 1f with 2f continue
            if (
                allC.Where(s => s.Length == 3 && s.AroundFreeCells.Count == 1)
                    .Sum(s => FreeCellsInDirection(x, y, s.Type).Count()) == 2) return resultC;
            resultC--;

            //P to 4 can be blocked - seqs with same type and length >= 3 + 4f
            if (allO.GroupBy(s => s.Type).Any(g => g.Sum(s => s.Length) >= 3 && g.Sum(s => s.AroundFreeCells.Count) == 4)) return resultC;

            //P can extend to four...n* 3 + 2f
            if (allO.Count(s => s.Length == 3 && s.AroundFreeCells.Count == 2) > 0) return resultC+allC.Sum(s=>s.Length)/10.0;

            //P can extend to four...2* 3 + 1f with 2f continue
            if (
                allO.Where(s => s.Length == 3 && s.AroundFreeCells.Count == 1)
                    .Sum(s => FreeCellsInDirection(x, y, s.Type).Count()) == 2) return resultC;
            resultC--;

            //C can extend to four...3+1f + 1c
            if (allC.Any(s => s.Length == 3 && FreeCellsInDirection(x, y, s.Type).Any())) return resultC;

            //C can extend to four...seqs sum == 3 + 3f
            if (allC.GroupBy(s => s.Type)
                .Any(g => g.Sum(s => s.Length) == 3 && g.Sum(s => s.AroundFreeCells.Count) == 3)) return resultC;
            --resultC;

            //P can extend to four...3+1f + 1c
            if (allO.Any(s => s.Length == 3 && FreeCellsInDirection(x, y, s.Type).Any())) return resultC;

            //C can extend to four...seqs sum == 3 + 3f
            if (allO.GroupBy(s => s.Type)
                .Any(g => g.Sum(s => s.Length) == 3 && g.Sum(s => s.AroundFreeCells.Count) == 3)) return resultC;
            --resultC;

            //C can extend to 3...n*2+2f
            if (allC.Any(s => s.Length == 2 && s.AroundFreeCells.Count == 2)) return resultC + allO.Sum(s => s.Length) / 10.0;

            //C can extend to 3---1+2f + 1+2f T
            if (
                allC.Where(s => s.Length == 1 && s.AroundFreeCells.Count == 2)
                    .GroupBy(s => s.Type)
                    .Any(g => g.Count() >= 2)) return resultC;
            --resultC;

            //P can extend to 3...n*2+2f
            if (allO.Any(s => s.Length == 2 && s.AroundFreeCells.Count == 2)) return resultC + allC.Sum(s => s.Length) / 10.0;

            //P can extend to 3---1+2f + 1+2f T
            if (
                allO.Where(s => s.Length == 1 && s.AroundFreeCells.Count == 2)
                    .GroupBy(s => s.Type)
                    .Any(g => g.Count() >= 2)) return resultC;
            --resultC;


            //C can extend to 2*2...2*1+2f
            if (allC.Count(s=>s.Length==1 && s.AroundFreeCells.Count ==2) >= 2) return resultC;
            resultC--;

            //P can extend to 2*2...2*1+2f
            if (allO.Count(s => s.Length == 1 && s.AroundFreeCells.Count == 2) >=2) return resultC;
            resultC--;

            //C can extend to 3--2+1f +2c
            if (allC.Any(s => s.Length == 2 && FreeCellsInDirection(x,y,s.Type,2).Count()==2)) return resultC;
            resultC--;

            //P can extend to 3--2+1f +2c
            if (allO.Any(s => s.Length == 2 && FreeCellsInDirection(x,y,s.Type,2).Count()==2)) return resultC;
            resultC--;

            //C can extend to 2...n*1+2f
            if (allC.Any(s => s.Length == 1 && s.AroundFreeCells.Count == 2)) return resultC;
            resultC--;

            //P can extend to 2...n*1+2f
            if (allO.Any(s => s.Length == 1 && s.AroundFreeCells.Count == 2)) return resultC;
            resultC--;

            //C can extend to 2...n*1+1f
            if (allC.Any(s => s.Length == 1 && FreeCellsInDirection(x,y,s.Type).Any())) return resultC;
            resultC--;

            //P can extend to 2...n*1+1f
            if (allO.Any(s => s.Length == 1 && FreeCellsInDirection(x,y,s.Type).Any())) return resultC;
            //resultC--;

            return 0;
        }

        public IEnumerable<Tuple<int, int>> GetAroundCells(int x, int y, int length = 1)
        {
            var list = new List<Tuple<int,int>>();
            for (int i = y-length; i <= y+length && i < _size; i++)
            {
                if (i < 0) continue;
                for (int j = x-length; j < _size && j <= x+length; j++)
                {
                    if (j<0) continue;
                    if (i == y && j == x) continue;
                    if (_grid[i][j]!=0) continue;
                    list.Add(new Tuple<int, int>(j,i));
                }
            }
            return list;
        }

        public IEnumerable<Tuple<int, int>>  CellsInDirection(int x, int y, int xC, int yC, int length = 1)
        {
            List<Tuple<int,int>> result = new List<Tuple<int, int>>();
            for (int i = x+xC, j = y+yC;
                i != x + (length+1) * xC && j != y + (length+1) * yC && i < _size && j < _size && i >= 0 && j >= 0;
                i += xC, j += yC)
            {
                if (_grid[j][i] != 0) break;
                result.Add(new Tuple<int, int>(i,j));
            }
            return result;
        }

        public IEnumerable<Tuple<int, int>> FreeCellsInDirection(int x, int y, SequenceType type, int length = 1)
        {
            switch (type)
            {
                case SequenceType.Horizontal:
                    return CellsInDirection(x, y, -1, 0, length).Concat(CellsInDirection(x, y, 1, 0, length));
                case SequenceType.Vertical:
                    return CellsInDirection(x, y, 0, -1, length).Concat(CellsInDirection(x, y, 0, 1, length));
                case SequenceType.TopDown:
                    return CellsInDirection(x, y, -1, -1, length).Concat(CellsInDirection(x, y, 1, 1, length));
                case SequenceType.BottomUp:
                    return CellsInDirection(x, y, -1, 1, length).Concat(CellsInDirection(x, y, 1, -1, length));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public List<Sequence> FindAllSequences()
        {
            var horizontal = FindHorizontalSequences();
            var vertical = FindVerticalSequences();
            var upDownDiagonal = FindUpDownDiagonalSequences();
            var downUpDiagonal = FindDownUpDiagonalSequences();

            return new List<List<Sequence>> { horizontal, vertical, upDownDiagonal, downUpDiagonal }.SelectMany(x => x).ToList();
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
                            seq.Y2 = y - 1;
                            seq.AroundFreeCells.Add(new Tuple<int, int>(x, y));
                            sequences.Add(seq);
                            seq = null;
                        }
                        else
                        {
                            if (seq != null)
                            {
                                seq.X2 = x;
                                seq.Y2 = y - 1;
                                sequences.Add(seq);
                            }
                            seq = new Sequence()
                            {
                                AroundFreeCells = last == 0 && y > 0 ? new List<Tuple<int, int>>() { new Tuple<int, int>(x, y - 1) } : new List<Tuple<int, int>>(),
                                X1 = x,
                                Y1 = y,
                                Length = 1,
                                Type = SequenceType.Vertical,
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
                    seq.Y2 = _size - 1;
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
                            seq.AroundFreeCells.Add(new Tuple<int, int>(x, y));
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
                                AroundFreeCells = last == 0 && x > 0 ? new List<Tuple<int, int>> { new Tuple<int, int>(x - 1, y) } : new List<Tuple<int, int>>(),
                                X1 = x,
                                Y1 = y,
                                Length = 1,
                                Type = SequenceType.Horizontal,
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
                    seq.X2 = _size - 1;
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
                for (int y = 0, x = i; y < _size && x < _size; y++, x++)
                {
                    if (x < 0) continue;

                    if (_grid[y][x] != last)
                    {
                        if (_grid[y][x] == 0)
                        {
                            seq.X2 = x - 1;
                            seq.Y2 = y - 1;
                            seq.AroundFreeCells.Add(new Tuple<int, int>(x, y));
                            sequences.Add(seq);
                            seq = null;
                        }
                        else
                        {
                            if (seq != null)
                            {
                                seq.X2 = x - 1;
                                seq.Y2 = y - 1;
                                sequences.Add(seq);
                            }
                            seq = new Sequence()
                            {
                                AroundFreeCells = last == 0 && x > 0 && y > 0 ? new List<Tuple<int, int>> { new Tuple<int, int>(x - 1, y - 1) } : new List<Tuple<int, int>>(),
                                X1 = x,
                                Y1 = y,
                                Length = 1,
                                Type = SequenceType.TopDown,
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
                    seq.X2 = i + _size - 1;
                    seq.Y2 = _size - 1;
                    sequences.Add(seq);
                }

            }
            return sequences;
        }

        private List<Sequence> FindDownUpDiagonalSequences()
        {
            var sequences = new List<Sequence>();
            for (int i = 2 * _size; i >= 0; i--)
            {
                int last = 0;
                Sequence seq = null;
                for (int y = 0, x = i; y < _size && x >= 0; y++, x--)
                {
                    if (x >= _size) continue;

                    if (_grid[y][x] != last)
                    {
                        if (_grid[y][x] == 0)
                        {
                            seq.X2 = x + 1;
                            seq.Y2 = y - 1;
                            seq.AroundFreeCells.Add(new Tuple<int, int>(x, y));
                            sequences.Add(seq);
                            seq = null;
                        }
                        else
                        {
                            if (seq != null)
                            {
                                seq.X2 = x + 1;
                                seq.Y2 = y - 1;
                                sequences.Add(seq);
                            }
                            seq = new Sequence()
                            {
                                AroundFreeCells = last == 0 && x < _size - 1 && y > 0 ? new List<Tuple<int, int>> { new Tuple<int, int>(x + 1, y - 1) } : new List<Tuple<int, int>>(),
                                X1 = x,
                                Y1 = y,
                                Length = 1,
                                Type = SequenceType.BottomUp,
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
                    seq.X2 = i - _size + 1;
                    seq.Y2 = _size - 1;
                    sequences.Add(seq);
                }

            }
            return sequences;
        }

        #region Local

        private Sequence GetLeft(int x, int y)
        {
            Sequence seq = null;
            var last = 0;
            for (var x1 = x - 1; x1 >= 0; x1--)
            {
                if (_grid[y][x1] == 0)
                {
                    last = 0;
                    break;
                }
                if (x1 == x - 1)
                {
                    seq = new Sequence { AroundFreeCells = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) }, Length = 1, Number = _grid[y][x1], X1 = x1, Y1 = y,Type = SequenceType.Horizontal};
                    last = _grid[y][x1];
                    continue;
                }
                if (_grid[y][x1] != last)
                {
                    last = _grid[y][x1];
                    break;
                }
                ++seq.Length;
            }
            if (seq != null)
            {
                seq.Y2 = y;
                seq.X2 = x - seq.Length;
                if (last == 0)
                {
                    seq.AroundFreeCells.Add(new Tuple<int, int>(x - seq.Length - 1, y));
                }
            }
            return seq;
        }
        private Sequence GetRight(int x, int y)
        {
            Sequence seq = null;
            var last = 0;
            for (var x1 = x + 1; x1 < _size; x1++)
            {
                if (_grid[y][x1] == 0)
                {
                    last = 0;
                    break;
                }
                if (x1 == x + 1)
                {
                    seq = new Sequence { AroundFreeCells = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) }, Length = 1, Number = _grid[y][x1], X1 = x1, Y1 = y,Type = SequenceType.Horizontal};
                    last = _grid[y][x1];
                    continue;
                }
                if (_grid[y][x1] != last)
                {
                    last = _grid[y][x1];
                    break;
                }
                ++seq.Length;
            }
            if (seq != null)
            {
                seq.Y2 = y;
                seq.X2 = x + seq.Length;
                if (last == 0)
                {
                    seq.AroundFreeCells.Add(new Tuple<int, int>(x + seq.Length + 1, y));
                }
            }
            return seq;
        }

        private Sequence GetUp(int x, int y)
        {
            Sequence seq = null;
            var last = 0;
            for (var y1 = y - 1; y1 >= 0; y1--)
            {
                if (_grid[y1][x] == 0)
                {
                    last = 0;
                    break;
                }
                if (y1 == y - 1)
                {
                    seq = new Sequence { AroundFreeCells = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) }, Length = 1, Number = _grid[y1][x], X1 = x, Y1 = y1,Type = SequenceType.Vertical};
                    last = _grid[y1][x];
                    continue;
                }
                if (_grid[y1][x] != last)
                {
                    last = _grid[y1][x];
                    break;
                }
                ++seq.Length;
            }
            if (seq != null)
            {
                seq.Y2 = y - seq.Length;
                seq.X2 = x;
                if (last == 0)
                {
                    seq.AroundFreeCells.Add(new Tuple<int, int>(x, y - seq.Length - 1));
                }
            }
            return seq;
        }

        private Sequence GetDown(int x, int y)
        {
            Sequence seq = null;
            var last = 0;
            for (var y1 = y + 1; y1 < _size; y1++)
            {
                if (_grid[y1][x] == 0)
                {
                    last = 0;
                    break;
                }
                if (y1 == y + 1)
                {
                    seq = new Sequence { AroundFreeCells = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) }, Length = 1, Number = _grid[y1][x], X1 = x, Y1 = y1,Type = SequenceType.Vertical};
                    last = _grid[y1][x];
                    continue;
                }
                if (_grid[y1][x] != last)
                {
                    last = _grid[y1][x];
                    break;
                }
                ++seq.Length;
            }
            if (seq != null)
            {
                seq.Y2 = y + seq.Length;
                seq.X2 = x;
                if (last == 0)
                {
                    seq.AroundFreeCells.Add(new Tuple<int, int>(x, y + seq.Length + 1));
                }
            }
            return seq;
        }

        private Sequence GetLeftUp(int x, int y)
        {
            Sequence seq = null;
            var last = 0;
            for (int y1 = y - 1, x1 = x - 1; y1 >= 0 && x1 >= 0; y1--, x1--)
            {
                if (_grid[y1][x1] == 0)
                {
                    last = 0;
                    break;
                }
                if (y1 == y - 1)
                {
                    seq = new Sequence { AroundFreeCells = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) }, Length = 1, Number = _grid[y1][x1], X1 = x1, Y1 = y1,Type = SequenceType.TopDown};
                    last = _grid[y1][x1];
                    continue;
                }
                if (_grid[y1][x1] != last)
                {
                    last = _grid[y1][x1];
                    break;
                }
                ++seq.Length;
            }
            if (seq != null)
            {
                seq.Y2 = y - seq.Length;
                seq.X2 = x - seq.Length;
                if (last == 0)
                {
                    seq.AroundFreeCells.Add(new Tuple<int, int>(x - seq.Length - 1, y - seq.Length - 1));
                }
            }
            return seq;
        }

        private Sequence GetLeftDown(int x, int y)
        {
            Sequence seq = null;
            var last = 0;
            for (int y1 = y + 1, x1 = x - 1; y1 < _size && x1 >= 0; y1++, x1--)
            {
                if (_grid[y1][x1] == 0)
                {
                    last = 0;
                    break;
                }
                if (y1 == y + 1)
                {
                    seq = new Sequence { AroundFreeCells = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) }, Length = 1, Number = _grid[y1][x1], X1 = x1, Y1 = y1,Type = SequenceType.BottomUp};
                    last = _grid[y1][x1];
                    continue;
                }
                if (_grid[y1][x1] != last)
                {
                    last = _grid[y1][x1];
                    break;
                }
                ++seq.Length;
            }
            if (seq != null)
            {
                seq.Y2 = y + seq.Length;
                seq.X2 = x - seq.Length;
                if (last == 0)
                {
                    seq.AroundFreeCells.Add(new Tuple<int, int>(x - seq.Length - 1, y + seq.Length + 1));
                }
            }
            return seq;
        }

        private Sequence GetRightUp(int x, int y)
        {
            Sequence seq = null;
            var last = 0;
            for (int y1 = y - 1, x1 = x + 1; y1 >= 0 && x1 < _size; y1--, x1++)
            {
                if (_grid[y1][x1] == 0)
                {
                    last = 0;
                    break;
                }
                if (y1 == y - 1)
                {
                    seq = new Sequence { AroundFreeCells = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) }, Length = 1, Number = _grid[y1][x1], X1 = x1, Y1 = y1,Type = SequenceType.BottomUp};
                    last = _grid[y1][x1];
                    continue;
                }
                if (_grid[y1][x1] != last)
                {
                    last = _grid[y1][x1];
                    break;
                }
                ++seq.Length;
            }
            if (seq != null)
            {
                seq.Y2 = y - seq.Length;
                seq.X2 = x + seq.Length;
                if (last == 0)
                {
                    seq.AroundFreeCells.Add(new Tuple<int, int>(x + seq.Length + 1, y - seq.Length - 1));
                }
            }
            return seq;
        }

        private Sequence GetRightDown(int x, int y)
        {
            Sequence seq = null;
            var last = 0;
            for (int y1 = y + 1, x1 = x + 1; y1 < _size && x1 < _size; y1++, x1++)
            {
                if (_grid[y1][x1] == 0)
                {
                    last = 0;
                    break;
                }
                if (y1 == y + 1)
                {
                    seq = new Sequence { AroundFreeCells = new List<Tuple<int, int>> { new Tuple<int, int>(x, y) }, Length = 1, Number = _grid[y1][x1], X1 = x1, Y1 = y1,Type = SequenceType.TopDown};
                    last = _grid[y1][x1];
                    continue;
                }
                if (_grid[y1][x1] != last)
                {
                    last = _grid[y1][x1];
                    break;
                }
                ++seq.Length;
            }
            if (seq != null)
            {
                seq.Y2 = y + seq.Length;
                seq.X2 = x + seq.Length;
                if (last == 0)
                {
                    seq.AroundFreeCells.Add(new Tuple<int, int>(x + seq.Length + 1, y + seq.Length + 1));
                }
            }
            return seq;
        }

        #endregion

        #region Debug

        public void DrawGrid()
        {
            var comp = (_moves % 2 == 0 && _nextMoveComputer) || (_moves % 2 == 1 && !_nextMoveComputer) ? "X" : "O";
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
