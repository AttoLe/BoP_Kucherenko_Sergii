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

namespace lab2
{
    public static class CheckBoxExtension
    {
        static BrushConverter bc = new BrushConverter();
        public static void Settings(this CheckBox checkbox, Grid grid, (int c, int r) loc, string col)
        {
            checkbox.Background = (Brush)bc.ConvertFrom(col);
            Grid.SetColumn(checkbox, loc.Item1);
            Grid.SetRow(checkbox, loc.Item2);
            checkbox.HorizontalAlignment = HorizontalAlignment.Center;
            checkbox.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(checkbox);
        }
    }
    public static class LabelExtension
    {
        static BrushConverter bc = new BrushConverter();

        public static void Setting(this Label l, Grid grid, (int c, int r) loc, string col)
        {
            Grid.SetColumn(l, loc.Item1);
            Grid.SetRow(l, loc.Item2);
            grid.Children.Add(l);
            l.Background = (Brush)bc.ConvertFrom(col);
        }

        public static void TextLabel(this Label l, string t, (FontFamily, int) TextFamilySize)
        {
            l.Content = t;
            l.FontFamily = TextFamilySize.Item1;
            l.FontSize = TextFamilySize.Item2;
        }
    }
    public static class ButtonExtension
    {
        static BrushConverter bc = new BrushConverter();

        public static void Settings(this Button b, Grid grid, (int c, int r) loc, string col)
        {
            Grid.SetColumn(b, loc.Item1);
            Grid.SetRow(b, loc.Item2);
            grid.Children.Add(b);
            b.Background = (Brush)bc.ConvertFrom(col);
        }

        public static void TextButton(this Button b, string t, (FontFamily, int) TextFamilySize)
        {
            b.Content = t;
            b.FontFamily = TextFamilySize.Item1;
            b.FontSize = TextFamilySize.Item2;
        }   

    }
    public static class GridExtension
    {
        static BrushConverter bc = new BrushConverter();

        public static void LocateInAnother(this Grid grid, Grid grid2, (int c, int r) loc)
        {          
            Grid.SetColumn(grid2, loc.Item1);
            Grid.SetRow(grid2, loc.Item2);           
        }

        public static void SetRows(this Grid grid, List<double> l)
        {
            foreach (double d in l)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(d, GridUnitType.Star);
                grid.RowDefinitions.Add(row);
            }
        }

