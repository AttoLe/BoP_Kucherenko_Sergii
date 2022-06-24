using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using coursework.Addons;
using static coursework.MainWindow;
using static coursework.Addons.SqlInteraction;
namespace coursework.Pages.TablePages;

public partial class ChangeMeetRoasterPage : Page
{
    private DataGridInteraction _dgi;
    private List<(int, string)> _comboData;
    private int _idMeet, _idTeam;
    private (((int, int) locate, (int, int) span) past, ((int, int) locate, (int, int) span) future) _datagrids;
    
    public ChangeMeetRoasterPage(int idMeet, int idTeam,
        ((int, int) locate, (int, int) span) past, ((int, int) locate, (int, int) span) future)
    {
        InitializeComponent();

        _idMeet = idMeet;
        _idTeam = idTeam;
        _datagrids = (past, future);
        
        List<string> names = new List<string>
        {
            "№", "player name", "player surname", "player lastname",
            "player role", "player bd", "number", "delete"
        };
        string com = $"select * from dbo.get_player_by_team_by_meet ({idMeet}, {idTeam});";

        _dgi = new DataGridInteraction(com, names);
        _dgi.TableType1(Grid, (1,1), (5, 1), () => CreatePLayerColumn(past, future), true );

        var dataTable = new DataTable();
        FillTable($"select * from dbo.get_player_by_meet_not_by_team ({idMeet}, {idTeam})", dataTable); 
        _comboData = dataTable.Select().Select(x => ((int) x.ItemArray[0] , x.ItemArray[1] as string)).ToList();
        _comboData.ForEach(x => Players.Items.Add(x.Item2));
        
        AddPlayer.IsEnabled = false;
        AddPlayer.Click += (sender, args) => AddPlayer_Click(past, future);

        CheckIfOutPossible();

        ToPrev.Click += ToPrev_Click;

    }

    private void CreatePLayerColumn(((int, int) locate, (int, int) span) past, ((int, int) locate, (int, int) span) future)
    {
        _dgi.CreateButtonColumn((sender, eventArgs) => DeletePlayer_Clicked(past, future), "delete");
        _dgi.CreateStandardColumns();
    }

    private void DeletePlayer_Clicked(((int, int) locate, (int, int) span) past, ((int, int) locate, (int, int) span) future)
    {
        AddPlayer.IsEnabled = true;
        ToPrev.IsEnabled = false;
        
        ExecuteInput("execute dbo.delete_player_by_meet", 
            new List<object>{  _idMeet, _dgi.Get_Clicked_Column<int>(0)});
        
        Mainframe.Navigate(new ChangeMeetRoasterPage(_idMeet, _idTeam, past, future));
    }

    private void CheckIfOutPossible()
    {
        if (_dgi.Get_RowCount() != 11)
        {
            ToPrev.IsEnabled = false;
            AddPlayer.IsEnabled = true;
            return;
        }

        var g = GetOneObject<int>("select dbo.get_player_role_by_meet",
            new List<object> {_idMeet, _idTeam, "goalkeeper"});
        var d = GetOneObject<int>("select dbo.get_player_role_by_meet",
            new List<object> {_idMeet, _idTeam, "goalkeeper"});
        var m = GetOneObject<int>("select dbo.get_player_role_by_meet",
            new List<object> {_idMeet, _idTeam, "goalkeeper"}); 
        var f = GetOneObject<int>("select dbo.get_player_role_by_meet",
            new List<object> {_idMeet, _idTeam, "goalkeeper"});

        if (g != 1 || d <= 0 || m <= 0 || f <= 0)
        {
            ToPrev.IsEnabled = false;
            AddPlayer.IsEnabled = true;
            return;
        }
        
        ToPrev.IsEnabled = true;
        AddPlayer.IsEnabled = false;
    }
    
    private void AddPlayer_Click(((int, int) locate, (int, int) span) past, ((int, int) locate, (int, int) span) future)
    {
        if (Players.SelectedIndex == -1)
        {
            MessageBox.Show("choose something");
            return;
        }
        
        ExecuteInput("execute dbo.set_meet_member", 
            new List<object>{_idMeet, _comboData[Players.SelectedIndex].Item1});

        Mainframe.Navigate(new ChangeMeetRoasterPage(_idMeet, _idTeam, past, future));
    }
    
    private void ToPrev_Click(object sender, RoutedEventArgs args)
    {
        ToPrev.Click -= ToPrev_Click;

        if (_datagrids.past.locate != (0, 0))
            OrganizeChamp.instance.LoadPastDataGrid(_datagrids.past.locate, _datagrids.past.span);
        OrganizeChamp.instance.LoadFutureDataGrid(_datagrids.future.locate, _datagrids.future.span);
        Mainframe.Navigate(OrganizeChamp.instance);
    }
}