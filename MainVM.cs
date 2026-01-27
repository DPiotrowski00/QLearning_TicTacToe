using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;

namespace TicTacToeSolver
{
    public partial class MainVM : ObservableObject
    {
        private Board Board;
        private Board UserBoard;
        private readonly List<AIAgent> Players;

        public RelayCommand<BoardButton> CellCommand { get; }

        [ObservableProperty]
        private ObservableCollection<BoardButton> cells = [];

        [ObservableProperty]
        private int boardSize = 3;
        [ObservableProperty]
        private int trainingDepth = 1000;
        [ObservableProperty]
        private int timesTrained = 0;

        partial void OnBoardSizeChanged(int oldValue, int newValue)
        {
            UserBoard = new(newValue);
            DrawBoard();
        }

        private void OnCellClicked(BoardButton cell)
        {
            if (UserBoard.TryPlacingMarker(1, [cell.Row, cell.Column]))
            {
                cell.Content = "X";
                if (!UserBoard.CheckForGameEnd(out int winner))
                {
                    Players.Where(p => p.Marker == -1).First().MakeAnEducatedMove(ref UserBoard, out int row, out int col);
                    Cells.Where(b => b.Row == row && b.Column == col).First().Content = "O";

                    Debug.WriteLine(UserBoard.PossibleTransformations());
                }
            }
        }

        public MainVM()
        {
            Board = new(boardSize);
            UserBoard = new(boardSize);

            CellCommand = new RelayCommand<BoardButton>(OnCellClicked);
            DrawBoard();
            
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
                Board.Clear();
                int winner = 0;
                bool GameLoop = true;

                while (GameLoop)
                {
                    foreach (var player in Players)
                    {
                        player.MakeAMove(ref Board, out _, out _);
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
                    player.MakeAnEducatedMove(ref Board, out _, out _);
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

        [RelayCommand]
        private void DrawBoard()
        {
            Cells.Clear();
            UserBoard.Clear();

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    Cells.Add(new BoardButton(i, j, CellCommand));
                }
            }
        }
    }
}