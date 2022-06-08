using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using coursework.Addons;

namespace coursework.Pages;

public partial class ChangeDataMainPage : Page
{
    public ChangeDataMainPage(Page p)
    {
        InitializeComponent();

        Subframe.Content = p;
        
        ToChangeExistingTeam.Click += (sender, e) =>
        {
            MainWindow.Mainframe.NavigationService.Navigate(new ChangeDataMainPage(new ChangeDataSubPage2()));
            ToChangeExistingTeam.IsEnabled = false;
        };

        if (p.GetType() == typeof(ChangeDataSubPage1))
        {
            MainWindow.ToPrev.Click += (sender, args) =>
                { MainWindow.Mainframe.Navigate(new MainPage()); };
        }
        else
        {
            ToChangeExistingTeam.IsEnabled = false;
            MainWindow.ToPrev.Click += (sender, args) => 
                { MainWindow.Mainframe.Navigate(new ChangeDataMainPage(new ChangeDataSubPage1())); };
        }
    }
}