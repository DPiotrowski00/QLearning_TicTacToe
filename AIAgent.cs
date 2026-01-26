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
        private List<Move> _moves = [];

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

            var jeden = qMap.Where(q => q.Key.StartsWith(codeStart));
            var dwa = jeden.OrderByDescending(q => q.Value);
            var trzy = dwa.First();
            var bestMoveCode = trzy.Key;
            //var bestMoveCode = qMap.Where(q => q.Key.StartsWith(codeStart)).OrderByDescending(q => q.Value).First().Key;

            var bestMove = bestMoveCode.Substring(bestMoveCode.Length - 2);
            int[] move = [Int32.Parse(bestMove[0].ToString()), Int32.Parse(bestMove[1].ToString())];
            board.TryPlacingMarker(this.Marker, move);
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