/*
 
    * Name: Ashton Summers
    * ID: 11414124
    * Date created: 9/19/16
 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CptS321
{
    //Internal class that inherits from abstract base class
    internal class CellChild : Cell
    {
        //calls abstract base class constructor
        public CellChild(int rowIndex, int columnIndex) : base(rowIndex, columnIndex)
        {
        }

    }
}
