using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using coursework.Addons;
using coursework.Pages.ChangeDataPages.InputInteraction;
using coursework.Pages.TablePages;
using static coursework.MainWindow;
using static coursework.Addons.SqlInteraction;

namespace coursework.Pages;

public partial class OrganizeChamp : Page
{
    private DataGridInteraction _dgi1, _dgi2;
    
    private int _idPairless = -1, _startPrice, _amountMatchesRemain;
    private readonly int _maxPower, _idTournament, _matchesStart;
    private readonly List<int> _idStadiums, _futureMeets;
    private DateTime _dt, _finish;
    private bool _final;
    private int _third1 = -1, _third2 = -1, _final1 = -1, _final2 = -1, _phase;
    private ((int, int) locate, (int, int) span) _past, _future;
    
    public OrganizeChamp(List<int> idTeams, List<int> idStadiums)
    {
        InitializeComponent();
        
        instance = this;
        
        _idStadiums = idStadiums;
        _dt = DateTime.Now;
        _dt = new DateTime(_dt.Year, _dt.Month, _dt.Day, 12, 0, 0, 0);
        _startPrice = 0;
        _amountMatchesRemain = idTeams.Count  - 1;
        _finish = _dt.Add(new TimeSpan(26 * 7, 0, 0, 0));
        _maxPower = (int) Math.Log(_amountMatchesRemain + 1, 2);
        _matchesStart = _amountMatchesRemain;
        
        inf.Text = "Skip the time\n12:00 pm by def\nonly future\n< 0.5 year ahead";
        TimeSkip.SelectedDate = _dt;
        TimeSkip.SelectedDateChanged += (sender, args) => TimeSkip_Changed();
        
        ExecuteInput("execute dbo.create_tournament", new List<object>{DateTime.Today.Year});
        _idTournament = GetOneObject<int>("select dbo.get_ids", new List<object>());

        foreach (var v in idTeams)
            ExecuteInput("execute dbo.create_tournament_team", new List<object>{_idTournament, v});
        
        _futureMeets = new List<int>();
        
        var teamsVal = 
            idTeams.Select(v => 
                (v, GetOneObject<int>("select dbo.get_team_last_res", new List<object> {v}))).ToList();
        teamsVal = teamsVal.OrderBy(x => x.Item2).ToList();

        for (var i = 0; i < teamsVal.Count / 2; i++)
        {
            var idMeet = CreateMeet(teamsVal[i].Item1, teamsVal[teamsVal.Count - i - 1].Item1, 1);
            _futureMeets.Add(idMeet);
        }

        _dt = DateTime.Now;
        _future = ((0, 1), (5, 1));
        LoadFutureDataGrid((0, 1), (5, 1));
        info.Content = "Future matches";

        ToQueries.IsEnabled = false;
        ToQueries.Click += (sender, args) => 
            Mainframe.Navigate(new TempQueryPage(_idTournament, _dt.ToString("MM.dd.yyyy")));

        ToPrev.Click += (sender, args) => 
            Mainframe.Navigate(new MainPage());
    }

    #region DataGridMethods
    
    public void LoadPastDataGrid((int, int) locate, (int, int) span)
    {
        var names = new List<string>
        {
            "№", "first team name", "second team name", "city", "stadium name",
            "stadium capacity", "ticket price (grn)", "date", "score"
        };
        var com = $"select * from dbo.get_past_meets ({_idTournament}, '{_dt.ToString("MM.dd.yyyy")}')";
        _dgi1 = new DataGridInteraction(com, names);
        _dgi1.TableType1(grid, locate, span, () => 
            CreateColumn( (sender, args) => 
                Mainframe.Navigate(new DetailedPast(_dgi1.Get_Clicked_Column<int>(0))),
                "detailed", _dgi1), true);
    }

