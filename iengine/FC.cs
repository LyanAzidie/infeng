using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iengine
{
    public  class FC
    {
        private Dictionary<Expression, int> _count = new Dictionary<Expression, int>();
        private Dictionary<string, bool> _inferred = new Dictionary<string, bool>();
        private List<Expression> _agenda = new List<Expression>();
        private Expression _kb;
        private Expression _goal;

        public FC(Expression kb, Expression asked)
        {
            Kb = kb;
            Goal = asked;
            List<string> temp = GetAllVariables(kb);
            temp = temp.Distinct().ToList();
            foreach (string var in temp)
            {
                _inferred.Add(var, false);
            }

            foreach (Expression child in Kb.Children)
            {
                _count.Add(child, child.Height);

                if (child.Height == 0)
                {
                    _agenda.Add(child);
                    //_inferred[child.Variable] = true;
                }

            }

            
        }

        public Expression Kb { get => _kb; set => _kb = value; }
        public Expression Goal { get => _goal; set => _goal = value; }
        

        public bool FCEntail()
        {
            while(_agenda.Count > 0)
            {
                Expression CurrentSearchNode = _agenda[0];
                _agenda.Remove(CurrentSearchNode);
                if (!_inferred[CurrentSearchNode.Variable])
                {
                    _inferred[CurrentSearchNode.Variable] = true;
                    foreach (Expression child in Kb.Children)
                    {
                        if (child.Variable == "")
                        {
                            if (CurrentSearchNode.Variable == child.Children[0].Variable)
                            {
                                _count[child]--;
                                if (_count[child] == 0)
                                {
                                    if (child.Children[1].Variable == Goal.Variable)
                                    {
                                        _inferred[Goal.Variable] = true;
                                        return true;
                                    }
                                    _agenda.Add(child.Children[1]);
                                }

                            }
                            else if(HasVars(child.Children[0], CurrentSearchNode))
                            {
                                if(child.Children[0].Operation == "&")
                                {
                                    Expression curLeft = child.Children[0].Children[1];
                                    Expression curRight = child.Children[0].Children[0];

                                    for (int i = child.Height; i > 1; i--)
                                    {
                                        if (curLeft.Variable != "")
                                        {
                                            if (_inferred[curLeft.Variable] && curLeft.Variable == CurrentSearchNode.Variable)
                                                _count[child]--;
                                        }

                                        if (curRight.Variable != "")
                                        {
                                            if (_inferred[curRight.Variable] && curRight.Variable == CurrentSearchNode.Variable)
                                                _count[child]--;
                                        }
                                        else
                                        {
                                            curLeft = curRight.Children[1];
                                            curRight = curRight.Children[0];
                                        }
                                    }

                                    if (_count[child] == 0)
                                    {
                                        if (child.Children[1].Variable == Goal.Variable)
                                        {
                                            _inferred[Goal.Variable] = true;
                                            return true;
                                        }
                                        _agenda.Add(child.Children[1]);
                                    }
                                }

                            }
                        }
                        
                    }
                }
            }
            return false;
        }
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
        public List<string> Path()
        {
            List<string> lpath = new List<string>();
            foreach (KeyValuePair<string, bool> kvp in _inferred)
            {
                if (kvp.Value)
                {
                    lpath.Add(kvp.Key);
                }
                
            }
            return lpath;
        }
        private bool HasVars(Expression node, Expression CurrentSearchNode)
        {
            if (node.Height > 0)
            {
                if (node.Children[0].Variable == CurrentSearchNode.Variable || node.Children[1].Variable == CurrentSearchNode.Variable)
                {
                    return true;
                }

                return HasVars(node.Children[0], CurrentSearchNode) || HasVars(node.Children[1], CurrentSearchNode);
            }
            else
            {
                return node.Variable == CurrentSearchNode.Variable;
            }
            return false;
        }

    }
}
