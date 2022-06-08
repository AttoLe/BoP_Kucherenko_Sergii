using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using coursework.Addons;
using static coursework.MainWindow;

namespace coursework.Pages;

public partial class InteractTeamMainPage : Page
{
    private InteractTeamDataSubPage _sp1;
    private InteractPlayerDataSubPage _sp2;

    #region CreateNewTeam
    public InteractTeamMainPage()
    {
        InitializeComponent();

        int counter = 0, idTeam = -1;
        TextBlock info = new TextBlock();
        MainLabel.Content = "CreateNewTeamWindow";
        submit.Click += (sender, args) => { Create_Team_Click(info, counter, ref idTeam); };
        
        _sp1 = new InteractTeamDataSubPage();
        subframe.NavigationService.Navigate(_sp1);
        
        info = new TextBlock();
        grid.LocateOn(info, (2, 3));
        Grid.SetColumnSpan(info, 2);

        ToPrev.Click += ToChangeDataMainPage_Click;
    }

    private void Create_Team_Click(TextBlock info, int counter, ref int idTeam)
    {
        if (subframe.Content.GetType() == typeof(InteractTeamDataSubPage))
        {
            try
            {
                _sp1.SetTeamData();
                
                idTeam = _sp1.FindIdTeam();
                MessageBox.Show("Info about team has been stored \nNow enter info about its players");

                _sp2 = new InteractPlayerDataSubPage(DateTime.Now);
                subframe.NavigationService.Navigate(_sp2);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                _sp1.ClearInput();
            }
        }
        else
        {
            if (counter > 20)
            {
                MessageBox.Show("Team has maximum amount of players (20)");
                Mainframe.Navigate(new ChangeDataMainPage(new ChangeDataSubPage1()));
            }

            try
            {
                _sp2.SetPlayerData(idTeam);
                info.Text = _sp2.GetTeamRoleData(idTeam);
                MessageBox.Show("Info about player has been stored");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            _sp2.ClearInput();
        }
    }
    
    private void ToChangeDataMainPage_Click(object sender, RoutedEventArgs e)
    {
        Mainframe.Navigate(new ChangeDataMainPage(new ChangeDataSubPage1()));
        ToPrev.Click -= ToChangeDataMainPage_Click;
    }
    #endregion
    
    #region UpdateRoaster
    public InteractTeamMainPage(int number, string type)
    {
        InitializeComponent();

        switch (type)
        {
            case "UpdateTeamData":
                UpdateTeamData(number);
                break;
            
            case "CreatePlayerData":
                CreatePlayerData(number);
                break;
        }
    }
    
    #region UpdateTeamData
    
    private void UpdateTeamData(int id_team)
    {
        MainLabel.Content = "ChangeNewTeamWindow";
        submit.Click += (sender, args) => { UpdateTeam_Submit_Click(id_team); };
        
        _sp1 = new InteractTeamDataSubPage();
        subframe.NavigationService.Navigate(_sp1);
        ToPrev.Click += ToTableTeam_Click;
    }

    private void UpdateTeam_Submit_Click(int id_team)
    {
        try
        {
            if(!_sp1.UpdateData(id_team))
                MessageBox.Show("Enter data");
            else
            {
                MessageBox.Show("Data updated");
                Mainframe.Navigate(new DataGridPage("UpdateTeamData"));
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        _sp1.ClearInput();
    }
    
    private void ToTableTeam_Click(object sender, RoutedEventArgs args)
    {
        Mainframe.Navigate(new DataGridPage("UpdateTeamData"));
        ToPrev.Click -= ToTableTeam_Click;
    }
    #endregion
    
    #region UpdatePlayerData
    
    public InteractTeamMainPage(int idPlayer, string dat, int idTeam)
    {
        InitializeComponent();
        
        DateTime.TryParse(dat, out DateTime date);
        MainLabel.Content = "ChangeNewTeamWindow";
        submit.Click += (sender, args) => { UpdatePlayer_Submit_Click(idPlayer, idTeam); };

        _sp2 = new InteractPlayerDataSubPage(date);
        subframe.NavigationService.Navigate(_sp2);
        
        ToPrev.Click += (sender, args) => { ToTableRoasterTeam_Click(idTeam); };
    }

    private void UpdatePlayer_Submit_Click(int idPlayer, int idTeam)
    {
        try
        {
            _sp2.UpdatePlayerData(idPlayer);
            Mainframe.Navigate(new DataGridPage(idTeam.ToString()));
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        _sp2.ClearInput();
    }
    
    #endregion
    
    #region CreatePlayerData
    private void CreatePlayerData(int idTeam)
    {
        MainLabel.Content = "CreatePlayerWindow";
        
        TextBlock info = new TextBlock();
        grid.LocateOn(info, (2, 3));
        Grid.SetColumnSpan(info, 2);
        
        submit.Click += (sender, args) => { CreatePlayer(idTeam, info); };

        _sp2 = new InteractPlayerDataSubPage(DateTime.Now);
        subframe.NavigationService.Navigate(_sp2);
        ToPrev.Click += (sender, args) => {ToTableRoasterTeam_Click(idTeam); };
    }

    private void CreatePlayer(int idTeam, TextBlock info)
    {
        var counter = SqlInteraction.GetOneObject<int>("select count(*) from get_all_player_by_team", new List<object>{idTeam});
        
        if (counter > 20)
        {
            MessageBox.Show("Team has maximum amount of players (20)");
            Mainframe.Navigate(new DataGridPage(idTeam.ToString()));
        }

        try
        {
            _sp2.SetPlayerData(idTeam);
            info.Text = _sp2.GetTeamRoleData(idTeam);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        _sp2.ClearInput();
    }
    #endregion
    
    private void ToTableRoasterTeam_Click(int idTeam)
    {
        Mainframe.Navigate(new DataGridPage(idTeam.ToString()));
        ToPrev.Click -= (sender, args) => { ToTableRoasterTeam_Click(idTeam); };
    }
    #endregion
}
