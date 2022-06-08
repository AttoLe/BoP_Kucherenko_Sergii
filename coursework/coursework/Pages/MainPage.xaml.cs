using System.Collections.Generic;
using System.Windows.Controls;
using coursework.Addons;

namespace coursework.Pages;

public partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
 
        ToChangeData.Click += (sender, args) => 
            MainWindow.Mainframe.Navigate(new ChangeDataMainPage(new ChangeDataSubPage1()));
        
        ToPickTeams.Click += (sender, args) => 
            MainWindow.Mainframe.Navigate(new DataGridPage("ChooseTeam"));
    }
}