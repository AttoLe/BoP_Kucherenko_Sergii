using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
namespace Prac2
{
    #region Extensions
    public static class GridExtensions
    {
        public static void CreateRows(this Grid grid, List<double> rows)
        {
            foreach (double r in rows)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(r, GridUnitType.Star);
                grid.RowDefinitions.Add(row);
            }
        }
        public static void CreateColumns(this Grid grid, List<double> columns)
        {
            foreach (double c in columns)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(c, GridUnitType.Star);
                grid.ColumnDefinitions.Add(column);
            }
        }
    }
    public static class LabelExtensions
    {
        static BrushConverter bc = new();
        public static void Locate(this Label label, Grid grid, (int, int) location, string color)
        {
            Grid.SetRow(label, location.Item1);
            Grid.SetColumn(label, location.Item2);
            label.Background = (Brush)bc.ConvertFrom(color);
            grid.Children.Add(label);
        }

        public static void TextSetting(this Label label, string text, string FontFamily, double size, double bt)
        {
            label.BorderThickness = new Thickness(bt);
            label.BorderBrush = (Brush)bc.ConvertFrom("#FF000000");
            label.Content = text;
            label.FontFamily = (FontFamily)(new FontFamilyConverter().ConvertFrom(FontFamily));
            label.FontSize = size;
        }
    }
    public static class TextBoxExtensions
    {
        static BrushConverter bc = new();
        public static void Locate(this TextBox textbox, Grid grid, (int, int) location, string color)
        {
            Grid.SetRow(textbox, location.Item1);
            Grid.SetColumn(textbox, location.Item2);
            textbox.Background = (Brush)bc.ConvertFrom(color);
            grid.Children.Add(textbox);
        }

        public static void TextSettings(this TextBox textbox, string text, string FontFamily, double size, double bt)
        {
            textbox.BorderThickness = new Thickness(bt, 0, bt, 0);
            textbox.BorderBrush = (Brush)bc.ConvertFrom("#FF000000");
            textbox.Text = text;
            textbox.FontSize = size;
            textbox.FontFamily = (FontFamily)(new FontFamilyConverter().ConvertFrom(FontFamily));
        }
    }
    public static class ButtonExtensions
    {
        public static void Locate(this Button button, Grid grid, (int, int) location, string color)
        {
            Grid.SetRow(button, location.Item1);
            Grid.SetColumn(button, location.Item2);
            button.Background = (Brush)(new BrushConverter().ConvertFrom(color));
            grid.Children.Add(button);
        }

        public static void TextSettings(this Button button, string text, string FontFamily, double size)
        {
            button.Content = text;
            button.FontSize = size;
            button.FontFamily = (FontFamily)(new FontFamilyConverter().ConvertFrom(FontFamily));
        }
    }
    public static class ComboBoxExtensions
    {
        public static void Locate(this ComboBox cb, Grid grid, (int, int) location, string color)
        {
            Grid.SetRow(cb, location.Item1);
            Grid.SetColumn(cb, location.Item2);
            cb.Background = (Brush)(new BrushConverter().ConvertFrom(color));
            grid.Children.Add(cb);
        }

        public static void TextSettings(this ComboBox cb, string text, string FontFamily, double size)
        {
            cb.Text = text;
            cb.FontSize = size;
            cb.FontFamily = (FontFamily)(new FontFamilyConverter().ConvertFrom(FontFamily));
        }
    }
    #endregion

    #region Algorithms
    public class GA
    {
        public class Chroma
        {
            public PointCollection path = new PointCollection();
            public double distance = 0;

            public Chroma(PointCollection path, double distance)
            {
                this.path = path;
                this.distance = distance;
            }
        }

        int pointnum;
        double bestof, chance, dis;
        PointCollection points = new PointCollection();

        List<Chroma> population = new List<Chroma>();
        double finalres = 0, counter = 0, maxinteration;

        Stopwatch sw = new Stopwatch();

        public GA(PointCollection p, double koef, double ch, double max, int radius)
        {
            pointnum = p.Count();
            points = p;
            bestof = koef;
            chance = ch;
            maxinteration = max;
            dis = 2 * radius * Math.PI;
        }

        public void Start()
        {
            MainWindow.state = true;     
            population.Clear();
            sw.Restart();
            GeneratePopulation();
            population = population.OrderBy(x => x.distance).ToList();
            MainWindow.dt.Start();
        }
        private void GeneratePopulation()
        {
            Random rnd = new Random();
            int populationnum = Convert.ToInt32(bestof);
            for (int i = 0; i < populationnum; i++)
            {
                PointCollection path = new PointCollection();
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
        public PointCollection NewGeneration()
        {
            population = population.OrderBy(x => x.distance).ToList();
            finalres = population[0].distance;
            List<PointCollection> parents = population.Select(x => x.path).ToList();
            for (int i = 0; i < parents.Count - 1; i++)
            {
                Random rnd = new Random();
                int crosspoint = rnd.Next(1, pointnum - 1);

                List<Point> part1 = CreatePart1(parents, crosspoint, i);
                List<Point> part2 = CreatePart2(parents, crosspoint, i);
                List<Point> part3 = CreatePart1(parents, crosspoint, i + 1);
                List<Point> part4 = CreatePart2(parents, crosspoint, i + 1);

                List<Point> child1l = part1.Union(part4).Union(part2).ToList();
                child1l.Add(child1l[0]);

                List<Point> child2l = part3.Union(part2).Union(part4).ToList();
                child2l.Add(child2l[0]);

                List<Point> child3l = part2.Union(part3).Union(part1).ToList();
                child3l.Add(child3l[0]);

                List<Point> child4l = part4.Union(part1).Union(part3).ToList();
                child4l.Add(child4l[0]);

                PointCollection child1m = new PointCollection(child1l);
                PointCollection child2m = new PointCollection(child2l);
                PointCollection child3m = new PointCollection(child3l);
                PointCollection child4m = new PointCollection(child4l);

                child1m = Mutation(child1l, child1m);
                child2m = Mutation(child2l, child2m);
                child3m = Mutation(child3l, child3m);
                child4m = Mutation(child4l, child4m);

                population.Add(new Chroma(child1m, DistanceCalc(child1m)));
                population.Add(new Chroma(child2m, DistanceCalc(child2m)));
                population.Add(new Chroma(child3m, DistanceCalc(child3m)));
                population.Add(new Chroma(child4m, DistanceCalc(child4m)));
            }
            population = population.OrderBy(x => x.distance).ToList().GetRange(0, Convert.ToInt32(bestof));
            CheckIfDone();
            return population[0].path;
        }
        private void CheckIfDone()
        {
            if (MainWindow.circle == false)
            {
                if (finalres == population[0].distance)
                    counter++;
                else counter = 0;
                if (counter == maxinteration)                  
                    End($"The process is finished... \n Distance is {population[0].distance} \nTime spend: {sw.Elapsed}");              
            }
            else
            {
                if (population[0].distance <= dis * 1.1)
                    End($"The process is finished... \n Distance is {population[0].distance}\nIdeal distance is: {dis} \nTime spend: {sw.Elapsed}");
            }
        }
        private void End(string ans)
        {
            MainWindow.state = false;
            sw.Stop();
            MessageBox.Show(ans);
            MainWindow.ReStart();
        }

        private PointCollection Mutation(List<Point> l, PointCollection child)
        {
            Random rnd = new Random();
            if (rnd.NextDouble() > chance)
            {
                int mutationpoint1 = rnd.Next(2, pointnum - 1);
                int mutationpoint2 = rnd.Next(1, mutationpoint1);
                l.Reverse(mutationpoint2, mutationpoint1 - mutationpoint2);
                child = new PointCollection(l);
            }
            return child;
        }
        private List<Point> CreatePart1(List<PointCollection> parents, int crosspoint, int i)
            => parents[i].ToList().GetRange(0, crosspoint);
        private List<Point> CreatePart2(List<PointCollection> parents, int crosspoint, int i) 
            => parents[i].ToList().GetRange(crosspoint, pointnum - crosspoint);

        public double DistanceCalc(PointCollection path)
        {
            double distance = 0;
            for (int i = 0; i < pointnum; i++)
                distance += Math.Sqrt(Math.Pow(path[i].X - path[i+1].X, 2)
                    + Math.Pow(path[i].Y - path[i + 1].Y, 2));
            return distance;
        }
    }
    public class GR
    {
        int pointnum, counter = 0;
        double dis;
        PointCollection points = new PointCollection();
        PointCollection solution = new PointCollection();
        Stopwatch sw = new Stopwatch();
        Point p;

        public GR(PointCollection pc, int radius)
        {
            points = pc;
            pointnum = pc.Count;
            dis = dis = 2 * radius * Math.PI;
        }

        public void Start()
        {
            MainWindow.state = true;
            Random rnd = new Random();
            p = points[rnd.Next(points.Count)];
            sw.Start();
            MainWindow.dt.Start();         
        }
        public PointCollection OneStep()
        {
            int index = 0;
            if (points.Count > 1)
            {         
                double min = DistanceBetween(p, index);
                for (int j = 1; j < points.Count; j++)
                {
                    double dis = DistanceBetween(p, j);
                    if (dis < min)
                    {
                        min = dis;
                        index = j;
                    }
                }
                p = points[index];
                solution.Add(p);
                points.Remove(p);
            }
            else
            {
                if (points.Count == 1)
                {
                    solution.Add(points[0]);
                    points.Clear();
                }
                else
                {
                    solution.Add(solution[0]);
                    sw.Stop();
                    MainWindow.state = false;
                    string ans;
                    if (MainWindow.circle)
                       ans = $"The process is finished... \n Distance is {WholeDistance(solution)}\nIdeal distance is: {dis} \nTime spend: {sw.Elapsed}";
                    else 
                        ans = $"The process is finished... \n Distance is {WholeDistance(solution)}\nTime spend: {sw.Elapsed}";
                    MessageBox.Show(ans);                     
                    MainWindow.ReStart();
                }
                
            }
            counter++;
            return solution;
        }    

        private double DistanceBetween(Point p, int i2) => 
            Math.Sqrt(Math.Pow(p.X - points[i2].X, 2) + Math.Pow(p.Y - points[i2].Y, 2));
        public double WholeDistance(PointCollection path)
        {
            double distance = 0;
            for (int i = 0; i < pointnum - 1; i++)
                distance += Math.Sqrt(Math.Pow(path[i].X - path[i + 1].X, 2)
                    + Math.Pow(path[i].Y - path[i + 1].Y, 2));
            return distance;
        }
    }
    #endregion

    public partial class MainWindow : Window
    {    
        static TextBox tb1, tb2;
        static Button button;
        static ComboBox cb1, cb2;

        static bool IsGA = false;        
        static int pointnum;

        static GA ga;
        static GR gr;

        public static DispatcherTimer dt = new DispatcherTimer();
        public static List<Ellipse> ellipses = new List<Ellipse>();
        public static bool state = true, circle = false;      
        static double winwidth, winheight;
        public MainWindow()
        {
            InitializeComponent();
            string color = "#FFE6E1E1";
            string FontFamily = "Times New Roman";
            double FontSize = 20, bt = 1, pointradius = 15;
            int radius = 150;

            winheight = this.Height;
            winwidth = this.Width;         
            this.SizeChanged += (object sender, SizeChangedEventArgs e) => { Window_SizeChanged(pointradius, e); };

            Grid grid1 = new Grid();
            this.Content = grid1;
            List<double> r1 = new() {1, 70, 1};
            List<double> c1 = new() {1, 50, 1, 20, 1};
            grid1.CreateRows(r1);
            grid1.CreateColumns(c1);
            Grid grid2 = new Grid();                     
            grid1.Children.Add(grid2);
            Grid.SetColumn(grid2, 3);
            Grid.SetRow(grid2, 1);
            List<double> r2 = new() { 1, 1, 1, 1, 1, 1, 1, 3};
            grid2.CreateRows(r2);

            Label label1 = new Label();
            label1.Locate(grid2, (2, 0), color);
            label1.TextSetting("Кількість пунктів", FontFamily, FontSize, bt);

            Label label2 = new Label();
            label2.Locate(grid2, (4, 0), color);
            label2.TextSetting("Швидкість (у мс)", FontFamily, FontSize, bt);

            tb1 = new();
            tb1.Locate(grid2, (3, 0), "#FFFFFFFF");
            tb1.TextSettings("...", FontFamily, FontSize, bt);
            tb1.MouseEnter += Tb_MouseEnter;
            tb1.MouseLeave += Tb_MouseLeave;
            tb1.TextChanged += Tb1_TextChanged;

            tb2 = new();
            tb2.Locate(grid2, (5, 0), "#FFFFFFFF");
            tb2.TextSettings("...", FontFamily, FontSize, bt);
            tb2.MouseEnter += Tb_MouseEnter;
            tb2.MouseLeave += Tb_MouseLeave;
            tb2.TextChanged += Tb2_TextChanged;
            InitializeComponent();

            Canvas canvas = new Canvas();
            Grid.SetRow(canvas, 1);
            Grid.SetColumn(canvas, 1);
            grid1.Children.Add(canvas);

            button = new Button();
            button.Locate(grid2, (6, 0), color);
            button.IsEnabled = false;
            button.TextSettings("Почати/Зупинити", FontFamily, FontSize);
            //there could be changes with "k" and "chance"
            button.Click += (object sender, RoutedEventArgs e) => { But_Click(canvas, pointradius, 2, 0.1, 100, radius); };
            
            cb1 = new ComboBox();
            cb1.Locate(grid2, (1, 0), color);
            cb1.TextSettings("Вибір алгоритму", FontFamily, FontSize);
            cb1.Items.Add("Генетичний алгоритм"); cb1.Items.Add("Жадібний алгоритм");
            cb1.DropDownClosed += (object sender, EventArgs e) => Check_DropDownClosed1(cb1, canvas);
                     
            cb2 = new ComboBox();
            cb2.Locate(grid2, (0, 0), color);
            cb2.TextSettings("Вибір генерації точок", FontFamily, FontSize);
            cb2.Items.Add("По колу"); cb2.Items.Add("Псевдовипадково");
            cb2.DropDownClosed += (object sender, EventArgs e) => { Check_DropDownClosed2(cb2, canvas); };

            dt.Tick += (object sender, EventArgs e) => { Dt_Tick(canvas); };       
        }
        public static void ReStart()
        {
            dt.Stop();
            if (state == false && button.IsEnabled == true)
            {
                dt.Interval = new TimeSpan(0, 0, 0, 0, 0);
                pointnum = 0;
                tb1.IsEnabled = true; tb2.IsEnabled = true; cb1.IsEnabled = true;
                tb1.Text = "..."; tb2.Text = "...";
                button.IsEnabled = false;
            }
        }

        private void Dt_Tick(Canvas canvas)
        {
            canvas.Children.Clear();
            PointCollection pc = new PointCollection();
            if (IsGA)
                pc = ga.NewGeneration();
            else
                pc = gr.OneStep();
            PrintPoints(MainWindow.ellipses, canvas);
            PrintWay(pc, canvas);
        }

        private void But_Click(Canvas canvas, double pointradius, double k, double chance, double max, int radius)
        {  
            if (state == true)
            {
                tb1.IsEnabled = false; tb2.IsEnabled = false; cb1.IsEnabled = false;
                if (canvas.Children.Count < 1)
                    CreateNewStart(canvas, pointradius, k, chance, max, radius);
                else
                {
                    if (dt.IsEnabled)
                        dt.Stop();
                    else
                        dt.Start();
                }
            }
            else
            {
                canvas.Children.Clear();
                ellipses.Clear();
                CreateNewStart(canvas, pointradius, k, chance, max, radius);
            }          
        }

        private static void CreateNewStart(Canvas canvas, double pointradius, double k, double chance, double max, int radius)
        {
            PointCollection points = circle == true ? GeneratePointsOnCircle(pointradius, radius) : GeneratePoints(pointradius);
            PrintPoints(ellipses, canvas);
            if (points.Count > 3)
            {
                if (IsGA)
                {
                    ga = new GA(points, k, chance, max, radius);
                    ga.Start();
                }
                else
                {
                    gr = new GR(points, radius);
                    gr.Start();
                }
            }
            else
            {
                state = true;
                points.Add(points[0]);
                PrintWay(points, canvas);
                if (pointnum == 1)
                    MessageBox.Show("Для такої кількості точок не існує маршруту");
                else
                    MessageBox.Show("Для такої кількості точок існує тільки одне рішення");
                state = false;
                ReStart();
            }
        }

        private static PointCollection GeneratePointsOnCircle(double radius, int r)
        {
            PointCollection points = new PointCollection();
            double phi = 2 * Math.PI / pointnum;
            for (int i = 0; i <= pointnum; i++)
            {
                Point p = new Point();
                p.X = winwidth / 3 + r * Math.Cos(phi * i);
                p.Y = winheight / 2 + r * Math.Sin(phi * i);
                points.Add(p);
                Ellipse el = new Ellipse();
                ellipses.Add(el);
                el.Width = el.Height = radius;
                el.Fill = Brushes.Azure;
                el.Stroke = Brushes.Gray;
                Canvas.SetLeft(ellipses[i], points[i].X - radius / 2);
                Canvas.SetTop(ellipses[i], points[i].Y - radius / 2);
            }
            return points;
        }
        private static PointCollection GeneratePoints(double radius)
        {
            Random rnd = new Random();
            PointCollection points = new PointCollection();
            for (int i = 0; i < pointnum; i++)
            {
                Point p = new Point();
                p.X = rnd.Next((int)(winheight / 175), (int)(winwidth / 72 * 47));
                p.Y = rnd.Next((int)(radius * 0.25), (int)(winheight * 0.86));
                points.Add(p);
                Ellipse el = new Ellipse();
                ellipses.Add(el);
                el.Width = el.Height = radius;
                el.Fill = Brushes.Azure;
                el.Stroke = Brushes.Gray;
                Canvas.SetLeft(ellipses[i], points[i].X - radius / 2);
                Canvas.SetTop(ellipses[i], points[i].Y - radius / 2);
            }
            return points;
        }

        private static void PrintPoints(List<Ellipse> ellipses, Canvas canvas)
        {
            foreach (Ellipse el in ellipses)
                canvas.Children.Add(el);
        }
        private static void PrintWay(PointCollection p, Canvas canvas)
        {
            Polyline polyline = new Polyline();
            polyline.Points = p;
            polyline.Stroke = Brushes.Purple;
            polyline.StrokeThickness = 1;
            canvas.Children.Add(polyline);
        }

        private static int CheckInput(object sender, int origin)
        {
            int temp = 0;
            string text = ((TextBox)sender).Text;
            if (text != "" && text != "...")
            {
                if (text[0] != '0')
                {
                    if (char.IsNumber(text.Last()))
                        temp = Convert.ToInt32(text);
                    else
                    {
                        ((TextBox)sender).Text = text.Substring(0, text.Length - 1);
                        MessageBox.Show("Enter numbers!!!");                       
                    }
                }
                else
                {
                    ((TextBox)sender).Text = "";
                    MessageBox.Show("First number can't be zero!!!");                   
                }
            }
            return temp == 0 ? origin : temp; 
        }
        private void Tb1_TextChanged(object sender, TextChangedEventArgs e) => pointnum = CheckInput(sender, 0);
        private void Tb2_TextChanged(object sender, TextChangedEventArgs e) => dt.Interval = new TimeSpan(0, 0, 0, 0, CheckInput(sender, 0));
        private void Tb_MouseLeave(object sender, MouseEventArgs e) 
        {
            if (((TextBox)sender).Text == "")
                ((TextBox)sender).Text = "...";
            if (tb1.Text.All(x => char.IsNumber(x)) && tb2.Text.All(x => char.IsNumber(x)) && cb1.SelectedIndex != -1)
                button.IsEnabled = true;
        }
        private void Tb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (((TextBox)sender).Text == "...")
                ((TextBox) sender).Text = "";
        }
        private void Check_DropDownClosed1(ComboBox cb, Canvas canvas)
        {
            canvas.Children.Clear();
            IsGA = cb.SelectedIndex == 0 ? true : false;
        }
        private void Check_DropDownClosed2(ComboBox cb, Canvas canvas)
        {
            canvas.Children.Clear();
            circle = cb.SelectedIndex == 0 ? true : false;
        }

        //may be usefull if it would be done further
        private void Window_SizeChanged(double radius, SizeChangedEventArgs e)
        {
            double kx = e.NewSize.Width / e.PreviousSize.Width;
            double ky = e.NewSize.Height / e.PreviousSize.Height;
            if (kx.ToString().All(x => char.IsNumber(x)) && ky.ToString().All(x => char.IsNumber(x)))
                radius *= (kx + ky) / 2;
            else radius = 15;
            winheight = this.Height;
            winwidth = this.Width;
        }
    }
}
