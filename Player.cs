using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeSolver
{
    public class Player(string name, BoardStates marker)
    {
        public string Name { get; set; } = name;
        public BoardStates Marker { get; set; } = marker;
    }
}
