using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TicTacToeSolver
{
    public record State(string BoardCode, int Player);
    public record Action(int Row, int Col);

    public class QAgent(string name, int marker) : Player(name, marker)
    {
        private readonly Random random = new();
        private readonly Dictionary<(State, Action), double> qMap = [];

        private double epsilon = 1.0d;
        private readonly double epsilonMin = 0.01d;
        private readonly double epsilonDecay = 0.99995d;

        private readonly double alpha = 0.1d;
        private readonly double gamma = 0.95d;

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
            int CounterMarker = Marker == 1 ? -1 : 1;

            while (true)
            {
                row = (int)random.NextInt64(0, board.GetSize());
                col = (int)random.NextInt64(0, board.GetSize());

                State state = new(board.EncodeThisBoard(), Marker);
                Action action = new(row, col);

                if (board.TryPlacingMarker(Marker, [row, col]))
                {
                    State updatedState = new(board.EncodeThisBoard(), CounterMarker);

                    var possibleSolutions = qMap.Where(q => q.Key.Item1 == updatedState).OrderByDescending(i => i.Value);
                    double bestNextMoveReward = 0;

                    if (possibleSolutions.Any())
                    {
                        bestNextMoveReward = possibleSolutions.First().Value;
                    }

                    int reward = 0;

                    if (board.CheckForGameEnd(out int winner))
                    {
                        if (winner == 0)
                        {
                            reward = 0;
                        }
                        else
                        {
                            if (Marker == winner)
                            {
                                reward = 1;
                            }
                            else
                            {
                                reward = -1;
                            }
                        }
                    }
                    if (qMap.TryGetValue((state, action), out _))
                    {
                        qMap[(state, action)] += alpha * (reward + gamma * bestNextMoveReward - qMap[(state, action)]);
                    }
                    else
                    {
                        qMap.Add((state, action), reward);
                    }

                    break;
                }
            }
        }

        public void MakeAnEducatedMove(ref Board board, out int row, out int col)
        {
            State currentState = new (board.EncodeThisBoard(), Marker);

            var possibilities = qMap.Where(q => q.Key.Item1 == currentState).OrderByDescending(i => i.Value);

            if (possibilities.Any())
            {
                Action bestAction = possibilities.First().Key.Item2;

                row = bestAction.Row;
                col = bestAction.Col;

                int CounterMarker = Marker == 1 ? -1 : 1;

                State updatedState = new(board.EncodeThisBoard(), CounterMarker);

                var possibleSolutions = qMap.Where(q => q.Key.Item1 == updatedState).OrderByDescending(i => i.Value);
                KeyValuePair<(State, Action), double> bestSolution;
                double bestNextMoveReward = 0;

                if (possibleSolutions.Any())
                {
                    bestSolution = possibleSolutions.First();
                    bestNextMoveReward = bestSolution.Value;
                }
                else
                {
                    return;
                }

                int reward = 0;

                if (board.TryPlacingMarker(Marker, [row, col]))
                {
                    if (board.CheckForGameEnd(out int winner))
                    {
                        if (winner == 0)
                        {
                            reward = 0;
                        }
                        else
                        {
                            if (Marker == winner)
                            {
                                reward = 1;
                            }
                            else
                            {
                                reward = -1;
                            }
                        }
                    }

                    qMap[(currentState, bestAction)] += alpha * (reward + gamma * bestNextMoveReward - qMap[(currentState, bestAction)]);
                }
            }
            else
            {
                this.MakeARandomMove(ref board, out row, out col);
            }
        }
    }
}