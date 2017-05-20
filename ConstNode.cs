using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
    * Name: Ashton Summers
    * Date: 9/30/16
    * ID: 11414124
    * Assignment Description: Creates an expression tree based on user inputs. For this assignment, users will not enter an
    *              expression with multiple operators. Once the tree is built, the user has the choice to evaluate 
    *              its result 
*/

namespace CptS321
{
    public class ConstNode : Node
    {
        //member variables
        private double mValue;
        
        //constructor
        public ConstNode(double newValue)
        {
            mValue = newValue;
        }

        //public field that exposes private mValue member variable
        public double Value
        {
            get { return mValue; }
            set { mValue = value; }
        }
    }
}
