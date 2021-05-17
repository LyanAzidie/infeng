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
                            }
                            else if (checkRight.BCEntail())
                            {
                                foreach (string s in checkRight.Path)
                                {
                                    if (!Path.Contains(s))
                                        Path.Add(s);
                                }
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
                                        Path.Add(s);
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
