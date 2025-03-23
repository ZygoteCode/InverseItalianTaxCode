using System;
using System.Collections.Generic;
using InverseItalianTaxCode.Properties;

public class FiscalCodeSharp
{
    private static CSVFile csvFile;
    private static char[] consonants, letters, vocals;
    private static List<Tuple<char, int, int>> values;

    static FiscalCodeSharp()
    {
        csvFile = new CSVFile(Resources.comuni);
        vocals = "AEIOU".ToCharArray();
        consonants = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Replace("A", "").Replace("E", "").Replace("I", "").Replace("O", "").Replace("U", "").ToCharArray();
        letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        values = new List<Tuple<char, int, int>>()
        {
            new Tuple<char, int, int>('A', 0, 1),
            new Tuple<char, int, int>('0', 0, 1),
            new Tuple<char, int, int>('B', 1, 0),
            new Tuple<char, int, int>('1', 1, 0),
            new Tuple<char, int, int>('C', 2, 5),
            new Tuple<char, int, int>('2', 2, 5),
            new Tuple<char, int, int>('D', 3, 7),
            new Tuple<char, int, int>('3', 3, 7),
            new Tuple<char, int, int>('E', 4, 9),
            new Tuple<char, int, int>('4', 4, 9),
            new Tuple<char, int, int>('F', 5, 13),
            new Tuple<char, int, int>('5', 5, 13),
            new Tuple<char, int, int>('G', 6, 15),
            new Tuple<char, int, int>('6', 6, 15),
            new Tuple<char, int, int>('H', 7, 17),
            new Tuple<char, int, int>('7', 7, 17),
            new Tuple<char, int, int>('I', 8, 19),
            new Tuple<char, int, int>('8', 8, 19),
            new Tuple<char, int, int>('J', 9, 21),
            new Tuple<char, int, int>('9', 9, 21),
            new Tuple<char, int, int>('K', 10, 2),
            new Tuple<char, int, int>('L', 11, 4),
            new Tuple<char, int, int>('M', 12, 18),
            new Tuple<char, int, int>('N', 13, 20),
            new Tuple<char, int, int>('O', 14, 11),
            new Tuple<char, int, int>('P', 15, 3),
            new Tuple<char, int, int>('Q', 16, 6),
            new Tuple<char, int, int>('R', 17, 8),
            new Tuple<char, int, int>('S', 18, 12),
            new Tuple<char, int, int>('T', 19, 14),
            new Tuple<char, int, int>('U', 20, 16),
            new Tuple<char, int, int>('V', 21, 10),
            new Tuple<char, int, int>('W', 22, 22),
            new Tuple<char, int, int>('X', 23, 25),
            new Tuple<char, int, int>('Y', 24, 24),
            new Tuple<char, int, int>('Z', 25, 23),
        };
    }

