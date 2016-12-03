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

            Ai ai = new Ai();
            Ai ai2 = new Ai();


            int c = 0;
            int d = 0;
            int p = 0;
            for (int i=0;i<1;++i)
            {
                var starts = computerTurn;
                computer = false;
                draw = false;
                grid = new Grid(size, computerTurn);
                grid2 = new Grid(size, !computerTurn);

                for (int t=0;;t++)
                {
                    if (computerTurn)
                    {
                        var bla = t== 0? new Tuple<int,int>(size/2,size/2) :(grid.RateGrid(true));
                        var move = new Node() {X = bla.Item1, Y=bla.Item2};
                        //var move = NextMoveNew2(grid);
                        //var move = ai.NextMove(grid);
                        if (move == null)
                        {
                            draw = true;
                            break;
                        }
                        grid.Add(move.X, move.Y);
                        grid2.Add(move.X, move.Y);
                       Console.WriteLine($"{move.X} {move.Y}");
                    }
                    else
                    {
//                                                var split = Console.ReadLine().Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
//                                                if (split[0] < 0 || split[1] < 0) return;
//                                                grid.Add(split[0], split[1]);

                        var bla = t == 0 ? new Tuple<int, int>(size / 2, size / 2) : (grid2.RateGrid(true));
                        var move = new Node() { X = bla.Item1, Y = bla.Item2 };
                        //var move = NextMoveNew2(grid2);
                        //var move = ai2.NextMove(grid2);
                        if (move == null)
                        {
                            draw = true;
                            break;
                        }
                        grid2.Add(move.X, move.Y);
                        grid.Add(move.X, move.Y);
                        Console.WriteLine($"{move.X} {move.Y}");
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
                    d++;
                    //Console.WriteLine("Draw");
                }
                else
                {
                    c += (computer && starts) || (!computer && !starts) ? 1 : 0;
                    p += (computer && !starts) || (!computer && starts) ? 1 : 0;
                    //Console.WriteLine(computer ? "Computer wins!" : "Player wins.");
                }
                Debug.WriteLine($"{i}: {c} / {d} / {p} sC:{starts}");
            }

            Console.ReadLine();
        }
    }
}