        public static void SetColumns(this Grid grid, List<double> l)
        {
            foreach (double d in l)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(d, GridUnitType.Star);
                grid.ColumnDefinitions.Add(column);
            }
        }

        public static void SetColor(this Grid grid, string c) => grid.Background = (Brush)bc.ConvertFrom(c);
    }
    public static class WindowExtension
    {
        static BrushConverter bc = new BrushConverter();
        public static void Settings(this Window window, (int w, int h) size, string t, string color)
        {          
            window.Title = t;
            window.Width = size.Item1;
            window.Height = size.Item2;
            window.Background = (Brush)bc.ConvertFrom(color);
        }

    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.Settings((900, 450), "MainWindow", "#FF79B2B2");

            FontFamily ff = new FontFamily("Times New Roman");

            Grid mygrid = new Grid();
            this.Content = mygrid;
            List<double> rowsizes = new() { 1, 1.25, 1, 1.5, 0.5, 1.5, 1 };
            mygrid.SetRows(rowsizes);
            List<double> columnsizes = new() { 0.75, 2.5, 0.5, 1.25, 1.25, 1, 1.75, 0.75 };
            mygrid.SetColumns(columnsizes);

            Label title = new Label();
            title.Setting(mygrid, (1, 1), "#FF4D7C90");
            Grid.SetColumnSpan(title, 3);
            title.TextLabel("This is main window", (ff, 40));

            Button Exit = new Button();
            Exit.TextButton("Exit", (ff, 30));
            Exit.Settings(mygrid, (6, 5), "#FFCE3A3A");
            Exit.Click += GoToAnother;

            Button[] buttons = new Button[4];
            int buttoncounter = 1;
            for (int i = 0; i < 4; i += 2)
                for (int j = 0; j < 4; j +=2)
                {                  
                    buttons[i / 2 + j / 2] = new Button();
                    if (j > 1)
                        Grid.SetColumnSpan(buttons[i / 2 + j / 2], 2);
                    buttons[i / 2 + j / 2].Settings(mygrid, (1 + j, 3 + i), "#FFCE3A3A");                   
                    buttons[i / 2 + j / 2].TextButton("Go to window №" + buttoncounter, (ff, 30));
                    buttons[i / 2 + j / 2].Name = "To" + (buttoncounter);
                    buttons[i / 2 + j / 2].Click += GoToAnother;
                    buttoncounter++;
                }
        }
        private void GoToAnother(object sender, RoutedEventArgs e)
        {          
            string s = ((Button)sender).Name;
            this.Hide(); 
            switch (s)
            {
                case "To1":
                    Window1 w = new Window1();
                    w.Show();
                    break;

                case "To2":
                     Window2 w2 = new Window2();
                    w2.Show();
                    break;

                case "To3":
                    Window3 w3 = new Window3();
                    w3.Show();
                    break;

                case "To4":
                    Window4 w4 = new Window4();
                    w4.Show();
                    break;

                default:
                    System.Windows.Application.Current.Shutdown();
                    break;
            }        
        }
    }
    public partial class Window1 : Window
    {
        static string res = "";
        static string path = @"D:\temporary files";
        static Label Output;
        TextBox Inputbox = new TextBox();

        public Window1()
        {
            this.Settings((850, 400), "Window1", "#FF79B2B2");

            FontFamily ff = new FontFamily("Times New Roman");
            string ButtonColor = "#FFCE3A3A";
            string LabelColor = "#FF4D7C90";

            Grid mygrid = new Grid();
            this.Content = mygrid;
            List<double> rows = new List<double>() { 0.5, 0.75, 0.5, 0.5, 0.1, 0.75, 0.1, 0.45, 0.5 };
            mygrid.SetRows(rows);
            List<double> columns = new List<double>() { 0.5, 1.1, 0.15, 1.2, 0.05, 1.5, 0.3, 0.5 };
            mygrid.SetColumns(columns);

            Label title = new Label();
            title.Setting(mygrid, (1, 1), LabelColor);         
            title.TextLabel("This is first window", (ff, 40));         
            Grid.SetColumnSpan(title, 3);
     
            Button save = new Button();
            save.TextButton("Save student", (ff, 30));
            save.Settings(mygrid, (3, 3), ButtonColor);
            save.Click += SaveButton;

            Button delete = new Button();
            delete.TextButton("Delete student", (ff, 30));
            delete.Settings(mygrid, (5, 3), ButtonColor);
            delete.Click += DeleteButton;

            Button ToMain = new Button();
            ToMain.TextButton("Go to main window", (ff, 30));
            ToMain.Settings(mygrid, (5, 5), ButtonColor);
            Grid.SetRowSpan(ToMain, 3);
            Grid.SetColumnSpan(ToMain, 2);

            ToMain.Click += (object sender, RoutedEventArgs e) => {
                this.Hide();
                MainWindow mw = new MainWindow();
                mw.Show();
            };
            Output = new Label();
            Output.Setting(mygrid, (1, 5), LabelColor);
            Output.TextLabel("", (ff, 30));
            Grid.SetColumnSpan(Output, 3);

            Label Form = new Label();
            Form.Setting(mygrid, (1, 7), LabelColor);
            Form.TextLabel("Write like: full name +phone_number email", (ff, 20));
            Grid.SetColumnSpan(Form, 3);

            BrushConverter bc = new BrushConverter();

            Grid.SetColumn(Inputbox, 1);
            Grid.SetRow(Inputbox, 3);
            Inputbox.Background = (Brush)bc.ConvertFrom("#FF4D7C90");
            Inputbox.FontFamily = ff;
            Inputbox.FontSize = 30;
            mygrid.Children.Add(Inputbox);
            Inputbox.KeyUp += KeyUp;
            Inputbox.MouseEnter += MouseEnter;
            Inputbox.MouseLeave += MouseLeave;
        }

        private new void MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBox)sender).IsReadOnly = true;
            ((TextBox)sender).Text = "Enter text here";
            if (res != null)
                Output.Content = res.Length > 25 ? res.Substring(res.Length - 25) : res;
        }
        private new void MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBox)sender).Text = null;
            ((TextBox)sender).IsReadOnly = false;
            if (res != null)
                Output.Content = res.Length > 25 ? res.Substring(res.Length - 25) : res;
        }
        private new void KeyUp(object sender, KeyEventArgs e)
        {
            string input = ((TextBox)sender).Text;
            if (input.Length > 25)
                Output.Content = input.Substring(input.Length - 25);
            else
                Output.Content = input;
            res = input;
        }

        public void DeleteButton(object sender, RoutedEventArgs e)
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
        private void SaveButton(object sender, RoutedEventArgs e)
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
                {
                    File.AppendAllText(path, name.Match(res) + " - " + number.Match(res) + " - " + email.Match(res) + "\n");
                    MessageBox.Show("Writed!!!");
                }       
                else
                    MessageBox.Show("Incorrect Input!!!");
                res = null;
                Output.Content = null;               
            }
        }
    }  
    public partial class Window2 : Window
    {
        static ComboBox[,] mat = new ComboBox[5, 5];
        static string last = "";
        public Window2()
        {
            this.Settings((800, 450), "Window2", "#FF79B2B2");

            FontFamily ff = new FontFamily("Times New Roman");
            string ButtonColor = "#FFCE3A3A";
            string LabelColor = "#FF4D7C90";

            Grid mygrid1 = new Grid();
            this.Content = mygrid1;           
            List<double> rows1 = new List<double>() { 1, 2.25, 1, 6, 2.75, 1 };
            mygrid1.SetRows(rows1);
            List<double> columns1 = new List<double>() { 1.75, 6, 1, 1, 4, 1.75 };
            mygrid1.SetColumns(columns1);

            Label title = new Label();
            title.Setting(mygrid1, (1,1), LabelColor);
            title.TextLabel("This is second window", (ff, 40));
            Grid.SetColumnSpan(title, 3);

            Button ToMain = new Button();
            ToMain.TextButton("Go to main window", (ff, 30));
            ToMain.Settings(mygrid1, (3, 4), ButtonColor);
            Grid.SetColumnSpan(ToMain, 2);
            ToMain.Click += (object sender, RoutedEventArgs e) => {
                this.Hide();
                MainWindow mw = new MainWindow();
                mw.Show();
            };

            Grid mygrid2 = new Grid();
            mygrid1.Children.Add(mygrid2);
            mygrid1.LocateInAnother(mygrid2, (1, 3));
            Grid.SetRowSpan(mygrid2, 2);
            List<double> rows2 = new List<double>() { 1, 0.05, 1, 0.05, 1, 0.05, 1, 0.05, 1 };
            mygrid2.SetRows(rows2);
            List<double> columns2 = new List<double>() { 1, 0.05, 1, 0.05, 1, 0.05, 1, 0.05, 1 };
            mygrid2.SetColumns(columns2);
            TextBox d = new TextBox();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    mat[i, j] = new ComboBox();
                    Grid.SetRow(mat[i, j], i * 2);
                    Grid.SetColumn(mat[i, j], j * 2);
                    mat[i, j].Background = Brushes.White;
                    mat[i, j].FontSize = 30;
                    mat[i, j].Items.Add("x");
                    mat[i, j].Items.Add("o");
                    mat[i, j].DropDownClosed += (object sender, EventArgs e) => { Check_DropDownClosed(sender, i, j); };
                    mygrid2.Children.Add(mat[i, j]);
                }
            }
        }

        private void Check_DropDownClosed(object sender, int i, int j)
        {
            string s = ((ComboBox)sender).Text.ToString();

            if (s != "")
            {
                if (Check1())
                {
                    MessageBox.Show("The end");
                    foreach (ComboBox n in mat)
                    {
                        n.IsEnabled = true;
                        n.Text = "";
                    }
                }
                else
                {
                    ((ComboBox)sender).IsEnabled = false;
                    foreach (ComboBox n in mat)
                    {
                        if (n.IsEnabled == true) 
                        {
                            if (last != "")
                                n.Items.Add(last);
                            n.Items.Remove(s);
                        }                       
                    }
                    last = s;
                }                               
            }
        }

        private static bool Check1()
        {
            return Check2() || Check3();
        }
        private static bool Check2()
        {
            for (int j1 = 0; j1 < 2; j1++)
                for (int i1 = 0; i1 < 5; i1++)
                {
                    int count1 = 0, count2 = 0, count3 = 0, count4 = 0;
                    int p = 0;
                    for (int n = 0; n < 4; n++)
                    {
                        if (p + j1 < 5)
                        {
                            if (mat[p + j1, i1].Text == "x")
                                count3++;
                            if (mat[p + j1, i1].Text == "o")
                                count4++;
                            if (mat[i1, j1 + p].Text == "x")
                                count1++;
                            if (mat[i1, j1 + p].Text == "o")
                                count2++;
                        }
                        p++;
                        if (count1 == 4 || count2 == 4 || count3 == 4 || count4 == 4)
                            return true;
                    }

                }
            return false;
        }
        private static bool Check3()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int i2 = 0, j2 = 0, count5 = 0, count6 = 0;
                    for (int n = 0; n < 4; n++)
                    {
                        if (i + i2 < 5 && j + j2 < 5)
                        {
                            if (mat[i + i2, j + j2].Text == "x")
                                count5++;
                            if (mat[i + i2, j + j2].Text == "o")
                                count6++;
                        }
                        i2++; j2++;
                        if (count5 == 4 || count6 == 4)
                            return true;
                    }
                }
            }
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int i2 = 4, j2 = 0, count5 = 0, count6 = 0;
                    for (int n = 0; n < 4; n++)
                    {
                        if (i2 - i >= 0 && j + j2 < 5)
                        {
                            if (mat[i2 - i, j + j2].Text == "x")
                                count5++;

                            if (mat[i2 - i, j + j2].Text == "o")
                                count6++;
                        }
                        i2--; j2++;
                        if (count5 == 4 || count6 == 4)
                            return true;
                    }

                }
            }
            return false;
        }       
    }
    public partial class Window3 : Window
    {
        static Label Output;
        static string temp;
        static Regex nandmultp = new Regex(@"(((\-|\+)?(\d|\.)*)(\*)((\-|\+)?(\d|\.)*))");
        static Regex nanddiv = new Regex(@"(((\-|\+)?(\d|\.)*)\/((\-|\+)?(\d|\.)*))");
        static Regex num = new Regex(@"((\-|\+)?(\d|\.)*)");
        public Window3()
        {
            this.Settings((800, 450), "Window3", "#FF79B2B2");

            FontFamily ff = new FontFamily("Times New Roman");
            string ButtonColor = "#FFCE3A3A";
            string LabelColor = "#FF4D7C90";

            Grid mygrid1 = new Grid();
            this.Content = mygrid1;
            List<double> rows1 = new List<double>() { 1, 1.5, 1.5, 3, 3, 1 };
            mygrid1.SetRows(rows1);            
            List<double> columns1 = new List<double>() { 1, 3.25, 0.75, 3, 1 };
            mygrid1.SetColumns(columns1);

            Label title = new Label();
            title.Setting(mygrid1, (1, 1), LabelColor);
            title.TextLabel("This is third window", (ff, 40));
            Grid.SetColumnSpan(title, 2);

            Button ToMain = new Button();
            ToMain.TextButton("Go to main window", (ff, 30));
            ToMain.Settings(mygrid1, (3, 4), ButtonColor);
            ToMain.Click += (object sender, RoutedEventArgs e) => {
                this.Hide();
                MainWindow mw = new MainWindow();
                mw.Show();
            };           

            Grid mygrid2 = new Grid();
            mygrid1.Children.Add(mygrid2);
            mygrid1.LocateInAnother(mygrid2, (1, 3));
            Grid.SetRowSpan(mygrid2, 2);
            
            List<double> rows2 = new List<double>() { 1, 0.1, 1, 0.1, 1, 0.1, 1 };
            mygrid2.SetRows(rows2);
            List<double> columns2 = new List<double>() { 0.8, 0.075, 0.8, 0.075, 0.8, 0.075, 0.8, 0.075, 0.8 };
            mygrid2.SetColumns(columns2);   
            
            Button[,] mat = new Button[3, 3];
            int counter = 9;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 2; j >= 0; j--)
                {
                    mat[i, j] = new Button();
                    mat[i, j].Settings(mygrid2, (2 + j * 2, 2 + i * 2 ), "#FFDDDDDD");
                    mat[i, j].TextButton(counter.ToString(), (ff, 30));
                    mat[i, j].Click += Number_Click;
                    counter--;
                }
            }

            Button zero = new Button();
            zero.Settings(mygrid2, (0, 6), "#FFDDDDDD");
            zero.TextButton("0", (ff, 30));
            zero.Click += Number_Click;

            Button minus = new Button();
            minus.Settings(mygrid2, (0, 4), "#FFDDDDDD");
            minus.TextButton("-", (ff, 30));
            minus.Click += (object sender, RoutedEventArgs e) =>
                            { WriteOp_Click('-'); }; 

            Button plus = new Button();
            plus.Settings(mygrid2, (0, 2), "#FFDDDDDD");
            plus.TextButton("+", (ff, 30));
            plus.Click += (object sender, RoutedEventArgs e) =>
                            { WriteOp_Click('+'); };

            Button equal = new Button();
            equal.Settings(mygrid2, (4, 0), "#FFDDDDDD");
            equal.TextButton("=", (ff, 30));
            equal.Click += Calc_Click;

            Button backspace = new Button();
            backspace.Settings(mygrid2, (6, 0), "#FFDDDDDD");
            backspace.TextButton("<=", (ff, 30));
            backspace.Click += Backspase_Click;

            Button C = new Button();
            C.Settings(mygrid2, (8, 0), "#FFDDDDDD");
            C.TextButton("C", (ff, 30));
            C.Click += DeleteAll_Click;

            Button mult = new Button();
            mult.Settings(mygrid2, (8, 2), "#FFDDDDDD");
            mult.TextButton(Char.ConvertFromUtf32(215), (ff, 30));
            mult.Click += (object sender, RoutedEventArgs e) => 
                            { WriteOp_Click('*'); };

            Button div = new Button();
            div.Settings(mygrid2, (8, 4), "#FFDDDDDD");
            div.TextButton(Char.ConvertFromUtf32(247), (ff, 30));
            div.Click += (object sender, RoutedEventArgs e) =>
                            { WriteOp_Click('/'); };

            Button notdot = new Button();
            notdot.Settings(mygrid2, (8, 6), "#FFDDDDDD");
            notdot.TextButton(".", (ff, 30));
            notdot.Click += NotDot_Click;

            Output = new Label();
            Grid.SetColumnSpan(Output, 3);
            Output.Setting(mygrid2, (0, 0), LabelColor);
            Output.TextLabel("", (ff, 20));          
        }

        private void Number_Click(object sender, RoutedEventArgs e) => 
            Output.Content += ((Button)sender).Content.ToString();
        private void WriteOp_Click(char op)
        {
            if(Output.Content.ToString() == "" && (op == '*' || op == '/'))
                MessageBox.Show("Enter numbers");
            else
            if (Output.Content.ToString() == "" && op == '-')
                Output.Content += op.ToString();
            else
            {
                char last = Output.Content.ToString().ToArray().Last();
                if (Output.Content.ToString() == "" || Output.Content.ToString() != "" && last != '+' && last != '-'
                    && ((op != '*' && op != '/') || last != '*' && last != '/'))
                        Output.Content += op.ToString();
                else MessageBox.Show("Enter numbers");
            }
        }
        private void NotDot_Click(object sender, RoutedEventArgs e)
        {
            if (Output.Content.ToString() != "")
            {
                char last = Output.Content.ToString().ToArray().Last();
                if (last == '.')
                    MessageBox.Show("Enter numbers");
                else
                    if (Output.Content.ToString().Any(x => !char.IsNumber(x)) && Output.Content.ToString().ToArray().Where(x => !char.IsNumber(x)).Last() == '.')
                        MessageBox.Show("Enter numbers");
                    else
                        Output.Content += ".";
            }
            else
                Output.Content += "0.";
        }         

        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            Output.Content = "";
        }
        private void Backspase_Click(object sender, RoutedEventArgs e)
        {
            if (Output.Content.ToString() != "")
                Output.Content = Output.Content.ToString().Substring(0, Output.Content.ToString().Length - 1);
            else MessageBox.Show("Enter numbers");
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {           
            temp = Output.Content.ToString();
            while (temp.Contains('*'))
            {
                MultpAndDivCalc('*');
            }
            while (temp.Contains('/'))
            {
                Output.Content = "s " + temp;
                MultpAndDivCalc('/');
            }                  
            while (temp[0] != '+' && (temp.Contains('-') || temp.Contains('+')) && !(temp[0] == '-' && temp.Where(x => !char.IsNumber(x) && x != '.').Count() <= 1))
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
        private static void MultpAndDivCalc(char c)
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
    }
    public partial class Window4 : Window
    {
        public Window4()
        {
            this.Settings((800, 450), "Window4", "#FF79B2B2");

            FontFamily ff = new FontFamily("Times New Roman");
            string ButtonColor = "#FFCE3A3A";
            string LabelColor = "#FF4D7C90";

            Grid mygrid = new Grid();
            this.Content = mygrid;            
            List<double> rows = new List<double>() { 0.5, 0.75, 0.5, 0.5, 0.1, 0.5, 0.1, 0.5, 0.1, 0.5, 0.5 };
            mygrid.SetRows(rows);
            List<double> columns = new List<double>() { 0.35, 0.5, 6, 0.2, 2.75, 0.35 };
            mygrid.SetColumns(columns);

            Label title = new Label();
            title.Setting(mygrid, (2, 1), LabelColor);
            title.TextLabel("This is fourth window", (ff, 40));
 
            List<string> lines = new List<string>(){ " Brief information about author:",
                " Kucherenko Sergii Olegovych", " 17y.o - Date of birth - 2004.08.06",
                " NTUU on Faculty of Applied Mathematics"};
           
            List<Label> labels = new List<Label>();
            List<CheckBox> checkboxes = new List<CheckBox>();

            for(int i = 0; i < 4; i++)
            {
                checkboxes.Add(new CheckBox());
                checkboxes[i].Settings(mygrid, (1, 3 + i * 2), LabelColor);
                labels.Add(new Label());
                labels[i].Setting(mygrid, (2, 3 + i*2), LabelColor);
                labels[i].TextLabel(lines[i], (ff, 20));
            }
           
            Button ToMain = new Button();
            ToMain.Settings(mygrid, (4, 7), ButtonColor);
            ToMain.TextButton("Go to main window", (ff, 25));
            Grid.SetRowSpan(ToMain, 3);
            ToMain.Click += (object sender, RoutedEventArgs e) => {
                this.Hide();
                MainWindow mw = new MainWindow();
                mw.Show();
            }; 
        }
    }
}