    public static string GenerateFiscalCode(string name, string surname, string birthPlace, string dateOfBirth, FiscalCodeGender gender)
    {
        if (name == null)
        {
            throw new Exception("Name cannot be null.");
        }

        if (surname == null)
        {
            throw new Exception("Surname cannot be null.");
        }

        if (birthPlace == null)
        {
            throw new Exception("Birth place cannot be null.");
        }

        if (dateOfBirth == null)
        {
            throw new Exception("Date of birth cannot be null.");
        }

        if (gender == null)
        {
            throw new Exception("Gender cannot be null.");
        }

        name = name.Replace(" ", "").Replace('\t'.ToString(), "").ToUpper().Replace("'", "").Replace("È", "E").Replace("À", "A").Replace("Ò", "O").Replace("Ù", "U").Replace("Ì", "I");

        if (name == "")
        {
            throw new Exception("Name cannot be empty.");
        }

        surname = surname.Replace(" ", "").Replace('\t'.ToString(), "").ToUpper().Replace("'", "").Replace("È", "E").Replace("À", "A").Replace("Ò", "O").Replace("Ù", "U").Replace("Ì", "I");

        if (surname == "")
        {
            throw new Exception("Surname cannot be empty.");
        }

        birthPlace = birthPlace.Replace(" ", "").Replace('\t'.ToString(), "").ToUpper().Replace("È", "E'").Replace("À", "A'").Replace("Ò", "O'").Replace("Ù", "U'").Replace("Ì", "I'");

        if (birthPlace == "")
        {
            throw new Exception("Birth place cannot be empty.");
        }

        dateOfBirth = dateOfBirth.Replace(" ", "").Replace('\t'.ToString(), "").Replace("-", "/");

        if (dateOfBirth.Length != 10)
        {
            throw new Exception("Date of birth required length is 10.");
        }

        if (!dateOfBirth.Contains("/"))
        {
            throw new Exception("Date of birth requires '/' separator.");
        }

        string[] dateSplitted = dateOfBirth.Split('/');

        if (dateSplitted.Length != 3)
        {
            throw new Exception("Date of birth requires 3 separators.");
        }

        if (dateSplitted[0].Length != 2)
        {
            throw new Exception("Date of birth required day length is 2.");
        }

        if (dateSplitted[1].Length != 2)
        {
            throw new Exception("Date of birth required month length is 2.");
        }

        if (dateSplitted[2].Length != 4)
        {
            throw new Exception("Date of birth required year length is 4.");
        }

        string sDay = dateSplitted[0], sMonth = dateSplitted[1], sYear = dateSplitted[2];

        if (sDay.StartsWith("0"))
        {
            sDay = sDay.Substring(1);
        }

        if (sMonth.StartsWith("0"))
        {
            sMonth = sMonth.Substring(1);
        }

        int day = int.Parse(sDay), month = int.Parse(sMonth), year = int.Parse(sYear);

        if (day < 0 || day > 31)
        {
            throw new Exception("Day of birth must be between 1 and 31.");
        }

        if (month < 0 || month > 12)
        {
            throw new Exception("Month of birth must be between 1 and 12.");
        }

        if (year < 1900 || year > DateTime.Now.Year)
        {
            throw new Exception($"Month of birth must be between 1900 and the current year {DateTime.Now.Year}.");
        }

        string fiscalCode = "", surnamePart = "", namePart = "";
        int found = 0;

        foreach (char c1 in surname)
        {
            foreach (char c2 in consonants)
            {
                if (c1.Equals(c2))
                {
                    surnamePart += c1;
                    found++;

                    break;
                }
            }

            if (found == 3)
            {
                break;
            }
        }

        if (surnamePart.Length != 3)
        {
            foreach (char c1 in surname)
            {
                foreach (char c2 in vocals)
                {
                    if (c1.Equals(c2))
                    {
                        surnamePart += c1;
                        found++;

                        break;
                    }
                }

                if (surnamePart.Length == 3)
                {
                    break;
                }
            }
        }

        while (surnamePart.Length != 3)
        {
            surnamePart += "X";
        }

        fiscalCode += surnamePart;
        int totalConsonants = 0;

        foreach (char c1 in name)
        {
            foreach (char c2 in consonants)
            {
                if (c1.Equals(c2))
                {
                    totalConsonants++;
                    break;
                }
            }

            if (totalConsonants == 3)
            {
                break;
            }
        }

        found = 0;

        if (totalConsonants < 3)
        {
            foreach (char c1 in name)
            {
                foreach (char c2 in consonants)
                {
                    if (c1.Equals(c2))
                    {
                        namePart += c2;
                        found++;

                        break;
                    }
                }

                if (found == 3)
                {
                    break;
                }
            }
        }
        else
        {
            foreach (char c1 in name)
            {
                foreach (char c2 in consonants)
                {
                    if (c1.Equals(c2))
                    {
                        if (found == 0 || found == 2 || found == 3)
                        {
                            namePart += c2;
                        }

                        found++;

                        break;
                    }
                }

                if (found == 4)
                {
                    break;
                }
            }
        }

        if (namePart.Length != 3)
        {
            foreach (char c1 in name)
            {
                foreach (char c2 in vocals)
                {
                    if (c1.Equals(c2))
                    {
                        namePart += c1;
                        found++;

                        break;
                    }
                }

                if (namePart.Length == 3)
                {
                    break;
                }
            }
        }

        while (namePart.Length != 3)
        {
            namePart += "X";
        }

        fiscalCode += namePart + dateSplitted[2].Substring(2);

        switch (month)
        {
            case 1:
                fiscalCode += "A";
                break;
            case 2:
                fiscalCode += "B";
                break;
            case 3:
                fiscalCode += "C";
                break;
            case 4:
                fiscalCode += "D";
                break;
            case 5:
                fiscalCode += "E";
                break;
            case 6:
                fiscalCode += "H";
                break;
            case 7:
                fiscalCode += "L";
                break;
            case 8:
                fiscalCode += "M";
                break;
            case 9:
                fiscalCode += "P";
                break;
            case 10:
                fiscalCode += "R";
                break;
            case 11:
                fiscalCode += "S";
                break;
            case 12:
                fiscalCode += "T";
                break;
        }

        if (gender.Equals(FiscalCodeGender.FEMALE))
        {
            fiscalCode += (day + 40).ToString();
        }
        else
        {
            fiscalCode += day;
        }

        bool exists = false;

        foreach (CSVRow row in csvFile.rows)
        {
            if (row.columns[0].Equals(birthPlace))
            {
                exists = true;
                fiscalCode += row.columns[1];
                break;
            }
        }

        if (!exists)
        {
            throw new Exception("Birth place does not exist in the Italian database.");
        }

        int total = 0;

        for (int i = 0; i < fiscalCode.Length; i++)
        {
            foreach (Tuple<char, int, int> tuple in values)
            {
                if (tuple.Item1.Equals(fiscalCode[i]))
                {
                    if ((i + 1) % 2 == 0)
                    {
                        total += tuple.Item2;
                    }
                    else
                    {
                        total += tuple.Item3;
                    }

                    break;
                }
            }
        }

        return fiscalCode + letters[total % 26];
    }

