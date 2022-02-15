using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Web.UI.DataVisualization.Charting;
namespace Prac1
{
    /// <summary>
    /// Логика взаимодействия для StudyMode.xaml
    /// </summary>
    public partial class StudyMode : Window
    {
        static int AttemptCount = 0, countdone = 0, wcounter = 0;
        static string line = "";
        static List<double> data = new List<double>();
        static Stopwatch sw = new Stopwatch();           
        static double[] student = { 6.314, 2.92, 2.353, 2.132, 2.015, 1.943, 1.895, 1.86, 1.833, 1.813, 1.8, 1.782, 1.761, 1.75, 1.75, 1.74, 1.734, 1.725, 1.72 };
       
        public StudyMode()
        {   
            InitializeComponent();
        }

        private void Restart_click(object sender, RoutedEventArgs e)
        {          
            CountProt.IsEnabled = true;
            CountProt.SelectedIndex = -1;
            Input.IsEnabled = false;
            Input.Text = "";
        }
        public (double, double, double) Calc(int i, List<double> dat)
        {          
            int n = dat.Count();         
            double mi = dat.Sum() / Convert.ToDouble(n);           
            double si = dat.Select(x => Math.Pow(x - mi, 2)).ToList().Sum() / (n - 1.0);           
            double t = 0;
            if(data.Count() != 0)
                t = Math.Abs((data[i] - mi) / Math.Sqrt(si));         
            return (mi, si, t);
        }
        public bool Check1()
        {
            int n = data.Count() - 1;
            for (int i = 0; i <= n; i++)
            {              
                var f = Calc(i, data.Where(x => x != data[i]).ToList()); 
                if (f.Item3 > student[n - 2])
                    return false;             
            }
            return true;          
        }
        private void Input_Changed(object sender, TextChangedEventArgs e)
        {
            line = ((TextBox)sender).Text;
            if (line != "" && line.Length <= CodeWord.Content.ToString().Length)
            {
                if (line.Last() == CodeWord.Content.ToString()[wcounter])
                {
                    if (wcounter == 0)
                        sw.Start();
                    else
                    {
                        data.Add(sw.ElapsedMilliseconds / 1000.0);
                        sw.Restart();
                    }
                    wcounter++;
                }
                else
                {
                    MessageBox.Show($"Ти помилився. Ця спроба залишається. \n Спробуй знову, треба ввести ще {AttemptCount - countdone} разів");
                    Redo();                   
                }
                if (line == CodeWord.Content.ToString())
                {
                    if (Check1())
                    {
                        countdone++;
                        var v = Calc(0, data);
                        File.AppendAllText(@"E:\study\DATA.txt", v.Item1 + "\t" + v.Item2 + "\n");

                        if (countdone != AttemptCount)
                            MessageBox.Show($"Це успіх, ця спроба була записана. \n Проте треба ще, приблизно {AttemptCount - countdone} разів");
                        else
                        {
                            countdone = 0;
                            MessageBox.Show("Це недосяжний успіх, все було записано");
                            CountProt.IsEnabled = true;
                            CountProt.SelectedIndex = -1;
                            Input.IsEnabled = false;
                        }
                    }
                    else
                        MessageBox.Show($"Ти помилився з коеф. Стюдента. Ця спроба залишається. \n Спробуй знову, треба ввести ще {AttemptCount - countdone} разів");
                    Redo();
                }
                SymbolCount.Content = wcounter.ToString();
            }
            
                           
        }

        private void Redo()
        {
            sw.Reset();
            Input.Text = "";
            data.Clear();
            wcounter = 0;
        }

        private void Attempt_DropClosed(object sender, EventArgs e)
        {          
            if(CountProt.SelectionBoxItem.ToString() != "")
            {
                AttemptCount = Convert.ToInt32(CountProt.SelectionBoxItem.ToString());
                CountProt.IsEnabled = false;
                Input.IsEnabled = true;
                data.Clear();
            }         
        }
        private void ExitToMain_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            Hide();
            mw.Show();
        }       
    }
}
