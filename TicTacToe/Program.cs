using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var grid = new Grid(size,computerTurn);
            var ai =new Ai();

            var ai2 = new Ai();
            var grid2 = new Grid(size, !computerTurn);
            for (;;)
            {
                if (computerTurn)
                {
                    var move = ai.NextMove(grid);
                    if (move == null) return;
                    grid.Add(move.X, move.Y);
                    grid2.Add(move.X, move.Y);
                    Debug.WriteLine($"{move.X} {move.Y}");
                }
                else
                {
                    //var split = Console.ReadLine().Split(' ').Select(x=>Convert.ToInt32(x)).ToArray();
                    //if (split[0]<0 || split[1] <0) return;
                    //grid.Add(split[0],split[1]);

                    var move = ai2.NextMove(grid2);
                    if (move == null) return;
                    grid2.Add(move.X, move.Y);
                    grid.Add(move.X, move.Y);
                    Debug.WriteLine($"{move.X} {move.Y}");
                }

                grid.DrawGrid();

                var seqs = grid.FindAllSequences();
                if (seqs.Any(x => x.Length >= 5)) break;

                computerTurn = !computerTurn;

                //Console.ReadLine();
            }


            Console.ReadLine();
        }

    }
}
