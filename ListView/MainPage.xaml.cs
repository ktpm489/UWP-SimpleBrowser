using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace ListView
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // 为不同的菜单创建不同的List类型
        private List<NavMenuItem> navMenuPrimaryItem = new List<NavMenuItem>(
            new[]
            {
                new NavMenuItem()
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Icon = "\xE10F",
                    Label = "哔哩哔哩",
                    Selected = Visibility.Visible,
                    DestUri = "http://m.bilibili.com/index.html"
                },

                new NavMenuItem()
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Icon = "\xE11A",
                    Label = "AcFun",
                    Selected = Visibility.Collapsed,
                    DestUri = "http://m.acfun.cn/"
                },

                new NavMenuItem()
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Icon = "\xE121",
                    Label = "嘀哩嘀哩",
                    Selected = Visibility.Collapsed,
                    DestUri = "http://m.dilidili.wang/"
                },

                new NavMenuItem()
                {
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    Icon = "\xE122",
                    Label = "动漫之家",
                    Selected = Visibility.Collapsed,
                    DestUri = "http://m.dmzj.com/"
                }

            });

        //标签云TagCloudItem


        public MainPage()
        {
            this.InitializeComponent();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false; //显示旧的titlebar
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            // 绑定导航菜单
            NavMenuPrimaryListView.ItemsSource = navMenuPrimaryItem;

            // SplitView 开关
            PaneOpenButton.Click += (sender, args) =>
            {
                RootSplitView.IsPaneOpen = !RootSplitView.IsPaneOpen;
            };
            // 导航事件
            NavMenuPrimaryListView.ItemClick += NavMenuListView_ItemClick;
            NavMenuSecondaryListView.ItemClick += NavMenuListView_ItemClick;

            //设置user-agent
            ChangeUserAgent("Mozilla/5.0 (Linux; Android 4.1.1; Nexus 7 Build/JRO03D) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166  Safari/535.19");

            // 默认页
            webview.Navigate(new Uri("http://m.bilibili.com/index.html"));

            //将默认页压栈
            historyStack.Add(new Uri("http://m.bilibili.com/index.html"));
        }
        

        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

        const int URLMON_OPTION_USERAGENT = 0x10000001;
        public void ChangeUserAgent(string Agent)
        {
            UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, Agent, Agent.Length, 0);
        }

        /// <summary>
        /// frame后退
        /// </summary>
        bool IsClicks = false;
        private async void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame currentFrame = this.Frame;

            if (currentFrame.CanGoBack)
            {
                e.Handled = true;
                currentFrame.GoBack();
            }
            else
            {

                if (e.Handled == false)
                {
                    if (IsClicks)
                    {
                        Application.Current.Exit();
                    }
                    else
                    {
                        IsClicks = true;
                        e.Handled = true;
                        messShow.Show("再按一次退出应用", 1500);
                        await Task.Delay(1500);
                        IsClicks = false;
                    }
                }

            }
        }

        /// <summary>
        /// 获取当前frame的page名称
        /// </summary>
        /// <returns></returns>
        public string GetCurrentPageName()
        {
            Type currentPage = frame.SourcePageType;
            return (currentPage == null) ? null : currentPage.Name;
        }


        /// <summary>
        /// 更改title名称，更改listview选中项
        /// </summary>
        public void Back_ListView_ItemSelect()
        {
            string listview_name = GetCurrentPageName();
            ApplicationView appView = ApplicationView.GetForCurrentView();

            foreach (var np in navMenuPrimaryItem)
            {
                np.Selected = Visibility.Collapsed;
            }


            switch (listview_name)
            {
                case "Page1":
                    navMenuPrimaryItem[0].Selected = Visibility;
                    appView.Title = navMenuPrimaryItem[0].Label;
                    break;
                case "Page2":
                    navMenuPrimaryItem[1].Selected = Visibility;
                    appView.Title = navMenuPrimaryItem[1].Label;
                    break;
                case "Page3":
                    navMenuPrimaryItem[2].Selected = Visibility;
                    appView.Title = navMenuPrimaryItem[2].Label;
                    break;
                case "Page4":
                    navMenuPrimaryItem[3].Selected = Visibility;
                    appView.Title = navMenuPrimaryItem[3].Label;
                    break;
                default:
                    break;
            }
        }

        private void NavMenuListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // 遍历，将选中Rectangle隐藏
            foreach (var np in navMenuPrimaryItem)
            {
                np.Selected = Visibility.Collapsed;
            }

            NavMenuItem item = e.ClickedItem as NavMenuItem;
            // Rectangle显示并导航
            item.Selected = Visibility.Visible;
            if (item.DestUri != null)
            {
                webview.Stop(); //停止当前的活动

                webview.Navigate(new Uri(item.DestUri));        //转入新网页地址

                historyStack.Add(new Uri(item.DestUri));    //网址压栈

                ApplicationView appView = ApplicationView.GetForCurrentView();
                appView.Title = item.Label;
            }
            RootSplitView.IsPaneOpen = false;
        }


        /// <summary>
        /// 标题栏显示后退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frame_Navigated(object sender, NavigationEventArgs e)
        {
            Frame currentFrame = this.Frame;
            if (currentFrame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }



        List<Uri> historyStack = new List<Uri>();
        private void webview_back(object sender, RoutedEventArgs e)
        {
            if (historyStack.Count > 1)
            {
                webview.Stop(); //停止当前的活动

                historyStack.RemoveAt(historyStack.Count - 1);
                webview.Navigate(historyStack[historyStack.Count - 1]); //主窗口转向新页面
            }
        }

        private void webview_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs e)
        {
            e.Handled = true; //这个一定要
            if (e.Uri != null && e.Uri.ToString() != null)
            {
                //可以添加让当前浏览停止的代码
                historyStack.Add(e.Uri);    //压栈

                webview.Stop(); //停止当前的活动

                webview.Navigate(e.Uri); //主窗口转向新页面

                //跳转后需要处理的代码

            }
        }

        private void webview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (historyStack[historyStack.Count - 1] != args.Uri)
            {
                historyStack.Add(args.Uri);
            }

        }

        private void webview_full(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();

            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
                btn_full.Visibility = Visibility.Visible;
                btn_full2.Visibility = Visibility.Collapsed;
            }
            else
            {
                try
                {
                    view.TryEnterFullScreenMode();
                    btn_full.Visibility = Visibility.Collapsed;
                    btn_full2.Visibility = Visibility.Visible;
                }
                catch
                {
                    //未成功进入全屏
                }
            }

        }

        private void btn_Hide(object sender, RoutedEventArgs e)
        {
            if (grid_btns.Visibility == Visibility.Visible)
            {
                grid_btns.Visibility = Visibility.Collapsed;
            }
            else
            {
                grid_btns.Visibility = Visibility.Visible;
            }
        }

        private void btn_setting_Click(object sender, RoutedEventArgs e)
        {
            SettingSplitView.IsPaneOpen = !SettingSplitView.IsPaneOpen;
        }

        private void btn_clear_ClearCache(object sender, RoutedEventArgs e)
        {
            WebView.ClearTemporaryWebDataAsync().AsTask().Wait();
            messShow.Show("正在清除缓存数据...",5000);
        }

        private void btn_translate_Click(object sender, RoutedEventArgs e)
        {
            string str = @"https://www.baidu.com/s?wd=+";
            switch (combox_search.SelectedIndex)
            {
                case 0:
                    str = @"https://www.baidu.com/s?wd=+";
                    break;
                case 1:
                    str = @"http://www.google.com/search?q=";
                    break;
                case 2:
                    str = @"https://zh.wikipedia.org/wiki/";
                    break;
                case 3:
                    str = @"http://cn.bing.com/search?q=";
                    break;
                case 4:
                    str = @"https://www.pixiv.net/member.php?id=";
                    break;
                case 5:
                    str = @"https://translate.google.com/#auto/zh-CN/";
                    break;
                case 6:
                    str = @"https://www.zhihu.com/search?type=content&q=";
                    break;
                case 7:
                    str = @"http://maps.google.com/maps?q=";
                    break;
                default:
                    break;
            }

            if (textbox_search.Text != "")
            {
                str = str + textbox_search.Text;
                try
                {
                    webview.Stop(); //停止当前的活动

                    webview.Navigate(new Uri(str));
                    Tag_History_Add(isAddHistory);
                }
                catch (Exception)
                {
                    messShow.Show(@"。゜゜(´□｀｡)°゜。出错了", 3000);
                }
                finally
                {
                    
                }
            }
        }

        private void textbox_search_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                isAddHistory = true;

                btn_translate_Click(null,null);
                e.Handled = true;
            }
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {

                textbox_search.Text = search_textbox.Text;

                //search.Visibility = Visibility.Collapsed;
                //btn_search.Visibility = Visibility.Visible;
                isAddHistory = true;

                btn_translate_Click(null, null);
                e.Handled = true;
            }
        }

        private void search_textbox_LostFocus(object sender, RoutedEventArgs e)
        {
            //search.Visibility = Visibility.Collapsed;
            //btn_search.Visibility = Visibility.Visible;
        }

        private void search_combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            combox_search.SelectedIndex = search_combox.SelectedIndex;
        }

        private void combox_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            search_combox.SelectedIndex = combox_search.SelectedIndex;
        }

        private void search_open(object sender, RoutedEventArgs e)
        {
            search_combox.SelectedIndex = combox_search.SelectedIndex;
            btn_search.Visibility = Visibility.Collapsed;
            search.Visibility = Visibility.Visible;
            //search.Margin = new Thickness(-356, 0, 0, 0);
            search_textbox.Focus(FocusState.Pointer);

        }

        private void search_close(object sender, RoutedEventArgs e)
        {
            btn_search.Visibility = Visibility.Visible;
            search.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// isAdd:是否添加到历史combox
        /// </summary>
        private bool isAddHistory = true;
        private void Tag_History_Add(bool isAdd)
        {
            if (isAdd)
            {
                TagListBox.Items.Add(new TagCloudItem() { Tag = textbox_search.Text });
            }
        }

        private void combox_useragent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (combox_useragent.SelectedIndex)
            {
                case 0:
                    ChangeUserAgent("Mozilla/5.0 (Linux; Android 4.1.1; Nexus 7 Build/JRO03D) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166  Safari/535.19");
                    break;
                case 1:
                    ChangeUserAgent("Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 59.0.3071.115 Safari / 537.36");
                    break;
                default:
                    break;
            }
        }

        private void webview_refresh(object sender, RoutedEventArgs e)
        {
            webview.Refresh();
        }

        private void ToSettingPage(object sender, RoutedEventArgs e)
        {
            Frame currentFrame = this.Frame;
            currentFrame.Navigate(typeof(SettingPage));

            frame_Navigated(null,null);

        }

        private void RootSplitView_IsOpen(object sender, RoutedEventArgs e)
        {
            RootSplitView.IsPaneOpen = !RootSplitView.IsPaneOpen;
        }
    }
}
