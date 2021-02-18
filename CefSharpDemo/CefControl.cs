using System;
using System.Windows.Forms;
using CefSharp.WinForms.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.IO;
using CefSharp;
using CefSharp.WinForms;
using System.Web; // "工程需添加引用, 程序集->框架->System.Web"

namespace CefSharpDemo
{
    public partial class CefControl: UserControl
    {
        /*
        ChromiumWebBrowser Default;
        */
        private ChromiumWebBrowser _browser;

        OpenFileDialog openFileDialog;
        string fileName = "fakename";
        string workingFolderName = @"C:\fakepath";

        public CefControl(/*string url*/)
        {
            InitializeComponent();

            /*
            if (!Cef.IsInitialized)
            {
                // "cef settings"
                var settings = new CefSettings();
                Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            }
            string url = "https://www.google.com";
            Default = new ChromiumWebBrowser(url);
            // "Add it to the form and fill it to the form window."
            this.Controls.Add(Default);
            Default.Dock = DockStyle.Fill;

            */
            // "init editor"
            CreateEditor();
        }

        private void CreateEditor()
        {
            if (!Cef.IsInitialized)
            {
                CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
                // "cef settings"
                var settings = new CefSettings();
                settings.RegisterScheme(new CefCustomScheme
                {
                    SchemeName = CefSharpSchemeHandlerFactory.SchemeName,
                    SchemeHandlerFactory = new CefSharpSchemeHandlerFactory(),
                    IsSecure = true
                    // 'treated with the same security rules as those applied to "https" URLs'

                });
                Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            }

            _browser = new ChromiumWebBrowser("custom://cefsharp/wen/editor.html")
            {
                Dock = DockStyle.Fill
            };

            // "For legacy biding we'll still have support for"
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;

            _browser.RegisterJsObject("cefsharp", new CefSharpCallTest());

            this.Controls.Add(_browser);

            _browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
        }

        public void New(object sender, EventArgs e)
        {
            if (_browser != null)
            {
                // "new file"
                _browser.ExecuteScriptAsync("wen();");

                fileName = "fakename";
            }
        }

