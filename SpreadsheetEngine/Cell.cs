/*
 
    * Name: Ashton Summers
    * ID: 11414124
    * Date created: 9/19/16
 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{

    public abstract class Cell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //Member variables
        private int mRowIndex;
        private int mColumnIndex;
        private string mText;
        private int mBGColor;
        protected string mValue;


        //Constructor
        public Cell(int rowIndex, int columnIndex)
        {
            mRowIndex = rowIndex;
            mColumnIndex = columnIndex;
            mText = "";
            mValue = "";
            mBGColor = -1;
        }

        //notifies subscribers when changes are made to the cell class properties
        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //Public read-only property that exposes mRowIndex field to outside world
        public int RowIndex
        {
            get
            {
                return mRowIndex;
            }
        }

        //public read-only property that exposes mColumnIndex field to outside world
        public int ColumnIndex
        {
            get
            {
                return mColumnIndex;
            }
        }

        //property that accesses protected member variable mText
        //property changed event is raised when mText value is changed
        public string Text
        {
            get { return mText; }

            set
            {
                if (value != this.mText)
                {
                    this.mText = value;
                    NotifyPropertyChanged("Text"); //notify subsribers that property has changed
                }
            }
        }

        //public background color property that exposes private mBGColor
        public int BGColor
        {
            get { return mBGColor; }

            set
            {
                if (value != this.mBGColor)
                {
                    this.mBGColor = value;
                    NotifyPropertyChanged("BGColor");
                }
            }
        }

        //public read only property that exposes the mValue field
        public string Value
        {
            get { return mValue; }
        }

        //allows inheriting classes to set the value of mValue. Inheriting class is internal so the Spreadsheet class is the 
        //only source that can touch mValue
        internal void SetValue(string newValue)
        {
            mValue = newValue;
        }

    }

}

