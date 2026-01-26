using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TicTacToeSolver
{
    public partial class MainVM : ObservableObject
    {
        private Board Board = new(3);
        private readonly ObservableCollection<AIAgent> Players;

        [ObservableProperty]
        private int trainingDepth = 1000;

        public MainVM()
        {
            Players ??= [];
            Players.Clear();
            Players.Add(new AIAgent("Player 1", BoardStates.X));
            Players.Add(new AIAgent("Player 2", BoardStates.O));
        }

        [RelayCommand]
        private void Play()
        {
            for (int i = 0; i < TrainingDepth; i++)
            {
                Board.Clear();
                BoardStates? winner = BoardStates.Null;
                bool GameLoop = true;

                while (GameLoop)
                {
                    foreach (var player in Players)
                    {
                        player.MakeAMove(ref Board);
                        if (Board.CheckForGameEnd(out winner))
                        {
                            GameLoop = false;
                            break;
                        }
                    }
                }

                if (winner == BoardStates.Null)
                {
                    foreach (var p in Players)
                    {
                        p.ApplyRewards(1);
                    }
                }
                else
                {
                    var winners = Players.Where(p => p.Marker == winner);
                    var losers = Players.Where(p => p.Marker != winner);

                    foreach (var w in winners)
                    {
                        w.ApplyRewards(2);
                    }
                    foreach (var l in losers)
                    {
                        l.ApplyRewards(-2);
                    }
                }

                foreach(var p in Players)
                {
                    p.DecayEpsilon();
                }
            }
            Debug.WriteLine($"Successfully trained {TrainingDepth} times.");
        }

        [RelayCommand]
        private void PlayWhatWasLearned()
        {
            Board.Clear();
            BoardStates? winner = BoardStates.Null;
            bool GameLoop = true;

            while (GameLoop)
            {
                foreach (var player in Players)
                {
                    player.MakeAnEducatedMove(ref Board);
                    if (Board.CheckForGameEnd(out winner))
                    {
                        GameLoop = false;
                        break;
                    }
                }
            }

            Board.WriteToConsole();
            
            switch (winner)
            {
                case BoardStates.X:
                    Debug.WriteLine($"X WON!");
                    break;
                case BoardStates.O:
                    Debug.WriteLine($"O WON!");
                    break;
                case BoardStates.Null:
                    Debug.WriteLine($"NOBODY WON!");
                    break;
            }

            //var winners = Players.Where(p => p.Marker == winner);
            //var losers = Players.Where(p => p.Marker != winner);

            //foreach (var w in winners)
            //{
            //    w.ApplyRewards(true);
            //}
            //foreach (var l in losers)
            //{
            //    l.ApplyRewards(false);
            //}
        }
    }
}