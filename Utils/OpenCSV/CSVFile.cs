using System.Collections.Generic;

public class CSVFile
{
    private string csvContent; // CSV file content string.
    private int index; // Index number of current character to analyze.
    private char currentChar; // Current character to analyze.
    public List<CSVRow> rows; // Parsed CSV rows after parse.

    /// <summary>
    /// The main constructor. After declaring that, the CSV will be parsed. Exception will be thrown in case of failure.
    /// If you wanna get all the obtained rows, simply get the 'rows' attribute (List<CSVRow>).
    /// </summary>
    /// <param name="csvContent"></param>
    public CSVFile(string csvContent, char separator = ';')
    {
        this.csvContent = csvContent;

        index = -1;
        rows = new List<CSVRow>();
        currentChar = default(char);

        NextChar();
        Parse(separator);
    }

    /// <summary>
    /// Skip the CSV file to the next character to analyze.
    /// </summary>
    private void NextChar()
    {
        index++;
        currentChar = index < csvContent.Length ? csvContent[index] : default(char);
    }

    /// <summary>
    /// Parse the CSV file taking in consideration the number of total columns.
    /// You can also use what separator you want, that can be a comma.
    /// </summary>
    private void Parse(char separator = ';')
    {
        int columnCount = 0, totalColumns = 0;
        string actualContent = "";
        CSVRow row = new CSVRow();

        while (currentChar != default(char) && currentChar != '\n')
        {
            if (currentChar == separator)
            {
                totalColumns++;
            }

            NextChar();
        }

        while (currentChar != default(char))
        {
            if (currentChar == separator)
            {
                row.columns.Add(actualContent.Trim());
                actualContent = "";
                columnCount++;
                NextChar();

                continue;
            }
            else if (currentChar == '\r' || currentChar == '\t' || currentChar == '\n')
            {
                actualContent += ' ';
                NextChar();
            }

            if (columnCount == totalColumns)
            {
                actualContent = "";
                columnCount = 0;

                while (currentChar != '\n' && currentChar != default(char))
                {
                    actualContent += currentChar;
                    NextChar();
                }

                row.columns.Add(actualContent.Trim());
                rows.Add(row);
                row = new CSVRow();
                actualContent = "";
            }
            else
            {
                actualContent += currentChar;
                NextChar();
            }
        }
    }
}