using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iengine
{
    public class BC
    {
        private Expression _kb;
        private Expression _goal;
        private Expression _curSearchNode;
        private List<string> _path = new List<string>();

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

        // initialise search process
        public bool BCEntail()
        {
            Path.Add(Goal.Variable);
            bool notExhausted = true;

            // could not figure out how to check which one is trueee
            while (Path.Last() !=  || notExhausted)
            {
                foreach (Expression child in Kb.Children)
                {
                    string temp = Path.Last();
                    CheckNode(child);
                    if (temp != Path.Last())
                    {
                        break;
                    }

                    if (Kb.Children.Last() == child)
                    {
                        notExhausted = false;
                    }
                }
            }

            // check goal is already found
            if (Path.Last() == Goal.Variable)
            {
                return true;
            }
            return false;
        }

        private void CheckNode(Expression node)
        {
            // check node is goal
            if (IsSameNode(CurSearchNode, node))
            {
                return;
            }
            // check child one
            else if (node.Children.Count == 2)
            {
                if (IsSameNode(CurSearchNode, node.Children[0]))
                {
                    Path.Add(node.Children[1].Variable);
                    CurSearchNode = node.Children[1];
                    return;
                }
                else if (IsSameNode(CurSearchNode, node.Children[1]))
                {
                    Path.Add(node.Children[0].Variable);
                    CurSearchNode = node.Children[0];
                    return;
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
