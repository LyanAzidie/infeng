
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace iengine
{
    public class FileParsingExtension
    {
        private List<string> clauses;
        private List<string> asked;
        private List<string> query;
        public string filePath;

        public List<string> Clauses { get => clauses; protected set => clauses = value; }
        public List<string> Asked { get => asked; protected set => asked = value; }
        public List<string> Query { get => query; set => query = value; }

        public FileParsingExtension(string path)
        {
            filePath = path;
            ReadFile();
        } 

        // read the file specified
        private void ReadFile()
        {
            string[] lines;
            // open and reads file
            try
            {
                lines = File.ReadAllLines(filePath);
            }
            catch (Exception e)
            {
                throw new FileLoadException("Cannot Open File");
            }

            bool writeTells = false;
            bool writeAsked = false;

            foreach (string line in lines)
            {
                if (line == "TELL")
                {
                    writeAsked = false;
                    writeTells = true;
                }
                else if (line == "ASK")
                {
                    writeTells = false;
                    writeAsked = true;
                }
                else if (writeTells)
                {
                    Query = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    writeAsked = false;
                    writeTells = false;
                }
                else if (writeAsked)
                {
                    Asked = new List<string> { line };
                    writeAsked = false;
                    writeTells = false;
                }
            }
        }
    }
}

