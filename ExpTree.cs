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
    public class ExpTree
    {
        private Dictionary<string, double> mVariables = new Dictionary<string, double>();
        private Node mRoot;

        //constructor
        public ExpTree(string expression)
        {
            mRoot = BuildTree(ref expression);
        }

        //Sets data for specified varaible in a dictionary
        public void SetVar(string varName, double varValue)
        {
            //If the key exists, update the value
            if (mVariables.ContainsKey(varName))
            {
                mVariables[varName] = varValue;
            }
            else //Key does not exist
            {
                mVariables.Add(varName, varValue);
            }
        }

        //constructs an exp tree based on expression passed into the constructor
        public Node BuildTree(ref string exp)
        {
            int index = GetOpIndex(ref exp); //gets the index of the right most operator
            string temp2 = exp;
            if (index == -1) //if at last portion of parsing, find whether it is a varable or number
            {
                return VarOrNum(exp);
            }

            Node temp = new OpNode(exp[index]); //create a temporary operator node to hold current operator

            if (temp != null)
            {
                exp = exp.Substring(index + 1);
                //finds everything to right of operator and determines if it a variable or number
                ((OpNode)temp).RightNode = BuildTree(ref exp);

                exp = temp2.Substring(0, index);
                //recursively breaks down the expression until we are at the beginning of the exp
                ((OpNode)temp).LeftNode = BuildTree(ref exp);
            }

            return temp;
        }

        //determines whether a substring is a variable or constant
        public Node VarOrNum(string s)
        {
            double num = 0;

            if (double.TryParse(s, out num)) // check to see if substr is a number
            {
                return new ConstNode(num); //return a constant node
            }
            else
            {

                VarNode var = new VarNode(s);
                if (!mVariables.ContainsKey(var.VarName)) //if key is not already in dictionary, give it initial value of 0
                {
                    mVariables.Add(var.VarName, 0);
                }

                return var; // return a variable node
            }
        }

        //Finds the index of the operator that should be performed last
        public int GetOpIndex(ref string exp)
        {
            int pCount = 0, index = -1;
            for (int i = exp.Length - 1; i >= 0; i--)
            {
                if ((exp[i] == '+' || exp[i] == '-') && pCount == 0)
                {
                    return i;
                }
                else if ((exp[0] == '(') && (exp[exp.Length - 1] == ')') && i == 0)
                {
                    exp = exp.Remove(0, 1);
                    exp = exp.Remove(exp.Length - 1, 1);
                    return GetOpIndex(ref exp);
                }
                else if (exp[i] == ')') { pCount++; }
                else if (exp[i] == '(') { pCount--; }
                else if ((exp[i] == '*' || exp[i] == '/') && pCount == 0 && index == -1)
                {
                    index = i;
                }

            }
            if (index != -1)
            {
                return index;
            }
            return -1;
        }

        //Finds and returns all variable names in the mVariables dictionary
        public string[] getVarNames()
        {
            List<string> list = new List<string>();

            foreach (KeyValuePair<string, double> kvp in mVariables)
            {
                list.Add(kvp.Key);
            }

            return list.ToArray();
        }


        //Evaluates an expression entered by the user
        public double Eval()
        {
            double result = CalculateValue(mRoot);

            return result;
        }

        //evaluates value to expression
        public double CalculateValue(Node n)
        {
            double left = 0.0, right = 0.0, result = 0.0;

            //check type of node passed in.
            if (n is ConstNode) { return ((ConstNode)n).Value; }
            else if (n is VarNode)
            {
                return mVariables[((VarNode)n).VarName]; //return the value for key varName
            }
            else
            {
                left = CalculateValue(((OpNode)n).LeftNode); //recursive call to evaluate left substree
                right = CalculateValue(((OpNode)n).RightNode); //recursive call to evaluate right subtree

                switch (((OpNode)n).Operator) //Do specific operation based on the operator in OpNode
                {
                    case '+':
                        result = left + right;
                        break;
                    case '*':
                        result = left * right;
                        break;
                    case '-':
                        result = left - right;
                        break;
                    case '/':
                        result = left / right;
                        break;
                }

                return result;
            }
        }
    }
}


