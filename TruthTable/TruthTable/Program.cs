using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TruthTable
{
    public class Program
    {
        static void Main(string[] args)
        {
            // assumed given data
            List<string> pLogic = new List<string>{ "p2=> p3", "p3 => p1", "c => e", "b&e => f", "f&g => h", "p1=>d", "p1&p3 => c", "a", "b", "p2" };
            List<string> asked = new List<string> { "c" };

            // the KB expression is a conjunction of all expressions
            Expression kb = new Expression("", "&");
            kb.BuildExpression(pLogic);

            // goal expression created using the reusable function
            Expression goal = new Expression();
            goal.BuildExpression(asked);

            TT ttInf = new TT(kb, goal);
            string yesNo = "NO: ";
            if (ttInf.TT_Entail())
                yesNo = "YES: ";
            Console.WriteLine(yesNo + ttInf.ModelNum);
            Console.ReadLine();
        }
    }
}
