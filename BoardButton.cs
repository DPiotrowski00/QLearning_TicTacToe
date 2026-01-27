using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace TicTacToeSolver
{
    public partial class BoardButton : ObservableObject
    {
        [ObservableProperty]
        private string content;

        public int Row { get; }
        public int Column { get; }

        public ICommand Command { get; }

        public BoardButton(int row, int column, ICommand command, string content = "")
        {
            Row = row;
            Column = column;
            Command = command;
            Content = content;
        }
    }
}