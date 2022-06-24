using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using coursework.Addons;
using coursework.Pages.ChangeDataPages.NavigationPages;
using coursework.Pages.TablePages;
using static coursework.MainWindow;

namespace coursework.Pages.ChangeDataPages.InputInteraction;

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
                Mainframe.Navigate(new ChangeDataTeamMainPage(new ChangeTeamDataSubPage1()));
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
        Mainframe.Navigate(new ChangeDataTeamMainPage(new ChangeTeamDataSubPage1()));
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
        MainLabel.Content = "ChangeTeamWindow";
        submit.Click += (sender, args) => { UpdateTeam_Submit_Click(id_team); };
        
        _sp1 = new InteractTeamDataSubPage();
        subframe.NavigationService.Navigate(_sp1);
        ToPrev.Click += (sender, args) => 
            Mainframe.Navigate(new TeamDataUpdatePage());
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
                Mainframe.Navigate(new TeamDataUpdatePage());
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        _sp1.ClearInput();
    }
    #endregion
    
    #region UpdatePlayerData
    
    public InteractTeamMainPage(int idPlayer, string dat, int idTeam)
    {
        InitializeComponent();

        var date = DateTime.ParseExact(dat, "dd.MM.yyyy", new CultureInfo("en-US"));
        MainLabel.Content = "ChangeTeamWindow";
        submit.Click += (sender, args) => 
            { UpdatePlayer_Submit_Click(idPlayer, idTeam); };

        _sp2 = new InteractPlayerDataSubPage(date);
        subframe.NavigationService.Navigate(_sp2);
        
        ToPrev.Click += (sender, args) => { new TeamRosterUpdatePage(idTeam); };
    }

    private void UpdatePlayer_Submit_Click(int idPlayer, int idTeam)
    {
        try
        {
            _sp2.UpdatePlayerData(idPlayer);
            Mainframe.Navigate(new TeamRosterUpdatePage(idTeam));
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
        ToPrev.Click += (sender, args) => 
            {Mainframe.Navigate(new TeamRosterUpdatePage(idTeam)); };
    }

    private void CreatePlayer(int idTeam, TextBlock info)
    {
        var counter = SqlInteraction.GetOneObject<int>(
            "select count(*) from get_all_player_by_team", new List<object>{idTeam});
        
        if (counter > 19)
        {
            MessageBox.Show("Team has maximum amount of players (20)");
            Mainframe.Navigate(new TeamRosterUpdatePage(idTeam));
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
    
    #endregion
}
