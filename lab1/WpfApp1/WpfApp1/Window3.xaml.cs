using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    public partial class Window3 : Window
    {
        static string temp;
        static Regex nandmultp = new Regex(@"(((\-|\+)?(\d|\,)*)(\*)((\-|\+)?(\d|\,)*))");
        static Regex nanddiv = new Regex(@"(((\-|\+)?(\d|\,)*)\/((\-|\+)?(\d|\,)*))");
        static Regex num = new Regex(@"((\-|\+)?(\d|\,)*)");
        public Window3()
        {
            InitializeComponent();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            Hide();
            mw.Show();
        }

        private void WriteOp(string s)
        {
            if (Output.Content.ToString().ToArray().Length == 0 && s == "-")
                Output.Content += s;
            else
            {
                char last = Output.Content.ToString().ToArray().Last();
                if (Output.Content.ToString() != "" && last != '+' && last != '-' && ((s != "*" && s != "/") || last != '*' && last != '/'))
                    Output.Content += s;
                else MessageBox.Show("Enter numbers");
            }
        }

        private void PlusAndMin_Click(object sender, RoutedEventArgs e)
        {
            string b = ((Button)sender).Content.ToString();
            WriteOp(b);
        }

        private void Div_Click(object sender, RoutedEventArgs e)
        {
            WriteOp("/");
        }

        private void Mulp_Click(object sender, RoutedEventArgs e)
        {
            WriteOp("*");
        }

        private void Backspase_Click(object sender, RoutedEventArgs e)
        {
            if (Output.Content.ToString() != "")
                Output.Content = Output.Content.ToString().Substring(0, Output.Content.ToString().Length - 1);
            else MessageBox.Show("Enter numbers");
        }

        private void C_Click(object sender, RoutedEventArgs e)
        {
            Output.Content = "";
        }

        private void notdot_Click(object sender, RoutedEventArgs e)
        {
            if (Output.Content.ToString() != "")
            {
                char last = Output.Content.ToString().ToArray().Last();
                if (last == ',')
                    MessageBox.Show("Enter numbers");
                else
                    if (Output.Content.ToString().Any(x => !char.IsNumber(x)))
                    {
                        if (Output.Content.ToString().ToArray().Where(x => !char.IsNumber(x)).Last() == ',')
                            MessageBox.Show("Enter numbers");
                        else
                            Output.Content += ",";
                    }
                    else
                        Output.Content += ",";
            }
            else
                Output.Content += "0,";
        }
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            var b = ((Button)sender).Content.ToString();
            Output.Content += b;
        }
        
        private static void MultpAndDiv(char c)
        {
            string whole;
            if (c == '*')
                whole = nandmultp.Match(temp).ToString();
            else
                whole = nanddiv.Match(temp).ToString();
            string s1 = whole.Split(c).First(); string s2 = whole.Split(c).Last();
            double n1 = Convert.ToDouble(s1); double n2 = Convert.ToDouble(s2);
            temp = c == '*' ? temp.Replace(whole, "+" + Convert.ToString(n1 * n2)) : temp.Replace(whole, "+" + Convert.ToString(n1 / n2));
            temp = temp.Replace("+-", "-");
            temp = temp.Replace("--", "-");
            temp = temp.Replace("-+", "-");
            temp = temp[0] == '+' ? temp.Substring(1) : temp;
        }
    
        private void Calc_Click(object sender, RoutedEventArgs e)
        {         
            temp = Output.Content.ToString();
            while (temp.Contains('*'))
            {
                MultpAndDiv('*');
            }
            while (temp.Contains('/'))
            {
                Output.Content = "s " + temp;
                MultpAndDiv('/');
            }
            while (temp[0] != '+' && (temp.Contains('-') || temp.Contains('+')) && !(temp[0] == '-' && temp.Where(x => !char.IsNumber(x) && x != ',').Count() <= 1))
            {         
                string s1 = num.Match(temp).ToString();
                double n1 = Convert.ToDouble(s1);
                char c = temp.Substring(s1.Length)[0];
                temp = temp.Substring(s1.Length + 1);
                string s2 = num.Match(temp).ToString();
                double n2 = Convert.ToDouble(s2);
                if (temp.Length <= s2.Length)
                    temp = "";
                else
                    temp = temp.Substring(s2.Length);
                temp = (c == '-' ? (n1 - n2) : (n1 + n2)) + temp;
            }
            if (temp.Length > 10)
            {
                int p = temp[10] > 5 ? 1 : 0;
                temp = temp.Substring(0, 9) + (Convert.ToInt16(temp[10].ToString()) + p);
            }
            Output.Content = temp;                      
        }
    }
}
