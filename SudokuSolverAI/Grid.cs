using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverAI
{
    class Grid
    {
        #region fields
        public int CountTotal { get; set; }
        public int CountRow { get; set; }
        public int CountColumn { get; set; }
        Cube[,] Cubes;

        public event EventHandler<CubeBoxEventArgs> ValueAssigned;
        #endregion

        public Grid()
        {
            Cubes = new Cube[3, 3];
            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Cubes[i, j] = new Cube();
                }
            }
        }

        /// <summary>
        /// Sets the values of the boxes based on the absolute position of the box in the grid
        /// </summary>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void setBoxValue(int value, int row, int column)
        {
            Cubes[row / 3, column / 3].setBoxValue(value, row, column);
            OnValueAssigned(value, row, column);
        }

        public void setBoxValue(int num, int[] cubeBoxLocation)
        {
            getCube(cubeBoxLocation[0], cubeBoxLocation[1]).setBoxValue(num, cubeBoxLocation[2], cubeBoxLocation[3]);
            OnValueAssigned(num, (cubeBoxLocation[0] * 3) + cubeBoxLocation[2], (cubeBoxLocation[1] * 3) + cubeBoxLocation[3]);
        }

        public Cube getCube(int row, int column)
        {
            return Cubes[row, column];
        }

        public int[] getEmptyBoxLocation(int cubeRow, int cubeColumn, int lastBoxRow, int lastBoxColumn)
        {
            int[] boxLocation = Cubes[cubeRow, cubeColumn].getEmptyBoxLocation(lastBoxRow, lastBoxColumn);

            if (boxLocation[0] != -1)
            { return new int[] { boxLocation[0], boxLocation[1] }; }

            return new int[] { -1, -1 };
        }

        /// <summary>
        /// Returns the relative cubeBoxLocation
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public int[] findOneNumber(int num)
        {
            int count = 0;
            int[] workingLocations = getEmptyBoxLocation(0, 0, 0, 0);
            int[] boxLocations = { 0, 0 };

            // Cube Row
            for (int i = 0; i < 3; i++)
            {
                // Cube Column
                for (int j = 0; j < 3; j++)
                {
                    while (workingLocations[0] != -1)
                    {
                        // Number cannot have two possible locations to be placed
                        if (count != 2)
                        {
                            // If the number is in the cube, dont search
                            if (!getCube(i, j).isInCube(num))
                            {
                                // Is the number in the column or row or is the spot filler already?
                                while (isInColumn(num, j, workingLocations[1]) || isInRow(num, i, workingLocations[0]) || getCube(i, j).isFilled(workingLocations[0], workingLocations[1]))
                                {
                                    if (workingLocations[1] + 1 > 2)
                                    {
                                        workingLocations[1] = 0;
                                        if (workingLocations[0] + 1 > 2)
                                        { goto CubeIterated; }
                                        else
                                        { workingLocations[0] += 1; }
                                    }
                                    else
                                    { workingLocations[1] += 1; }
                                }
                            }
                            else { goto CubeIterated; }
                            
                            // Possible location found; save it
                            workingLocations = getEmptyBoxLocation(i, j, workingLocations[0], workingLocations[1]);

                            boxLocations[0] = workingLocations[0];
                            boxLocations[1] = workingLocations[1];
                            count++;

                            // Reached the end of the column?
                            if (workingLocations[1] + 1 > 2)
                            {
                                // Reset to first column and move to next row
                                workingLocations[1] = 0;
                                if (workingLocations[0] + 1 > 2)
                                { goto CubeIterated; }
                                else
                                { workingLocations[0] += 1; }
                            }
                            else
                            { workingLocations[1] += 1; }

                        }
                        else { break; }
                    }
                    CubeIterated:

                    // Is there only one place this number can go?
                    if (count == 1)
                    {
                        return new int[] { i, j, boxLocations[0], boxLocations[1] };
                    }
                    boxLocations[0] = 0;
                    boxLocations[1] = 0;
                    workingLocations[0] = 0;
                    workingLocations[1] = 0;
                    count = 0;
                }
            }

            return new int[] { -1, -1 };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <param name="cubeRow">Relative</param>
        /// <param name="boxRow">relative</param>
        /// <returns></returns>
        public bool isInRow(int num, int cubeRow, int boxRow)
        {
            for (int i = 0; i < 3; i++)
            {
                if (getCube(cubeRow, i).isInRow(num, boxRow))
                { return true; }
            }
            return false;
        }

        public bool isInColumn(int num, int cubeColumn, int boxColumn)
        {
            for (int i = 0; i < 3; i++)
            {
                if (getCube(i, cubeColumn).isInColumn(num, boxColumn))
                { return true; }
            }
            return false;
        }
        
        /// <summary>
        /// Returns an array of Boxes that appear in the Grid column
        /// </summary>
        /// <param name="requiredCount"></param>
        /// <param name="column">Absolute number</param>
        /// <returns></returns>
        public Box[] getColumn(int column)
        {
            Box[] boxColumn = new Box[0];

            for (int i = 0; i < 3; i++)
            { boxColumn = boxColumn.Concat(Cubes[i, column / 3].getColumn(column % 3)).ToArray(); }
          
            return boxColumn;
        }

        /// <summary>
        /// Returns an array of Boxes that appear in the Row column
        /// </summary>
        /// <param name="requiredCount"></param>
        /// <param name="row">Absolute number</param>
        /// <returns></returns>
        public Box[] getRow(int row)
        {
            Box[] boxRow = new Box[0];

            for (int i = 0; i < 3; i++)
            { boxRow = boxRow.Concat(Cubes[row / 3, i].getRow(row % 3)).ToArray(); }

            return boxRow;
        }

        public void determinePossibleColumn(Box[] boxColumn, int column)
        {
            int[] missingNums = findMissingNumbers(boxColumn);

            // Each box 
            for (int i = 0; i < boxColumn.Length; i++)
            {
                // that is empty
                if (boxColumn[i].Value == 0)
                {
                    // will check with each missing num
                    for (int j = 0; j < missingNums.Length; j++)
                    {
                        // and added to possible if valid
                        if (!isInRow(missingNums[j], boxColumn[i].RowID / 3, boxColumn[i].RowID % 3)
                            && !getCube(boxColumn[i].RowID / 3, boxColumn[i].ColumnID / 3).isInCube(missingNums[j]))
                        {
                            boxColumn[i].PossibleValues.Add(missingNums[j]);
                        }
                    }
                }
            }

            assignActualValue(boxColumn);

            foreach (Box b in boxColumn)
                b.PossibleValues.Clear();

            // CubeColumn to Column Compare

            // Cube Row
            /*for (int i = 0; i < 3; i++)
            {
                Cube workingCube = Cubes[i, column / 3];
                Box[] workingBoxColumn = workingCube.getColumn(column % 3);

                int emptyBoxCount = 0;

                // If all missing boxes are in the same column
                if (workingCube.TotalCount >= 6)
                {
                    foreach (Box b in workingBoxColumn)
                    {
                        if (b.Value == 0)
                            emptyBoxCount++;
                    }

                    if (workingCube.TotalCount + emptyBoxCount == 9)
                    {
                        // Remove their possibles from others in the column
                        switch(i)
                        {
                            case 0:
                                // Each each possible value in each of the working boxes, remove them from the other possibles
                                for (int j = 3; j < boxColumn.Length; j++)
                                {
                                    foreach (Box b in workingBoxColumn)
                                    {
                                        foreach (int v in b.PossibleValues)
                                        {
                                            if (boxColumn[j].PossibleValues.Contains(v))
                                                boxColumn[j].PossibleValues.Remove(v);
                                        }
                                    }
                                }
                                break;
                            case 1:
                                for (int j = 0; j < boxColumn.Length; j++)
                                {
                                    if (j > 2 && j < 6)
                                        continue;

                                    foreach (Box b in workingBoxColumn)
                                    {
                                        foreach (int v in b.PossibleValues)
                                        {
                                            if (boxColumn[j].PossibleValues.Contains(v))
                                                boxColumn[j].PossibleValues.Remove(v);
                                        }
                                    }
                                }
                                break;
                            case 2:
                                for (int j = 6; j < boxColumn.Length; j++)
                                {
                                    foreach (Box b in workingBoxColumn)
                                    {
                                        foreach (int v in b.PossibleValues)
                                        {
                                            if (boxColumn[j].PossibleValues.Contains(v))
                                                boxColumn[j].PossibleValues.Remove(v);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            */
            //assignActualValue(boxColumn);
        }

        public void determinePossibleRow(Box[] boxRow, int row)
        {
            int[] missingNums = findMissingNumbers(boxRow);

            // Each box 
            for (int i = 0; i < boxRow.Length; i++)
            {
                // that is empty
                if (boxRow[i].Value == 0)
                {
                    // will check with each missing num
                    for (int j = 0; j < missingNums.Length; j++)
                    {
                        // and added to possible if valid
                        if (!isInColumn(missingNums[j], boxRow[i].ColumnID / 3, boxRow[i].ColumnID % 3)
                            && !getCube(boxRow[i].RowID / 3, boxRow[i].ColumnID / 3).isInCube(missingNums[j]))
                        {
                            boxRow[i].PossibleValues.Add(missingNums[j]);
                        }
                    }
                }
            }

            assignActualValue(boxRow);

            foreach (Box b in boxRow)
                b.PossibleValues.Clear();

            // CubeRow to Row Compare

            // Cube Column
            /*for (int i = 0; i < 3; i++)
            {
                Cube workingCube = Cubes[row / 3, i];
                Box[] workingBoxRow = workingCube.getRow(row % 3);

                int emptyBoxCount = 0;

                // If all missing boxes are in the same row
                if (workingCube.TotalCount >= 6)
                {
                    foreach (Box b in workingBoxRow)
                    {
                        if (b.Value == 0)
                            emptyBoxCount++;
                    }

                    if (workingCube.TotalCount + emptyBoxCount == 9)
                    {
                        // Remove their possibles from others in the row
                        switch (i)
                        {
                            case 0:
                                // Each each possible value in each of the working boxes, remove them from the other possibles
                                for (int j = 3; j < boxRow.Length; j++)
                                {
                                    foreach (Box b in workingBoxRow)
                                    {
                                        foreach (int v in b.PossibleValues)
                                        {
                                            if (boxRow[j].PossibleValues.Contains(v))
                                                boxRow[j].PossibleValues.Remove(v);
                                        }
                                    }
                                }
                                break;
                            case 1:
                                for (int j = 0; j < boxRow.Length; j++)
                                {
                                    if (j > 2 && j < 6)
                                        continue;

                                    foreach (Box b in workingBoxRow)
                                    {
                                        foreach (int v in b.PossibleValues)
                                        {
                                            if (boxRow[j].PossibleValues.Contains(v))
                                                boxRow[j].PossibleValues.Remove(v);
                                        }
                                    }
                                }
                                break;
                            case 2:
                                for (int j = 6; j < boxRow.Length; j++)
                                {
                                    foreach (Box b in workingBoxRow)
                                    {
                                        foreach (int v in b.PossibleValues)
                                        {
                                            if (boxRow[j].PossibleValues.Contains(v))
                                                boxRow[j].PossibleValues.Remove(v);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            */
            //assignActualValue(boxRow);
        }

        public void assignActualValue(Box[] boxes)
        {
            int count = -1;
            Dictionary<int, int> possibleCount = new Dictionary<int, int>();

            while (count != 0)
            {
                count = 0;
                possibleCount.Clear();

                foreach (Box b in boxes)
                {
                    // Check for only one
                    if (b.PossibleValues.Count == 1)
                    {
                        setBoxValue(b.PossibleValues[0], b.RowID, b.ColumnID);
                        // remove value from other possibles
                        foreach (Box b2 in boxes)
                        {
                            if (b2.PossibleValues.Contains(b.Value))
                            {
                                b2.PossibleValues.Remove(b.Value);
                                int num; 

                                if(possibleCount.TryGetValue(b.Value, out num))
                                    possibleCount[b.Value]--;
                            }
                        }
                        count++;
                    }
                    else
                    {
                        // Count the occurence of each number of all the possibles
                        foreach(int i in b.PossibleValues)
                        {
                            int value;
                            if(possibleCount.TryGetValue(i, out value))
                            {
                                possibleCount[i] = ++value;
                            }
                            else
                            {
                                possibleCount.Add(i, 1);
                            }
                        }
                    }
                }
            }

            count = -1;

            while (count != 0)
            {
                count = 0;

                Dictionary<int, int> tempPossibleCount = new Dictionary<int, int>(possibleCount);

                // Iterate through all the keys' values looking for one; set that as value
                foreach (KeyValuePair<int, int> kv in possibleCount)
                {
                    // If value occurs once in total in all the possibles, but that box still has two possibles, it still counts as the only possible value
                    if (kv.Value == 1)
                    {
                        foreach (Box b in boxes)
                        {
                            if (b.PossibleValues.Contains(kv.Key))
                            {
                                setBoxValue(kv.Key, b.RowID, b.ColumnID);
                                count++;

                                // Rewmoved the box value from the possible count and cleared the box's possible
                                foreach (int i in b.PossibleValues)
                                    tempPossibleCount[i]--;

                                b.PossibleValues.Clear();
                                break;
                            }
                        }
                    }
                }

                possibleCount = tempPossibleCount;
            }
        }

        public int[] findMissingNumbers(Box[] numbers)
        {
            int[] nums = new int[9];

            for (int i = 0; i < numbers.Length; i++)
                nums[i] = numbers[i].Value;

            return Enumerable.Range(1, 9).Except(nums).ToArray();
        }

        /// <summary>
        /// Theses are absolute values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        protected virtual void OnValueAssigned(int value, int row, int column)
        {
            if (ValueAssigned != null)
                ValueAssigned(this, new CubeBoxEventArgs() { Value = value, Column = column, Row = row });
        }

        public void print()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Cubes[i, j].print();
                }
            }
        }
    }
}
