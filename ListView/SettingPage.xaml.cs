using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ListView
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();

            ShowMessage();

            NewWindow();
        }

        private async void ShowMessage()
        {
            //呵呵哒
            await Task.Delay(500);
            messShow.Show(@"你真调皮 （ '▿ ' ）", 3000);  
        }

        private async void NewWindow()
        {
            await Task.Delay(4000);

            messShow.Show(@"正在为您打开新窗口...", 3000);

            await Task.Delay(3000);

            var currentAV = ApplicationView.GetForCurrentView();
            var newAV = CoreApplication.CreateNewView();
            await newAV.Dispatcher.RunAsync(
            CoreDispatcherPriority.Normal,
            async () =>
            {
                var newWindow = Window.Current;
                var newAppView = ApplicationView.GetForCurrentView();
                newAppView.Title = "新窗口";

                //窗口标题栏样式 Title bar setting 

                var frame = new Frame();
                frame.Navigate(typeof(MainPage), null);
                newWindow.Content = frame;
                newWindow.Activate();

                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
                    newAppView.Id,
                    ViewSizePreference.UseMinimum,
                    currentAV.Id,
                    ViewSizePreference.UseMinimum);
            });
        }

    }
}
