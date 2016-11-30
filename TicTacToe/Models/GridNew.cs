using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class GridNew
    {
        public const int Computer = 1;
        public const int Player = -1;

        private readonly int _size;
        private bool _nextMoveComputer;
        private readonly List<Sequence> _sequencesC = new List<Sequence>();
        private readonly List<Sequence> _sequencesP = new List<Sequence>();
        private int _moves;

        public GridNew(int size, bool computerFirst)
        {
            _size = size;
            _nextMoveComputer = computerFirst;
        }

        public GridNew Add(int x, int y)
        {
            var cur = _nextMoveComputer ? _sequencesC : _sequencesP;
            var enemy = _nextMoveComputer ? _sequencesP : _sequencesC;

            var ps = enemy.Where(s => s.AroundFreeCells.Any(f => f.Item1 == x && f.Item2 == y)).ToList();
            var cs = cur.Where(s => s.AroundFreeCells.Any(f => f.Item1 == x && f.Item2 == y)).ToList();

            foreach (var group in cs.GroupBy(s => s.Type))
            {
                if (group.Count() != 2) continue;

                var newSeq = Sequence.Connect(x, y, group.ToList());
                cur.RemoveAll(s => group.Contains(s));
                cur.Add(newSeq);
            }

            foreach (var sequence in cs)
            {
                int stepX;
                int stepY;
                switch (sequence.Type)
                {
                    case SequenceType.Horizontal:
                        stepX = 1;
                        stepY = 0;
                        break;
                    case SequenceType.Vertical:
                        stepX = 0;
                        stepY = 1;
                        break;
                    case SequenceType.TopDown:
                        stepX = 1;
                        stepY = 1;
                        break;
                    case SequenceType.BottomUp:
                        stepX = 1;
                        stepY = -1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                int nX = x + stepX == sequence.X1 || x + stepX == sequence.X2 ? x - stepX : x + stepX;
                int nY = nX == x + stepX ? y + stepY : y - stepY;

                bool start = nX == x - stepX && nY == y - stepY;

                if (ps.Any(s => s.Type == sequence.Type))
                {
                    sequence.Add(x, y,start);
                }
                else
                {
                    sequence.Add(x, y,start, new Tuple<int, int>(nX, nY));
                }

                
            }

            cur.AddRange(from SequenceType type in Enum.GetValues(typeof(SequenceType)) where cs.All(s => s.Type != type) select Sequence.New(x, y, _nextMoveComputer ? Computer : Player, type, ps.Where(s => s.Type == type)));

            foreach (var seq in ps)
            {
                seq.Block(x, y);
            }

            ++_moves;
            _nextMoveComputer = !_nextMoveComputer;
            return this;
        }

        public List<Node> GetPossibleMoves()
        {
            var moves = _sequencesC.Concat(_sequencesP).SelectMany(x => x.AroundFreeCells).GroupBy(s=>new {s.Item1,s.Item2}).Select(g=>g.First()).Select(t=>new Node{X = t.Item1,Y= t.Item2}).ToList();
            return moves.Any() ? moves : new List<Node> {new Node {X = _size / 2, Y = _size / 2}};
        }

        public GridNew Clone()
        {
            var grid = new GridNew(_size, _nextMoveComputer) {_moves = _moves};
            grid._sequencesC.AddRange(_sequencesC.Select(s => s.Clone()));
            grid._sequencesP.AddRange(_sequencesP.Select(s => s.Clone()));
            return grid;
        }

        public List<Sequence> GetAllSequences()
        {
            return _sequencesC.Concat(_sequencesP).ToList();
        }
    }
}
