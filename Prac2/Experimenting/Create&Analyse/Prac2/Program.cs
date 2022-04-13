using System.Diagnostics;
using System.Text.RegularExpressions;

public class GA
{
    public class Chroma
    {
        public List<(double, double)> path;
        public double distance;

        public Chroma(List<(double, double)> path, double distance)
        {
            this.path = path;
            this.distance = distance;
        }
    }

    private int pointnum, bestof;
    private List<(double, double)> points;

    private List<Chroma> population = new();
    private double dis, chance;
    private bool done;
    
    Stopwatch sw = new ();
    private string path;
    public GA(List<(double, double)> p, double koef, double ch, int radius, string path)
    {
        pointnum = p.Count;
        points = p;
        bestof = (int) koef;
        chance = ch;
        dis = 2 * radius * Math.PI;
        this.path = path;
    }

    public void Start()
    {
        done = false;
        population.Clear();
        sw.Start();
        GeneratePopulation();
        while (done == false)
            NewGeneration();
    }
    private void GeneratePopulation()
    {
        Random rnd = new Random();
        int populationnum = Convert.ToInt32(bestof);
        for (int i = 0; i < populationnum; i++)
        {
            List<(double, double)> path = new List<(double, double)>();
            for (int j = 0; j < pointnum; j++)
            {
                int num = rnd.Next(pointnum);
                while (path.Contains(points[num]))
                    num = rnd.Next(pointnum);
                path.Add(points[num]);
            }
            path.Add(path[0]);
            population.Add(new Chroma(path, DistanceCalc(path)));
        }
    }
    private void NewGeneration()
    {
        population = population.OrderBy(x => x.distance).ToList();
        List<List<(double, double)>> parents = population.Select(x => x.path).ToList();
        for (int i = 0; i < parents.Count - 1; i++)
        {
            Random rnd = new Random();
            int crosspoint = rnd.Next(1, pointnum - 1);

            List<(double, double)> part1 = CreatePart1(parents, crosspoint, i);
            List<(double, double)> part2 = CreatePart2(parents, crosspoint, i);
            List<(double, double)> part3 = CreatePart1(parents, crosspoint, i + 1);
            List<(double, double)> part4 = CreatePart2(parents, crosspoint, i + 1);

            List<(double, double)> child1L = part1.Union(part4).Union(part2).ToList();
            child1L.Add(child1L[0]);

            List<(double, double)> child2L = part3.Union(part2).Union(part4).ToList();
            child2L.Add(child2L[0]);

            List<(double, double)> child3L = part2.Union(part3).Union(part1).ToList();
            child3L.Add(child3L[0]);

            List<(double, double)> child4L = part4.Union(part1).Union(part3).ToList();
            child4L.Add(child4L[0]);

            List<(double, double)> child1M = child1L;
            List<(double, double)> child2M = child2L;
            List<(double, double)> child3M = child3L;
            List<(double, double)> child4M = child4L;

            child1M = Mutation(child1L, child1M);
            child2M = Mutation(child2L, child2M);
            child3M = Mutation(child3L, child3M);
            child4M = Mutation(child4L, child4M);

            population.Add(new Chroma(child1M, DistanceCalc(child1M)));
            population.Add(new Chroma(child2M, DistanceCalc(child2M)));
            population.Add(new Chroma(child3M, DistanceCalc(child3M)));
            population.Add(new Chroma(child4M, DistanceCalc(child4M)));
        }
        population = population.OrderBy(x => x.distance).ToList().GetRange(0, Convert.ToInt32(bestof));
        CheckIfDone();
    }
    private void CheckIfDone()
    {
        if (population[0].distance <= dis * 1.1)
        {
            sw.Stop();
            File.AppendAllText(path, $"{sw.Elapsed}\t{pointnum}\t{bestof}\t{1-chance}\n");
            done = true;
        }
    }

    private List<(double, double)> Mutation(List<(double, double)> l, List<(double, double)> child)
    {
        Random rnd = new Random();
        if (rnd.NextDouble() > chance)
        {
            int mutationpoint1 = rnd.Next(2, pointnum - 1);
            int mutationpoint2 = rnd.Next(1, mutationpoint1);
            l.Reverse(mutationpoint2, mutationpoint1 - mutationpoint2);
            child = l;
        }
        return child;
    }
    private List<(double, double)> CreatePart1(List<List<(double, double)>> parents, int crosspoint, int i)
        => parents[i].ToList().GetRange(0, crosspoint);
    private List<(double, double)> CreatePart2(List<List<(double, double)>> parents, int crosspoint, int i)
        => parents[i].ToList().GetRange(crosspoint, pointnum - crosspoint);

    private double DistanceCalc(List<(double, double)> path)
    {
        double distance = 0;
        for (int i = 0; i < pointnum; i++)
            distance += Math.Sqrt(Math.Pow(path[i].Item1 - path[i + 1].Item1, 2)
                                  + Math.Pow(path[i].Item2 - path[i + 1].Item2, 2));
        return distance;
    }
}
public class Analysis
{
    private List<string> data;
    private Regex reg1 = new(@"\d{2}\:"), reg2 = new(@"\d{2}\.\d*");
    private string safe;
    public Analysis(string data, string safe)
    {
        this.data = data.Split("\n").ToList().Where(x => x.Length > 10)
            .OrderBy(x => Convert.ToInt32(x.Split("\t")[1])).
            ThenBy(x => Convert.ToInt32(x.Split("\t")[2])).
            ThenBy(x => x.Split("\t")[0]).ToList();
        this.safe = safe;
    }

