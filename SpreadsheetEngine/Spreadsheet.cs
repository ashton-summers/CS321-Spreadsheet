/*
 
    * Name: Ashton Summers
    * ID: 11414124
    * Date created: 9/19/16
 
 */

using CptS321;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SpreadsheetEngine
{

    public class Spreadsheet
    {

        public event PropertyChangedEventHandler CellPropertyChanged;


        //member variables
        private int mRows;
        private int mColumns;
        private Cell[,] mCellArray;
        public Dictionary<string, HashSet<Cell>> mDependencies;
        protected UndoRedo mUndoRedo = new UndoRedo();


        //Constructor
        public Spreadsheet(int rows, int columns)
        {
            mRows = rows;
            mColumns = columns;
            mCellArray = new Cell[rows, columns]; //Creates a new instance of a Cell array
            mDependencies = new Dictionary<string, HashSet<Cell>>();
        }


        //notifies all subscribers that cell has changed
        private void NotifyPropertyChanged(object sender, string propertyName)
        {
            if (CellPropertyChanged != null)
            {
                //sender must be cell otherwise we don't know what cell we are looking at from Form1
                CellPropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
            }
        }


        //Event handler that sets the value of the cell when property has changed
        private void eventHandler(object sender, PropertyChangedEventArgs e)
        {
            double result = 0.0;
            Cell temp = sender as Cell;
            ExpTree tree;
            if (e.PropertyName == "Text")
            {

                if (temp.Text.StartsWith("=") || double.TryParse(temp.Text, out result)) //if first character is '='
                {
                    if (double.TryParse(temp.Text, out result)) //if text is just a double value, make tree with just text
                    {
                        tree = new ExpTree(temp.Text);
                    }
                    else //else make a tree with an entire expression
                    {
                        tree = new ExpTree(temp.Text.Substring(1));
                    }

                    if (CheckBadRef(tree.getVarNames())) //check to see if there is a bad reference 
                    {
                        temp.SetValue("!(bad ref)");
                    }
                    else
                    {
                        removeOldDependencies(temp, temp.Text.Substring(1));
                        setTreeVars(ref tree);
                        addDependencies(temp, tree.getVarNames());

                        if (CheckSelfRef(temp))
                        {
                            temp.SetValue("!(self ref)");
                        }
                        else if (CheckCircRef(temp, temp))
                        {
                            temp.SetValue("!(circ ref)");
                        }
                        else
                        {

                            temp.SetValue(tree.Eval().ToString());
                        }
                    }
                }
                else // just a string
                {
                    temp.SetValue(temp.Text);
                }

                NotifyPropertyChanged(temp, temp.Value); //Notify subscribers

            }

            else if (e.PropertyName == "BGColor")
            {
                NotifyPropertyChanged(sender, "BGColor"); // fire off property changed event for color
            }

        }

        //Property that returns the number of columns in the spreadsheet
        public int ColumnCount
        {
            get
            {
                return mColumns;
            }
        }

        //Property that returns the number of rows in the spreadsheet
        public int RowCount
        {
            get
            {
                return mRows;
            }
        }

        //Initializes all cells for every row and column
        public void initializeCells()
        {
            for (int i = 0; i < mRows; i++)
            {
                for (int j = 0; j < mColumns; j++)
                {
                    mCellArray[i, j] = new CellChild(i, j);
                    mCellArray[i, j].PropertyChanged += new PropertyChangedEventHandler(eventHandler); //subscribe too all cell's property changed event
                }
            }
        }

        //returns the cell at a particular row and column
        public Cell getCell(int rowIndex, int columnIndex)
        {
            if (rowIndex > mRows || columnIndex > mColumns) // validates that the cell is within range
            {
                return null;
            }
            return mCellArray[rowIndex, columnIndex];
        }

        //Finds the coordinates of a cell given the cell name. Returns a tuple
        public Tuple<int, int> findCell(string cellName)
        {
            int colIndex = -1, rowIndex = -1, result = 0;
            string row = null;
           
            if (int.TryParse(cellName[1].ToString(), out result))
            {
                colIndex = cellName[0] - 65;
                row = cellName.Substring(1);
                rowIndex = Convert.ToInt32(row) - 1; // converts rowIndex to int
            }

            Tuple<int, int> t = new Tuple<int, int>(rowIndex, colIndex); //wrap rowIndex and colIndex in tuple

            return t;
        }


        //removes old cell dependencies
        public void removeOldDependencies(Cell curCell, string exp)
        {
            List<string> list = new List<string>();
            foreach (string name in mDependencies.Keys)
            {
                if (!exp.Contains(name)) //if cell that is referenced is not the expression
                {
                    mDependencies[name].Remove(curCell); //remove dependency
                }
            }
        }


        //adds dependencies to the dependency table
        public void addDependencies(Cell curCell, string[] varNames)
        {
            HashSet<Cell> hs = new HashSet<Cell>();
            foreach (string varName in varNames) //for every variable name
            {
                if (mDependencies.Keys.Contains(varName))
                {
                    mDependencies[varName].Add(curCell);
                }
                else
                {
                    hs.Add(curCell);
                    mDependencies.Add(varName, hs);
                }

            }
        }

        //reclculates all cells that are dependent on a changing cell.
        public void Recalculate(Cell curCell)
        {
            double result = 0.0;

            if (mDependencies.Keys.Contains(getCellName(curCell)) && mDependencies[getCellName(curCell)].Count > 0)
            {
                foreach (Cell c in mDependencies[getCellName(curCell)].ToList()) //for every cell in list of references
                {
                    ExpTree tree;
                    if (c.Text == "")
                    {
                        mCellArray[c.RowIndex, c.ColumnIndex].SetValue(0.ToString());
                    }
                    else
                    {
                        tree = new ExpTree(c.Text.Substring(1));
                        setTreeVars(ref tree);
                        result = tree.Eval();
                        mCellArray[c.RowIndex, c.ColumnIndex].SetValue(result.ToString());
                        NotifyPropertyChanged(c, result.ToString());
                    }
                }
            }

        }


        //function that sets the var values in the tree dictionary for any given tree
        public void setTreeVars(ref ExpTree t)
        {
           
                foreach (string varName in t.getVarNames())
                {
                    int rowIndex = findCell(varName).Item1;
                    int columnIndex = findCell(varName).Item2;
                    double value = 0.0;
                    //set variable value in tree to value in the spreadsheet
                    if (double.TryParse(mCellArray[rowIndex, columnIndex].Value, out value))
                    {
                        t.SetVar(varName, value);
                    }
                }
            
        }

        public string getCellName(Cell cell)
        {
            string letter = ((char)(cell.ColumnIndex + 65)).ToString();
            string rowNumber = (cell.RowIndex + 1).ToString();
            return (letter + rowNumber);
        }


        //Adds an undo cmd to the undo stack
        public void AddUndo(UndoRedoCollection item)
        {
            mUndoRedo.AddUndo(item);
        }

        public void Undo()
        {
            mUndoRedo.performUndo().Undo(); // perform undo of item at top of undo stack
        }

        public void Redo()
        {
            mUndoRedo.performRedo().Redo(); // perform redo of item at top of redo stack
        }

        public bool CanUndo()
        {
            if (mUndoRedo.IsUndoEmpty() == true) // empty so cant undo
            {
                return false;
            }

            return true; // not empty
        }

        public bool CanRedo()
        {
            if (mUndoRedo.IsRedoEmpty() == true)
            {
                return false;
            }

            return true; // not empty
        }

        //returns task in string form for menu
        public string GetUndoTask()
        {
            return mUndoRedo.GetUndoMessage();
        }

        public string GetRedoTask()
        {
            return mUndoRedo.getRedoMessage();
        }

        //Loads spreadsheet data from a file
        public void Load(Stream source)
        {
            XDocument doc = XDocument.Load(source);

            foreach (XElement element in doc.Root.Elements("Cell"))
            {
                Cell cell =
                    mCellArray[
                        int.Parse(element.Element("Row").Value.ToString()), //parse out the row and column
                        int.Parse(element.Element("Column").Value.ToString())];
                cell.Text = element.Element("Text").Value.ToString();
                cell.BGColor = int.Parse(element.Element("BGColor").Value.ToString());//grab bg color
            }
        }

        //saves spreadsheet data to an xml document
        public void Save(Stream destination)
        {
            XmlWriter xml = XmlWriter.Create(destination);

            xml.WriteStartDocument();
            xml.WriteStartElement("Spreadsheet");

            foreach (Cell cell in mCellArray) // go through every cell in the logic layer
            {
                if (cell.BGColor != -1 || cell.Value != "" || cell.Text != "") // check if bg color, text, or value has been changed and needs to be saved
                {
                    xml.WriteStartElement("Cell"); // creates starting element tag, Cell for start
                    xml.WriteElementString("Value", cell.Value.ToString()); // save value
                    xml.WriteElementString("Text", cell.Text.ToString()); // save text
                    xml.WriteElementString("BGColor", cell.BGColor.ToString()); // text bg color
                    xml.WriteElementString("Column", cell.ColumnIndex.ToString()); // save column 
                    xml.WriteElementString("Row", cell.RowIndex.ToString()); // save row
                    xml.WriteEndElement(); // end
                }
            }

            xml.WriteEndElement();
            xml.Close();
        }

        //checks whether or not there is a circular reference in any of the dependencies
        private bool CheckCircRef(Cell cell, Cell cell2)
        {
            if (mDependencies.ContainsKey(getCellName(cell2)) && //if there is a circ reference
                mDependencies[getCellName(cell2)].Contains(cell))
            {
                return true;
            }

            Stack<Cell> temp = new Stack<Cell>();

            foreach (string name in mDependencies.Keys)
            {
                if (mDependencies[name].Contains(cell))
                {
                    //push cell to the stack
                    temp.Push(getCell(Convert.ToInt32(name.Substring(1)) - 1, Convert.ToInt32(name[0] - 65)));
                }
            }

            while (temp.Count > 0)
            {
                if (CheckCircRef(temp.Pop(), cell2))
                {
                    return true;
                }
            }

            return false;

        }

        //checks to see if the cell references itself
        private bool CheckSelfRef(Cell curCell)
        {
            if (mDependencies.ContainsKey(getCellName(curCell)) && mDependencies[getCellName(curCell)].Contains(curCell))
            {
                return true;
            }

            return false;
        }

        //Determines whether or not there is a bad reference in a formula
        //uses var names from the current tree being built for evaluation
        private bool CheckBadRef(string[] varNames)
        {
            int result = 0;
            foreach (string name in varNames)
            {
                if (!int.TryParse(name[1].ToString(), out result))
                {
                    return true;
                }
                else if (int.TryParse(name.Substring(1), out result)) //try to parse the row number of var name
                {
                    if (result > 50 || result < 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        //changes value of random cells to show that UI updates work from logic layer
        public void demo()
        {
            int randCol = 0, randRow = 0;
            Random ranNum = new Random();
            for (int i = 0; i < 50; i++) // 50 random hello worlds
            {
                randCol = ranNum.Next(26);
                randRow = ranNum.Next(50);
                mCellArray[randRow, randCol].Text = "Hello World";
            }

            for (int i = 0; i < 50; i++) // changes every cell in column B to statement (i.e "B1"
            {
                mCellArray[i, 1].Text = "This is Cell B" + (i + 1).ToString();
            }

            for (int i = 0; i < 50; i++) // changes every cell in column A to value to the right of it (i.e "B2" since col B is next to A)
            {
                mCellArray[i, 0].Text = "=B" + (i + 1).ToString();
            }
        }

    }
}
