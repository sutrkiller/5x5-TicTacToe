using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Models;

namespace TicTacToe
{
    public class Ai
    {
        private const int DefaultDepth = 4;

        private int _cutted=0;
        private int _counted=0;

        public Node NextMove(Grid grid, int depth = DefaultDepth)
        {
            _cutted = 0;
            _counted = 0;

            var possibleMoves = grid.GetPossibleMoves();
            if (possibleMoves.Count == 0) return null;
            foreach (var move in possibleMoves)
            {
                move.IsMaxNode = false;
                NextMove(grid, move, depth);
            }

            Debug.WriteLine("Cutted: " +_cutted);
            Debug.WriteLine("Counted: "+_counted);
            Debug.WriteLine("Counted/Cutted: "+_counted/(double)_cutted);
//            var max = possibleMoves.Max(x => x.Value);
//            return possibleMoves.FirstOrDefault(x => x.Value == max);

            //var ch = possibleMoves.SelectMany(x => x.Children).SelectMany(x => x.Children).SelectMany(x => x.Children).SelectMany(x=>x.Children).ToList();
            var max = possibleMoves.Max(x => x.Value);
            var min = possibleMoves.Min(x => x.Value);
            var all = possibleMoves.Where(x => x.Value == max).ToList();
            return possibleMoves.Aggregate((a, b) => a.Value >= b.Value ? a : b);
        }

        private Node NextMove(Grid grid, Node node, int depth = DefaultDepth)
        {
            if (node == null) return null;

            var gridClone = grid.Clone();
            gridClone.Add(node.X, node.Y);
            var nextMoves = gridClone.GetPossibleMoves();

            if (depth == 0 || !nextMoves.Any())
            {
                ++_counted;
                node.Value = grid.RateGrid(node);
                return node;
            }
            

            if (node.IsMaxNode)
            {
                node.Value = int.MinValue;

                //foreach (var nextMove in nextMoves.OrderByDescending(x=>x.PlayerNeighbours + x.ComputerNeighbours))
                foreach (var nextMove in nextMoves.OrderByDescending(x=>gridClone.Clone().Add(x.X,x.Y).RateGrid(x)))
                {
                    node.Children.Add(nextMove);


                    grid.RateLocal(nextMove);

                    nextMove.Max = node.Max;
                    nextMove.Min = node.Min;
                    nextMove.IsMaxNode = false;
                    node.Value = Math.Max(node.Value, NextMove(gridClone, nextMove, depth - 1).Value);
                    node.Min = Math.Max(node.Min, node.Value);
                    if (node.Max <= node.Min)
                    {
                        ++_cutted;
                        break;
                    }
                }
                return node;
            }
            else
            {
                node.Value = int.MaxValue;

                //foreach (var nextMove in nextMoves.OrderByDescending(x=>x.ComputerNeighbours + x.PlayerNeighbours))
                foreach (var nextMove in nextMoves.OrderBy(x => gridClone.Clone().Add(x.X, x.Y).RateGrid(x)))
                {
                    node.Children.Add(nextMove);

                    nextMove.Max = node.Max;
                    nextMove.Min = node.Min;
                    nextMove.IsMaxNode = true;
                    node.Value = Math.Min(node.Value, NextMove(gridClone, nextMove, depth - 1).Value);
                    node.Max = Math.Min(node.Max, node.Value);
                    if (node.Max <= node.Min)
                    {
                        ++_cutted;
                        break;
                    }
                }
                return node;
            }
        }
    }
}
