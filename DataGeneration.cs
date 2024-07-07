using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ITCS_6114_Project_1
{
    internal class DataGeneration
    {
        public static int[] GenerateSortingInput(int n)
        {
            Random rand = new Random();
            int[] array = new int[n];
            for (int i  = 0; i < n; i++)
            {
                array[i] = rand.Next(0, n + 1);
            }
            return array;
        }

        public static void GenerateAndSaveSortingInput(int n, int k)
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string folderPath = Path.Combine(projectDirectory, "Inputs", n.ToString());
            Directory.CreateDirectory(folderPath);

            for (int i = 0; i < k; i++)
            {
                int[] array = GenerateSortingInput(n);
                string json = JsonSerializer.Serialize(array);

                string filePath = Path.Combine(folderPath, $"array_{i + 1}.json");
                File.WriteAllText(filePath, json);

                Console.WriteLine($"Array {i + 1} saved to {filePath}");
            }
        }

        public static void GenerateAndSaveSortedInput(int n, int k)
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string folderPath = Path.Combine(projectDirectory, "Inputs");
            Directory.CreateDirectory(folderPath);

            for (int i = 0; i < k; i++)
            {
                // Generate sorted array
                int[] sortedArray = GenerateSortingInput(n);
                //hehe
                Array.Sort(sortedArray);
                string sortedJson = JsonSerializer.Serialize(sortedArray);

                // Save sorted array to file
                string sortedFilePath = Path.Combine(folderPath, "Sorted", $"array_{i + 1}.json");
                Directory.CreateDirectory(Path.GetDirectoryName(sortedFilePath));
                File.WriteAllText(sortedFilePath, sortedJson);

                Console.WriteLine($"Sorted array {i + 1} saved to {sortedFilePath}");

                // Generate reverse-sorted array
                int[] reverseSortedArray = GenerateSortingInput(n);
                //hehe
                Array.Sort(reverseSortedArray);
                Array.Reverse(reverseSortedArray);
                string reverseSortedJson = JsonSerializer.Serialize(reverseSortedArray);

                // Save reverse-sorted array to file
                string reverseSortedFilePath = Path.Combine(folderPath, "ReverseSorted", $"array_{i + 1}.json");
                Directory.CreateDirectory(Path.GetDirectoryName(reverseSortedFilePath));
                File.WriteAllText(reverseSortedFilePath, reverseSortedJson);

                Console.WriteLine($"ReverseSorted array {i + 1} saved to {reverseSortedFilePath}");
            }
        }

        public static List<int> DeserializeInputFromFile(int n, int k)
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string filePath = Path.Combine(projectDirectory, "Inputs", n.ToString(), $"array_{k}.json");

            string jsonString = File.ReadAllText(filePath);
            List<int> arr = JsonSerializer.Deserialize<List<int>>(jsonString);
            return arr;
        }

        public static List<int> DeserializeInputFromFile(string specialCaseName, int k)
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string filePath = Path.Combine(projectDirectory, "Inputs", specialCaseName.ToString(), $"array_{k}.json");

            string jsonString = File.ReadAllText(filePath);
            List<int> arr = JsonSerializer.Deserialize<List<int>>(jsonString);
            return arr;
        }
    }
}
