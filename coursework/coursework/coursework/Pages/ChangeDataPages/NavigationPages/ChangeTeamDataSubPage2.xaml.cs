using System.Windows.Controls;
using coursework.Pages.TablePages;

namespace coursework.Pages.ChangeDataPages.NavigationPages;

public partial class ChangeTeamDataSubPage2 : Page
{
    public ChangeTeamDataSubPage2()
    {
        InitializeComponent();

        ToChangeRoaster.Click += (sender, args) =>
            { MainWindow.Mainframe.Navigate(new ChooseTeamRosterPage()); };
        
        ToChangeTeamData.Click += (sender, args) =>
            { MainWindow.Mainframe.Navigate(new TeamDataUpdatePage()); };
    }
}