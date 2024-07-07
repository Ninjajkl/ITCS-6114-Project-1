using ITCS_6114_Project_1;
using System.Diagnostics;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using OxyPlot.Axes;
using OxyPlot.Legends;
using System.Drawing;


int[] inputSizes = { 100, 1000, 2000, 3000, 5000, 10000, 40000, 50000, 60000, 100000 };
int k = 10;  //Number of samples for each input size

//Generate the Samples
//Only needs to be ran once, but I kept it here for you
/*
foreach (int n in inputSizes)
{
    DataGeneration.GenerateAndSaveSortingInput(n, k);
}

DataGeneration.GenerateAndSaveSortedInput(50000, k);
*/

List<Action<List<int>>> sortingAlgorithms = new List<Action<List<int>>>
{
    list => SortingAlgorithms.InsertionSort(list, 0, list.Count),
    list => { var result = SortingAlgorithms.MergeSort(list); list.Clear(); list.AddRange(result); },
    list => { var result = SortingAlgorithms.HeapSort(list); list.Clear(); list.AddRange(result); },
    list => SortingAlgorithms.InPlaceQuickSort(list, 0, list.Count - 1),
    list => SortingAlgorithms.ModifiedQuickSort(list, 0, list.Count - 1)
};

List<string> algorithmNames = new List<string>
{
    "InsertionSort",
    "MergeSort",
    "HeapSort",
    "InPlaceQuickSort",
    "ModifiedQuickSort"
};

string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
string csvFilePath = Path.Combine(projectDirectory, "SortingAlgorithmResults.csv");

//Uncomment to run yourself - warning may take a while
/*
using (var writer = new StreamWriter(csvFilePath,false))
{
    writer.WriteLine("AlgorithmName,InputSize,AverageExecutionTime,SpecialCaseType");

    //Uncomment to run yourself - warning may take a while
    
    foreach (int size in inputSizes)
    {
        for (int i = 0; i < sortingAlgorithms.Count; i++)
        {
            var algorithm = sortingAlgorithms[i];
            string algorithmName = algorithmNames[i];

            Console.WriteLine($"Running {algorithmName}:");
            double averageTime = MeasureAverageTime(algorithm, size, k);

            writer.WriteLine($"{algorithmName},{size},{averageTime},null");
        }
    }
    

    //Special Cases!
    for (int i = 0; i < sortingAlgorithms.Count; i++)
    {
        var algorithm = sortingAlgorithms[i];
        string algorithmName = algorithmNames[i];

        Console.WriteLine($"Running {algorithmName}:");
        double averageTime = MeasureSpecialAverageTime(algorithm, "Sorted", k);

        writer.WriteLine($"{algorithmName},{"Sorted"},{averageTime},Sorted");

        averageTime = MeasureSpecialAverageTime(algorithm, "ReverseSorted", k);

        writer.WriteLine($"{algorithmName},{"ReverseSorted"},{averageTime},ReverseSorted");
    }
    
}
*/

PlotResults(csvFilePath);


static double MeasureAverageTime(Action<List<int>> sortingAlgorithm, int inputSize, int k)
{
    List<double> executionTimes = new List<double>();

    for (int i = 1; i <= k; i++)
    {
        Console.WriteLine($"Running inputSize: {inputSize}, sample number: {i}");
        List<int> input = DataGeneration.DeserializeInputFromFile(inputSize, i);

        Stopwatch stopwatch = Stopwatch.StartNew();
        sortingAlgorithm(input);
        stopwatch.Stop();

        //Check if failing test
        if (!SortingAlgorithms.IsSorted(input))
        {
            Console.WriteLine($"ERROR: FAILING SORTING ALGORITHM");
            throw new Exception("failed sorting algorithm");
        }
        executionTimes.Add(stopwatch.Elapsed.TotalMilliseconds);
    }

    return executionTimes.Average();
}

static double MeasureSpecialAverageTime(Action<List<int>> sortingAlgorithm, string specialCaseName, int k)
{
    List<double> executionTimes = new List<double>();

    for (int i = 1; i <= k; i++)
    {
        Console.WriteLine($"Running Special Case: {specialCaseName}, sample number: {i}");
        List<int> input = DataGeneration.DeserializeInputFromFile(specialCaseName, i);

        Stopwatch stopwatch = Stopwatch.StartNew();
        sortingAlgorithm(input);
        stopwatch.Stop();

        //Check if failing test
        if (!SortingAlgorithms.IsSorted(input))
        {
            Console.WriteLine($"ERROR: FAILING SORTING ALGORITHM");
            throw new Exception("failed sorting algorithm");
        }
        executionTimes.Add(stopwatch.Elapsed.TotalMilliseconds);
    }

    return executionTimes.Average();
}

static void PlotResults(string csvFilePath)
{
    var plotModel = new PlotModel { Title = "Sorting Algorithms Performance" };

    // Set logarithmic axes
    var xAxis = new LogarithmicAxis
    {
        Position = AxisPosition.Bottom,
        Title = "Input Size",
        Base = 10,
        Minimum = 75,
        Maximum = 150000
    };
    plotModel.Axes.Add(xAxis);

    var yAxis = new LogarithmicAxis
    {
        Position = AxisPosition.Left,
        Title = "Average Execution Time (ms)",
        Base = 10,
        Maximum = 20000
    };
    plotModel.Axes.Add(yAxis);

    var seriesDictionary = new Dictionary<string, LineSeries>();

    using (var reader = new StreamReader(csvFilePath))
    {
        reader.ReadLine(); // Skip header

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');

            if (values[3] != "null")
            {
                break;
            }

            string algorithmName = values[0];
            int inputSize = int.Parse(values[1]);
            double averageExecutionTime = double.Parse(values[2]);

            if (!seriesDictionary.ContainsKey(algorithmName))
            {
                var series = new LineSeries
                {
                    Title = algorithmName,
                    MarkerType = MarkerType.Circle,
                    LabelFormatString = "{1:0.00000}"
                };
                seriesDictionary[algorithmName] = series;
                plotModel.Series.Add(series);
            }

            var dataPoint = new DataPoint(inputSize, averageExecutionTime);
            seriesDictionary[algorithmName].Points.Add(dataPoint);
        }
    }

    plotModel.IsLegendVisible = true;
    plotModel.Legends.Add(new Legend()
    {
        LegendTitle = "Sorting Algorithms Legend",
        LegendPosition = LegendPosition.TopLeft,
    });

    var plotView = new PlotView { Model = plotModel };
    var form = new System.Windows.Forms.Form
    {
        Text = "Sorting Algorithms Performance",
        ClientSize = new System.Drawing.Size(800, 600)
    };

    plotView.Dock = System.Windows.Forms.DockStyle.Fill;
    form.Controls.Add(plotView);
    System.Windows.Forms.Application.Run(form);
}
