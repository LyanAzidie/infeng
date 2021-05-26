using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace iengine
{
    public class BC
    {
        private Expression _kb;
        private Expression _goal;
        private Expression _curSearchNode;
        private List<string> _path = new List<string>();
        private bool _pathFound = false;

        public BC(Expression kb, Expression asked)
        {
            Kb = kb;
            Goal = asked;
            if (asked.Operation == "")
                CurSearchNode = asked;
        }

        public Expression Kb { get => _kb; set => _kb = value; }
        public Expression Goal { get => _goal; set => _goal = value; }
        public Expression CurSearchNode { get => _curSearchNode; set => _curSearchNode = value; }
        public List<string> Path { get => _path; set => _path = value; }
        public bool PathFound { get => _pathFound; set => _pathFound = value; }

        // initialise search process
        public bool BCEntail()
        {
            // check if the goal asked was more than just a variable (more than 0 in height)
            if (Goal.Variable == "")
            {
                // a children of only 1 represents a NOT operation
                if (Goal.Children.Count == 1)
                {
                    BC child = new BC(Kb, Goal.Children[0]);
                    bool entails = child.BCEntail();

                    if (!entails)
                    {
                        foreach (String e in child.Path)
                        {
                            Path.Add(e);
                        }
                    }
                    return !entails;
                }
                else
                {
                    // envaluate both sides and add to path if true
                    BC leftChild = new BC(Kb, Goal.Children[0]);
                    BC rightChild = new BC(Kb, Goal.Children[1]);
                    bool entailLeft = leftChild.BCEntail();
                    bool entailRight = rightChild.BCEntail();
                    if (entailLeft)
                    {
                        foreach (String e in leftChild.Path)
                        {
                            Path.Add(e);
                        }
                    }
                    if (entailRight)
                    {
                        foreach (String e in rightChild.Path)
                        {
                            Path.Add(e);
                        }
                    }

                    // based on the operation, return the corresponding result
                    if (Goal.Operation == "&")
                    {
                        return entailLeft && entailRight;
                    }
                    else if (Goal.Operation == "||")
                    {
                        return entailLeft || entailRight;
                    }
                    else if (Goal.Operation == "=>")
                    {
                        if (entailLeft && !entailRight)
                        {
                            return false;
                        }
                        return true;
                    }
                    else if (Goal.Operation == "<=>")
                    {
                        return entailLeft == entailRight;
                    }
                }
                return false;
            }

            Path.Add(Goal.Variable);
            bool notExhausted = true;

            // loop until exhausted
            while (notExhausted)
            {
                foreach (Expression child in Kb.Children)
                {
                    string temp = Path.Last();
                    CheckNode(child);
                    if (temp != Path.Last() || PathFound)
                    {
                        if (PathFound)
                            notExhausted = false;
                        break;
                    }
                    if (Kb.Children.Last() == child)
                    {
                        notExhausted = false;
                    }
                }
            }

            return PathFound;
        }

        private void CheckNode(Expression node)
        {
            // check node is goal
            if (IsSameNode(CurSearchNode, node))
            {
                PathFound = true;
                return;
            }
            // check children
            else if (node.Children.Count == 2)
            {
                if (Regex.IsMatch(node.Children[1].Variable, CurSearchNode.Variable))
                {
                    if (node.Children[0].Operation != "")
                    {
                        CheckNode(node.Children[0]);
                    }
                    if (node.Children[1].Operation != "")
                    {
                        CheckNode(node.Children[1]);
                    }
                    if (IsSameNode(CurSearchNode, node.Children[1]))
                    {
                        // case all variables (=> or <=>)
                        if (node.Children[0].Operation == "" && node.Children[1].Operation == "")
                        {
                            Path.Add(node.Children[0].Variable);
                            CurSearchNode = node.Children[0];
                            return;
                        }
                        // case &
                        if (node.Children[0].Operation == "&")
                        {
                            // its trying to find the sign so finding variable ""
                            BC checkLeft = new BC(Kb, node.Children[0].Children[0]);
                            BC checkRight = new BC(Kb, node.Children[0].Children[1]);
                            if (checkLeft.BCEntail() && checkRight.BCEntail())
                            {
                                foreach (string s in checkLeft.Path)
                                {
                                    if (!Path.Contains(s))
                                        Path.Add(s);
                                }
                                foreach (string s in checkRight.Path)
                                {
                                    if (!Path.Contains(s))
                                        Path.Add(s);
                                }
                                PathFound = true;
                                return;
                            }
                            return;
                        }
                        // case ||
                        else if (node.Children[0].Operation == "||")
                        {
                            BC checkLeft = new BC(Kb, node.Children[0].Children[0]);
                            BC checkRight = new BC(Kb, node.Children[0].Children[1]);
                            if (checkLeft.BCEntail())
                            {
                                foreach (string s in checkLeft.Path)
                                {
                                    if (!Path.Contains(s))
                                        Path.Add(s);
                                }
                                PathFound = true;
                            }
                            if (checkRight.BCEntail())
                            {
                                foreach (string s in checkRight.Path)
                                {
                                    if (!Path.Contains(s))
                                        Path.Add(s);
                                }
                                PathFound = true;
                            }
                            return;
                        }
                        // case ~
                        else if (node.Children[0].Operation == "~")
                        {
                            BC checkLeft = new BC(Kb, node.Children[0].Children[0]);
                            if (!checkLeft.BCEntail())
                            {
                                foreach (string s in checkLeft.Path)
                                {
                                    if (!Path.Contains(s))
                                    {
                                        Path.Add(s);
                                        PathFound = true;
                                    }
                                }
                            }
                            return;
                        }
                    }
                }
            }
            else
            {
                foreach (Expression child in node.Children)
                {
                    CheckNode(child);
                }
            }
        }

        private bool IsSameNode(Expression aNode, Expression bNode)
        {
            return aNode.Variable == bNode.Variable && aNode.Operation == bNode.Operation;
        }
    }
}
