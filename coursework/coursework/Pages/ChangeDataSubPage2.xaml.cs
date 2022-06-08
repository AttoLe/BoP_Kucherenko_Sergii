using System.Collections.Generic;
using System.Windows.Controls;
using coursework.Addons;

namespace coursework.Pages;

public partial class ChangeDataSubPage2 : Page
{
    public ChangeDataSubPage2()
    {
        InitializeComponent();

        ToChangeRoaster.Click += (sender, args) =>
            { MainWindow.Mainframe.Navigate(new DataGridPage("UpdateTeamRoster")); };
        
        ToChangeTeamData.Click += (sender, args) =>
            { MainWindow.Mainframe.Navigate(new DataGridPage("UpdateTeamData")); };
    }
}