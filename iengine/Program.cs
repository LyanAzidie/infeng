using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iengine
{
    public class Program
    {
        static void Main(string[] args)
        {
            /********* Parse Files From Given Arguments **************/
            string fileName = "";
            string method = "";

            if (args.Length == 0)
            {
                throw new ArgumentNullException("No arguments were provided.");
            }
            else if (args.Length != 2)
            {
                throw new ArgumentException("Use of arguments: search <filename> <method>");
            }
            else
            {
                fileName = args[1];
                method = args[0];
            }

            // get pLogic and asked
            FileParsingExtension file = new FileParsingExtension(fileName);
            List<string> pLogic = file.Query;
            List<string> asked = file.Asked;

            // set up for algo
            Expression kb = new Expression("", "&");
            Expression goal = new Expression();
            string yesNo = "NO";

            // tested data
            //List<string> pLogic = new List<string> { "p2=> p3", "p3 => p1", "c => e", "b&e => f", "f&g => h", "p1=>d", "p1&p3 => c", "a", "b", "p2" };
            //List<string> pLogic = new List<string> { "a", "b"};
            //List<string> pLogic = new List<string> { "a&p2=> p3", "~a||~b||d&p2 => p1", "a", "b", "p2", "d" };
            //List<string> pLogic = new List<string> { "p1 & p2 & p3 & p4 & p5 & p6 & p7 => p8", "p1 & p3 & p4 & p6 & p7 & p8 => p9", "p6 & p7 & p4 & p5 & p9 => p10", "p13 & p12 & p11 => p14", "p11 & p12 & p4 & p5 & p10 => p13", "p11 & p1 & p4 & p5 & p6 => p12", "p1 & p2 & p6 & p8 & p10 => p11", "p1 & p2 & p6 & p8 => p", "p1", "p2", "p3", "p4", "p5", "p6", "p7"};
            //List<string> pLogic = new List<string> { "a <=> c => ~d & b & b => a", "c", "~f || g" };
            //List<string> asked = new List<string> { "d" };

            /********* TTEntails **************/
            if (method == "TT")
            {
                // the KB expression is a conjunction of all expressions
                kb.BuildExpression(pLogic, false);

                // goal expression created using the reusable function
                goal.BuildExpression(asked, false);

                TTEntail ttInf = new TTEntail(kb, goal);
                if (ttInf.TT_Entail())
                {
                    yesNo = "YES: ";
                    Console.WriteLine(yesNo + ttInf.ModelNum);
                }
                else
                {
                    Console.WriteLine(yesNo);
                }
            }
            else if (method == "FC")
            {
                kb.BuildExpression(pLogic, true);
                goal.BuildExpression(asked, false);

                FC fc = new FC(kb, goal);
                yesNo = "NO";
                if (fc.FCEntail())
                {
                    yesNo = "YES: ";
                    Console.Write(yesNo);
                    for (int i = 0; i < fc.Path().Count; i++)
                    {
                        Console.Write(fc.Path()[i]);
                        if (i != fc.Path().Count - 1)
                            Console.Write(", ");
                    }
                }
                else
                {
                    Console.Write(yesNo);
                }
            }
            /********** End : TT Entail **************/

            /********* TEST : BCEntails **************/
            else if (method == "BC")
            {
                kb.BuildExpression(pLogic, true);
                goal.BuildExpression(asked, false);

                BC bc = new BC(kb, goal);
                yesNo = "NO";
                if (bc.BCEntail())
                {
                    yesNo = "YES: ";
                    Console.Write(yesNo);
                    for (int i = bc.Path.Count; i > 0; i--)
                    {
                        Console.Write(bc.Path[i - 1]);
                        if (i != 1)
                            Console.Write(", ");
                    }
                }
                else
                {
                    Console.Write(yesNo);
                }
            }
            /********** End : BCEntails Test **************/

            Console.ReadLine();
        }
    }
}
