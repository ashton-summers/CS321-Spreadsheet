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
    public class OpNode : Node
    {
        //member variables
        private char mOperator;
        private Node mLeft;
        private Node mRight;

        //constructor
        public OpNode(char newOperator)
        {
            mOperator = newOperator;
            mLeft = null;
            mRight = null;
        }

        //public field that exposes private mOperator member variable
        public char Operator
        {
            get { return mOperator; }
            set { mOperator = value; }
        }

        //public field that exposes mLeft private member variable
        public Node LeftNode
        {
            get { return mLeft; }
            set { mLeft = value; }
        }

        //public field that exposes mRight member variable
        public Node RightNode
        {
            get { return mRight; }
            set { mRight = value; }
        }

    }
}
