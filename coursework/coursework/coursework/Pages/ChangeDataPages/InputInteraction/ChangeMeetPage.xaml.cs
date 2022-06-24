using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using coursework.Pages.TablePages;
using static coursework.MainWindow;
using static coursework.Addons.SqlInteraction;

namespace coursework.Pages.ChangeDataPages.InputInteraction;

public partial class ChangeMeetPage : Page
{
    private DateTime _dt;
    private readonly DateTime _finish;
    private readonly DateTime _that;
    private readonly int _idMeet;
    private int _newPrice;
    private (((int, int) locate, (int, int) span) past, ((int, int) locate, (int, int) span) future) _datagrids;

    public ChangeMeetPage(int idMeet, string team1, string team2, DateTime dt, DateTime finish, DateTime that, 
        ((int, int) locate, (int, int) span) past, ((int, int) locate, (int, int) span) future)
    {
        InitializeComponent();

        _dt = dt;
        _idMeet = idMeet;
        _finish = finish;
        _that = that;
        _datagrids = (past, future);
        
        newdate.SelectedDate = _that;
        
        dateinfo.Text = "Enter new date of match";
        priceinfo.Text = "Enter new price of ticket\n(only numbers > 0)";
        
        UpdatePrice.Click += (sender, args) => ChangePrice(past, future);
        UpdateDate.Click += (sender, args) => ChangeDate(past, future);
        
        var idTeam1 = GetOneObject<int>("select dbo.find_id_team", new List<object> {team1});
        var idTeam2 = GetOneObject<int>("select dbo.find_id_team", new List<object> {team2});

        ToChangeTeam1.Content = $"To change roaster of\n\"{team1}\" for that meet";
        ToChangeTeam1.Click += (sender, args) =>
        {
            ToPrev.Click -= ToPrev_Click;
            Mainframe.Navigate(new ChangeMeetRoasterPage(idMeet, idTeam1, past, future));
        };
        
        ToChangeTeam2.Content = $"To change roaster of\n\"{team2}\" for that meet";
        ToChangeTeam2.Click += (sender, args) =>
        {
            ToPrev.Click -= ToPrev_Click;
            Mainframe.Navigate(new ChangeMeetRoasterPage(idMeet, idTeam2, past, future));
        };

        ToPrev.Click += ToPrev_Click;
    }

    private void ChangePrice(((int, int) locate, (int, int) span) past, ((int, int) locate, (int, int) span) future)
    {
        var price = newprice.Text;
        newprice.Text = default;
        
        if (price.Any(x => !char.IsNumber(x)))
        {
            MessageBox.Show("Incorrect price");
            
            return;
        }
        ExecuteInput("execute dbo.set_price", new List<object>{_idMeet, price});
    }

    private void ChangeDate(((int, int) locate, (int, int) span) past, ((int, int) locate, (int, int) span) future)
    {
        var b = DateTime.TryParse(newdate.SelectedDate.ToString(), out DateTime time);
        
        if(b && _that == time)
            return;
        
        if (!b || time <= _dt || time > _finish)
        {
            MessageBox.Show("Incorrect date");
            newdate.SelectedDate = _dt;
            return;
        }
        _dt = time;
        ExecuteInput("execute dbo.set_meet_date", new List<object>{_idMeet, _dt});
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