    public static bool IsFiscalCodeValid(string fiscalCode, string name, string surname, string birthPlace, string dateOfBirth, FiscalCodeGender gender)
    {
        return GenerateFiscalCode(name, surname, birthPlace, dateOfBirth, gender) == fiscalCode;
    }

    private static bool IsNumeric(string s)
    {
        foreach (char c in s)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }

        return true;
    }

    public static FiscalCodeGender GetGender(string fiscalCode)
    {
        if (fiscalCode.Length != 16)
        {
            throw new Exception("Length of the fiscal code must be 16.");
        }

        string sDay = fiscalCode[9].ToString() + fiscalCode[10].ToString();
        string _sDay = sDay;

        if (_sDay.StartsWith("0"))
        {
            _sDay = _sDay.Substring(1);
        }

        if (!IsNumeric(_sDay))
        {
            throw new Exception("Day must be numeric.");
        }

        if (int.Parse(_sDay) > 40)
        {
            return FiscalCodeGender.FEMALE;
        }

        return FiscalCodeGender.MALE;
    }

    public static string GetBirthPlace(string fiscalCode)
    {
        if (fiscalCode.Length != 16)
        {
            throw new Exception("Length of the fiscal code must be 16.");
        }

        string code = fiscalCode[11].ToString() + fiscalCode[12].ToString() + fiscalCode[13].ToString() + fiscalCode[14].ToString();

        foreach (CSVRow row in csvFile.rows)
        {
            if (row.columns[1].Equals(code))
            {
                return row.columns[0];
            }
        }

        throw new Exception("Invalid fiscal code birth place specified.");
    }

    public static string GetFirstDateOfBirth(string fiscalCode)
    {
        Tuple<string, string> tuple = GetDateOfBirth(fiscalCode);
        return tuple.Item1 + "/19" + tuple.Item2;
    }

    public static string GetSecondDateOfBirth(string fiscalCode)
    {
        Tuple<string, string> tuple = GetDateOfBirth(fiscalCode);
        return tuple.Item1 + "/20" + tuple.Item2;
    }

    public static string GetMostProbableDateOfBirth(string fiscalCode)
    {
        Tuple<string, string> tuple = GetDateOfBirth(fiscalCode);
        string _sYear = tuple.Item2;

        if (_sYear.StartsWith("0"))
        {
            _sYear = _sYear.Substring(1);
        }

        if (int.Parse(_sYear) > int.Parse(DateTime.Now.Year.ToString().Substring(2)))
        {
            return tuple.Item1 + "/19" + tuple.Item2;
        }
        else
        {
            return tuple.Item1 + "/20" + tuple.Item2;
        }
    }

    private static Tuple<string, string> GetDateOfBirth(string fiscalCode)
    {
        if (fiscalCode.Length != 16)
        {
            throw new Exception("Length of the fiscal code must be 16.");
        }

        string dateOfBirth = "";

        string sDay = fiscalCode[9].ToString() + fiscalCode[10].ToString();
        string _sDay = sDay;

        if (_sDay.StartsWith("0"))
        {
            _sDay = _sDay.Substring(1);
        }

        if (!IsNumeric(_sDay))
        {
            throw new Exception("Day must be numeric.");
        }

        int day = int.Parse(_sDay);

        if (day > 40)
        {
            day -= 40;

            if (day.ToString().Length == 1)
            {
                dateOfBirth = "0" + day.ToString();
            }
            else
            {
                dateOfBirth = day.ToString();
            }
        }
        else
        {
            dateOfBirth = sDay;
        }

        switch (fiscalCode[8])
        {
            case 'A':
                dateOfBirth += "/01";
                break;
            case 'B':
                dateOfBirth += "/02";
                break;
            case 'C':
                dateOfBirth += "/03";
                break;
            case 'D':
                dateOfBirth += "/04";
                break;
            case 'E':
                dateOfBirth += "/05";
                break;
            case 'H':
                dateOfBirth += "/06";
                break;
            case 'L':
                dateOfBirth += "/07";
                break;
            case 'M':
                dateOfBirth += "/08";
                break;
            case 'P':
                dateOfBirth += "/09";
                break;
            case 'R':
                dateOfBirth += "/10";
                break;
            case 'S':
                dateOfBirth += "/11";
                break;
            case 'T':
                dateOfBirth += "/12";
                break;
            default:
                throw new Exception($"Invalid month letter specified in the fiscal code: {fiscalCode[8]}.");
        }

        string sYear = fiscalCode[6].ToString() + fiscalCode[7].ToString();
        string _sYear = sYear;

        if (_sYear.StartsWith("0"))
        {
            _sYear = _sYear.Substring(1);
        }

        if (!IsNumeric(_sYear))
        {
            throw new Exception("Year must be numeric.");
        }

        return new Tuple<string, string>(dateOfBirth, sYear);
    }
}