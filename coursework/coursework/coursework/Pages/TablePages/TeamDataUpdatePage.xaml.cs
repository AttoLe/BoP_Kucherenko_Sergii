using System;
using System.Collections.Generic;
using System.Windows.Controls;
using coursework.Addons;
using coursework.Pages.ChangeDataPages.InputInteraction;
using coursework.Pages.ChangeDataPages.NavigationPages;
using static coursework.MainWindow;

namespace coursework.Pages.TablePages;

public partial class TeamDataUpdatePage : Page
{
    private DataGridInteraction _dgi;
    
    public TeamDataUpdatePage()
    {
        InitializeComponent();
        
        List<string> names = new List<string>
        {
            "№", "team name", "coach name", "coach surname", "coach lastname", 
            "city name", "last results", "choice", "delete"
        };
        string com = "select * from dbo.get_all_team_detailed;";
        
        _dgi = new DataGridInteraction(com, names);
        
        _dgi.TableType1(Grid, (1,1), (1, 1), CreateTeamDataColumns, true );

        ToPrev.Click += (sender, args) => 
            { Mainframe.Navigate(new ChangeDataTeamMainPage(new ChangeTeamDataSubPage2())); };
    }
    
    private void CreateTeamDataColumns()
    {
        _dgi.CreateButtonColumn((sender, args) => { ChangeTeamData_Clicked(); }, "change");
        _dgi.CreateButtonColumn((sender, args) => { DeleteTeamData_Clicked(); }, "delete");
        _dgi.CreateStandardColumns();
    }
    
    private void DeleteTeamData_Clicked()
    {
        SqlInteraction.ExecuteInput("execute dbo.team_delete", new List<object> {_dgi.Get_Clicked_Column<int>(0)});
        Mainframe.Navigate(new TeamDataUpdatePage());
    }
    
    private void ChangeTeamData_Clicked() => 
        Mainframe.Navigate(new InteractTeamMainPage(
            Convert.ToInt32(_dgi.Get_Clicked_Column<int>(0)), "UpdateTeamData"));
}