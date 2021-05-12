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

        public Expression BuildExpression(List<string> expressions)
        {
            // create a tree for pLogic
            foreach (string l in expressions)
            {
                // erase all spaces in each expressions
                // NOTE: a good idea to put this when splitting the big string
                string nSp = Regex.Replace(l, @"\s+", "");

                Children.Add(CheckOps(nSp));

                //Console.WriteLine(Regex.Match(nSp, @"(\w+=>\w+)|(\w+&\w+=>\w+)|(\w+\|\w+=>\w+)"));
            }
            return this;
        }

        // check and build expressions if there are operations
        private Expression CheckOps(string exp)
        {
            Expression newExp = new Expression();

            // add as variables if no operations are found
            // guard against all the statement below
            if (!HasOps(exp))
            {
                newExp.Variable = exp;
                return newExp;
            }

            // determine what operation exists
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

            // split into rhs and lhs
            string[] sections = exp.Split(new string[] { op }, StringSplitOptions.RemoveEmptyEntries);
            newExp.Operation = op;

            // check the children nodes (creating a recursion effect)
            newExp.Children.Add(CheckOps(sections[0]));
            newExp.Children.Add(CheckOps(sections[1]));

            return newExp;
        }

        // check if there are operations in the string
        private bool HasOps(string exp)
        {
            return Regex.IsMatch(exp, @"[&\|=><=>~]");
        }
    }
}
