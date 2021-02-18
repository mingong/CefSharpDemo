using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.WinForms;

namespace CefSharpDemo
{
    public partial class AboutControl : UserControl
    {
        public AboutControl()
        {
            InitializeComponent();

            // "Create the about page..."
            CreateAboutPage();
        }

        private void CreateAboutPage()
        {
            var browser = new ChromiumWebBrowser("custom://cefsharp/wen/_book/index.html")
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(browser);
        }
    }
}
