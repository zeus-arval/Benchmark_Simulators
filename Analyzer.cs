using System;
using System.IO;
using System.Collections.Generic;

namespace Analyzer
{
    public class Programm
    {
        public static int lineNum = 1;
        public static string path = "SystemData.log";
        public static void Main(string[] args)
        {
            //List<string[]> systemDataList = FillList();
            List<string[]> systemDataList = new List<string[]>
            {
                new string[] {"--------------------",
                                "CPU: 42.3",
                                "MEM: 2.5",
                                "GPU-MEM: 1410MiB",
                                "GPU-USG: 36%"}
            };
            Dictionary<string, decimal[]> systemDataIntDict = new Dictionary<string, decimal[]>
            {
                {"CPU-USG", ReturnNumArray(systemDataList, 1)},
                {"RAM-MEM-USG", ReturnNumArray(systemDataList, 2)},
                {"GPU-USG", ReturnNumArray(systemDataList, 4)},
                {"GPU-MEM-USG", ReturnNumArray(systemDataList, 3)}
            };
            FillFile(systemDataIntDict);
            Console.WriteLine("Done");
        }

        public static decimal GetAvarage(decimal[] array)
        {
            decimal sum = 0m;
            foreach (decimal value in array) sum += value;
            return sum / (decimal)lineNum;
        }

        public static void FillFile(Dictionary<string, decimal[]> dict)
        {
            using StreamWriter writer = new StreamWriter("Results.txt");
            writer.Write("Summary");
            foreach (KeyValuePair<string, decimal[]> kvp in dict)
            {
                writer.Write($"{kvp.Key}: {GetAvarage(kvp.Value)}\n");
            }
            writer.Close();
        }

        public static decimal[] ReturnNumArray(List<string[]> list, int row)
        {
            decimal[] numArray = new decimal[lineNum];
            for (int i = 0; i < lineNum; i++)
            {
                //string[] array = list[i][row].Split(':');
                //string numStr = array[1].Trim(' ', '%');
                string numStr = list[i][row].Split(':')[1].Trim(' ', '%');
                numStr = numStr.Contains("MiB") ? numStr[..^4] : numStr;
                numArray[i] = decimal.Parse(numStr);
            }
            return numArray;
        }

        public static List<string[]> FillList()
        // ["--------------------",
        // "CPU: 42.3  ",
        // "MEM: 2.5",
        // "GPU-MEM: 1410MiB",
        // "GPU-USG: 36%"],..
        {
            List<string> list = ReadDataFile();
            List<string[]> finalList = new List<string[]>();

            Predicate<string> match = IsDash;
            int arraySize = list.FindIndex(1, match),
            count = 0;
            string[] stringArray = new string[arraySize];

            foreach (string line in list)
            {
                if (line != " ")
                {
                    if (line[0] == '-')
                    {
                        finalList.Add(stringArray);
                        stringArray = new string[arraySize];
                        count = 0;
                    }
                    stringArray[count] = line;
                    count++;
                }
            }
            return finalList;
        }

        public static bool IsDash(string str) => str.Equals("-");

        public static List<string> ReadDataFile()
        {
            List<string> list = new List<string>();
            using StreamReader reader = new StreamReader(path);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                list.Add(line);
            }
            reader.Close();
            return list;
        }
    }
}