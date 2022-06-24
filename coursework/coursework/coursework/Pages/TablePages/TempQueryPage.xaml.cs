using System.Collections.Generic;
using System.Windows.Controls;
using coursework.Addons;
using static coursework.MainWindow;

namespace coursework.Pages.TablePages;

public partial class TempQueryPage : Page
{
    public TempQueryPage(int idTournament, string date)
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
            "player role", "player bd"
        };
        var com1 = $"select * from dbo.get_youngest_player ({idTournament});";
        CreateDataGrid(names1, com1, (1, 1));
        
        var names2 = new List<string>
        {
            "team name", "player name", "player surname", "player lastname",
            "player role", "player bd", "scored"
        };
        var com2 = $"select * from dbo.get_most_scored_player ({idTournament});";
        CreateDataGrid(names2, com2, (1, 3));
        
        var names3 = new List<string>
        {
            "team name", "team city", "coach name", "coach surname", "coach lastname",
            "scored & conceded score", "scored-conceded"
        };
        var com3 = $"select distinct * from dbo.get_scored_and_conceded ({idTournament}, '{date}') order by score desc;";
        CreateDataGrid(names3, com3, (1, 5));
        
        ToPrev.Click += (sender, args) =>
            { Mainframe.Navigate(OrganizeChamp.instance); };
    }
}