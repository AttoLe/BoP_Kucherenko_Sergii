using System.Windows.Controls;
using coursework.Pages.ChangeDataPages.InputInteraction;
using coursework.Pages.TablePages;
using static coursework.MainWindow;

namespace coursework.Pages.ChangeDataPages.NavigationPages;

public partial class ChangeDataStadiumMainPage : Page
{
    public ChangeDataStadiumMainPage()
    {
        InitializeComponent();
        
        ToChangeExistingStadium.Click += (sender, args) => 
            { Mainframe.Navigate(new StadiumDataUpdatePage()); };
        
        ToCreateNewStadium.Click += (sender, args) => 
            { Mainframe.Navigate(new InteractStadiumDataPage()); };

        ToPrev.Click += (sender, args) => { Mainframe.Navigate(new ChangeDataMainPage()); };
    }
}