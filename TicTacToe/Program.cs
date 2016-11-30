using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Models;

namespace TicTacToe
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var size = Convert.ToInt32(Console.ReadLine());
            var computerTurn = Console.ReadLine().ToUpper() == "X";

            bool computer = false;
            bool draw = false;
            var grid = new Grid(size, computerTurn);
            var grid2 = new Grid(size, !computerTurn);

            for (;;)
            {
                if (computerTurn)
                {
                    var move = NextMoveNew2(grid);
                    if (move == null)
                    {
                        draw = true;
                        break;
                    }
                    grid.Add(move.X, move.Y);
                    grid2.Add(move.X, move.Y);
                    Debug.WriteLine($"{move.X} {move.Y}");
                }
                else
                {
                    //var split = Console.ReadLine().Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
                    //if (split[0] < 0 || split[1] < 0) return;
                    //grid.Add(split[0], split[1]);

                    var move = NextMoveNew2(grid2);
                    if (move == null)
                    {
                        draw = true;
                        break;
                    }
                    grid2.Add(move.X, move.Y);
                    grid.Add(move.X, move.Y);
                    Debug.WriteLine($"{move.X} {move.Y}");
                }

                grid.DrawGrid();

                var seqs = grid.FindAllSequences();
                if (seqs.Any(x => x.Length >= 5))
                {
                    var s = seqs.Find(x => x.Length >= 5);
                    computer = s.Number == Grid.Computer;
                    break;
                }

                computerTurn = !computerTurn;
            }

            if (draw)
            {
                Debug.WriteLine("Draw");
            }
            else
            {
                Debug.WriteLine(computer ? "Computer wins!" : "Player wins.");
            }
        }

        private static Random _rand = new Random();
        private static Node NextMoveNew2(Grid grid)
        {
            var all = grid.FindAllSequences();
            var moves = grid.GetPossibleMoves();
            var ratings = moves.Select(x => new {X = x.X, Y = x.Y, Value = grid.Rate(x.X, x.Y, all)}).ToList();
            var max = ratings.Max(x => x.Value);
            var best = ratings.Where(x => Math.Abs(x.Value - max) < 0.01).ToList();
            int index = _rand.Next(0, best.Count);
            return new Node {X = best[index].X, Y = best[index].Y};
        }

        private static Node NextMoveNew(Grid grid)
        {
            int deb = 0;
            var all = grid.FindAllSequences();
            var moves = grid.GetPossibleMoves();

            if (all.Count == 0)
            {
                return moves.FirstOrDefault();
            }

            var allC = all.Where(x => x.AroundFreeCells.Any() && x.Number == Grid.Computer).ToList();
            var allP = all.Where(x => x.AroundFreeCells.Any() && x.Number == Grid.Player).ToList();

            //Console.WriteLine(++deb);

            //Computer can win (has 4)
            var five = allC.FirstOrDefault(x => x.Length == 4);
            if (five != null)
            {
                return new Node {X = five.AroundFreeCells.First().Item1, Y = five.AroundFreeCells.First().Item2};
            }

            //Console.WriteLine(++deb);
            //Computer has more sequences of the same type of lenght >= 4
            var withGap =
                allC.SelectMany(
                        x => new List<Sequence> {x, x.Clone().Block(x.AroundFreeCells[0].Item1, x.AroundFreeCells[0].Item2)})
                    .Where(x => x.AroundFreeCells.Count >= 1).GroupBy(x=>new{x.AroundFreeCells[0].Item1,x.AroundFreeCells[0].Item2,x.Type}).FirstOrDefault(x=>x.Count()>1 && x.Sum(a=>a.Length) >= 4);
            if (withGap != null)
            {
                return new Node {X = withGap.Key.Item1, Y = withGap.Key.Item2};
            }

            //Console.WriteLine(++deb);
            //Blocking player's 4
            var four = allP.FirstOrDefault(x => x.Length == 4);
            if (four != null)
            {
                //select better spot
                if (four.AroundFreeCells.Count == 1)
                {
                    return new Node {X = four.AroundFreeCells[0].Item1, Y = four.AroundFreeCells[0].Item2};
                }
                else //unnecessary -> player will win next move
                {   
                    var onThis1 = all.Where(a => a.AroundFreeCells.Any(x =>(x.Item1 == four.AroundFreeCells[0].Item1 && x.Item2 == four.AroundFreeCells[0].Item2)));
                    var onThis2 = all.Where(a => a.AroundFreeCells.Any(x =>(x.Item1 == four.AroundFreeCells[1].Item1 && x.Item2 == four.AroundFreeCells[1].Item2)));

                    if (onThis1.Count() >= onThis2.Count())
                    {
                        return new Node {X = four.AroundFreeCells[0].Item1, Y = four.AroundFreeCells[0].Item2};
                    }
                    else
                    {
                        return new Node { X = four.AroundFreeCells[1].Item1, Y = four.AroundFreeCells[1].Item2 };
                    }
                }
            }

            //Console.WriteLine(++deb);
            //Player has more sequences of the same type of lenght >= 4
            var withGapP =
                allP.SelectMany(
                        x => new List<Sequence> { x, x.Clone().Block(x.AroundFreeCells[0].Item1, x.AroundFreeCells[0].Item2) })
                    .Where(x => x.AroundFreeCells.Count >= 1).GroupBy(x => new { x.AroundFreeCells[0].Item1, x.AroundFreeCells[0].Item2, x.Type }).FirstOrDefault(x => x.Count() > 1 && x.Sum(a => a.Length) >= 4);
            if (withGapP != null)
            {
                return new Node { X = withGapP.Key.Item1, Y = withGapP.Key.Item2 };
            }

            //Console.WriteLine(++deb);
            //Computer has 2+2f && 1+2f in same dir
            var two2One2 = moves.Select((x, i)=>
                        allC.Where(
                            a =>
                                ((a.Length == 2 && a.AroundFreeCells.Count == 2) ||
                                 (a.Length == 1 && a.AroundFreeCells.Count == 2)) &&
                                a.AroundFreeCells.Any(c => c.Item1 == x.X && c.Item2 == x.Y)).GroupBy(s=>new{s.Type,Index =i}).ToList()
                    ).FirstOrDefault(x => x.Any(a=>a.Sum(c=>c.Length) >= 3));
            if (two2One2 != null)
            {
                return new Node {X = moves[two2One2.First().Key.Index].X, Y = moves[two2One2.First().Key.Index].Y};
            }

            //Console.WriteLine(++deb);
            //Player has 2+2f && 1+2f in same dir
            var two2One2P = moves.Select((x, i) =>
                        allP.Where(
                            a =>
                                ((a.Length == 2 && a.AroundFreeCells.Count == 2) ||
                                 (a.Length == 1 && a.AroundFreeCells.Count == 2)) &&
                                a.AroundFreeCells.Any(c => c.Item1 == x.X && c.Item2 == x.Y)).GroupBy(s => new { s.Type, Index = i }).ToList()
                    ).FirstOrDefault(x => x.Any(a => a.Sum(c => c.Length) >= 3));
            if (two2One2P != null)
            {
                return new Node { X = moves[two2One2P.First().Key.Index].X, Y = moves[two2One2P.First().Key.Index].Y };
            }


            //Console.WriteLine(++deb);
            //Computer has 3 +2f
            var three2 = allC.FirstOrDefault(x => x.Length == 3 && x.AroundFreeCells.Count == 2);
            if (three2 != null)
            {
                return new Node { X = three2.AroundFreeCells[0].Item1, Y = three2.AroundFreeCells[0].Item2 };
            }

            //Console.WriteLine(++deb);
            //Computer has 3+1 && 2+2
            var three21 = moves.Select(
                (x,i) => new { Items =
                    allC.Where(
                        a =>
                            a.AroundFreeCells.Any(c => c.Item1 == x.X && c.Item2 == x.Y) &&
                            ((a.Length == 2 && a.AroundFreeCells.Count == 2) || (a.Length == 3))), Index = i }).Where(x=>x.Items.Count() >= 2).OrderByDescending(x=>x.Items.Sum(a=>a.Length)).FirstOrDefault();
            if (three21 != null)
            {
                return new Node {X = moves[three21.Index].X, Y = moves[three21.Index].Y};
            }

            //Console.WriteLine(++deb);
            //Player has 3+2 -- find better spot
            var three2P =
                allP.Where(x => x.Length == 3 && x.AroundFreeCells.Count == 2)
                    .SelectMany(x => x.AroundFreeCells)
                    .OrderByDescending(
                        x =>
                            allP.Where(a => a.AroundFreeCells.Any(c => c.Item1 == x.Item1 && c.Item2 == x.Item2))
                                .Sum(a => a.Length))
                    .FirstOrDefault();
            if (three2P != null)
            {
                return new Node {X = three2P.Item1, Y = three2P.Item2};
            }

            //Console.WriteLine(++deb);
            //Player has 3+1 && 2+2
            var three21P = moves.Select(
                (x, i) => new {
                    Items =
                    allP.Where(
                        a =>
                            a.AroundFreeCells.Any(c => c.Item1 == x.X && c.Item2 == x.Y) &&
                            ((a.Length == 2 && a.AroundFreeCells.Count == 2) || (a.Length == 3))),
                    Index = i
                }).Where(x => x.Items.Count() >= 2).OrderByDescending(x => x.Items.Sum(a => a.Length)).FirstOrDefault();
            if (three21P != null)
            {
                return new Node { X = moves[three21P.Index].X, Y = moves[three21P.Index].Y };
            }

            //Console.WriteLine(++deb);
            //Computer has 2* 2+2
            var two22 =
                moves.Select(
                        (m, i) =>
                            new
                            {
                                Items =
                                allC.Where(
                                    x =>
                                        (x.Length == 2 && x.AroundFreeCells.Count == 2) &&
                                        x.AroundFreeCells.Any(a => a.Item1 == m.X && a.Item2 == m.Y)),
                                Index = i
                            })
                    .Where(x => x.Items.Count() >= 2)
                    .OrderByDescending(x => x.Items.Sum(a => a.Length))
                    .FirstOrDefault();
            if (two22 != null)
            {
                return new Node{X=moves[two22.Index].X, Y = moves[two22.Index].Y};
            }

            //Console.WriteLine(++deb);
            //Computer has 3+1...
            var three1 = allC.FirstOrDefault(x => x.Length == 3 && x.AroundFreeCells.Count == 1);
            if (three1 != null)
            {
                return new Node {X = three1.AroundFreeCells[0].Item1, Y = three1.AroundFreeCells[0].Item2};
            }

            //Console.WriteLine(++deb);
            //Player has 3+1
            var three1P = allP.FirstOrDefault(x => x.Length == 3 && x.AroundFreeCells.Count == 1);
            if (three1P != null)
            {
                return new Node { X = three1P.AroundFreeCells[0].Item1, Y = three1P.AroundFreeCells[0].Item2 };
            }

            //Console.WriteLine(++deb);
            //Computer has 2+2
            var two2 =
                allC.Where(x => x.Length == 2 && x.AroundFreeCells.Count == 2)
                    .OrderByDescending(
                        x => x.AroundFreeCells.Sum(c => grid.FreeCellsInDirection(c.Item1, c.Item2, x.Type).Count())).FirstOrDefault();
            if (two2 != null)
            {
                return two2.AroundFreeCells.OrderByDescending(
                    x => grid.FreeCellsInDirection(x.Item1, x.Item2, two2.Type).Count()).Select(x=>new Node{X= x.Item1, Y=x.Item2}).First();
            }

            //Console.WriteLine(++deb);
            //TODO: finish later
            //Rest
            return all.Where(x=>x.AroundFreeCells.Count > 0).OrderByDescending(x => x.Length * 2 + x.AroundFreeCells.Count).Select(x=>new Node{X=x.AroundFreeCells[0].Item1,Y=x.AroundFreeCells[0].Item2}).FirstOrDefault();

        }

    }
}
