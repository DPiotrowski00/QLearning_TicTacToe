using System.Diagnostics;

namespace TicTacToeSolver
{
    public class Board
    {
        private BoardStates[,] _board;
        private readonly int boardSize = 0;

        public Board(int boardSize)
        {
            this.boardSize = boardSize;
            _board = new BoardStates[boardSize, boardSize];
            Clear();
        }

        public string EncodeBoard()
        {
            string result = "";
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    var state = _board[i, j];
                    switch (state)
                    {
                        case BoardStates.X:
                            result += "X";
                            break;
                        case BoardStates.O:
                            result += "O";
                            break;
                        case BoardStates.Null:
                            result += "N";
                            break;
                    }
                }
                result += "-";
            }
            return result;
        }

        public void Clear()
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    _board[i, j] = BoardStates.Null;
                }
            }
        }

        public int GetSize()
        {
            return boardSize;
        }

        public bool CheckForGameEnd(out BoardStates? WinningMarker)
        {
            for (int i = 0; i < boardSize; i++)
            {
                BoardStates marker = _board[i, 0];
                if (marker == BoardStates.Null) continue;
                for (int j = 0; j < boardSize; j++)
                {
                    if (_board[i, j] == marker)
                    {
                        if (j == boardSize - 1)
                        {
                            WinningMarker = marker;
                            return true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < boardSize; i++)
            {
                BoardStates marker = _board[0, i];
                if (marker == BoardStates.Null) continue;
                for (int j = 0; j < boardSize; j++)
                {
                    if (_board[j, i] == marker)
                    {
                        if (j == boardSize - 1)
                        {
                            WinningMarker = marker;
                            return true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            {
                int i = 0;
                int j = 0;
                BoardStates marker = _board[i, j];
                while (i < boardSize && j < boardSize)
                {
                    if (marker == BoardStates.Null) break;
                    if (_board[i, j] == marker)
                    {
                        if (i == boardSize - 1 && j == boardSize - 1)
                        {
                            WinningMarker = marker;
                            return true;
                        }
                    }
                    else
                    {
                        break;
                    }

                    i++;
                    j++;
                }
            }
            {
                int i = boardSize - 1;
                int j = boardSize - 1;
                BoardStates marker = _board[i, j];
                while (i >= 0 && j >= 0)
                {
                    if (marker == BoardStates.Null) break;
                    if (_board[i, j] == marker)
                    {
                        if (i == 0 && j == 0)
                        {
                            WinningMarker = marker;
                            return true;
                        }
                    }
                    else
                    {
                        break;
                    }

                    i--;
                    j--;
                }
            }

            foreach (var value in _board)
            {
                if (value == BoardStates.Null)
                {
                    WinningMarker = BoardStates.Null;
                    return false;
                }
            }

            WinningMarker = BoardStates.Null;
            return true;
        }

        public bool TryPlacingMarker(BoardStates marker, int[] position)
        {
            if (position.Length == 0) return false;
            if (_board[position[0], position[1]] == BoardStates.Null && marker != BoardStates.Null)
            {
                _board[position[0], position[1]] = marker;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void WriteToConsole()
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    Debug.Write(_board[i, j] + " ");
                }
                Debug.Write("\n");
            }
        }
    }
}