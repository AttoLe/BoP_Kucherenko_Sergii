using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;


namespace prac3
{
    public class Tech
    {
        private static readonly SqlConnection connection = new SqlConnection(@"Data Source=SERHII; Initial Catalog=prac3; Integrated Security = True");

        public static bool FitRestriction(string text) => 
            text.Any(x => char.IsUpper(x))
            && text.Any(x => char.IsLower(x))
            && text.Any(x => !char.IsLetter(x));

        private static SqlCommand CreateCommand(string text) =>
             new SqlCommand(text, connection);

        public static string GetOneString(string com)
        {
            connection.Open();
            SqlCommand command = CreateCommand(com);
            string text = (string)command.ExecuteScalar();
            connection.Close();
            return text;
        }
        public static List<string> GetMultipleString(string text)
        {
            connection.Open();
            List<string> data = new List<string>();
            SqlCommand command = CreateCommand(text);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
                for (int i = 0; i < reader.FieldCount; i++)
                    data.Add(reader[i].ToString());

            connection.Close();
            return data;
        }

        public static void UpdateOneString(string com)
        {
            connection.Open();
            SqlCommand command = CreateCommand(com);            
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static void FillTable(string com, DataTable dataTable)
        {
            connection.Open();
            SqlDataAdapter ad = new SqlDataAdapter(com, connection);
            ad.Fill(dataTable);                                  
            connection.Close();
        }

        public static string InsertOneString(string text)
        {
            string s;
            try
            {
                connection.Open();
                SqlCommand command = CreateCommand(text);
                command.ExecuteNonQuery();     
                s = "User has successfully created";
            }
            catch
            {
                s = "User with this Login has already created";
            }
            connection.Close();
            return s;
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {       
            InitializeComponent();
        }

        private void ToAdminWindow_Click(object sender, RoutedEventArgs e)
        {
            AdminLoginWindow alw = new AdminLoginWindow();
            Hide();
            alw.Show();
        }

        private void ToUserWindow_Click(object sender, RoutedEventArgs e)
        {
            UserMainWindow usw = new UserMainWindow();
            Hide();
            usw.Show();
        }

        private void ToInfWindow_Click(object sender, RoutedEventArgs e)
        {
            InfWindow iw = new InfWindow();
            Hide();
            iw.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

    }
    public partial class InfWindow : Window
    {
        public InfWindow()
        {
            InitializeComponent();
        }

        private void ToMainWindow(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            Hide();
            mw.Show();
        }
    }

    #region Admin features
    public partial class AdminLoginWindow : Window
    {
        static string pass;
        static int counter = 1;
        public AdminLoginWindow()
        {
            InitializeComponent();

            string com = "Select Password From INF where Login = 'ADMIN'";
            pass = Tech.GetOneString(com);
        }

        private void Authorize_Click(object sender, RoutedEventArgs e)
        {
            if (Password.Password != pass)
            {             
                Password.Clear();
                MessageBox.Show($"Incorrect password (attempt №{counter})");
                counter++;
                if (counter == 4)
                    System.Windows.Application.Current.Shutdown();
            }
            else
            {
                MainAdmin ma = new MainAdmin();
                Hide();
                ma.Show();
            } 
        }

        private void ToMainWindow(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            Hide();
            mw.Show();
        }
    }
    public partial class MainAdmin : Window
    {
        public MainAdmin()
        {
            InitializeComponent();
        }

        private void ToPasswordWindow_Click(object sender, RoutedEventArgs e)
        {
            AdminPasswordWindow apw = new AdminPasswordWindow();
            Hide();
            apw.Show();
        }

        private void ToControlWindow_Click(object sender, RoutedEventArgs e)
        {
            AdminControlWindow acw = new AdminControlWindow();
            Hide();
            acw.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            Hide();
            mw.Show();
        }
    }
    public partial class AdminControlWindow : Window
    {
        static DataTable dataTable;
        static int index = 0;

        public AdminControlWindow()
        {
            InitializeComponent();
            UpdateTable();
            UpdateUserData();      
        }

        private void UpdateTable()
        {
            dataTable = new DataTable("Користувачі системи:");
            string com = "Select Name as name, Surname as surname, Login as login, Password as password," +
                "PasswordStatus as restriction, ActivityStatus as activity from INF";

            Tech.FillTable(com, dataTable);
            datagrid.ItemsSource = dataTable.DefaultView;
        }

        private void UpdateUserData()
        {
            UserName.Content = NotNull(0);
            UserSurname.Content = NotNull(1);
            UserLogin.Content = NotNull(2);
            UserPasswordRestriction.Content = NotNull(4);          
            UserStatus.Content = NotNull(5);        
            ActivityBox.IsChecked = UserStatus.Content.ToString() == "TRUE";
            PasswordRestrictionBox.IsChecked = UserPasswordRestriction.Content.ToString() == "TRUE";
        }
        private string NotNull(int i1) => 
            string.IsNullOrEmpty(dataTable.Rows[index][i1].ToString()) ? "NO DATA" : dataTable.Rows[index][i1].ToString();


        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (index < dataTable.Rows.Count - 1)
            {
                index++;
                UpdateUserData();
            }
            else
                MessageBox.Show("That's already last user");
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (index > 0)
            {
                index--;
                UpdateUserData();
            }
            else
                MessageBox.Show("That's already first user");
        }

        private void ChangeActivity_Click(object sender, RoutedEventArgs e)
        {
            string com = $"Select ActivityStatus From INF where Login = '{UserLogin.Content}'";
            string stats = Tech.GetOneString(com);

            if (stats != (((bool)ActivityBox.IsChecked) ? "TRUE" : "FALSE"))
            {
                com = "update INF set ActivityStatus = " +
                ((bool)ActivityBox.IsChecked ? "'TRUE'" : "'FALSE'") +
                $"where Login = '{UserLogin.Content}'";
                UpdateWholeData(com);
            }
        }

        private void ChangePasswordRestriction_Click(object sender, RoutedEventArgs e)
        {
            string com = $"Select PasswordStatus From INF where Login = '{UserLogin.Content}'";
            string stats = Tech.GetOneString(com); ;

            if (stats != (((bool)PasswordRestrictionBox.IsChecked) ? "TRUE" : "FALSE"))
            {
                com = "update INF set PasswordStatus = " +
                    ((bool)PasswordRestrictionBox.IsChecked ? "'TRUE'" : "'FALSE'") +
                    $"where Login = '{UserLogin.Content}'";
                UpdateWholeData(com);
            }
        }


        private void UpdateWholeData(string com)
        {
            Tech.UpdateOneString(com);

            UpdateTable();
            UpdateUserData();
        }

        #region WindowButtonsEvents
        private void ToMainAdminWindow_Click(object sender, RoutedEventArgs e)
        {
            MainAdmin ma = new MainAdmin();
            Hide();
            ma.Show();
        }

        private void ToAdminNewUserWindow_Click(object sender, RoutedEventArgs e)
        {
            AdminNewUserWindow anuw = new AdminNewUserWindow();
            Hide();
            anuw.Show();

        }
        #endregion
    }
    public partial class AdminNewUserWindow : Window
    {
        public AdminNewUserWindow()
        {
            InitializeComponent();
            AddNewNull.IsEnabled = false;
        }

        private void Input_Changed(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(Input.Text))
                AddNewNull.IsEnabled = true;
            else
                AddNewNull.IsEnabled = false;
        }

        private void AddNewNull_Click(object sender, RoutedEventArgs e)
        {
            string s = Tech.InsertOneString($"insert into INF (Login) values ('{Input.Text}');");

            AddNewNull.IsEnabled = false;
            Input.Text = null;
            MessageBox.Show(s);
        }

        private void ToAdminContolWindow(object sender, RoutedEventArgs e)
        {
            AdminControlWindow acw = new AdminControlWindow();
            Hide();
            acw.Show();
        }   
    }
    public partial class AdminPasswordWindow : Window
    {
        static int counter = 0;