    public void LoadFutureDataGrid((int, int) locate, (int, int) span)
    {
        var names = new List<string>
        {
            "№", "first team name", "second team name", "city", "stadium name",
            "stadium capacity", "ticket price (grn)", "date", "change"
        };
        var com = $"select * from dbo.get_future_meets ({_idTournament}, '{_dt.ToString("MM.dd.yyyy")}')";
        
        _dgi2 = new DataGridInteraction(com, names);
        
        _dgi2.TableType1(grid, locate, span, () =>
            CreateColumn( (sender, args) => 
                Mainframe.Navigate(new ChangeMeetPage(_dgi2.Get_Clicked_Column<int>(0), 
                    _dgi2.Get_Clicked_Column<string>(1), 
                    _dgi2.Get_Clicked_Column<string>(2), 
                    _dt, _finish, 
                    DateTime.ParseExact(_dgi2.Get_Clicked_Column<string>(7), "hh:mm, dd.MM.yyyy", new CultureInfo("en-US")),
                    _past, _future)),
                "change", _dgi2), true);
    }

    private void CreateColumn(RoutedEventHandler e, string name, DataGridInteraction dg)
    {
        dg.CreateButtonColumn(e, name);
        dg.CreateStandardColumns();
    }

    #endregion

    private int CreateMeet(int index1, int index2, int importance)
    {
        var rnd = new Random();
        var idStadium = _idStadiums[rnd.Next(_idStadiums.Count - 1)];
        var dayAdd = importance switch
        {
            1 => rnd.Next(1, (int) ((_finish - _dt).TotalDays / _amountMatchesRemain)),
            2 => _matchesStart > 5 ? (int) ((_finish - _dt).TotalDays - 10) : (int) ((_finish - _dt).TotalDays - 25),
            3 => _matchesStart > 5 ? (int) ((_finish - _dt).TotalDays - 5) : (int) ((_finish - _dt).TotalDays - 10),
            _ => -1
        };
        
        var newDate = new DateTime(_dt.Year, _dt.Month, _dt.Day, rnd.Next(10, 20), 0, 0);
        newDate = newDate.Add(new TimeSpan(dayAdd, 0, 0, 0, 0));
        
        ExecuteInput("execute dbo.create_meet", 
            new List<object>{_idTournament, index1, index2, idStadium, newDate, _startPrice});
        
        var idMeet = GetOneObject<int>("select dbo.get_last_meet_id", new List<object>());
        
        MakeRoaster(ExecuteScalar<int>($"select id_meet_team1 from meet where id_meet = {idMeet}"), idMeet);
        MakeRoaster(ExecuteScalar<int>($"select id_meet_team2 from meet where id_meet = {idMeet}"), idMeet);
        
        return idMeet;
    }
    
    private void MakeRoaster(int idTeam, int idMeet)
    {
        int d = 4, f = 2, m = 4;
        if (GetOneObject<int>($"select dbo.get_team_role_num", new List<object>{idTeam, "defender"}) == 4)
            f = d = 3;

        void SetPlayersByRole(int amount, string role)
        {
            var l = GetList<int>("select * from dbo.get_player_by_role", new List<object> {idTeam, role});

            for (var i = 0; i < amount; i++)
            {
                var rnd = new Random();
                var index = rnd.Next(l.Count);
                ExecuteInput("execute dbo.set_meet_member", new List<object> {idMeet, l[index]});
                l.Remove(l[index]);
            }
        }
        
        SetPlayersByRole(d, "defender");
        SetPlayersByRole(m, "midfielder");
        SetPlayersByRole(f, "forward");
        SetPlayersByRole(1, "goalkeeper");
    }
    
    private void GenerateMeetResults(int idMeet, int index)
    {
        double fk = 0.8, mk = 0.6, dk = 0.4, gk = 0.2;
        var rnd = new Random();
        
        if (rnd.NextDouble() < 0.25) 
            return;

        void SetGoalByRole(double k, string name)
        {
            rnd = new Random();
            for (var i = 0; i < rnd.Next((int) (5 * k)) + 1; i++)
                if (rnd.NextDouble() > 1 - k)
                    ExecuteInput("execute dbo.create_player_goal", new List<object>{idMeet, index, name});
        }
        
        SetGoalByRole(fk, "forward");
        SetGoalByRole(mk, "midfielder");
        SetGoalByRole(dk, "defender");
        SetGoalByRole(gk, "goalkeeper");
    }
    
