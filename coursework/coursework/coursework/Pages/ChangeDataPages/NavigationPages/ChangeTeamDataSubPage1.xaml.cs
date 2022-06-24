using System.Windows.Controls;
using coursework.Pages.ChangeDataPages.InputInteraction;

namespace coursework.Pages;

public partial class ChangeTeamDataSubPage1 : Page
{
    public ChangeTeamDataSubPage1()
    {
        InitializeComponent();

        ToCreateNewTeam.Click += (sender, args) =>
            MainWindow.Mainframe.Navigate(new InteractTeamMainPage());
    }
}