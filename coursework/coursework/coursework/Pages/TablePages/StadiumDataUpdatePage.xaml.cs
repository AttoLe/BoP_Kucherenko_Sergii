using System;
using System.Collections.Generic;
using System.Windows.Controls;
using coursework.Addons;
using coursework.Pages.ChangeDataPages.InputInteraction;
using coursework.Pages.ChangeDataPages.NavigationPages;
using static coursework.MainWindow;
namespace coursework.Pages.TablePages;

public partial class StadiumDataUpdatePage : Page
{
    private DataGridInteraction _dgi;
    
    public StadiumDataUpdatePage()
    {
        InitializeComponent();

        List<string> names = new List<string>
        {
            "№", "stadium name", "stadium city", "stadium capacity",  "choice", "delete"
        };
        string com = "select * from dbo.get_all_stadium;";

        
        _dgi = new DataGridInteraction(com, names);
        
        _dgi.TableType1(Grid, (1, 1), (1,1), CreateStadiumColumns, true);
        ToPrev.Click += (sender, args) =>
            { Mainframe.Navigate(new ChangeDataStadiumMainPage()); };
    }

    private void CreateStadiumColumns()
    {
        _dgi.CreateButtonColumn((sender, args) =>
        { Mainframe.Navigate(new InteractStadiumDataPage(Convert.ToInt32(_dgi.Get_Clicked_Column<int>(0)))); },
            "choose");
        
        _dgi.CreateButtonColumn((sender, args) =>
            { DeleteStadium(); }, "delete");
        
        _dgi.CreateStandardColumns();
    }

    private void DeleteStadium()
    {
        SqlInteraction.ExecuteInput("execute dbo.stadium_delete", new List<object>(){ _dgi.Get_Clicked_Column<int>(0) });
        Mainframe.Navigate(new StadiumDataUpdatePage());
    }
}