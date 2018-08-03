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
    public class RestoreText : UndoRedoCollection
    {
        //constructor - inherits from UndoRedoCollection
        public RestoreText(string task, Stack<string> prevText, Stack<string> curText,
            Stack<Cell> prevCell, Stack<Cell> curCell)
        {
            mTask = task;
            mPrevText = prevText;
            mCurText = curText;
            mPrevCell = prevCell;
            mCurCell = curCell;
        }

        public override void Undo()
        {
            Stack<Cell> tempCell = new Stack<Cell>(mPrevCell); // create copy of previous cells
            Stack<string> tempText = new Stack<string>(mPrevText); // create copy of old text

            while (mPrevCell.Count > 0)
            {
                mPrevCell.Pop().Text = mPrevText.Pop(); // set cell text back to previous text
            }

            mPrevText = tempText; // save for redos
            mPrevCell = tempCell;
        }

        public override void Redo()
        {
            Stack<Cell> tempCell = new Stack<Cell>(mCurCell); // create copy of previous cells
            Stack<string> tempText = new Stack<string>(mCurText); // create copy of old text

            while (mCurCell.Count > 0)
            {
                mCurCell.Pop().Text = mCurText.Pop(); // set cell text back to previous text
            }

            mCurText = tempText; // save for redos
            mCurCell = tempCell;
        }

    }

}
