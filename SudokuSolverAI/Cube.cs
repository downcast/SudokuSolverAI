using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverAI
{
    class Cube
    {
        #region fields
        public int TotalCount { get; set; }
        public int[] RowCount { get; set; }
        public int[] ColumnCount { get; set; }
        Box[,] Boxes;
        #endregion

        public Cube()
        {
            TotalCount = 0;
            RowCount = new int[] { 0, 0, 0 };
            ColumnCount = new int[] { 0, 0, 0 };
            Boxes = new Box[3,3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                { Boxes[i, j] = new Box();  }
            }
        }

        public void setBoxValue(int value, int row, int column)
        {
            Boxes[row % 3, column % 3].Value = value;
            Boxes[row % 3, column % 3].RowID = row;
            Boxes[row % 3, column % 3].ColumnID = column;

            if (value != 0)
            {
                TotalCount++;
                RowCount[row % 3]++;
                ColumnCount[column % 3]++;
            }
        }

        public bool isInCube(int num)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Boxes[i,j].Value == num)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool isInRow(int num, int row)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Boxes[row, i].Value == num)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isInColumn(int num, int column)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Boxes[i, column].Value == num)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isFilled(int row, int column)
        {
            if (Boxes[row, column].Value == 0)
            { return false;  }

            return true;
        }

        public Box[] getColumn(int column)
        { return new Box[] { Boxes[0, column], Boxes[1, column], Boxes[2, column] }; }

        public Box[] getRow(int row)
        { return new Box[] { Boxes[row, 0], Boxes[row, 1], Boxes[row, 2] }; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastRow">The last row that this function found an emptybox or no more boxes</param>
        /// <param name="lastColumn">The last column that this function found an emptybox or no more boxes</param>
        /// <returns>The location of the empty box within the cube. The values + 1 serve as the next last positions. If no empty boxes found, returns -1s</returns>
        public int[] getEmptyBoxLocation(int lastRow, int lastColumn)
        {
            for (int i = lastRow; i < 3; i++)
            {
                for (int j = lastColumn; j < 3; j++)
                {
                    if (Boxes[i, j].Value == 0)
                    {
                        return new int[] { i, j };
                    }
                }
                lastColumn = 0;
            }
            return new int[] { -1, -1 };
        }
        
        public void print()
        {
            StringBuilder temp = new StringBuilder();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                { temp.Append(Boxes[i, j].Value + ","); }

                Console.WriteLine(temp);
                temp.Clear();
            }
        }
    }
}
