using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iengine
{
    public class TTEntail
    {
        private Expression _kb;
        private Expression _goal;
        //private int i = 0;
        private int _modelNum = 0;

        public int ModelNum { get => _modelNum; set => _modelNum = value; }

        public TTEntail(Expression KB, Expression asked)
        {
            _kb = KB;
            _goal = asked;
        }

        // handles the process of gathering all variables
        // get everything ready for TT-algorithm
        public bool TT_Entail()
        {
            // get all existing variables
            List<string> variables = GetAllVariables(_kb);
            variables.AddRange(GetAllVariables(_goal));

            Dictionary<string, bool> model = new Dictionary<string, bool>();

            // making sure there are no duplications of elements
            variables = variables.Distinct().ToList();

            return TT_Check(variables, model);
        }

        // check all expressions for entailment
        public bool TT_Check(List<string> variables, Dictionary<string, bool> model)
        {
            if (variables.Count == 0)
            {
                // USE THIS BLOCK OF CODE TO PRINT THE TRUTH TABLE
                /*
                Console.Write(++i + "/ ");
                foreach (KeyValuePair<string, bool> m in model)
                {
                    Console.Write(m.Key + " - " + m.Value + "; ");
                }
                Console.Write("KB: " + PLTrue(_kb, model) + ". " + "KB|="+ _goal.Children[0].Variable + ": " + PLTrue(_goal, model));
                Console.WriteLine();
                */

                // check if KB is true
                if (PLTrue(_kb, model))
                {
                    // count models
                    if (model[_goal.Variable])
                        ModelNum++;
                    return PLTrue(_goal, model);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                // first variable
                string firstVar = variables[0];

                // the rest of the variables not including first
                List<string> otherVars = new List<string>();
                for (int i = 1; i < variables.Count; i++)
                {
                    otherVars.Add(variables[i]);
                }

                // add a new argument with firstVar == true
                Dictionary<string, bool> firstTrue = new Dictionary<string, bool>(model);
                firstTrue.Add(firstVar, true);

                // add a new argument with firstVar == false
                Dictionary<string, bool> firstFalse = new Dictionary<string, bool>(model);
                firstFalse.Add(firstVar, false);

                return TT_Check(otherVars, firstTrue) && TT_Check(otherVars, firstFalse);
            }
        }

        // check that the logical statement fits the model given
        private bool PLTrue(Expression exp, Dictionary<string, bool> model)
        {
            if (exp.Variable != "")
            {
                return model[exp.Variable];
            }
            else if (exp.Operation == "&")
            {
                foreach (Expression child in exp.Children)
                {
                    if (PLTrue(child, model) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (exp.Operation == "||")
            {
                foreach (Expression child in exp.Children)
                {
                    if (PLTrue(child, model) == true)
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (exp.Operation == "=>")
            {
                Expression lHS = exp.Children[0];
                Expression rHS = exp.Children[1];

                if (PLTrue(lHS, model) == true && PLTrue(rHS, model) == false)
                {
                    return false;
                }
                return true;
            }
            else if (exp.Operation == "<=>")
            {
                Expression lHS = exp.Children[0];
                Expression rHS = exp.Children[1];

                return PLTrue(lHS, model) == PLTrue(rHS, model);
            }
            else if (exp.Operation == "~")
            {
                return !PLTrue(exp.Children[0], model);
            }
            // buffer
            else if (exp.Operation == "")
            {
                return PLTrue(exp.Children[0], model);
            }
            return false;
        }

        // get all the variables in a tree
        private List<string> GetAllVariables(Expression exp)
        {
            List<string> result = new List<string>();

            // add variables to the list if it exists
            // statement act as a guard for statement below
            if (exp.Variable != "")
            {
                result.Add(exp.Variable);
                return result;
            }

            // check the children's variables
            foreach (Expression child in exp.Children)
            {
                result.AddRange(GetAllVariables(child));
            }
            return result;
        }
    }
}
