using CptS321;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 
    * Name: Ashton Summers
    * ID: 11414124
    * Date created: 9/19/16
 
 */

namespace SpreadsheetEngine
{
    public class UndoRedoCollection // collection of commands
    {
        protected string mTask;
        protected Stack<string> mPrevText = new Stack<string>();
        protected Stack<string> mCurText = new Stack<string>();
        protected Stack<int> mPrevColor = new Stack<int>();
        protected Stack<int> mCurColor = new Stack<int>();
        protected Stack<Cell> mPrevCell = new Stack<Cell>();
        protected Stack<Cell> mCurCell = new Stack<Cell>();

        public virtual void Undo() { }
        public virtual void Redo() { }

        public string Task
        {
            get { return mTask; }
        }

    }
}