        public AdminPasswordWindow()
        {
            InitializeComponent();
        }
     
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            string s;
            string com = "select Password from INF where Login ='ADMIN';";
            string userpassword = Tech.GetOneString(com);

            s = CheckPassword(userpassword);

            NewPassword1.Password = null;
            NewPassword2.Password = null;
            OldPassword.Password = null;
            MessageBox.Show(s);
        }
        private string CheckPassword(string userpassword)
        {
            if (userpassword != OldPassword.Password)
            {
                counter++;
                if (counter == 3)
                {
                    MessageBox.Show($"Old password incorrect (attempt №{counter})");
                   System.Windows.Application.Current.Shutdown();
                }
                    
                return $"Old password incorrect (attempt №{counter})";              
            }                   

            if (NewPassword1.Password != NewPassword2.Password)
                return "New passwords are different";


            if (userpassword == NewPassword1.Password)
                return "New password is identical to old one";

            if (!CheckPasswordOnRestriction("ADMIN", NewPassword1.Password))
                return "New password isn't fit restrictions";

            string com = $"update INF set Password = '{NewPassword1.Password}' where Login ='ADMIN';";
            Tech.UpdateOneString(com);
            counter = 0;
            return "New password has successfuly set";
        }

        public bool CheckPasswordOnRestriction(string Login, string Password)
        {
            string com = $"select PasswordStatus from INF where Login = '{Login}';";
            string PassRes = Tech.GetOneString(com);
            return PassRes != "TRUE" || Tech.FitRestriction(Password);
        }

