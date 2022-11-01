using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace neolink.Services
{
    public static class DataReader
    {

        public static string ReadJsonFile(string filename)
        {
            return FileManager.LoadEmbeddedFile(filename);

        }


        public static List<int> CreateDataListFromInput(string input)
        {
            List<int> cleanedList = new List<int>();

            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };

            string[] words = input.Split(delimiterChars);

            foreach (var word in words)
            {
                string digits = new string(word.Where(c => char.IsDigit(c)).ToArray());
                if (digits.Length > 0 && digits.Length < 5)
                {
                    int number = (int)Int32.Parse(digits);
                    if (number >= 0 && number <= 1024)
                    {
                        try
                        {
                            cleanedList.Add((int)Int32.Parse(digits));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Nonsense number found");
                        }
                    }
                }
            }
            return cleanedList;
        }

        public static List<int> ParseJsonInput(string file)
        {
            List<int> stream = new List<int>();

            string json = DataReader.ReadJsonFile(file);

            stream = JObject.Parse(json)["data"].Select(x => (int)x).ToArray().ToList<int>();

            return stream;
        }



        public static List<int>Sanitize(List<int> input, int limit)
        {
            List<int> Sanitized = new List<int>();

            foreach(int num in input)
            {
                if (num <= limit)
                {
                    Sanitized.Add(num);
                }
            }
            return Sanitized;//.Distinct().ToList();
        }

        public static string SanitizeToString(string input, int numLimit)
        {
            string cleanedInput = "";

            try
            {
                if (input.Length > 3 && input.Contains(","))
                {
                    //List<int> cleanList = Sanitize(CreateDataListFromInput(input), 4);
                    List<int> cleanList = CreateDataListFromInput(input).Distinct().ToList();

                    foreach (int num in cleanList)
                    {
                        cleanedInput += num + ",";
                    }
                    return cleanedInput.Substring(0, cleanedInput.Length - 2);
                }
            }
            catch (Exception e) { }
            return "";
        }
    }

}

