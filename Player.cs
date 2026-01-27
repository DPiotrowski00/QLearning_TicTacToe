using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeSolver
{
    public class Player(string name, int marker)
    {
        public string Name { get; set; } = name;
        public int Marker { get; set; } = marker;
    }
}
