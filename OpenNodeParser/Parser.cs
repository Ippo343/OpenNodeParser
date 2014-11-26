using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenNodeParser
{
    public static class Parser
    {
        public static ConfigNode Parse(string nodeString)
        {
            Regex nameValueRegex = new Regex(@"(.+)=(.+)", RegexOptions.None);  // matches "name = value" pairs
            Regex wordRegex = new Regex(@"^[\w_]+$", RegexOptions.None);        // matches a single word on a line by itself

            #region Split lines and do sanity checks

            // Split the text in lines and trim each line
            List<string> lines = nodeString.Split(new string[] { "\r\n", "\n" },
                                                  StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < lines.Count(); i++)
                lines[i] = lines[i].Trim();

            // Remove comment lines
            lines.RemoveAll(l => l.StartsWith("//"));

            // Sanity checks
            if (!wordRegex.IsMatch(lines[0])) throw new ArgumentException("Invalid node name!");
            if (lines[1] != "{") throw new ArgumentException("Invalid node format!");
            if (lines.Last() != "}") throw new ArgumentException("Invalid node format!"); 

            #endregion


            ConfigNode result = new ConfigNode(lines[0]);


            #region Find and remove child nodes

            int cursor = 2;
            while (cursor < lines.Count())
            {
                // Find a single word with a single "{" on the next line
                // e.g: "PART \n {"
                // this should be the opening of a new child node
                if (wordRegex.IsMatch(lines[cursor]) && (lines[cursor + 1] == "{"))
                {
                    int matchBracketIdx = FindMatchingBracket(lines, cursor + 1);
                    KeyValuePair<int, int> range = new KeyValuePair<int, int>(cursor, (matchBracketIdx - cursor + 1));

                    // Remove the child node from the file and parse that too
                    List<string> childNodeLines = lines.GetRange(range.Key, range.Value);
                    lines.RemoveRange(range.Key, range.Value);

                    result.AddNode(Parser.Parse(String.Join(Environment.NewLine, childNodeLines.ToArray())));
                }
                else
                {
                    // Only increment if a node was not removed
                    cursor++;
                }
            } 

            #endregion


            #region Parse name / value pairs

            for (int i = 1; i < lines.Count(); i++)
            {
                // Match name/value pairs and add them to the result
                Match m = nameValueRegex.Match(lines[i]);
                if (m.Success)
                {
                    string name = m.Groups[1].Value.Trim();
                    string value = m.Groups[2].Value.Trim();
                    result.AddValue(name, value);
                }
            } 

            #endregion

            return result;
        }


        static int FindMatchingBracket(List<string> lines, int startFrom)
        {
            int brackets = 0;
            for (int i = startFrom; i < lines.Count(); i++)
            {
                if (lines[i].Trim() == "{") brackets++;
                if (lines[i].Trim() == "}") brackets--;

                if (brackets == 0)
                    return i;
            }

            throw new ArgumentOutOfRangeException("Could not find a matching bracket!");
        }
    }
}
