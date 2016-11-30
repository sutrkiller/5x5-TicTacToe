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
        }

        private Node NextMove(Grid grid, Node node, int depth = DefaultDepth, bool first = false)
        {
            if (node == null) return null;

            if (depth == 0)
            {
                ++_counted;
                //node.Value = grid.Rate(node.X, node.Y, grid.FindAllSequences());
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
    }
}
