using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace iengine
{
    /***
     *  All logical expressions are expressed by a tree
     *  variable represents all the variable (letters) in the expressions
     *  operation represents the logical operation the node is
     * ***/
    public class Expression
    {
        private string _variable = "";
        private string _operation = "";
        private int _height = 0;

        private List<Expression> _children = new List<Expression>();

        public Expression()
        { }

        public Expression(string variable, string operation)
        {
            _variable = variable;
            _operation = operation;
        }

        public string Variable { get => _variable; set => _variable = value; }
        public string Operation { get => _operation; set => _operation = value; }
        public List<Expression> Children { get => _children; set => _children = value; }
        public int Height { get => _height; set => _height = value; }

        // create a tree for expression, sorted decides whether to sort the list or not
        public Expression BuildExpression(List<string> expressions, bool sorted)
        {
            // create a tree for pLogic
            foreach (string l in expressions)
            {
                // erase all spaces in each expressions
                // NOTE: a good idea to put this when splitting the big string
                string nSp = Regex.Replace(l, @"\s+", "");

                Expression child = CheckOps(nSp, Height, sorted);

                // depending on whether the user decides to sort the tree or not
                if (!sorted)
                {
                    if (expressions.Count == 1)
                    {
                        this.Variable = child.Variable;
                        this.Operation = child.Operation;
                        return this;
                    }
                    Children.Add(child);
                }
                else
                {
                    int size = Children.Count;
                    // sort based on height - if needed
                    for (int i = 0; i < size; i++)
                    {
                        if (child.Height < Children[i].Height)
                        {
                            Children.Insert(i, child);
                            break;
                        }

                        // case it belongs in the end of the list
                        if (i == Children.Count - 1)
                        {
                            Children.Add(child);
                        }
                    }

                    if (Children.Count == 0)
                        Children.Add(child);
                }

                //Console.WriteLine(Regex.Match(nSp, @"(\w+=>\w+)|(\w+&\w+=>\w+)|(\w+\|\w+=>\w+)"));
            }
            return this;
        }

        // check and build expressions if there are operations
        private Expression CheckOps(string exp, int h, bool sorted)
        {
            Expression newExp = new Expression();

            // add as variables if no operations are found
            // guard against all the statement below
            if (!HasOps(exp))
            {
                newExp.Height = 0;
                newExp.Variable = exp;
                return newExp;
            }

            // determine what operation exists
            string op = RetrieveOps(exp);
            
            // split into rhs and lhs
            string[] sections = exp.Split(new string[] { op }, StringSplitOptions.RemoveEmptyEntries);
            newExp.Operation = op;

            // check the children nodes (creating a recursion effect)
            Expression childLeft = CheckOps(sections[0], h+1, sorted);
            Expression childRight = CheckOps(sections[1], h+1, sorted);

            newExp.Children.Add(childLeft);
            newExp.Children.Add(childRight);

            // update the height of the node based on how many ops they have
            newExp.Height = UpdateHeight(newExp, 0);

            // decide priorities (for sorting purpose) - swap (only when sorted is true)
            if (sorted && HasOps(sections[0]) || childLeft.Height < childRight.Height)
            {
                Expression temp = newExp.Children[0];
                newExp.Children[0] = newExp.Children[1];
                newExp.Children[1] = temp;
            }

            return newExp;
        }

        // check if there are operations in the string
        private bool HasOps(string exp)
        {
            return Regex.IsMatch(exp, @"[&\|=><=>~]");
        }

        // returns the operation that exists
        private string RetrieveOps(string exp)
        {
            string op = "";
            // check if => exists
            if (Regex.IsMatch(exp, @"(\w+=>\w+)|(\w+&\w+=>\w+)|(\w+\|\w+=>\w+)"))
            {
                op = "=>";
            }
            // check if <=> exists
            else if (Regex.IsMatch(exp, @"(\w+<=>\w+)|(\w+&\w+<=>\w+)|(\w+\|\w+<=>\w+)"))
            {
                op = "<=>";
            }
            // check if & exists
            else if (Regex.IsMatch(exp, @"(\w+&\w+)"))
            {
                op = "&";
            }
            // check if | exists
            else if (Regex.IsMatch(exp, @"(\w+\|\w+)"))
            {
                op = "|";
            }
            // check if - exists
            else if (Regex.IsMatch(exp, @"(\w+~\w+)"))
            {
                op = "~";
            }

            return op;
        }

        // figure out the height of the node
        private int UpdateHeight(Expression exp, int curHeight)
        {
            // check all children
            for (int i = 0; i < exp.Children.Count; i++)
            {
                int tempResult = 1;
                // there exists an operation
                if (exp.Children[i].Operation != "")
                {
                    tempResult += UpdateHeight(exp.Children[i], curHeight+1);
                }
                // use the largest height as the node's height
                if (tempResult > curHeight)
                {
                    curHeight = tempResult;
                }
            }
            return curHeight;
        }
    }
}
