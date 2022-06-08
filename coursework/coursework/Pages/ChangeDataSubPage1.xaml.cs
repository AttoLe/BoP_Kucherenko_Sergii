using System.Windows.Controls;

namespace coursework.Pages;

public partial class ChangeDataSubPage1 : Page
{
    public ChangeDataSubPage1()
    {
        InitializeComponent();
        
        ToCreateNewTeam.Click += (sender, args) =>
            MainWindow.Mainframe.Navigate(new InteractTeamMainPage());
    }
}