        public void Open(object sender, EventArgs e)
        {
            openFileDialog = new System.Windows.Forms.OpenFileDialog();

            // 
            // "openFileDialog"
            // 
            this.openFileDialog.Filter = "Text files (*.txt)|*.txt";
            this.openFileDialog.Title = "Open File";
            this.openFileDialog.ValidateNames = false;
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);

            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            fileName = openFileDialog.FileName;
            if (_browser != null)
            {
                // "load file"
                _browser.ExecuteScriptAsync(string.Format(@"
(function() {{
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function() {{
        if (xmlHttp.readyState == 4) {{
            if (xmlHttp.status == 200) {{
                loadText(xmlHttp.responseText);
                /*
                other...
                */
            }}
        }}
    }}
    /*
    xmlHttp.open('GET', 'custom://cefsharp/custom?path={0}&nocache=' + (new Date()).getTime(), false);
    */
    xmlHttp.open('GET', 'custom://cefsharp/custom?path={0}&nocache=' + (new Date()).getTime(), true);
    xmlHttp.send(null);
    /*
    loadText(xmlHttp.responseText);
    */
}})();
", HttpUtility.UrlEncode(fileName)));

                workingFolderName = Path.GetDirectoryName(fileName);
                fileName = Path.GetFileName(fileName);
            }
        }

        public void Save(object sender, EventArgs e)
        {
            _Save();
        }

        public void _Save()
        {
            if (string.Equals(fileName, "fakename", StringComparison.OrdinalIgnoreCase))
            {
                _SaveAs();
            }
            else
            {
                if (_browser != null && _browser.IsBrowserInitialized)
                {
                    var task = _browser.EvaluateScriptAsync("save();");

                    object result;
                    string fullPath;

                    fullPath = Path.Combine(workingFolderName, fileName);

                    task.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            result = response.Success ? (response.Result ?? "null") : response.Message;
                            /*
                            TextWriter wr = new StreamWriter(fullPath);
                            wr.Write(result);
                            wr.Close();
                            */
                            MessageBox.Show("result: " + "result");

                            // @todo
                        }
                        else
                        {
                            MessageBox.Show("is Faulted.");
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        public void SaveAs(object sender, EventArgs e)
        {
            _SaveAs();
        }

        public void _SaveAs(/*string filename*/)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            /*
            saveDialog.Title = "Save As";
            */
            saveDialog.DefaultExt = "txt";
            saveDialog.Filter = "Text files|*.txt";
            /*
            ** "是否覆盖当前文件"
            */
            saveDialog.OverwritePrompt = true;
            /*
            ** "还原目录"
            */
            saveDialog.RestoreDirectory = true;
            // "设置文件名"
            /*
            saveDialog.FileName = filename;

            */
            string fullPath;

            if (string.Equals(fileName, "fakename", StringComparison.OrdinalIgnoreCase))
            {
                fullPath = workingFolderName;
            }
            else
            {
                /*
                fullPath = Path.Combine(workingFolderName, fileName);

                saveDialog.FileName = fullPath;
                */
                saveDialog.FileName = fileName;
            }

            /*
            saveDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            */
            saveDialog.InitialDirectory = workingFolderName;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = saveDialog.FileName;

                workingFolderName = Path.GetDirectoryName(fileName);
                fileName = Path.GetFileName(fileName);

                _Save();
            }
            /*
            else
            {
                // "cancelled"
            }
            */
        }

        public void ChooseWorkingDirectory(object sender, EventArgs e)
        {
            FolderBrowserDialog workingFolderBrowserDialog = new FolderBrowserDialog();

            if (string.Equals(workingFolderName, @"C:\fakepath", StringComparison.OrdinalIgnoreCase))
            {
                workingFolderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            }
            else
            {
                workingFolderBrowserDialog.SelectedPath = workingFolderName;
            }

            /*
            ** "调用ShowDialog()方法显示该对话框, 该方法的返回值代表用户是否点击了确定按钮"
            */
            DialogResult workingDialogResult = workingFolderBrowserDialog.ShowDialog();

            if (workingDialogResult == DialogResult.Cancel)
            {
                return;
            }
            /*
            else
            {
                // "Todo"
            }
            */

            workingFolderName = workingFolderBrowserDialog.SelectedPath.Trim();
            /*
            if (_browser != null)
            {
                // "Set Working Directory"
                _browser.ExecuteScriptAsync(string.Format(@"choose_working_directory('{0}');", HttpUtility.UrlEncode(workingFolderName)));
            }
            */

            return;
        }

        public static void Shutdown()
        {
            if (Cef.IsInitialized)
                Cef.Shutdown();
        }

        public void CsCallJs1(object sender, EventArgs e)
        {
            if (_browser != null && _browser.IsBrowserInitialized)
            {
                _browser.ExecuteScriptAsync("alert('Call JavaScript from C#');");
            }
        }

        public void CsCallJs2(object sender, EventArgs e)
        {
            if (_browser != null && _browser.IsBrowserInitialized)
            {
                var task = _browser.EvaluateScriptAsync(@"
(function() {
    var body = document.body, html = document.documentElement;
    return Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight,
                    html.scrollHeight, html.offsetHeight);
})();");

                object result;
                task.ContinueWith(t =>
                {
                    if (!t.IsFaulted)
                    {
                        var response = t.Result;
                        result = response.Success ? (response.Result ?? "null") : response.Message;
                        MessageBox.Show("result: " + result);
                    }
                    else
                    {
                        MessageBox.Show("is Faulted.");
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public void DevTools(object sender, EventArgs e)
        {
            if (_browser != null)
            {
                _browser.ShowDevTools();
            }
        }

        public void JsCallCs(object sender, EventArgs e)
        {
            if (_browser != null)
            {
                _browser.ExecuteScriptAsync("callCs();");
            }
        }

        private void OnIsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs args)
        {
            if (args.IsBrowserInitialized)
            {
                // "Get the underlying browser host wrapper"
                var browserHost = _browser.GetBrowser().GetHost();
                var requestContext = browserHost.RequestContext;
                string errorMessage;
                // "Browser must be initialized before getting/setting preferences"
                // "Example of disable spellchecking"
                var success = requestContext.SetPreference("browser.enable_spellchecking", false, out errorMessage);
                if (!success)
                {
                    this.InvokeOnUiThreadIfRequired(() => MessageBox.Show("Unable to set preference browser.enable_spellchecking errorMessage: " + errorMessage));
                }
            }
        }
    }

    internal class CefSharpCallTest
    {
        public string StringProp { get; set; }

        public CefSharpCallTest()
        {
            StringProp = "Hello, CefSharp";
        }

        public void ShowHelloCefSharp()
        {
            MessageBox.Show("Hello, CefSharp!!!!");
        }
    }
}
