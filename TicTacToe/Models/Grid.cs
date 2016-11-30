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

        public int RateGrid(Node node)
        {

            var all = FindAllSequences();

            var allC = all.Where(x => x.Number == Computer).ToList();
            var allP = all.Where(x => x.Number == Player).ToList();

            if (allP.Any(x => x.Length >= 5)) return -10;
            if (allC.Any(x => x.Length >= 5)) return 10;
            if (allP.Any(x => x.Length == 4 && x.AroundFreeCells.Count == 2)) return -9;
            if (allC.Any(x => x.Length == 4 && x.AroundFreeCells.Count == 2)) return 9;
            if (allP.Any(x => x.Length == 4 && x.AroundFreeCells.Count == 1)) return -8;
            if (allC.Any(x => x.Length == 4 && x.AroundFreeCells.Count == 1)) return 8;

            var free = GetPossibleMoves();
            var listsC = new List<List<Sequence>>();
            var listsP = new List<List<Sequence>>();

            foreach (var freeNode in free)
            {
                listsC.Add(allC.Where(c => c.AroundFreeCells.Any(x => x.Item1 == freeNode.X && x.Item2 == freeNode.Y)).ToList());
                listsP.Add(allP.Where(p => p.AroundFreeCells.Any(x => x.Item1 == freeNode.X && x.Item2 == freeNode.Y)).ToList());
            }

            listsC = listsC.Where(x => x.Count > 1).ToList();
            listsP = listsP.Where(x => x.Count > 1).ToList();

            var resultsP = listsP.Select(list =>
            {
                int min = 0;
                foreach (SequenceType type in Enum.GetValues(typeof(SequenceType)))
                {
                    if (list.Where(x => x.Type == type).Sum(x => x.Length) >= 4)
                    {
                        min = min > -9 ? -9 : min;
                    }
                    if (list.Where(x => x.Type == type).Sum(x => x.Length) == 3 && list.Where(x => x.Type == type).Any(x => x.AroundFreeCells.Count == 2))
                    {
                        min = min > -7 ? -7 : min;
                    }
                }

                if (list.Sum(x => x.Length) >= 4)
                {
                    min = min > -7 ? -7 : min;
                }

                return min;
            }).ToList();

            var resultsC = listsC.Select(list =>
            {
                int max = 0;
                foreach (SequenceType type in Enum.GetValues(typeof(SequenceType)))
                {
                    if (list.Where(x => x.Type == type).Sum(x => x.Length) >= 4)
                    {
                        max = max < 9 ? 9 : max;
                    }
                    if (list.Where(x => x.Type == type).Sum(x => x.Length) == 3 && list.Where(x => x.Type == type).Any(x => x.AroundFreeCells.Count == 2))
                    {
                        max = max < 7 ? 7 : max;
                    }
                }

                if (list.Sum(x => x.Length) >= 4)
                {
                    max = max < 7 ? 7 : max;
                }

                return max;
            }).ToList();

            if (resultsP.Any())
            {
                if (resultsC.Any())
                {
                    return Math.Abs(resultsP.Min()) >= resultsC.Max() ? resultsP.Min() : resultsC.Max();
                }
                else
                {
                    return resultsP.Min();
                }
            }
            else if (resultsC.Any())
            {
                return resultsC.Max();
            }


            if (allP.Any(x => x.Length == 3 && x.AroundFreeCells.Count == 2)) return 7;
            if (allC.Any(x => x.Length == 3 && x.AroundFreeCells.Count == 2)) return -7;


            var other = all.OrderByDescending(x => x.Length).ThenBy(x => x.Number).FirstOrDefault();
            return other?.Length * other?.Number ?? 0;


            //            var all = FindAllSequences();
            //
            //            var allC = all.Where(x => x.Number == Computer).ToList();
            //            var allP = all.Where(x => x.Number == Player).ToList();
            //
            //            //5
            //            var resultC = allC.Count(x => x.Length >= 5) * 10;
            //            var resultP = allP.Count(x => x.Length >= 5) * 10;
            //
            //            //4
            //            resultC += allC.Count(x => x.Length == 4);
            //            resultP += allP.Count(x => x.Length == 4);
            //            resultC *= 10;
            //            resultP *= 10;
            //
            //            //3 + 2, 2x1
            //            resultC += allC.Count(x => x.Length == 3 && x.AroundFreeCells == 2);
            //            resultP += allP.Count(x => x.Length == 3 && x.AroundFreeCells == 2);
            //
            //            resultC *= 1000;
            //            resultP *= 1000;
            //            resultC += allC.Where(x => x.Length <= 2 || (x.Length == 3 && x.AroundFreeCells == 1)).Sum(x => (int)Math.Pow(10, x.Length - 1));
            //            resultP += allP.Where(x => x.Length <= 2 || (x.Length == 3 && x.AroundFreeCells == 1)).Sum(x => (int)Math.Pow(10, x.Length - 1));
            //
            //            return resultC - resultP;



            //            var all = FindAllSequences();
            //            var greaterThen3 = all.Where(x => x.Length >= 3 && x.Length + x.AroundFreeCells >= 5);
            //            if (greaterThen3.Any())
            //            {
            //                return greaterThen3.Aggregate(0,
            //                    (i, sequence) => i + (int) Math.Pow(10, sequence.Length - 2) * sequence.Number);
            //            }
            //            else
            //            {
            //                return all.Where(x=>x.AroundFreeCells == 2).Aggregate(0,
            //                    (i, sequence) => i + (int)Math.Pow(10, sequence.Length) * sequence.Number);
            //            }


        }

        public int RateLocal(Node node)
        {
            var left = GetLeft(node.X, node.Y) ?? new Sequence();
            var right = GetRight(node.X, node.Y) ?? new Sequence();
            var down = GetDown(node.X, node.Y) ?? new Sequence();
            var up = GetUp(node.X, node.Y) ?? new Sequence();
            var leftUp = GetLeftUp(node.X, node.Y) ?? new Sequence();
            var rightDown = GetRightDown(node.X, node.Y) ?? new Sequence();
            var leftDown = GetLeftDown(node.X, node.Y) ?? new Sequence();
            var rightUp = GetRightUp(node.X, node.Y) ?? new Sequence();

            var all = new List<Sequence> { left, right, down, up, leftUp, rightDown, leftDown, rightUp };

            var allC = all.Where(x => x.Number == Computer).ToList();
            var allP = all.Where(x => x.Number == Player).ToList();



            return 0;
        }


        private readonly List<int> _facts = new List<int> { 1, 2, 6, 24, 120 };
        //Rate whole grid after the move...
        public RatingResult Rate(Node node)
        {
            var all = FindAllSequences();
            int resC=0;
            int resP=0;
            int cCat = 0;
            int pCat = 0;
            foreach (var sequence in all)
            {
                if (sequence.Number == Computer)
                {
//                    resC += sequence.Length >= 5
//                        ? 2 * _facts[4]
//                        : sequence.AroundFreeCells.Count * _facts[sequence.Length];

                    int cat;
                    switch (sequence.Length)
                    {
                        case 1:
                            resC += sequence.AroundFreeCells.Count;
                            cat = 100 + sequence.AroundFreeCells.Count;
                            break;
                        case 2:
                            resC += 6 * sequence.AroundFreeCells.Count;
                            cat = 200 + sequence.AroundFreeCells.Count;
                            break;
                        case 3:
                            resC += 36 * sequence.AroundFreeCells.Count;
                            cat = 300 + sequence.AroundFreeCells.Count;
                            break;
                        case 4:
                            resC += 216 * sequence.AroundFreeCells.Count;
                            cat = 400 + sequence.AroundFreeCells.Count;
                            break;
                        default:
                            resC += 1296 * 2;
                            cat = 500 + sequence.AroundFreeCells.Count;
                            break;
                    }

                    if (cCat / 100 == cat / 100)
                    {
                        cCat += cat % 100;
                    } else if (cCat / 100 < cat / 100)
                    {
                        cCat = cat;
                    }
//                    if (sequence.Length > 0) resC += sequence.AroundFreeCells.Count;
//                    if (sequence.Length > 1) resC += 6* sequence.AroundFreeCells.Count;
//                    if (sequence.Length > 2) resC += 36* sequence.AroundFreeCells.Count;
//                    if (sequence.Length > 3) resC += 216* sequence.AroundFreeCells.Count;
//                    if (sequence.Length > 4) resC += 1296* 2;
                }
                else
                {
                    int cat;
                    switch (sequence.Length)
                    {
                        case 1:
                            resP += sequence.AroundFreeCells.Count;
                            cat = 100 + sequence.AroundFreeCells.Count;
                            break;
                        case 2:
                            resP+= 6 * sequence.AroundFreeCells.Count;
                            cat = 200 + sequence.AroundFreeCells.Count;
                            break;
                        case 3:
                            resP += 36 * sequence.AroundFreeCells.Count;
                            cat = 300 + sequence.AroundFreeCells.Count;
                            break;
                        case 4:
                            resP += 216 * sequence.AroundFreeCells.Count;
                            cat = 400 + sequence.AroundFreeCells.Count;
                            break;
                        default:
                            resP += 1296 * 2;
                            cat = 500 + sequence.AroundFreeCells.Count;
                            break;
                    }

                    if (sequence.AroundFreeCells.Count==0 && sequence.Length <5) continue;
                    if (pCat / 100 == cat / 100)
                    {
                        pCat += cat % 100;
                    }
                    else if (pCat / 100 < cat / 100)
                    {
                        pCat = cat;
                    }
                }
            }

            return new RatingResult{CCategory = cCat,CValue = resC,PCategory = pCat,PValue = resP,X = node.X,Y=node.Y};
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
            if (allC.Count(s => s.Length == 3 && s.AroundFreeCells.Count == 2) > 0) return resultC;
            //resultC--;

            //C can extend to four...2* 3 + 1f with 2f continue
            if (
                allC.Where(s => s.Length == 3 && s.AroundFreeCells.Count == 1)
                    .Sum(s => FreeCellsInDirection(x, y, s.Type).Count()) == 2) return resultC;
            resultC--;

            //C can extend to four...3+1f
            if (allC.Any(s => s.Length == 3)) return resultC;
            --resultC;

            //P to 4 can be blocked - seqs with same type and length >= 3 + 4f
            if (allO.GroupBy(s => s.Type).Any(g => g.Sum(s => s.Length) >= 3 && g.Sum(s => s.AroundFreeCells.Count) == 4)) return resultC;

            //P can extend to four...n* 3 + 2f
            if (allO.Count(s => s.Length == 3 && s.AroundFreeCells.Count == 2) > 0) return resultC+allO.Sum(s=>s.Length)/10.0;

            //P can extend to four...2* 3 + 1f with 2f continue
            if (
                allO.Where(s => s.Length == 3 && s.AroundFreeCells.Count == 1)
                    .Sum(s => FreeCellsInDirection(x, y, s.Type).Count()) == 2) return resultC;
            resultC--;

            //C can extend to four...3+1f
            if (allO.Any(s => s.Length == 3)) return resultC;
            --resultC;

            //C can extend to 3...n*2+2f
            if (allC.Any(s => s.Length == 2 && s.AroundFreeCells.Count == 2)) return resultC;

            //C can extend to 3---1+2f + 1+2f T
            if (
                allC.Where(s => s.Length == 1 && s.AroundFreeCells.Count == 2)
                    .GroupBy(s => s.Type)
                    .Any(g => g.Count() >= 2)) return resultC;
            --resultC;

            //P can extend to 3...n*2+2f
            if (allO.Any(s => s.Length == 2 && s.AroundFreeCells.Count == 2)) return resultC;

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

            //C can extend to 3--2+1f
            if (allC.Any(s => s.Length == 2)) return resultC;
            resultC--;

            //P can extend to 3--2+1f
            if (allO.Any(s => s.Length == 2)) return resultC;
            resultC--;

            //C can extend to 2...n*1+2f
            if (allC.Any(s => s.Length == 1 && s.AroundFreeCells.Count == 2)) return resultC;
            resultC--;

            //P can extend to 2...n*1+2f
            if (allO.Any(s => s.Length == 1 && s.AroundFreeCells.Count == 2)) return resultC;
            resultC--;

            //C can extend to 2...n*1+1f
            if (allC.Any(s => s.Length == 1)) return resultC;
            resultC--;

            //P can extend to 2...n*1+1f
            if (allO.Any(s => s.Length == 1)) return resultC;
            //resultC--;

            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<int, int>> GetAroundCells(int x, int y)
        {
            var list = new List<Tuple<int,int>>();
            for (int i = y-1; i <= y+1 && i < _size; i++)
            {
                if (i < 0) continue;
                for (int j = x-1; j < _size && j <= x+1; j++)
                {
                    if (j<0) continue;
                    if (i == y && j == x) continue;
                    if (_grid[i][j]!=0) continue;
                    list.Add(new Tuple<int, int>(j,i));
                }
            }
            return list;
        }

        public IEnumerable<Tuple<int, int>> FreeCellsInDirection(int x, int y, SequenceType type)
        {
            var list = GetAroundCells(x, y);
            switch (type)
            {
                case SequenceType.Horizontal:
                    list = list.Where(l => l.Item2 == y);
                    break;
                case SequenceType.Vertical:
                    list = list.Where(l => l.Item1 == x);
                    break;
                case SequenceType.TopDown:
                    list =list.Where(l => (l.Item1 == x + 1 && l.Item2 == y + 1) || (l.Item1 == x - 1 && l.Item2 == y - 1));
                    break;
                case SequenceType.BottomUp:
                    list = list.Where(l => (l.Item1 == x + 1 && l.Item2 == y - 1) || (l.Item1 == x - 1 && l.Item2 == y + 1));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return list;
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