        private void ToMainAdminWindow_Click(object sender, RoutedEventArgs e)
        {
            MainAdmin ma = new MainAdmin();
            Hide();
            ma.Show();
        }
    }
    #endregion

    #region User features
    public partial class UserMainWindow : Window
    {
        public UserMainWindow()
        {
            InitializeComponent();
        }

        private void ToUserAuthorizationWindow_Click(object sender, RoutedEventArgs e)
        {
            UserAuthorizationWindow uaw = new UserAuthorizationWindow();
            Hide();
            uaw.Show();
        }

        private void ToUserNewUserWindow_Click(object sender, RoutedEventArgs e)
        {
            UserNewUserWindow unuw = new UserNewUserWindow();
            Hide();
            unuw.Show();
        }

        private void ToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            Hide();
            mw.Show();
        }
    }
    public partial class UserAuthorizationWindow : Window
    {
        static string UserName, UserSurname, UserPassword, PasswordStatus, ActivityStatus;
        static int counter = 1;
        public UserAuthorizationWindow()
        {
            InitializeComponent();
            VisualChange(Visibility.Collapsed, false, false, false);
        }

        private void VisualChange(Visibility v, bool auth, bool sysout, bool update)
        {
            ChangesGrid.Visibility = v;
            Authorize.IsEnabled = auth;
            SystemOut.IsEnabled = sysout;
            Update.IsEnabled = update;
        }

        private void login_Changed(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(login.Text))
                Authorize.IsEnabled = true;
        }

        private void Authorize_Click(object sender, RoutedEventArgs e)
        {
            if (login.Text == "ADMIN")
            {
                OutputAndClear("Login isn't for users");
                return;
            }

            try
            {
                GetData();
            }
            catch
            {
                OutputAndClear("There is no user with this Login");
                return;
            }
         
            if (ActivityStatus == "FALSE")
            {
                OutputAndClear("This user has been blocked");
                return;
            }

            if (oldpassword.Password != UserPassword)
            {
                OutputAndClear($"Password is incorrect for this Login (attempt №{counter})");
                counter++;
                if (counter == 4)
                    System.Windows.Application.Current.Shutdown();
                return;
            }

            VisualChange(Visibility.Visible, false, true, true);
            login.IsEnabled = false;
            oldpassword.IsEnabled = false;
        }

        private void OutputAndClear(string s)
        {
            login.Text = null;
            oldpassword.Password = null;
            MessageBox.Show(s);
        }

        private void GetData()
        {
            string com = $"select Name, Surname, Password, PasswordStatus, ActivityStatus from INF where Login = '{login.Text}' ;";
            List<string> data = Tech.GetMultipleString(com);
            UserName = data[0];
            UserSurname = data[1];
            UserPassword = data[2];
            PasswordStatus = data[3];
            ActivityStatus = data[4];
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            string command = "";
            if (!string.IsNullOrEmpty(newname.Text))
            {
                if(!CheckT("Name", newname.Text, UserName))
                    return;

                command += $"update INF set Name = '{newname.Text}'where Login = '{login.Text}';";
            }

            if (!string.IsNullOrEmpty(newsurname.Text))
            {
                if (!CheckT("Surname", newsurname.Text, UserSurname))
                    return;

                command += $"update INF set Surname = '{newsurname.Text}'where Login = '{login.Text}';";
            }
            

            if (!(newpassword.Password == "" && newpassword2.Password == ""))
            {
                if (newpassword.Password != newpassword2.Password)
                {
                    OutputAndClear2("New passwords are different");
                    return;
                }

                if (newpassword.Password == UserPassword)
                {
                    OutputAndClear2("New password is identical to the old one");
                    return;
                }

                if (PasswordStatus == "TRUE" && !Tech.FitRestriction(newpassword.Password))
                {
                    OutputAndClear2("New password isn't fit restrictions");
                    return;
                }

                command += $"update INF set Password = '{newpassword.Password}' where Login = '{login.Text}';";
            }

            if (command == "")
            {
                OutputAndClear2("Nothing has been changed, try to enter different data");
                return;
            }

            Tech.UpdateOneString(command);
            OutputAndClear2("Data successfuly changed");
            GetData();
        }

        private void OutputAndClear2(string s)
        {
            newname.Text = null;
            newsurname.Text = null;
            newpassword.Password = null;
            newpassword2.Password = null;
            MessageBox.Show(s);
        }


        private bool CheckT(string type, string newvalue, string oldvalue)
        {
            bool NameCheck = newvalue.All(x => char.IsLetter(x) || x == '-');
            if (!NameCheck)
            {
                OutputAndClear2($"Incorrect input for {type}");
                return false;
            }

            if (newvalue == oldvalue)
            {
                OutputAndClear2($"New {type} is identical to the old one");
                return false;
            }

            return true;
        }

        private void SystemOut_Click(object sender, RoutedEventArgs e)
        {
            UserName = null;
            UserSurname = null;
            UserPassword = null;
            PasswordStatus = null;
            ActivityStatus = null;

            login.IsEnabled = true;
            oldpassword.IsEnabled = true;
            login.Text = null;
            oldpassword.Password = null;
            counter = 1;
            VisualChange(Visibility.Collapsed, false, false, false);
        }

        private void ToUserMainWindow_Click(object sender, RoutedEventArgs e)
        {
            UserMainWindow umw = new UserMainWindow();
            Hide();
            umw.Show();
        }
    }
    public partial class UserNewUserWindow : Window
    {
        public UserNewUserWindow()
        {
            InitializeComponent();
            Authorize.IsEnabled = false;
        }

        private void login_changed(object sender, RoutedEventArgs e)
        {
           if(!string.IsNullOrEmpty(login.Text))
                Authorize.IsEnabled = true;
        }

        private void Authorize_Click(object sender, RoutedEventArgs e)
        {
            if (password.Password != newpassword.Password)
            {
                OutputAndClear("Passwords are different");
                return;
            }

            if (!Tech.FitRestriction(newpassword.Password))
            {
                OutputAndClear("Passwords isn't fit restriction");
                return;
            }

            bool NameCheck = (string.IsNullOrEmpty(name.Text) || name.Text.All(x => char.IsLetter(x) || x == '-'))
                         && (string.IsNullOrEmpty(surname.Text) || surname.Text.All(x => char.IsLetter(x) || x == '-'));
            if (!NameCheck)
            {
                OutputAndClear("Incorrect input for Name or Surname");
                return;
            }

            string com = $"insert into INF (Name, Surname, Login, Password) values ('{name.Text}', '{surname.Text}', '{login.Text}', '{password.Password}')";
            string res = Tech.InsertOneString(com);
            OutputAndClear(res);
        }

        private void OutputAndClear(string s)
        {
            name.Text = null;
            surname.Text = null;
            login.Text = null;
            password.Password = null;
            newpassword.Password = null;
            Authorize.IsEnabled = false;
            MessageBox.Show(s);
        }

        private void ToUserMainWindow_Click(object sender, RoutedEventArgs e)
        {
            UserMainWindow umw = new UserMainWindow();
            Hide();
            umw.Show();
        }   
    }
    #endregion
}
