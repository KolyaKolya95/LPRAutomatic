using LPRAutomatic.ViewModel;
using System;
using System.Windows;
using System.Windows.Threading;

namespace LPRAutomatic
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
          
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (VideoLPRWindow.LocalWebCam != null)
                 Dispatcher.Invoke( (Action) (() => VideoLPRWindow.LocalWebCam.Stop()));
        }
    }
}
