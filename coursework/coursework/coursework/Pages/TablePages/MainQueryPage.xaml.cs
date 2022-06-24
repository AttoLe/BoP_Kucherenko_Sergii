using System.Collections.Generic;
using System.Windows.Controls;
using coursework.Addons;
using static coursework.MainWindow;

namespace coursework.Pages.TablePages;

public partial class MainQueryPage : Page
{
    public MainQueryPage(int idTournament)
    {
        InitializeComponent();
        
        var names = new List<string>
        {
            "place", "team name", "team city", "coach name", "coach surname", "coach lastname",
            "scored & conceded score"
        };
        var com = $"select * from dbo.get_all_team_data ({idTournament}) order by 1 asc;";
        
        var dgi = new DataGridInteraction(com, names);
        dgi.TableType1(Grid, (1, 1), (1,1),  () => dgi.CreateStandardColumns(), true);
        
        ToPrev.Click += (sender, args) =>
            { Mainframe.Navigate(OrganizeChamp.instance); };
    }
}