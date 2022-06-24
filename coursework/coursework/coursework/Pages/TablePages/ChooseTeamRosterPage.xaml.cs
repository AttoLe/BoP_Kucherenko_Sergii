using System;
using System.Collections.Generic;
using System.Windows.Controls;
using coursework.Addons;
using coursework.Pages.ChangeDataPages.NavigationPages;
using static coursework.MainWindow;

namespace coursework.Pages.TablePages;

public partial class ChooseTeamRosterPage : Page
{
    private DataGridInteraction _dgi;
    
    public ChooseTeamRosterPage()
    {
        InitializeComponent();
        
        List<string> names = new List<string>
        {
            "№", "team name", "city name", "last results", "amount of players",
            "playability", "choice"
        };
        string com = "select * from dbo.get_all_team_to_roaster;";

        _dgi = new DataGridInteraction(com, names);
        
        _dgi.TableType1(Grid, (1,1), (1, 1), CreateTeamRoasterColumns, true );
        
        ToPrev.Click += (sender, args) =>
            { Mainframe.Navigate(new ChangeDataTeamMainPage(new ChangeTeamDataSubPage2())); };
    }
    
    private void CreateTeamRoasterColumns()
    {
        _dgi.CreateButtonColumn((sender, args) => { ChangeTeamRoaster_Clicked(); }, "choose");
        _dgi.CreateStandardColumns();
    }

    private void ChangeTeamRoaster_Clicked() =>
        Mainframe.Navigate(new TeamRosterUpdatePage(Convert.ToInt32(_dgi.Get_Clicked_Column<int>(0))));
}