using System.Collections.Generic;
using System.Windows.Controls;
using coursework.Pages.ChangeDataPages.NavigationPages;
using coursework.Pages.TablePages;

namespace coursework.Pages;

public partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
 
        ToChangeData.Click += (sender, args) => 
            MainWindow.Mainframe.Navigate(new ChangeDataMainPage());
        
        ToPickTeams.Click += (sender, args) => 
            MainWindow.Mainframe.Navigate(new PickEntityPage());
    }
}