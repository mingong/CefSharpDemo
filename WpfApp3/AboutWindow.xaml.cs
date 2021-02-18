/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
*/
using System.Windows;

namespace WpfApp3
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            // init the about page
            initAboutPage();
        }

        private void initAboutPage()
        {
            
        }

        private void Shutdown()
        {
            aboutControl.Dispose();
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
    }
}


