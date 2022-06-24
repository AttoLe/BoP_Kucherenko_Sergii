using System.Windows.Controls;

namespace coursework.Pages.ChangeDataPages.NavigationPages;

public partial class ChangeDataTeamMainPage : Page
{
    public ChangeDataTeamMainPage(Page p)
    {
        InitializeComponent();

        Subframe.Content = p;
        
        ToChangeExistingTeam.Click += (sender, e) =>
        {
            MainWindow.Mainframe.NavigationService.Navigate(new ChangeDataTeamMainPage(new ChangeTeamDataSubPage2()));
            ToChangeExistingTeam.IsEnabled = false;
        };

        if (p.GetType() == typeof(ChangeTeamDataSubPage1))
        {
            MainWindow.ToPrev.Click += (sender, args) =>
                { MainWindow.Mainframe.Navigate(new ChangeDataMainPage()); };
        }
        else
        {
            ToChangeExistingTeam.IsEnabled = false;
            MainWindow.ToPrev.Click += (sender, args) => 
                { MainWindow.Mainframe.Navigate(new ChangeDataTeamMainPage(new ChangeTeamDataSubPage1())); };
        }
    }
}