using System;
using System.Collections.Generic;
using System.IO;
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
using LiveCharts;
using LiveCharts.Wpf;
using Separator = LiveCharts.Wpf.Separator;

namespace prac2._1
{

    public partial class MainWindow : Window
    {
        private List<List<string>> numb;
        public static List<string> pop = new (), mutation = new ();
        
        private double lowest;
        private Func<double, double> f = d => d.ToString().Length > 3 ? Math.Round(d, 5) : d; 
        public static Regex ms = new(@"\d*\.\d{5}"), popul = new(@"\d*");
        
        public static Brush[] color = {Brushes.White, CreateColor("#A4D1B0"), CreateColor("#7DBD8D"),
                CreateColor("#8EFFFFFF")}, br = {Brushes.Red, Brushes.Blue, Brushes.Indigo, Brushes.Pink, Brushes.WhiteSmoke};
        
        public MainWindow()
        {
            Background = color[1];
            
            string[] path = {@"D:\temporary files\res0.9.txt", @"D:\temporary files\res0.75.txt", 
                @"D:\temporary files\res0.5.txt", @"D:\temporary files\res0.25.txt",  @"D:\temporary files\res0.1.txt"};
            List<List<string>> data = new List<List<string>>();
            
            Grid mygrid = new Grid();
            Content = mygrid;
            double[] col = {0.25, 4.5, 7, 2, 0.25};
            double[] row = {0.25, 0.75, 0.1, 5, 1, 0.25};
            foreach (var c in col)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(c, GridUnitType.Star);
                mygrid.ColumnDefinitions.Add(cd);
            }
            foreach (var r in row)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(r, GridUnitType.Star);
                mygrid.RowDefinitions.Add(rd);
            }

            for (int i = 0 ; i < path.Length; i++)
            {
                List<string> l = File.ReadAllText(path[i]).Split("\n").ToList();
                mutation.Add(l[0].Split(" ")[1]);
                data.Add(l.GetRange(1, l.Count - 1));
            }

            Separator sep = new Separator {
                Step = 1,
                Stroke = color[3],
                StrokeDashArray = new DoubleCollection {5},
                StrokeThickness = 1
            };
            Separator sep2 = new Separator {
                Stroke = color[3],
                StrokeThickness = 1,
            };
            
            CartesianChart cs;
            SeriesCollection sc = new();
            
            DefaultLegend dl = new DefaultLegend {
                Orientation = Orientation.Vertical,
                BulletSize = 13,
                FontSize = 20,
                Foreground = color[0],
            };
            
            cs = new CartesianChart {
                Background = color[2],
                Series = sc ,
                ChartLegend = dl,
                LegendLocation = LegendLocation.Right
            };
            Grid.SetColumn(cs, 1); Grid.SetColumnSpan(cs, 3);
            Grid.SetRow(cs, 3); Grid.SetRowSpan(cs, 2);
            mygrid.Children.Add(cs);
            cs.AxisX.Add(new Axis {
                Title = "Amount of elements in population",
                FontSize = 20,
                Foreground = Brushes.White,
                Position = AxisPosition.LeftBottom,
                Separator = sep
            });
            cs.AxisY.Add(new Axis {
                Title = "Average time",
                FontSize = 20,
                Foreground = Brushes.White,
                Position = AxisPosition.LeftBottom,
                Separator = sep2
            });
            cs.DataClick += (sender, point) => { Dot_Click(point); };

            TextBlock tb = new TextBlock {
                Foreground = color[0],
                Background = color[2],
                FontSize = 25,
                TextAlignment = TextAlignment.Center
            };
            Grid.SetColumn(tb, 1);
            Grid.SetRow(tb, 1);
            mygrid.Children.Add(tb);
            
            Button b = new Button {
                Background = color[2],
                Foreground = Brushes.White,
                Content = "Next",
                FontSize = 25,
                BorderThickness = new Thickness(0),
            };
            b.Click += (object sender, RoutedEventArgs e) => { Button_Click(sc, data, cs, b, tb); };
            Grid.SetColumn(b, 3);
            Grid.SetRow(b, 1);
            mygrid.Children.Add(b);

            Button_Click(sc, data, cs, b, tb);
            InitializeComponent();
        }
        private void Dot_Click(ChartPoint point)
        {
            if (Math.Abs(Convert.ToDouble(point.Y) - lowest) < Math.Pow(10, -5))
                MessageBox.Show("This is the most quickly option");
            else 
                MessageBox.Show("This is NOT the most quickly option");
        }

        private void Button_Click(SeriesCollection sc, List<List<string>> data, CartesianChart cs, Button b, TextBlock tb)
        {
            if (data.Any(x => x.Count == 0))
            {
                MessageBox.Show("That's the end");
                System.Windows.Application.Current.Shutdown();
                return;
            }

            tb.Text = data[0][0];
            b.IsEnabled = false;
            numb = new List<List<string>>();
            for (int i = 0; i < data.Count; i++)
            {
                data[i] = data[i].GetRange(1, data[i].Count - 1);
                List<string> temp = data[i].Any(x => x.Contains("Number")) ? 
                    data[i].Where(x => data[i].IndexOf(x) < data[i].IndexOf(data[i].First(y => y.Contains("Number")))).ToList()
                        : data[i];
                if (i == 0)
                    pop = temp.Select(x => popul.Match(x).ToString()).ToList();
                numb.Add(temp.Select(x => ms.Match(x).ToString()).ToList());
                data[i] = data[i].GetRange(temp.Count, data[i].Count - temp.Count);
            }
            ChartChange(sc, cs, data, b);
        }
        private void ChartChange(SeriesCollection sc, CartesianChart s, List<List<string>> data, Button b)
        {
            if(sc.Count != 0)
                sc.Clear();

            s.AxisX[0].Labels = pop;
            s.AxisY[0].LabelFormatter = d => f(d) + "ms"; 
            
            for (int i = 0; i < data.Count; i++)
            {
                sc.Add(new LineSeries {
                    Title = "Mutation " + mutation[i],
                    Values = new ChartValues<double>(numb[i].Where(x => x.Length > 1).Select(x => Convert.ToDouble(x))),
                    Stroke = br[i],
                    Fill = Brushes.Transparent,
                    StrokeThickness = 2,
                });
            }
            
            lowest = Convert.ToDouble(sc[0].Values[0]);
            foreach (var sc2 in sc)
                foreach (var d in sc2.Values)
                    if (Convert.ToDouble(d) < lowest)
                        lowest = Convert.ToDouble(d);
            
            b.IsEnabled = true;
        }

        private static Brush CreateColor(string color)
        {
            BrushConverter bc = new();
            return (Brush) (bc.ConvertFrom(color));
        }
    }
}