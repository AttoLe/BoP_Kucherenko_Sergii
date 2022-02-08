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

namespace WpfApp1
{
    public partial class Window2 : Window
    {

        public Window2()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            Hide();
            mw.Show();
        }

        static ComboBox[,] mat = new ComboBox[5, 5];
        private void Table_Initialized(object sender, EventArgs e)
        {
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
                    Table.Children.Add(mat[i, j]);
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
                    ((ComboBox)sender).IsEnabled = false;
            }
        }
        private static bool Check1()
        {
            return Check2() || Check3();
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
        
    }
}