    private void TimeSkip_Changed()
    {
        var b = DateTime.TryParse(TimeSkip.SelectedDate.ToString(), out DateTime time);
        if (!b || time <= _dt || time > _finish)
        {
            MessageBox.Show("Incorrect date");
            TimeSkip.SelectedDate = _dt;
            return;
        }
        
        _dt = time;

        var temp = _futureMeets.Where(
            x => GetOneObject<bool>("select dbo.compare_meet_date", new List<object> {x, _dt}));
        foreach (var m in temp.ToList())
        {
            GenerateMeetResults(m, 1);
            GenerateMeetResults(m, 2);

            var s1 = GetOneObject<int>("select dbo.get_goal_by_team_in_meet", new List<object> {m, 0});
            var s2 = GetOneObject<int>("select dbo.get_goal_by_team_in_meet", new List<object> {m, 1});
            if (s1 == s2)
                ExecuteInput("execute dbo.penalty", new List<object> {m});

            _amountMatchesRemain--;
            _futureMeets.Remove(m);
            
            var t = Math.Log(_amountMatchesRemain + 1, 2);
            if (t - (int) t == 0)
            {
                _startPrice = (_maxPower - (int) t) * 2500;
                _phase = (int) t;
            }
            
            if(!_final)
                switch (_amountMatchesRemain)
                {
                    case > 3:
                    {
                        var indexLoser = GetOneObject<int>("select dbo.find_loser", new List<object> {m});
                        ExecuteInput("execute dbo.set_team_place",
                            new List<object> {_idTournament, indexLoser, Math.Pow(2, _phase)});

                        if (_idPairless == -1)
                            _idPairless = GetOneObject<int>("select dbo.find_winner", new List<object> {m});
                        else
                        {
                            var idMeet = CreateMeet(_idPairless, m, 1);
                            _futureMeets.Add(idMeet);
                        }

                        break;
                    }

                    case 2:
                    {
                        _third1 = GetOneObject<int>("select dbo.find_loser", new List<object> {m});
                        _final1 = GetOneObject<int>("select dbo.find_winner", new List<object> {m});
                        break;
                    }

                    case 1:
                    {
                        _third2 = GetOneObject<int>("select dbo.find_loser", new List<object> {m});
                        _final2 = GetOneObject<int>("select dbo.find_winner", new List<object> {m});

                        _startPrice = (_maxPower - 1) * 3500;
                        var idMeetT = CreateMeet(_third1, _third2, 2);
                        _futureMeets.Add(idMeetT);

                        _startPrice = _maxPower * 2500;
                        var idMeetF = CreateMeet(_final1, _final2, 3);
                        _futureMeets.Add(idMeetF);
                        
                        _final = true;
                        _amountMatchesRemain++;
                        
                        break;
                    }
                }
            else
            {
                var places = _amountMatchesRemain == 1 ? 4 : 2;
                
                var indexLoser = GetOneObject<int>("select dbo.find_loser", new List<object> {m});
                ExecuteInput("execute dbo.set_team_place",
                    new List<object> {_idTournament, indexLoser, places});
                        
                var indexWinner = GetOneObject<int>("select dbo.find_winner", new List<object> {m});
                ExecuteInput("execute dbo.set_team_place",
                    new List<object> {_idTournament, indexWinner, places - 1});
            }
        }
        
        
        if (_amountMatchesRemain != 0 && _amountMatchesRemain != _matchesStart)
        {
            ToQueries.IsEnabled = true;
            info.Content = "Past matches";
            _past = ((0, 1), (2, 1));
            LoadPastDataGrid((0, 1), (2, 1));
            _dgi2.dg.Visibility = Visibility.Hidden;
            _future = ((0, 4), (2, 1));
            LoadFutureDataGrid((0, 4), (2, 1));
            return;
        }
        
        if(_amountMatchesRemain == _matchesStart)
            return;
        
        _past = ((0, 1), (5, 1));
        LoadPastDataGrid((0, 1), (5, 1));
        _dgi2.dg.Visibility = Visibility.Hidden;
        
        MessageBox.Show("tournament is finished");
        TimeSkip.Visibility = Visibility.Hidden;
        inf.Visibility = Visibility.Hidden;

        Button toResult = new Button
        {
            Content = "ToMainReport"
            //style
        };
        grid.LocateOn(toResult, (4, 2));
        
        toResult.Click += (sender, args) =>
            Mainframe.Navigate(new MainQueryPage(_idTournament));
    }

    public static OrganizeChamp instance;
}