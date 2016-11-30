using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class Sequence
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public List<Tuple<int, int>> AroundFreeCells { get; set; } = new List<Tuple<int, int>>();
        public int Number { get; set; }
        public int Length { get; set; }
        public SequenceType Type { get; set; }

        public Sequence Clone()
        {
            return new Sequence()
            {
                X1 = X1,
                X2 =X2,
                Y1 = Y1,
                Y2 = Y2,
                Number = Number,
                Length = Length,
                Type = Type,
                AroundFreeCells = AroundFreeCells.Select(c=>new Tuple<int,int>(c.Item1,c.Item2)).ToList()
            };
        }

        public IEnumerable<Tuple<int, int>> EnumerateCells()
        {
            var xChange = (X2 - X1) / Length;
            var yChange = (Y2 - Y1) / Length;

            for (int x = X1, y = Y1; x < X2 && y < Y2; x += xChange, y += yChange)
            {
                yield return new Tuple<int, int>(x, y);
            }
        }

        public Sequence Block(int x, int y)
        {
            var fc = AroundFreeCells.FirstOrDefault(t => t.Item1 == x && t.Item2 == y);
            AroundFreeCells.Remove(fc);
            return this;
        }

        public static Sequence Connect(int x, int y, List<Sequence> list)
        {
            if (list.Count != 2) return null;
            var cells =
                list[0].EnumerateCells()
                    .Concat(list[1].EnumerateCells())
                    .OrderBy(s => s.Item1)
                    .ThenBy(s => s.Item2)
                    .ToList();

            Sequence newSeq = new Sequence
            {
                Length = list[0].Length + list[1].Length + 1,
                Number = list[0].Number,
                X1 = cells.First().Item1,
                Y1 = cells.First().Item2,
                X2 = cells.Last().Item1,
                Y2 = cells.Last().Item2,
                Type = list[0].Type,
                AroundFreeCells =
                    list[0].AroundFreeCells.Concat(list[1].AroundFreeCells)
                        .Where(s => s.Item1 != x && s.Item2 != y)
                        .ToList()
            };
            return newSeq;
        }

        public Sequence Add(int x, int y, bool start, Tuple<int, int> freeCell = null)
        {
            Block(x, y);
            ++Length;
            if (start)
            {
                X1 = x;
                Y1 = y;
            }
            else
            {
                X2 = x;
                Y2 = y;
            }

            if (freeCell != null)
            {
                AroundFreeCells.Add(freeCell);
            }
            return this;
        }

        public static Sequence New(int x, int y, int number, SequenceType type, IEnumerable<Sequence> neighbours)
        {
            var freeCells = new List<Tuple<int, int>>();
            switch (type)
            {
                case SequenceType.Horizontal:
                    freeCells.Add(new Tuple<int, int>(x - 1, y));
                    freeCells.Add(new Tuple<int, int>(x + 1, y));
                    break;
                case SequenceType.Vertical:
                    freeCells.Add(new Tuple<int, int>(x, y - 1));
                    freeCells.Add(new Tuple<int, int>(x, y + 1));
                    break;
                case SequenceType.TopDown:
                    freeCells.Add(new Tuple<int, int>(x + 1, y + 1));
                    freeCells.Add(new Tuple<int, int>(x - 1, y - 1));
                    break;
                case SequenceType.BottomUp:
                    freeCells.Add(new Tuple<int, int>(x - 1, y + 1));
                    freeCells.Add(new Tuple<int, int>(x + 1, y - 1));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return new Sequence
            {
                AroundFreeCells = freeCells.Where(
                c => !neighbours.Any(n => (n.X1 == c.Item1 && n.Y1 == c.Item2) || (n.X2 == c.Item1 && n.Y2 == c.Item2))).ToList(),
                Length = 1,
                Number = number,
                X1 = x,
                X2 = x,
                Y1 = y,
                Y2 = y,
                Type = type
            };

        }
    }
}