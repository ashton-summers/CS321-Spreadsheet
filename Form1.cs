/*
 
    * Name: Ashton Summers
    * ID: 11414124
    * Date created: 9/19/16
 
 */
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
using SpreadsheetEngine;
using CptS321;

namespace Spreadsheet_ASummers
{
    public partial class Form1 : Form
    {
        public Spreadsheet mSS;
        public Form1()
        {
            
            InitializeComponent();
            mSS = new Spreadsheet(50, 26);

            mSS.CellPropertyChanged += CellPropertyChanged; //subscribe to Spreadsheet's cell property changed event

            mSS.initializeCells();
            initializeColumns();
            initializeRows();

        }

        private void CellPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            Cell temp = sender as Cell;
            //find cell that was changed in dataGridView and then change its text to the cell's value that was changed

            dataGridView1.Rows[temp.RowIndex].Cells[temp.ColumnIndex].Value = temp.Value;
            if (temp.Value != "!(circ ref)" && temp.Value != "!(self ref)" && temp.Value != "!(bad ref)")
            {
                mSS.Recalculate(temp); //recalculate the value of all node's dependencies
            }

            if (e.PropertyName == "BGColor")
            {
                dataGridView1.Rows[temp.RowIndex].Cells[temp.ColumnIndex].Style.BackColor = Color.FromArgb(temp.BGColor);
            }
        }

        //Initializes the columns in the datagrid view
        public void initializeColumns()
        {
            //Adds rows A - Z onto the form
            for (int i = 65; i <= 90; i++)
            {
                //Convert ascii value to char and then add column
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = Convert.ToChar(i).ToString() });
            }
        }


        public void initializeRows()
        {
            //Create rows with number labels in the HeaderCell
            for (int j = 0; j < 50; j++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Value = (j + 1).ToString();
                dataGridView1.Rows.Add(row);
            }
        }

        //event for when the user begins entering any given cell
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
       
        {
            if (mSS == null) { return; }
            DataGridViewCell gridCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Cell ssCell = mSS.getCell(e.RowIndex, e.ColumnIndex);
            gridCell.Value = ssCell.Text;
        }

        //event for when the user stops editing any given cell
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (mSS == null) { return; }

            DataGridViewCell uiCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Cell dataCell = mSS.getCell(e.RowIndex, e.ColumnIndex);
            string s = dataCell.Text;

            if (s == null)
            {
                s = "";
            }

            if (uiCell.Value == null)
            {
                dataCell.Text = string.Empty;
            }
            else
            {
                dataCell.Text = uiCell.Value.ToString();
            }

            Stack<Cell> currentCells = new Stack<Cell>(); // holds data for updated cells
            Stack<Cell> prevCells = new Stack<Cell>(); // holds old data in case of undo

            Stack<string> currText = new Stack<string>();
            Stack<string> prevText = new Stack<string>();

            Cell cell = mSS.getCell(e.RowIndex, e.ColumnIndex);

            prevText.Push(s); // pushes old text onto stack

            currentCells.Push(cell); // pushs cell onto stack
            prevCells.Push(cell);


            currText.Push(cell.Text);

            //create new restore text command
            RestoreText cmd = new RestoreText("Text Change", prevText, currText, prevCells, currentCells); // constructor
            mSS.AddUndo(cmd); // add command to spreadsheets inner undo stack
            uiCell.Value = dataCell.Value;

        }

        //background color menu strip item in UI that allows user to select background color
        private void chooseBGColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
           // cd.ShowDialog();


            Stack<Cell> curCells = new Stack<Cell>(); // holds data for updated cells
            Stack<Cell> prevCells = new Stack<Cell>(); // holds old data in case of undo

            Stack<int> curColor = new Stack<int>(); // holds ints for current colors
            Stack<int> prevColor = new Stack<int>();

            //if okay was not pressed, then set the cd color to white
            if (cd.ShowDialog() != DialogResult.OK)
            {
                cd.Color = Color.FromArgb(-1);
            }

            //for all selected cells, change the BG color in logic layer
            foreach (DataGridViewCell gridCell in dataGridView1.SelectedCells)
            {
                Cell dataCell = mSS.getCell(gridCell.RowIndex, gridCell.ColumnIndex);
                prevCells.Push(dataCell);
                curCells.Push(dataCell);
                prevColor.Push(dataCell.BGColor);
                dataCell.BGColor = cd.Color.ToArgb();
                curColor.Push(dataCell.BGColor);
            }

            //create command to restore bg color
            RestoreBG cmd = new RestoreBG("BackGround Color Change", prevCells, curCells, prevColor, curColor);
            mSS.AddUndo(cmd);
            dataGridView1.ClearSelection();
        }

        //Edit button on the form
        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (mSS.CanUndo() == false) //disable button if there are no undos
            {
                undoToolStripMenuItem.Enabled = false; // turn off button
            }
            else
            {
                undoToolStripMenuItem.Enabled = true; // turn on button
                undoToolStripMenuItem.Text = "Undo";
                undoToolStripMenuItem.Text += " " + mSS.GetUndoTask(); // show user what the undo task is
            }

            if (mSS.CanRedo() == false)
            {
                redoToolStripMenuItem.Enabled = false;
            }
            else
            {
                redoToolStripMenuItem.Enabled = true;
                redoToolStripMenuItem.Text = "Redo";
                redoToolStripMenuItem.Text += " " + mSS.GetRedoTask(); // show user redo task
            }
        }

        //Undo menu strip item in UI
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSS.CanUndo() == true) //if undo stack is not empty, undo cmd
            {
                mSS.Undo();
            }
            else
            {
                return;
            }
        }

        //redo button in UI
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSS.CanRedo() == true) //if redo stack is not empty. Redo cmd
            {
                mSS.Redo();
            }
            else
            {
                return;
            }
        }

        //Clears all data from the data grid view back to default colors
        private void clearDataGrid()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Value = "";
                    cell.Style.BackColor = Color.FromArgb(-1);
                }
            }
        }

        //saves the current spreadsheet to an xml file
        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();

            if (sf.ShowDialog() == DialogResult.OK)
            {
                FileStream saveFile = new FileStream(sf.FileName, FileMode.Create, FileAccess.Write);
                mSS.Save(saveFile);
                saveFile.Close();
                saveFile.Dispose();
            }
        }

        //loads spreadsheet attributes from an xml file
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog menu = new OpenFileDialog();

            if (menu.ShowDialog() == DialogResult.OK) // file ok to open
            {
                mSS = new Spreadsheet(50, 26); //erase current spreadsheet logic layer
                mSS.initializeCells();
                mSS.CellPropertyChanged += CellPropertyChanged; //subscribe to spreadsheet's property changed event

                clearDataGrid(); //clear data from the dataGridView

                FileStream openFile = new FileStream(menu.FileName, FileMode.Open, FileAccess.Read);

                mSS.Load(openFile);

                openFile.Close();
                openFile.Dispose();
            }
        }
    }
}
