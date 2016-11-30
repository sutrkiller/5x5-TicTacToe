using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using TicTacToe.Models;

namespace TicTacToe
{
    public class Ai
    {
        private const int DefaultDepth = 4;

        private int _cutted = 0;
        private int _counted = 0;

        public Node NextMove(Grid grid, int depth = DefaultDepth)
        {
            _cutted = 0;
            _counted = 0;


            var node = new Node() { IsMaxNode = false };
            NextMove(grid, node, depth, true);
            var max = node.Children.Max(x => x.Value);
            var best = node.Children.FirstOrDefault(x => x.Value == max);
            Debug.Assert(best != null);
            return best;

            //            var possibleMoves = grid.GetPossibleMoves();
            //            if (possibleMoves.Count == 0) return null;
            //            foreach (var move in possibleMoves)
            //            {
            //                move.IsMaxNode = false;
            //                NextMove(grid, move, depth);
            //            }

            //            Debug.WriteLine("Cutted: " +_cutted);
            //            Debug.WriteLine("Counted: "+_counted);
            //            Debug.WriteLine("Counted/Cutted: "+_counted/(double)_cutted);
            ////            var max = possibleMoves.Max(x => x.Value);
            ////            return possibleMoves.FirstOrDefault(x => x.Value == max);

            //            //var ch = possibleMoves.SelectMany(x => x.Children).SelectMany(x => x.Children).SelectMany(x => x.Children).SelectMany(x=>x.Children).ToList();
            //            var max = possibleMoves.Max(x => x.Value);
            //            var min = possibleMoves.Min(x => x.Value);
            //            var all = possibleMoves.Where(x => x.Value == max).ToList();
            //            return possibleMoves.Aggregate((a, b) => a.Value >= b.Value ? a : b);
        }

        private Node NextMove(Grid grid, Node node, int depth = DefaultDepth, bool first = false)
        {
            if (node == null) return null;

            if (depth == 0)
            {
                ++_counted;
         //       node.Value = grid.Rate(node.X, node.Y, grid.FindAllSequences());
                return node;
            }

            var gridClone = grid.Clone();
            if (!first) gridClone.Add(node.X, node.Y);
            var nextMoves = gridClone.GetPossibleMoves();
            var allSeqs = gridClone.FindAllSequences();

            

            if (node.IsMaxNode)
            {
                node.Value = int.MinValue;
                foreach (var nextMove in nextMoves.OrderBy(x => gridClone.Rate(x.X, x.Y, allSeqs)))
                {
                    node.Children.Add(nextMove);

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

                foreach (var nextMove in nextMoves.OrderByDescending(x=>gridClone.Rate(x.X,x.Y,allSeqs)))
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

        private Node NextMoveN(Grid grid, Node node, int depth = DefaultDepth, bool first = false)
        {
            if (node == null) return null;

            var gridClone = grid.Clone();
            if (!first) gridClone.Add(node.X, node.Y);
            var nextMoves = gridClone.GetPossibleMoves();

            if (depth == 0 || !nextMoves.Any())
            {
                ++_counted;
                node.ValueN = grid.Rate(node);
                return node;
            }

            if (node.IsMaxNode)
            {
                node.ValueN = null;
                foreach (var nextMove in nextMoves)
                {
                    node.Children.Add(nextMove);

                    nextMove.MaxN = node.MaxN;
                    nextMove.MinN = node.MinN;
                    nextMove.IsMaxNode = false;
                    node.ValueN = Max(node.ValueN, NextMoveN(gridClone, nextMove, depth - 1).ValueN);
                    node.MinN = Max(node.MinN, node.ValueN);
                    if (node.MinN != null && node.MaxN != null && Max(node.MinN, node.MaxN) ==node.MinN)
                    {
                        ++_cutted;
                        break;
                    }
                }
                return node;
            }
            else
            {
                node.ValueN = null;

                foreach (var nextMove in nextMoves)
                {
                    node.Children.Add(nextMove);

                    nextMove.MaxN = node.MaxN;
                    nextMove.MinN = node.MinN;
                    nextMove.IsMaxNode = true;
                    var value = NextMoveN(gridClone, nextMove, depth - 1).ValueN;
                    node.ValueN = Min(value, node.ValueN);
                    node.MaxN = Min(node.MaxN, node.ValueN);
                    if (node.MinN != null && node.MaxN != null && Min(node.MaxN,node.MinN)==node.MaxN)
                    {
                        ++_cutted;
                        break;
                    }
                }
                return node;
            }
        }

        private static RatingResult Max(RatingResult first, RatingResult second)
        {
            if (first == null) return second;
            if (second == null) return first;
            var listTmp = new List<RatingResult> {first, second};

            var betterCat = listTmp.Where(x => x.CCategory / 100 > x.PCategory / 100).ToList();
            if (betterCat.Any())
                return betterCat.OrderByDescending(x => x.CValue).ThenBy(x => x.PValue).First();

            return
                listTmp.OrderBy(x => x.PCategory / 100)
                    .ThenBy(x => x.PValue)
                    .ThenByDescending(x => x.CValue)
                    .FirstOrDefault();
        }

        private static RatingResult Min(RatingResult first, RatingResult second)
        {
            if (first == null) return second;
            if (second == null) return first;
            return Max(first, second) == first ? second : first;
        }
    }
}
