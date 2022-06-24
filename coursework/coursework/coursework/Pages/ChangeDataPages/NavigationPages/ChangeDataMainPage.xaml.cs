using System.Windows.Controls;
using static coursework.MainWindow;
namespace coursework.Pages.ChangeDataPages.NavigationPages;

public partial class ChangeDataMainPage : Page
{
    public ChangeDataMainPage()
    {
        InitializeComponent();

        ToChangeDataTeamMain.Click += (sender, args) => 
            { Mainframe.Navigate(new ChangeDataTeamMainPage(new ChangeTeamDataSubPage1())); };
        
        ToChangeDataStadiumMain.Click += (sender, args) => 
            { Mainframe.Navigate(new ChangeDataStadiumMainPage()); };

        ToPrev.Click += (sender, args) => { Mainframe.Navigate(new MainPage()); };
    }
}