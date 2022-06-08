using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using coursework.Addons;
using coursework.Pages;

namespace coursework;

public partial class MainWindow : Window
{
    public static Frame Mainframe;
    public static Button ToPrev;
    
    public MainWindow()
    {
        InitializeComponent();
        
        Mainframe = new Frame
        {
            Content = new MainPage(),
            NavigationUIVisibility = NavigationUIVisibility.Hidden
            //style??
        };
        grid.LocateOn(Mainframe, (1, 1));
        Grid.SetColumnSpan(Mainframe, 2);
        
        ToPrev = new Button
        {
            Content = "Previous"
            //style..
        };
        grid.LocateOn(ToPrev, (2, 2));
        
        Mainframe.Navigated += (sender, e) => 
            { ToPrev.Visibility = Mainframe.Content.GetType() == typeof(MainPage)
                ? Visibility.Hidden : Visibility.Visible; };
    }
}