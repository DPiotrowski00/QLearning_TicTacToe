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

        public Move(BoardStates marker, string board, int[] position)
        {
            switch (marker)
            {
                case BoardStates.X:
                    code += "X";
                    break;
                case BoardStates.O:
                    code += "O";
                    break;
            }
            code += "_";
            code += board;
            code += "_";
            foreach (var p in position)
            {
                code += p;
            }
        }
    }

    public class AIAgent(string name, BoardStates marker) : Player(name, marker)
    { 
        private readonly Random random = new();
        private readonly Dictionary<string, int> qMap = [];
        private readonly List<Move> _moves = [];

        private double epsilon = 1.0d;
        private double epsilonMin = 0.01d;
        private double epsilonDecay = 0.99995d;

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
            string codeStart = "";
            switch (this.Marker)
            {
                case BoardStates.X:
                    codeStart += "X";
                    break;
                case BoardStates.O:
                    codeStart += "O";
                    break;
            }
            codeStart += "_";
            codeStart += board.EncodeBoard();
            codeStart += "_";

            var buffer = qMap.Where(q => q.Key.StartsWith(codeStart)).OrderByDescending(q => q.Value);
            if (buffer.Any())
            {
                var bestMoveCode = buffer.First().Key;

                var bestMove = bestMoveCode[^2..];
                int[] move = [Int32.Parse(bestMove[0].ToString()), Int32.Parse(bestMove[1].ToString())];
                board.TryPlacingMarker(this.Marker, move);
            }
            else
            {
                MakeARandomMove(ref board);
            }
            
        }

        public void ApplyRewards(int value)
        {
            foreach (var m in _moves)
            {
                qMap[m.code] += value;
            }

            _moves.Clear();
        }
    }
}