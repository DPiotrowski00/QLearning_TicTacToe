using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TicTacToeSolver
{
    public class Move
    {
        public string code = "";

        public Move(int marker, string board, int[] position)
        {
            code += marker + "_" + board + "_" + position[0] + position[1];
            foreach (var p in position)
            {
                code += p;
            }
        }
    }

    public class AIAgent(string name, int marker) : Player(name, marker)
    { 
        private readonly Random random = new();
        private readonly Dictionary<string, double> qMap = [];
        private readonly List<Move> _moves = [];

        private double epsilon = 1.0d;
        private readonly double epsilonMin = 0.01d;
        private readonly double epsilonDecay = 0.99995d;

        public void DecayEpsilon()
        {
            epsilon = Math.Max(epsilonMin, epsilon * epsilonDecay);
        }

        public void MakeAMove(ref Board board)
        {
            if (Random.Shared.NextDouble() < epsilon)
            {
                MakeARandomMove(ref board);
            }
            else
            {
                MakeAnEducatedMove(ref board);
            }
        }

        public void MakeARandomMove(ref Board board)
        {
            while (true)
            {
                int[] position = [(int)random.NextInt64(0, board.GetSize()), (int)random.NextInt64(0, board.GetSize())];

                Move move = new(this.Marker, board.EncodeBoard(), position);

                if (board.TryPlacingMarker(this.Marker, position))
                {
                    if (!qMap.TryGetValue(move.code, out _))
                    {
                        qMap.Add(move.code, 0);
                    }

                    _moves.Add(move);
                    return;
                }
                else
                {
                    continue;
                }
            }
        }

        public void MakeAnEducatedMove(ref Board board)
        {
            string codeStart = this.Marker + "_" + board.EncodeBoard() + "_";

            var buffer = qMap.Where(q => q.Key.StartsWith(codeStart));
            if (buffer.Any())
            {
                var bestMoveCode = buffer.MaxBy(m => m.Value).Key;

                var bestMove = bestMoveCode[^2..];
                int[] move = [Int32.Parse(bestMove[0].ToString()), Int32.Parse(bestMove[1].ToString())];
                board.TryPlacingMarker(this.Marker, move);
            }
            else
            {
                MakeARandomMove(ref board);
            }
        }

        public void ApplyRewards(double value)
        {
            for (int i = 0; i < _moves.Count; i++)
            {
                double weight = (double)(i + 1) / _moves.Count;
                qMap[_moves[i].code] += (int)(value * weight);
                qMap[_moves[i].code] = Math.Clamp(qMap[_moves[i].code], -10, 10);
            }

            _moves.Clear();
        }
    }
}