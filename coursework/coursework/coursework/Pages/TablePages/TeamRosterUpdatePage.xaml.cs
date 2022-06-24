using System;
using System.Collections.Generic;
using System.Windows.Controls;
using coursework.Addons;
using coursework.Pages.ChangeDataPages.InputInteraction;
using static coursework.MainWindow;

namespace coursework.Pages.TablePages;

public partial class TeamRosterUpdatePage : Page
{
    private DataGridInteraction _dgi;
    private int idTeam;
    
    public TeamRosterUpdatePage(int idTeam)
    {
        InitializeComponent();

        this.idTeam = idTeam;
        
        List<string> names = new List<string>
        {
            "№", "player name", "player surname", "player lastname",
            "player role", "player bd", "number", "choice",  "delete"
        };
        string com = $"select * from dbo.get_all_player_by_team ({idTeam});";
        
        _dgi = new DataGridInteraction(com, names);
        
        _dgi.TableType1(Grid, (1,1), (2, 1), CreatePLayerColumn, true );
       
        AddPlayer.IsEnabled = _dgi.Get_RowCount() < 20;
        AddPlayer.Click += (sender, args) => 
            Mainframe.Navigate(new InteractTeamMainPage(idTeam, "CreatePlayerData"));

        ToPrev.Click += (sender, args) => 
            { Mainframe.Navigate(new ChooseTeamRosterPage()); };
    }
    
    private void CreatePLayerColumn()
    {
        _dgi.CreateButtonColumn((sender, eventArgs) => ChangePlayer_Clicked(), "change");
        _dgi.CreateButtonColumn((sender, eventArgs) => DeletePlayer_Clicked(), "delete");
        _dgi.CreateStandardColumns();
    }

    private void DeletePlayer_Clicked()
    {
        SqlInteraction.ExecuteInput("execute dbo.player_delete", new List<object>{_dgi.Get_Clicked_Column<int>(0)});
        SqlInteraction.ExecuteInput($"exec dbo.change_team_activity", new List<object>{idTeam});
        Mainframe.Navigate(new TeamRosterUpdatePage(idTeam));
    }

    private void ChangePlayer_Clicked()
    {
        Mainframe.Navigate(new InteractTeamMainPage(
            _dgi.Get_Clicked_Column<int>(0), 
            _dgi.Get_Clicked_Column<string>(5), idTeam));
    }
       
}