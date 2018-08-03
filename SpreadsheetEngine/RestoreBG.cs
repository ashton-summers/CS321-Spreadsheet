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
    public class RestoreBG : UndoRedoCollection
    {

        //constructor
        public RestoreBG(string task, Stack<Cell> prevCell, Stack<Cell> curCell,
            Stack<int> prevColor, Stack<int> curColor)
        {
            mTask = task;

            mPrevCell = prevCell;
            mCurCell = curCell;

            mPrevColor = prevColor;
            mCurColor = curColor;
        }

        public override void Undo()
        {
            Stack<Cell> tempCell = new Stack<Cell>(mPrevCell); // create copy of previous cells
            Stack<int> tempCol = new Stack<int>(mPrevColor); // create copy of old colors

            while (mPrevCell.Count > 0)
            {
                if (mPrevColor.Peek() == 0) // check to see if color is orignal, unchanged white
                {
                    mPrevCell.Pop().BGColor = -1; // basic white color
                    mPrevColor.Pop();
                }
                else
                {
                    mPrevCell.Pop().BGColor = mPrevColor.Pop(); // set cell color back to old cell color
                }

            }

            mPrevColor = tempCol; // save for redos
            mPrevCell = tempCell;
        }

        public override void Redo()
        {
            Stack<Cell> tempCell = new Stack<Cell>(mPrevCell); // create copy of previous cells
            Stack<int> tempCol = new Stack<int>(mCurColor); // create copy of cur colors

            while (mCurCell.Count > 0)
            {
                mCurCell.Pop().BGColor = mCurColor.Pop(); // set cell text back to newer color
            }

            mCurColor = tempCol; // save for redos
            mCurCell = tempCell;
        }
    }
}
