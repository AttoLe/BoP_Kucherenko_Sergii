using System;
using System.Collections.Generic;
using System.Windows.Controls;
using coursework.Addons;
using coursework.Pages.ChangeDataPages.InputInteraction;
using coursework.Pages.ChangeDataPages.NavigationPages;
using static coursework.MainWindow;

namespace coursework.Pages.TablePages;

public partial class DetailedPast : Page
{
    public DetailedPast(int idMeet)
    {
        InitializeComponent();
        
        void CreateDataGrid(List<string> names, string com, (int, int) loc)
        {
            var dgi = new DataGridInteraction(com, names);
            dgi.TableType1(Grid,  loc, (1,1),  () => dgi.CreateStandardColumns(), true);
        }
        
        
        var names1 = new List<string>
        {
            "team name", "player name", "player surname", "player lastname",
            "player role", "player bd", "number"
        };
        var com1 = $"select * from dbo.get_player_by_meet ({idMeet});";
        CreateDataGrid(names1, com1, (1, 1));
        
        var names2 = new List<string>
        {
            "team name", "player name", "player surname", "player lastname",
            "player role", "number", "goal time"
        };
        var com2 = $"select * from dbo.get_goal_by_player_by_team ({idMeet});";
        CreateDataGrid(names2, com2, (1, 3));

        ToPrev.Click += (sender, args) => 
            Mainframe.Navigate(OrganizeChamp.instance); 
    }
}