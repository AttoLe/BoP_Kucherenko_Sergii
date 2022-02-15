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
using System.Text.RegularExpressions;

namespace Prac1
{
    /// <summary>
    /// Логика взаимодействия для LoginMode.xaml
    /// </summary>
    public partial class LoginMode : Window
    {    
        static bool author = false;
        static int AttemptCount = 0, wcounter = 0, countdone = 0, firight = 0, r = 0;
        static string line = "";
        static List<double> data = new List<double>();
        static Stopwatch sw = new Stopwatch();
        static double alpha = 0;
        static double[] student = {6.314, 2.92, 2.353, 2.132, 2.015, 1.943, 1.895, 1.86, 1.833, 1.813, 1.8, 1.782, 1.761, 1.75, 1.75, 1.74, 1.734, 1.725, 1.72};
        static double[] fisher = {161, 19.0, 9.28, 6.39, 5.05, 4.28, 3.79, 3.44, 3.18, 2.98, 2.82, 2.69};
        static StudyMode sm = new StudyMode();
        public LoginMode()
        {
            InitializeComponent();
        }

        private void ExitToMain_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            Hide();
            mw.Show();
        }      

        private void Input_Changes(object sender, TextChangedEventArgs e)
        {           
            line = Input.Text;
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
                    countdone++;
                    var v = sm.Calc(0, data);
                    if (countdone <= AttemptCount)
                    {
                        if(countdone != AttemptCount)
                            MessageBox.Show($"Це успіх, ця спроба була записана. \n Проте треба ще, приблизно {AttemptCount - countdone} разів");
                        
                        double n = data.Count();                       
                        double mi2 = v.Item1;
                        double si2 = v.Item2;
                        string[] examples = File.ReadAllText(@"E:\study\DATA.txt").Split("\n").Where(x => x != "" && x != null).ToArray();
                        foreach (string g in examples)
                        {
                            double si1 = Convert.ToDouble(g.Split("\t").Last());
                            double mi1 = Convert.ToDouble(g.Split("\t").First());
                            double fi = Math.Max(si1, si2) / Math.Min(si1, si2);
                            double s = Math.Sqrt((si1 + si2) * (n - 1.0) / (2.0 * n - 1));
                            double tp = Math.Abs(mi1 - mi2) / (s * Math.Sqrt(2.0 / n));

                            if (fi < fisher[data.Count() - 1])
                                firight++;

                            if (tp < student[Convert.ToInt32(n) - 2])
                                r++;

                        }
                    }
                    if (countdone == AttemptCount)
                    {
                        double max = File.ReadAllText(@"E:\study\DATA.txt").Split("\n").Where(x => x != "").Count();                      
                        Variance.Content = firight > AttemptCount * max / 2.0 ? "Однорідні" : "Неоднорідні";
                        Identification.Content = 1.0 * r / (max * AttemptCount);
                        if (author)
                        {
                            mist1.Content = 1.0 - 1.0 * r / (max * AttemptCount);
                            if (Convert.ToDouble(Identification.Content) > 0.75 && Variance.Content.ToString() == "Однорідні")
                                File.AppendAllText(@"E:\study\DATA.txt", v.Item1 + "\t" + v.Item2 + "\n");
                        }
                        else
                            mist2.Content = 1.0 * r / (max * AttemptCount);                       
                        countdone = 0;
                        CountProt.IsEnabled = true;
                        CountProt.SelectedIndex = -1;
                        Input.IsEnabled = false;
                        firight = 0; r = 0;
                        Author.IsEnabled = true;
                        
                    }
                    Redo();

                }
                SymbolCount.Content = wcounter.ToString();
            }
           
        }

        private void AuthorChange(object sender, RoutedEventArgs e)
        {
            ResetStats();
            author = Convert.ToBoolean(((CheckBox)sender).IsChecked);
            CheckAuthor();  
        }

        private void CheckAuthor()
        {
            if (author)
            {
                mist2.Content = "Вводить автор";
                mist1.Content = "Немає даних";
            }
            else
            {
                mist1.Content = "Вводить не автор";
                mist2.Content = "Немає даних";
            }
        }

        private void ResetStats()
        {       
            if(mist1.Content.ToString().Where(x => Char.IsNumber(x)).Count() > 0 || mist2.Content.ToString().Where(x => Char.IsNumber(x)).Count() > 0)
            {
                CheckAuthor();
                Identification.Content = "немає даних";
                Variance.Content = "немає даних";
            }
        }

        private void Redo()
        {
            sw.Reset();
            Input.Text = "";
            data.Clear();
            wcounter = 0;
        }

        private void Attempts_DropDown(object sender, EventArgs e)
        {
            ResetStats();
            if (CountProt.SelectionBoxItem.ToString() != "")
            {
                AttemptCount = Convert.ToInt32(CountProt.SelectionBoxItem.ToString());
                if (Alpha.SelectionBoxItem.ToString() != "")
                {
                    CountProt.IsEnabled = false;
                    Input.IsEnabled = true;
                    data.Clear();
                }
            }
        }

        private void Alpha_DropDownClosed(object sender, EventArgs e)
        {
            ResetStats();
            if (Alpha.SelectionBoxItem.ToString() != "") 
            {              
                alpha = Convert.ToDouble(Alpha.SelectionBoxItem.ToString());
                if(CountProt.SelectionBoxItem.ToString() != "")
                {
                    CountProt.IsEnabled = false;
                    Input.IsEnabled = true;
                    data.Clear();
                }

            }
        }
    }
}
