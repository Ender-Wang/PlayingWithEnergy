using System.Collections.Generic;
using System.IO;
using System.Text;

public class MyCsvParser
{
    public static string[] parse(string line)
    {
        List<string> items = new List<string>();
        bool inQuotes = false;
        int start = 0;
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (line[i] == '|' && !inQuotes)
            {
                string item = line.Substring(start, i - start).Trim('"');
                items.Add(item);
                start = i + 1;
            }
        }
        string lastItem = line.Substring(start).Trim('"');
        items.Add(lastItem);
        return items.ToArray();
    }
}