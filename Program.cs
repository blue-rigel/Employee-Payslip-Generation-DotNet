using System.Globalization;

string[] months = DateTimeFormatInfo.CurrentInfo.MonthNames;

// While True => to restart program on invalid file
while (true)
{

    try
    {
        Console.WriteLine("Enter Input file path: ");
        // Get FilePath
        String filePath = Console.ReadLine();
        // Validate path for null | exists | csv
        if (filePath == null || !File.Exists(Path.GetFullPath(filePath)) || !filePath.Contains(".csv"))
        {
            // Repeat code to get the right file
            Console.WriteLine("Invalid File or File doesn't exist");
            Console.WriteLine("Enter 'y' to try again, 'n' to quit: ");
            if (Console.ReadLine() == "y")
            {
                Console.Clear();
                continue;
            }
            else
            {
                break;
            }
        }else
        {
            // Read CSV
            var reader = new StreamReader(Path.GetFullPath(filePath));
            var writer = new StreamWriter($"{Path.GetDirectoryName(filePath)}/output.csv");
            // To track invalid lines
            int lineNo = 1;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                // Skip Invalid line
                if(values.Length != 5 || !months.Contains(values[4]))
                {
                    Console.WriteLine("Skipped invalid line " + lineNo);
                }else
                {
                    double income = double.Parse(values[2]);
                    double grossIncome = Math.Round(income / 12, 2);
                    double incomeTax = Math.Round(getTaxTotal(income) / 12, 2);
                    double netIncome = grossIncome - incomeTax;
                    double super = Math.Round((grossIncome * double.Parse(values[3].Replace("%", ""))) /100, 2);

                    writer.WriteLine(
                            $"{values[0]} {values[1]}," +
                            $"01 {values[4]} - {System.DateTime.DaysInMonth(DateTime.Now.Year, Array.IndexOf(months, values[4]) + 1)} {values[4]}," +
                            grossIncome.ToString("0.00") + "," + 
                            incomeTax.ToString("0.00") + "," + 
                            netIncome.ToString("0.00") + "," + 
                            super.ToString("0.00")
                        );
                }

                lineNo += 1;
            }
            reader.Close();
            writer.Close();
            Console.WriteLine("Output written to: " + $"{Path.GetDirectoryName(filePath)}/output.csv");
            break;
        }

    }catch(Exception e)
    {
        Console.WriteLine("Application Crashed with error: " + e.Message);
        break;
    }
}

double getTaxTotal(double income)
{
    // 14000 * 0.105 = 1470
    // 34000 * 0.175 = 5950
    // 22000 * 0.3   = 6600
    // 110000* 0.33  = 36300
    // Didn't add the static values to prevent confusion

    if (income <= 14000)
    {
        return income * 0.105;
    } else if(income > 14000 && income <= 48000)
    {
        return 1470 + ((income - 14000) * 0.175);
    } else if(income > 48000 && income <= 70000)
    {
        return 1470 + 5950 + ((income - 48000) * 0.3);
    } else if(income > 70000 && income <= 180000)
    {
        return 1470 + 5950 + 6600 + ((income - 70000) * 0.33);
    } else
    {
        // its greater than 180000
        return 1470 + 5950 + 6600 + 36300 + ((income - 180000) * 0.39);
    }
}