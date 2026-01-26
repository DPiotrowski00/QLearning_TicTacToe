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

        [ObservableProperty]
        private int timesTrained = 0;

        public MainVM()
        {
            Players ??= [];
            Players.Clear();
            Players.Add(new AIAgent("Player 1", 1));
            Players.Add(new AIAgent("Player 2", -1));
        }

        [RelayCommand]
        private void Play()
        {
            for (int i = 0; i < TrainingDepth; i++)
            {
                this.Board.Clear();
                int winner = 0;
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

                if (winner == 0)
                {
                    foreach (var p in Players)
                    {
                        p.ApplyRewards(0.5);
                    }
                }
                else
                {
                    var winners = Players.Where(p => p.Marker == winner);
                    var losers = Players.Where(p => p.Marker != winner);

                    foreach (var w in winners)
                    {
                        w.ApplyRewards(1.0d);
                    }
                    foreach (var l in losers)
                    {
                        l.ApplyRewards(-1.0d);
                    }
                }

                foreach(var p in Players)
                {
                    p.DecayEpsilon();
                }
            }
            Debug.WriteLine($"Successfully trained {TrainingDepth} times.");
            TimesTrained += TrainingDepth;
        }

        [RelayCommand]
        private void PlayWhatWasLearned()
        {
            Board.Clear();
            int winner = 0;
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
                case 1:
                    Debug.WriteLine($"X WON!");
                    break;
                case -1:
                    Debug.WriteLine($"O WON!");
                    break;
                case 0:
                    Debug.WriteLine($"NOBODY WON!");
                    break;
            }
        }
    }
}