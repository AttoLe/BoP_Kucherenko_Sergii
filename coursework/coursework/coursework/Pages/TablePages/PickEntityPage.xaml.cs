using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using coursework.Addons;
using static coursework.MainWindow;

namespace coursework.Pages.TablePages;

public partial class PickEntityPage : Page
{
    private List<int> _idEntity = new ();
    private string _entity, _com;
    private List<string> _columns;
    private DataGridInteraction _dgi;

    public PickEntityPage()
    {
        InitializeComponent();
        
        _entity = "team";
        _com = "select * from dbo.get_all_team_not_deleted;";
        _columns = new List<string>
        {
            "№", "team name", "city name", "last results", "players",
        };

        LoadData(() => Mainframe.Navigate(new MainPage()),
            () => Mainframe.Navigate(new PickEntityPage(_idEntity)));
    }

    public PickEntityPage(List<int> teams)
    {
        InitializeComponent();
        
        _entity = "stadium";
        _com = "select * from dbo.get_all_stadium;";
        _columns = new List<string>
        {
            "№", "stadium name", "stadium city", "stadium capacity"
        };
        LoadData(() => Mainframe.Navigate(new PickEntityPage()),
            () =>  Mainframe.Navigate(new OrganizeChamp(teams, _idEntity)));
    }

    private void LoadData(Action exit, Action next)
    {
        info.Text = $"0 {_entity}s has been chose.\nYou need {amount.Text} more";
        data.Text = $"Choose number\n of {_entity}s";
        start.Click += (sender, args) => { next(); };
        start.IsEnabled = false;

        _dgi = new DataGridInteraction(_com, _columns);
        _dgi.TableType1(Grid,(1, 1), (5, 1),  () => CreateColumns("CB_Click"), false);
        
        DataColumn check = new DataColumn("choice", typeof(bool));
        check.DefaultValue = false;
        _dgi.dt.Columns.Add(check);

        amount.DropDownClosed += (sender, args) => 
            {  Amount_Changed(_idEntity, _entity); };

        ToPrev.Click += (sender, args) => exit();
    }

    private void Amount_Changed(List<int> entities, string entity)
    {
        entities.Clear();
        
        info.Text = $"{entities.Count} {entity}s has been chosen.\n" +
                    $"You need {Convert.ToInt32(amount.Text) - entities.Count} more";
        start.IsEnabled = false;
        
        foreach (DataRow v in _dgi.dt.Rows)
            v[_dgi.dg.Columns.Count - 1] = false;
    }

    private void CreateColumns(string resource)
    {
        _dgi.CreateStandardColumns();

        for (int i = 0; i < _dgi.dg.Columns.Count; i++)
            _dgi.dg.Columns[i].IsReadOnly = i != _dgi.dg.Columns.Count - 1;

        (_dgi.dg.Columns[_dgi.dg.Columns.Count - 1] as DataGridCheckBoxColumn).ElementStyle = (Style) FindResource(resource);
    }

    private void CB_Clicked(object sender, RoutedEventArgs e)
    {
        var cb = (CheckBox) sender;
        var v = (DataRowView) _dgi.dg.CurrentCell.Item;
        var max = Convert.ToInt32(amount.Text);

        if ((bool) cb.IsChecked)
        {
            if (_idEntity.Count < max)
                _idEntity.Add((int) v.Row[0]);
            else
            {
                cb.IsChecked = false;
                MessageBox.Show($"It has been already chosen {max} {_entity}.\n" +
                                $"You are not allowed to choose more {_entity}");
                return;
            }
        }
        else
        {
            try
            {
                _idEntity.Remove((int) v.Row[0]);
                start.IsEnabled = false;
            }
            catch
            {
                return;
            }
        }

        info.Text = $"{_idEntity.Count} teams has been chosen.\nYou need {max - _idEntity.Count} more";
        if (_idEntity.Count == max)
            start.IsEnabled = true;
    }
}