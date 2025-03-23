using System.Collections.Generic;

public class CSVRow
{
    public List<string> columns; // Parsed CSV columns of this specified CSV row.

    /// <summary>
    /// Main constructor of CSVRow. Simply declaring a row of the CSV file.
    /// </summary>
    public CSVRow()
    {
        this.columns = new List<string>();
    }
}