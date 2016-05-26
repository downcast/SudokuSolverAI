using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuSolverAI
{
    public partial class Form1 : Form
    {
        Grid grid;
        int num = 0;
        Color color = Color.Empty;

        public Form1()
        {
            InitializeComponent();
            grid = new Grid();
            grid.ValueAssigned += this.OnValueAssigned;

            readFile();
        }

        private void readFile()
        {
            String line;
            int count = 0;
            StreamReader file = new StreamReader("SudokuBoard109H.txt");

            while ((line = file.ReadLine()) != null)
            {
                populateBoard(line, count);
                count++;
            }

            file.Close();

            color = Color.MediumOrchid;
        }

        private void populateBoard(String numbers, int row)
        {
            int count = 0;
            TextBox box;

            for (int i = 0; i < numbers.Length; i++)
            {
                if (char.IsDigit(numbers[i]))
                {
                    box = (TextBox)Controls.Find("box"+(row+1)+(count+1), true).FirstOrDefault();

                    //Grid.Cube[row,col].Box[row,col].Box.value
                    grid.setBoxValue((int)Char.GetNumericValue(numbers[i]), row, count);
                    //box.Text = Char.ToString(numbers[i]);
                    // Skip the comma
                    i++;
                }
                else
                {
                    // Add zero to be empty space
                    grid.setBoxValue(0, row, count);
                }
                count++;
            }

            if (count != 9)
            {
                //count++;
                // Add zero to be empty space
               grid.setBoxValue(0, row, count);
            }
        }
        
        private void btnFindOne_Click(object sender, EventArgs e)
        {
            num = num == 9 ? 1 : ++num;

            Console.WriteLine("Searching for {0}", num);


            int[] cubeBoxLocation = grid.findOneNumber(num);

            if (cubeBoxLocation[0] != -1)
            {
                grid.setBoxValue(num, cubeBoxLocation);
                //TextBox box = (TextBox)Controls.Find("box" + ((cubeBoxLocation[0] * 3) + cubeBoxLocation[2] +1) + ((cubeBoxLocation[1] * 3) + cubeBoxLocation[3] + 1), true).FirstOrDefault();
                //box.Text = num.ToString();
                //box.BackColor = Color.MediumOrchid;
            }
        }

        private void btnFindManyNumbers_Click(object sender, EventArgs e)
        {
            for (int count = 0; count < 9;)
            {
                num = num == 9 ? 1 : ++num;

                Console.WriteLine("Searching for {0}", num);
               

                int[] cubeBoxLocation = grid.findOneNumber(num);

                if (cubeBoxLocation[0] != -1)
                {
                    grid.setBoxValue(num, cubeBoxLocation);
                   // TextBox box = (TextBox)Controls.Find("box" + ((cubeBoxLocation[0] * 3) + cubeBoxLocation[2] + 1) + ((cubeBoxLocation[1] * 3) + cubeBoxLocation[3] + 1), true).FirstOrDefault();
                    //box.Text = num.ToString();
                    //box.BackColor = Color.MediumOrchid;

                    num = 0;
                    count = 0;
                }
                else
                { count++; }
            }

            Console.WriteLine("Can't find anymore numbers. Checking columns now.");
            
            for (int i = 0; i < 9; i++)
            {
                Console.WriteLine("Searching in column {0}", i);

                // Get column numbers
                Box[] boxColumn = grid.getColumn(i);
                // Assign the possible values and determine actual value
                grid.determinePossibleColumn(boxColumn, i);
            }

            Console.WriteLine("Can't find anymore numbers. Checking rows now.");

            for (int i = 0; i < 9; i++)
            {
                Console.WriteLine("Searching in row {0}", i);

                // Get Row numbers
                Box[] boxRow = grid.getRow(i);
                // Assign the possible values and determine actual value
                grid.determinePossibleRow(boxRow, i);
            }

            Console.WriteLine("Can't find anymore numbers.");
        }

        public void OnValueAssigned(object sender, CubeBoxEventArgs args)
        {
            TextBox box = (TextBox)Controls.Find("box" + (args.Row + 1) + (args.Column + 1), true).FirstOrDefault();
            if (args.Value != 0)
                box.Text = args.Value.ToString();
            box.BackColor = color;
        }
    }
}
