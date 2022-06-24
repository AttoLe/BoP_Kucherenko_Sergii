using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using coursework.Addons;

namespace coursework
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ExitCon(object sender, ExitEventArgs e) =>  SqlInteraction.ExitCon();
    }
}