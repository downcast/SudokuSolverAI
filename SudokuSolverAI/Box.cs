using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverAI
{
    class Box
    {
        #region fields
        /// <summary>
        /// Absolute Value
        /// </summary>
        public int RowID { get; set; }
        /// <summary>
        /// Absolute Value
        /// </summary>
        public int ColumnID { get; set; }
        public int Value { get; set; }
        public List<int> PossibleValues { get; }
        #endregion

       public Box()
        {
            RowID = 0;
            ColumnID = 0;
            Value = 0;
            PossibleValues = new List<int>();
        }
    }
}
