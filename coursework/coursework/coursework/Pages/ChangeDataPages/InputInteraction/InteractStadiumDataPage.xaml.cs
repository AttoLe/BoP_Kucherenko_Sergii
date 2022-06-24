using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using coursework.Addons;
using coursework.Pages.ChangeDataPages.NavigationPages;
using coursework.Pages.TablePages;
using static coursework.MainWindow;

namespace coursework.Pages.ChangeDataPages.InputInteraction;

public partial class InteractStadiumDataPage : Page
{
    private List<(string, object)> _data;

    #region CreateNewStadium
    
    public InteractStadiumDataPage()
    {
        InitializeComponent();
        
        _data = new List<(string, object)>()
        {
            ("stadium_name", tbsn), ("stadium_capacity", tbsc), ("id_stadium_city", tbcn)
        };
        
        submit.Click += (sender, args) => { CreateNewStadium(); }; 
        ToPrev.Click += (sender, args) => { Mainframe.Navigate(new ChangeDataStadiumMainPage()); };
    }

    private void CreateNewStadium()
    {
        try
        {
            var l = _data.Select(x => (object) (x.Item2 as TextBox).Text).ToList();
            SqlInteraction.ExecuteInput("execute dbo.create_stadium", l);
            MessageBox.Show("Info about stadium has been stored");
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        ClearInput();
    }
    
    #endregion
    
    #region UpdateStadiumData
    
    private Predicate<List<string>> _AnyNotNull = data 
        => data.Any(x => !string.IsNullOrWhiteSpace(x));
    
    public InteractStadiumDataPage(int idStadium)
    {
        InitializeComponent();
        
        _data = new List<(string, object)>()
        {
            ("stadium_name", tbsn), ("stadium_capacity", tbsc), ("id_stadium_city", tbcn)
        };
        
        submit.Click += (sender, args) => { UpdateStadiumData(idStadium); }; 
        ToPrev.Click += (sender, args) => { Mainframe.Navigate(new StadiumDataUpdatePage()); };
    }

    private void UpdateStadiumData(int idStadium)
    {
        try
        {
            var b1 = !string.IsNullOrWhiteSpace(tbcn.Text);
            if(b1)
                SqlInteraction.ExecuteInput("execute dbo.set_city_to_stadium", new List<object>{tbcn.Text, idStadium});

            var b2 = _AnyNotNull(_data.Where(x => x.Item1 != "id_stadium_city").
                Select(x => (x.Item2 as TextBox).Text).ToList());
            if(b2)
                SqlInteraction.UpdateExecute("stadium", $"where id_stadium = '{idStadium}'", 
                    _data.Where(x => x.Item1 != "id_stadium_city" 
                                     && !string.IsNullOrWhiteSpace((x.Item2 as TextBox).Text)).
                        Select(x => (x.Item1, (object)(x.Item2 as TextBox).Text)).ToList());
            if (!b1 && !b2)
                MessageBox.Show("Enter data");
            else
            {
                MessageBox.Show("Data has been updated");
                Mainframe.Navigate(new StadiumDataUpdatePage());
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        ClearInput();
    }
    
    #endregion
    
    private void ClearInput() => 
        tbsn.Text = tbsc.Text = tbcn.Text = "";
}