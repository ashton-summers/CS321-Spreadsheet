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
    public class VarNode : Node
    {
        //member variable
        private string mVarName;

        //constructor
        public VarNode(string newVarName)
        {
            mVarName = newVarName;
        }

        //public field that exposes the private mVarName member variables
        public string VarName
        {
            get { return mVarName; }
            set { mVarName = value; }
        }
    }
}
