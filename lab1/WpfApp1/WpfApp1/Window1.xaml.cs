using System;
using System.Collections.Generic;
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

using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    
    public partial class Window1 : Window
    {
        static string res = "";
        static string path = @"E:\study\DATA.txt";
        public Window1()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            Hide();
            mw.Show();
        }
        
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            string input = InputBox.Text;
            if (input.Length > 25)
                Output.Content = input.Substring(input.Length-25);
            else
               Output.Content = input;
            res = input;
        }

        private void InputBox_MouseEnter(object sender, MouseEventArgs e)
        {
            InputBox.Text = null;
            if(res != null)
                Output.Content = res.Length > 25 ? res.Substring(res.Length - 25) : res;
        }

        private void InputBox_MouseLeave(object sender, MouseEventArgs e)
        {
            InputBox.Text = "Enter text here";
            if (res != null)
                Output.Content = res.Length > 25 ? res.Substring(res.Length - 25) : res;
        }
       
        private void Delete_Click_1(object sender, RoutedEventArgs e)
        {
            if (res != null)
            {
                string dat = File.ReadAllText(path);
                
                string[] ar = dat.Split('\n').Where(c => c != null && !c.Contains(res)).ToArray();
                string ans = "";
                foreach (var st in ar)
                    if (!string.IsNullOrEmpty(st))
                        ans += st + "\n";
                if (ans == dat)
                    MessageBox.Show("Noone was founded!!!");
                else
                {
                    File.Delete(path);
                    File.AppendAllText(path, ans);
                    MessageBox.Show("Deleted!!!");
                }
                res = null;
                Output.Content = null;
            }
            else MessageBox.Show("Enter something!!!");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (res == null)
                MessageBox.Show("Enter something!!!");
            else
            {
                Regex name = new Regex(@"(((\D|\-&\S)*)\s){3}");
                Regex number = new Regex(@"\+(\d){11}");
                Regex email = new Regex(@"(\w|\d|\.)*\@(\w|\d){1,6}\.(\w|\d){2}");
                res = Regex.Replace(res, @"(\ )+", " ");
                if (!string.IsNullOrEmpty(name.Match(res).ToString()) && !string.IsNullOrEmpty(number.Match(res).ToString()) && !string.IsNullOrEmpty(email.Match(res).ToString()))
                    File.AppendAllText(path, name.Match(res) + " - " + number.Match(res) + " - " + email.Match(res)+"\n");
                res = null;
                Output.Content = null;
                MessageBox.Show("Writed!!!");
            }
        }

        
    }
}
