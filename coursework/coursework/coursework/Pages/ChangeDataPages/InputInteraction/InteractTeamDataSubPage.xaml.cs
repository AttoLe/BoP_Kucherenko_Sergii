using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using coursework.Addons;

namespace coursework.Pages.ChangeDataPages.InputInteraction;

public partial class InteractTeamDataSubPage : Page
{
    private List<(string, object)> _data;
    
    public InteractTeamDataSubPage()
    {
        InitializeComponent();
        
        _data = new List<(string, object)> 
        {
            ("team_name", tbname), ("city", tbcity), ("team_last_res", tbres), 
            ("coach_name", tbcname), ("coach_surname", tbcsurname), ("coach_lastname", tbclastname)
        };
    }
    
    #region CreateNewTeam
    
    public void SetTeamData() =>
    SqlInteraction.ExecuteInput($"exec dbo.create_team", 
                new List<object>(_data.Select(x => (x.Item2 as TextBox).Text)));

    public int FindIdTeam() => 
        SqlInteraction.GetOneObject<int>($"select dbo.find_id_team",  new List<object> {tbname.Text} );
    
    #endregion

    #region UpdateTeamData
    
    private Predicate<List<string>> _AnyNotNull = data 
        => data.Any(x => !string.IsNullOrWhiteSpace(x));
    
    public bool UpdateData(int id_team)
    {
        var b1 = _AnyNotNull(_data.Where(x => x.Item1.Contains("coach"))
            .Select(x => (x.Item2 as TextBox).Text).ToList());
        if (b1)
        {
            var id_coach = SqlInteraction.GetOneObject<int>("select dbo.find_id_coach_from_team", new List<object>{id_team});
            var l = _data.Where(x => x.Item1.Contains("coach") && !string.IsNullOrWhiteSpace((x.Item2 as TextBox).Text))
                .Select(y => (y.Item1, (y.Item2 as TextBox).Text as object)).ToList();

            l = l.Select(x => (x.Item1,
                (object) SqlInteraction.GetOneObject<string>("select dbo.change_string", new List<object> {x.Item2}))).ToList();

            SqlInteraction.UpdateExecute("coach", $"where id_coach={id_coach}", l);
        }

        var b2 = !string.IsNullOrWhiteSpace(tbcity.Text);
        if (b2)
            SqlInteraction.ExecuteInput($"execute dbo.set_city_to_team",new List<object>{tbcity.Text, id_team});
        
        var b3 = _AnyNotNull(_data.Where(x => x.Item1.Contains("team"))
            .Select(x => (x.Item2 as TextBox).Text).ToList());
        if (b3)
        {
            SqlInteraction.UpdateExecute("team", $"where id_team={id_team}",
                _data.Where(x => x.Item1.Contains("team") && !string.IsNullOrWhiteSpace((x.Item2 as TextBox).Text))
                    .Select(y => (y.Item1, (object)(y.Item2 as TextBox).Text)).ToList());
        }
       
        return b1 || b2 || b3;
    }
    #endregion
    
    public void ClearInput() =>
        tbname.Text = tbcity.Text = tbres.Text = tbcname.Text
            = tbcsurname.Text = tbclastname.Text = "";
}      