using System.Diagnostics;

namespace TicTacToeSolver
{
    public class Board
    {
        private int[,] _board;
        private readonly int boardSize = 0;

        public Board(int boardSize)
        {
            this.boardSize = boardSize;
            _board = new int[boardSize, boardSize];
            Clear();
        }

        public string EncodeBoard()
        {
            string result = "";
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    result += _board[i, j];
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
                    _board[i, j] = 0;
                }
            }
        }

        public int GetSize()
        {
            return boardSize;
        }

        public bool CheckForGameEnd(out int WinningMarker)
        {
            for (int i = 0; i < boardSize; i++)
            {
                int marker = _board[i, 0];
                if (marker == 0) continue;
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
                int marker = _board[0, i];
                if (marker == 0) continue;
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
                int marker = _board[i, j];
                while (i < boardSize && j < boardSize)
                {
                    if (marker == 0) break;
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
                int marker = _board[i, j];
                while (i >= 0 && j >= 0)
                {
                    if (marker == 0) break;
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
                if (value == 0)
                {
                    WinningMarker = 0;
                    return false;
                }
            }

            WinningMarker = 0;
            return true;
        }

        public bool TryPlacingMarker(int marker, int[] position)
        {
            if (position.Length == 0) return false;
            if (_board[position[0], position[1]] == 0 && marker != 0)
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