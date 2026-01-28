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
    public class Move(int marker, string board, int[] position)
    {
        public string code = marker + "_" + board + "_" + position[0] + position[1];
    }

    public class AIAgent(string name, int marker) : Player(name, marker)
    { 
        private readonly Random random = new();
        private readonly Dictionary<string, Dictionary<string, int>> qMap = [];
        private readonly List<Move> _moves = [];

        private double epsilon = 1.0d;
        private double epsilonMin = 0.01d;
        private double epsilonDecay = 0.99995d;

        public void DecayEpsilon()
        {
            epsilon = Math.Max(epsilonMin, epsilon * epsilonDecay);
        }

        public void MakeAMove(ref Board board, out int row, out int col)
        {
            if (Random.Shared.NextDouble() < epsilon)
            {
                MakeARandomMove(ref board, out row, out col);
            }
            else
            {
                MakeAnEducatedMove(ref board, out row, out col);
            }
        }

        public void MakeARandomMove(ref Board board, out int row, out int col)
        {            
            while (true)
            {
                row = (int)random.NextInt64(0, board.GetSize());
                col = (int)random.NextInt64(0, board.GetSize());


                int[] position = [row, col];

                Move move = new(this.Marker, board.EncodeThisBoard(), position);

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

        public void MakeAnEducatedMove(ref Board board, out int row, out int col)
        {
            string codeStart = this.Marker + "_" + board.EncodeThisBoard() + "_";
            
            var buffer = qMap.Where(q => q.Key.StartsWith(codeStart));
            if (buffer.Any())
            {
                var bestMoveCode = buffer.MaxBy(m => m.Value).Key;

                var bestMove = bestMoveCode[^2..];
                int[] move = [Int32.Parse(bestMove[0].ToString()), Int32.Parse(bestMove[1].ToString())];
                board.TryPlacingMarker(this.Marker, move);
                row = move[0];
                col = move[1];
            }
            else
            {
                MakeARandomMove(ref board, out row, out col);
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