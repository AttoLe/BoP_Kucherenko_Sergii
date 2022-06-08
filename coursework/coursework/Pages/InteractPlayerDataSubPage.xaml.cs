using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using coursework.Addons;

namespace coursework.Pages;

public partial class InteractPlayerDataSubPage : Page
{
    private List<(string,  object)> _data;
    private static DateTime _date;

    public InteractPlayerDataSubPage(DateTime dat)
    {
        InitializeComponent();

        _date = dat;
        dt.SelectedDate = _date;
        
        _data = new List<(string, object)>()
        {
            ("player_name", tbn), ("player_surname", tbs), ("player_lastname", tbl), 
            ("player_number", tbnum), ("player_role", cbrole), ("player_bd", dt)
        };
    }
    
    private object Transform(object s)
    {
        return s switch
        {
            TextBox tb => tb.Text,
            DatePicker dp => dp.SelectedDate,
            _ => (s as ComboBox).Text
        };
    }

    #region CreateNewPlayer
    
    private bool _full = true;
    
    public void SetPlayerData(int idTeam)
    {
        var values = _data.Select(x =>  Transform(x.Item2)).ToList();
        values.Add(idTeam);
        SqlInteraction.ExecuteInput($"exec dbo.create_player", values);
    }
    
    public string GetTeamRoleData(int idTeam)
    {
        var gNum = SqlInteraction.GetOneObject<int>($"select dbo.get_team_role_num", new List<object>{idTeam, "goalkeeper"});
        var dNum = SqlInteraction.GetOneObject<int>($"select dbo.get_team_role_num", new List<object>{idTeam, "defender"});
        var mNum = SqlInteraction.GetOneObject<int>($"select dbo.get_team_role_num", new List<object>{idTeam, "midfielder"});
        var fNum = SqlInteraction.GetOneObject<int>($"select dbo.get_team_role_num",  new List<object>{idTeam, "forward"});

        if (_full && gNum > 0 && dNum > 0 && mNum > 0 && fNum > 0 && gNum + dNum + mNum + fNum > 10)
        {
            SqlInteraction.ExecuteInput($"exec dbo.change_team_activity", new List<object>{idTeam});
            MessageBox.Show("Now this team can take part in tournament (11 players)");
            _full = false;
        }
        return 
            $"Team already has {gNum}G {dNum}D {mNum}M {fNum}F. It is obligated to have at least:" +
            $" \n1G, 1D, 1M, 1F and 11-20 players at all";
    }
    #endregion
    
    public void UpdatePlayerData(int idPlayer)
    {
        var d = _data.Select(x => (x.Item1, Transform(x.Item2)))
            .Where(x => x.Item2 is not string || !string.IsNullOrWhiteSpace(x.Item2.ToString()))
            .Select(x => x.Item1.Contains("name") 
                ? (x.Item1, SqlInteraction.GetOneObject<string>("select dbo.change_string", new List<object>{x.Item2}))
                : x).ToList();

        SqlInteraction.UpdateExecute("player", $"where id_player={idPlayer}", d);
    }

    public void ClearInput()
    {
        cbrole.Text = tbn.Text = tbs.Text = tbl.Text = tbnum.Text = "";
        dt.SelectedDate = _date;
    }
}