    public void Start()
    {
        File.AppendAllText(safe, $"Mutation: {Math.Round(Convert.ToDouble(data[0].Split("\t")[3]), 2)*100}%\n");
        OneFile(0); 
    }
    public void OneFile(int c)
    {
            if(c == data.Count())
                return;
            int num = Convert.ToInt32(data[c].Split("\t")[1]);
            File.AppendAllText(safe, $"Number of dots: {num}\n");
            List<string> ar = data.Where(x => Convert.ToInt32(x.Split("\t")[1]) == num).ToList();
            c += OnePointnum(0, 0, ar);
            OneFile(c);
    }
    public int OnePointnum(int c, int g, List<string> ar)
    {
        if (g == ar.Count())
        {
            c += g;
            return c;
        }
            
        string popnums = ar[g].Split("\t")[2];
        List<double> times = ar.Where(x => x.Split("\t")[2] == popnums).ToList()
            .Select(x => ConvertToMilliseconds(x.Split("\t")[0])).ToList();
        /*double min = times[0];
        double max = 0;
        foreach (double d in times)
        {
            if (d < min)
                min = d;
            if (d > max)
                max = d;
        }*/
        double avg = times.Average();
        File.AppendAllText(safe, $"{popnums}\t{avg}\n");
        return OnePointnum(c, g + times.Count, ar);
    }
    public double ConvertToMilliseconds(string time)
    {
        List<double> notmilli = new List<double>();
        while(reg1.Matches(time).Count != 0 )
        {
            var v = reg1.Match(time).Value;
            notmilli.Add(Convert.ToDouble(v.Substring(0, v.Length - 1)));
            time = time.Substring(v.Length);
        }
        return notmilli[0] * 3600000 + notmilli[1] * 60000 + Convert.ToDouble(reg2.Match(time).Value) * 1000;
    }
    
    
}

public class Program
{
    public static string safe = @"D:\temporary files\res0.1.txt";
    public static string safe2 = @"D:\temporary files\data0.1.txt";

    static void Main(string[] args)
    {
        CreateData();
        Analysis();
        
        //AppendSmth(safe2, safe);
    }

    private static void Analysis()
    {
        string data = File.ReadAllText(safe2);
        Analysis an = new Analysis(data, safe);
        an.Start();  
    }

    private static void CreateData()
    {
        int counter = 0;
        double[] pointnumbers = {10, 25, 75, 100, 150, 200};
        double[] bestofk = {2, 4, 6, 8, 10, 12, 15, 20, 25};
        double[] mutationchances = {0.75, 0.9}; //0.1, 0.25, 0.5,
        string[] path = CreatePathes(mutationchances);
        //and then m = 1 or just take 0.25 away from array
       // for (int m = 0; m < mutationchances.Count(); m++)
        {
            //for (int num = 0; num < pointnumbers.Count(); num++)
            for (int tries = 0; tries < 10; tries++)
            for (int k = 0; k < bestofk.Count(); k++)
            for (int tries2 = 0; tries2 < 10; tries2++)
            { 
                //if (bestofk[k] < pointnumbers[5])
                {
                    var ga = new GA(GeneratePointsOnCircle(pointnumbers[5], 150), bestofk[k],
                        mutationchances[0], 150, path[0]);
                    ga.Start();
                    counter++;
                    Console.WriteLine(counter);
                }
            }
        }
        //pointnumbers = {10, 25, 75, 100, 150, 200};
        //bestofk = {2, 4, 6, 8, 10, 12, 15, 20, 25};
        mutationchances = new []{0.9}; //0.1, 0.25, 0.5, 0.75, 
        path = CreatePathes(mutationchances);
        for (int m = 0; m < mutationchances.Count(); m++)
        {
            for (int num = 0; num < pointnumbers.Count(); num++)
            for (int tries = 0; tries < 10; tries++)
            for (int k = 0; k < bestofk.Count(); k++)
            for (int tries2 = 0; tries2 < 10; tries2++)
            { 
                if (bestofk[k] < pointnumbers[num])
                {
                    var ga = new GA(GeneratePointsOnCircle(pointnumbers[num], 150), bestofk[k],
                        mutationchances[m], 150, path[0]);
                    ga.Start();
                    counter++;
                    Console.WriteLine(counter);
                }
            }
        }
    }
    private static string[] CreatePathes(double[] mutation)
    {
        string[] path = new string[mutation.Count()];
        for (int i = 0; i < mutation.Count(); i++)
            path[i] = @"D:\temporary files\data" + (1-mutation[i]) + ".txt";
        return path;
    }
    private static List<(double, double)> GeneratePointsOnCircle(double pointnum, int r)
    {
        List<(double, double)> points = new List<(double, double)>();
        double phi = 2 * Math.PI / pointnum;
        for (int i = 0; i < pointnum; i++)
            points.Add((r * Math.Cos(phi * i),  r * Math.Sin(phi * i)));
        return points;
    }
    private static List<(double, double)> GeneratePoints(double pointnum)
    {
        Random rnd = new Random();
        List<(double, double)> points = new List<(double, double)>();
        for (int i = 0; i < pointnum; i++)
            points.Add((rnd.Next(0, 1000), rnd.Next(0, 1000)));
        return points;    
    }

    private static Regex reg = new(@"((\d*\:)+\d*\.\d*\\t\d*\\t\d*)");
    private static void AppendSmth(string path, string path2)
    {
        string[] s = File.ReadAllLines(path);
        foreach (var v in s)
        {
            string ge = "";
            var f = v.Split("\t").ToList().GetRange(0, 3);
            foreach (string d in f)
            {
                ge += d+"\t";
            }
            ge += "0.1\n";

            File.AppendAllText(path2, ge);
        }
    }
}
