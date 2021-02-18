using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using CefSharp.Wpf.Example.Controls;

namespace WpfApp3
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save));

            CommandBindings.Add(new CommandBinding(CefSharpCommands.Exit, Exit));
            CommandBindings.Add(new CommandBinding(CefSharpCommands.CsCallJs1, CsCallJs1));
            CommandBindings.Add(new CommandBinding(CefSharpCommands.CsCallJs2, CsCallJs2));
            CommandBindings.Add(new CommandBinding(CefSharpCommands.DevTools, DevTools));
            CommandBindings.Add(new CommandBinding(CefSharpCommands.JsCallCs, JsCallCs));
            CommandBindings.Add(new CommandBinding(CefSharpCommands.About, About));
        }

        private void Open(object sender, ExecutedRoutedEventArgs e)
        {
            cefControl.Open(sender, e);
        }

        private void Save(object sender, ExecutedRoutedEventArgs e)
        {
            cefControl.Save(sender, e);
        }

        private void SaveAs(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Shutdown()
        {
            // "快速退出."
            Process.GetCurrentProcess().Kill();
            //
            Close();
        }

        private void Exit(object sender, ExecutedRoutedEventArgs e)
        {
            Shutdown();
        }

        /*
        **  override OnClosing()函数: 禁止用户通过点击右上角的关闭按钮来关闭窗口
        */
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            /*
            e.Cancel = true;

            */
            Shutdown();
        }

        private void CsCallJs1(object sender, ExecutedRoutedEventArgs e)
        {
            cefControl.CsCallJs1(sender, e);
        }

        private void CsCallJs2(object sender, ExecutedRoutedEventArgs e)
        {
            cefControl.CsCallJs2(sender, e);
        }

        private void DevTools(object sender, ExecutedRoutedEventArgs e)
        {
            cefControl.DevTools(sender, e);
        }

        private void JsCallCs(object sender, ExecutedRoutedEventArgs e)
        {
            cefControl.JsCallCs(sender, e);
        }

        private void About(object sender, ExecutedRoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();

            aboutWindow.Show();
        }
    }
}
