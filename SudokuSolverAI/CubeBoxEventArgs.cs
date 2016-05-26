using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverAI
{
    public class CubeBoxEventArgs : EventArgs
    {
        public int Value { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
