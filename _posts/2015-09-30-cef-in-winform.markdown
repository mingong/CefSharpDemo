---
layout: post

title: Cef in WinForm

date: 2015-9-30 16:16

category: blog

tags:

  - C#
  
  - WinFrom
  
  - JavaScript
  
slug: cef-in-winform
---

# Cef in WinForm

### 这是一个看脸的时代

随着移动设备的崛起, Windows 应用已经被逐渐边缘化, 日常生活中大部分事情不用打开电脑了, 
掏出手机就可以完成. 但是Windows 应用终究还是要有的, 对没错, 我今天就是来讲Windows 应用开发的.

最近有做过一个Windows 上的应用, 大致是个编辑器. 既然是做Windows 应用, 那么一定要祭出VS神器, 
拖一拖控件什么的, 分分钟出来一个, 但是有一个问题: 太丑了. WPF 我没用过, 不敢妄加评论, 
这个应用我还是用的WinForm 实现的.

托互联网蓬勃发展的福, Web 页面越来越漂亮, 传统的Windows 控件由于定制性不强, 在这个看脸的时代, 
显然是入不了人们的法眼的. 于是有大把大把的Windows 应用设计都开始往Web 上靠.

由于做过一点前端, 我一开始就琢磨怎么用Web 页面实现这个功能, 这样样式也炒鸡好调, 功能也好加.

我看了[Electron](https://www.electronjs.org/) , Atom的核, 一个基于Web 技术的跨平台桌面应用开发框架. 
很好很好, 尝试了一下, 终于在读取本地文件的时候没搞出来, 又比较紧急加上打包有点麻烦, 
所以还是回到了WinForm 嵌浏览器的方案.

终于发现了今天的主角: CefSharp, 一个CEF 的C#绑定. 
CEF 是一个基于Chromium
的集成浏览器框架, 就是为在桌面应用中添加现代浏览器控件而生, 效率很高, 接近Chrome. 他有好多绑定, 因为我用C#
开发WinForm 应用, 我用了其中一个C#绑定[CefSharp](https://github.com/cefsharp/CefSharp).

CefSharp非常易用, 有了他分分钟可以做个Chrome出来. 你可以去它的GitHub页面上翻翻例子. 
这里是项目的[wiki](https://github.com/cefsharp/CefSharp/wiki/Frequently-asked-questions).

### 安装CefSharp

CefSharp可以自己装, 更方便的是通过[NuGet](https://www.nuget.org/) 安装, 如果安装了
NuGet , 在`Project > Manage NuGet Packages` 里打开包管理器, 搜索cef , 安装`CefSharp.Winforms`, 
这样会自动安装依赖, 并且安装完之后自动添加为项目的引用.

注意

`CefSharp`不支持默认的`AnyCPU`编译配置, 需要自己根据需求改成`x64/x86`

### 使用 CefSharp

效果图:

![MainWindow](../images/cef/winform.png)

### 加载在线资源

关于页面是个网页, 集成起来十分简单, 安装

```csharp
using System.Windows.Forms;
using CefSharp.WinForms;

/* ... */

        private void initAboutPage()
        {
            var browser = new ChromiumWebBrowser("http://localhost:4000")
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(browser);
        }
```

然后效果就是这样, 这可是一个完整的Chromium哦, 妈妈再也不担心我不会做浏览器了:

![About](../images/cef/about.png)

### 加载本地资源

如果我们要做一个本地应用, 不能联网, 那么我们会把所有的资源躲放到本地, 然后从本地进行加载.

比如我们使用[Pure.css](https://purecss.io/) 来做一个极简文本编辑器, 我们先写好页面:

```html
<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
    <link rel="stylesheet" href="./pure-min.css" media="screen" title="no title" charset="utf-8">
    <title>editor</title>
    <style media="screen">
      body, html {
        margin: 0;
        padding: 0;
        height: 100%;
      }

      form {
        height: 100%;
      }

      button {
        height: 40px;
      }

      textarea {
        height: calc(100% - 40px);
      }
    </style>
  </head>
  <body>
    <form class="pure-form">
		<textarea id="editor" class="pure-input-1"></textarea>
		<button type="submit" class="pure-button pure-input-1 pure-button-primary">Save</button>
    </form>
  </body>
</html>
```

效果很简单, 就一个文本框, 一个保存按钮:

![Editor](../images/cef/editor.png)

添加`wen` 文件夹, 把静态页面都放进去, 记住要在项目属性里选择将这些静态页面拷贝到输出目录, 
不然程序跑起来就找不到这些页面了.

`ChromiumWebBrowser`可以加载本地文件, 使用绝对路径:

```csharp
using System.Windows.Forms;
using CefSharp.WinForms;
using System.IO;

/* ... */

        private ChromiumWebBrowser _browser;

            var editor_info = new FileInfo(@".\wen\editor.html");
            _browser = new ChromiumWebBrowser(editor_info.FullName)
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(_browser);
```

### 与JavaScript进行交互

项目里都用了前端写, 如果不能用JavaScript那简直和没用一样啊, 我们来看看与JavaScript的交互, 
包括

### 在C#里调用JavaScript代码

简单的代码可以这样:

```csharp
using CefSharp.WinForms;

/* ... */

        private ChromiumWebBrowser _browser;

            if (_browser != null && _browser.IsBrowserInitialized)
            {
                _browser.ExecuteScriptAsync("alert('Call JavaScript from C#');");
            }
```

如果需要返回值, 不如我们写一个获取高度的函数:

```csharp
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.WinForms;

/* ... */

        private ChromiumWebBrowser _browser;

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
```

不过这里只能返回简单类型的数据, 不能返回自定义的复杂对象, 如果需要复杂类型, 可以返回json串, 然后序列化成C#对象.

### JavaScript里调用C#代码

这个时候我们需要把C#对象暴露给JavaScript使用:

```csharp
using System;
using System.Windows.Forms;
using CefSharp.WinForms;

/* ... */

        private ChromiumWebBrowser _browser;

    internal class CefsharpCallTest
    {
        public string StringProp { get; set; }

        public void ShowHelloCefsharp()
        {
            MessageBox.Show("Hello, Cefsharp!!!!");
        }

        public CefsharpCallTest()
        {
            StringProp = "Hello, Cefsharp";
        }
    }

/* ... */

            // For legacy biding we'll still have support for
            // "版本新点的cefsharp( 例如67.0.0 ), 已不建议再这样注册C#对象"
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            
            // "注册对象, 必须在browser 一创建后就注册"
            _browser.RegisterJsObject("cefsharp", new CefsharpCallTest());
```

然后在JavaScript里就可以放肆使用属性和方法了:

```javascript
    <script type="text/javascript">
      function callCs() {
        alert(cefsharp.stringProp);
        cefsharp.showHelloCefsharp();
      }
    </script>
```

注意

为了保证js代码看起来与其他部分风格一致, 这里在js里面调用的时候第一个字母变成了小写, 
这个可以在RegisterJsObject里第三个参数配置.

### DevTools

Chrome/Chromium 相当好用的开发者工具也是可以使用的.

```csharp
using CefSharp.WinForms;

/* ... */

        private ChromiumWebBrowser _browser;

            if (_browser != null)
            {
                _browser.ShowDevTools();
            }
```

### 自定义Scheme

把网页当本地文件会有很多限制, 比较好的方法是自定义Scheme然后在里面处理.

我们使用`custom://cefsharp/custom?path=xxx`的uri形式来获取文本内容. 我们这里自定义了一个 custom Scheme.

我们要定义处理请求的部分:

```csharp
// Copyright © 2010-2017 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using System.Web; // "工程需添加引用, 程序集->框架->System.Web"

namespace CefSharpDemo
{
    internal class CefSharpSchemeHandler : ResourceHandler
    {
        public override bool ProcessRequestAsync(IRequest request, ICallback callback)
        {
            // The 'host' portion is entirely ignored by this scheme handler.
            var uri = new Uri(request.Url);
            var fileName = uri.Authority + uri.AbsolutePath;
            var file = "." + uri.AbsolutePath;

            Task.Run(() =>
            {
                using (callback)
                {
                    String mimeType = "text/plain";
                    int statusCode = (int)HttpStatusCode.OK;
                    Stream stream = null;

                    if (string.Equals(fileName, "cefsharp/custom", StringComparison.OrdinalIgnoreCase))
                    {
                        if (request.Method == "GET")
                        {
                            var param = HttpUtility.ParseQueryString(uri.Query);
                            var path = param["path"];
                            if (File.Exists(path))
                            {
                                var fileExtension = Path.GetExtension(path);
                                mimeType = ResourceHandler.GetMimeType(fileExtension);
                                var content = File.ReadAllText(path);
                                stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                            }
                            else
                            {
                                mimeType = "text/plain";
                                statusCode = (int)HttpStatusCode.NotFound;
                                stream = new MemoryStream();
                            }
                        }
                        else
                        {
                            mimeType = "text/plain";

                            statusCode = (int)HttpStatusCode.NotFound;
                            stream = new MemoryStream();
                        }
                    }
                    else
                    {
                        if (File.Exists(file))
                        {
                            var fileExtension = Path.GetExtension(file);
                            mimeType = ResourceHandler.GetMimeType(fileExtension);
                            var bytes = File.ReadAllBytes(file);
                            stream = new MemoryStream(bytes);
                        }
                        else
                        {
                            statusCode = (int)HttpStatusCode.NotFound;
                            stream = new MemoryStream();
                        }
                    }

                    // Reset the stream position to 0 so the stream can be copied into the underlying unmanaged buffer
                    stream.Position = 0;
                    // Populate the response values - No longer need to implement GetResponseHeaders (unless you need to perform a redirect)
                    ResponseLength = stream.Length;
                    MimeType = mimeType;
                    StatusCode = statusCode;
                    Stream = stream;

                    callback.Continue();
                }
            });

            return true;
        }
    }
}

```

```csharp
// Copyright © 2010-2017 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using CefSharp;

namespace CefSharpDemo
{
    public class CefSharpSchemeHandlerFactory : ISchemeHandlerFactory
    {
        public const string SchemeName = "custom";

        static CefSharpSchemeHandlerFactory()
        {

        }

        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            // Notes:
            // - The 'host' portion is entirely ignored by this scheme handler.
            // - if you register a ISchemeHandlerFactory for http/https schemes you should also specify a domain name
            // - Avoid doing lots of processing in this method as it will affect performance.
            // - Use the Default ResourceHandler implementation

            if (schemeName == SchemeName)
            {
                return new CefSharpSchemeHandler();
            }

            return null;
        }
    }
}
//
```

然后注册到cef 里面

```csharp
using CefSharp;

/* ... */

                // cef settings
                var settings = new CefSettings();
                settings.RegisterScheme(new CefCustomScheme
                {
                    SchemeName = CefSharpSchemeHandlerFactory.SchemeName,
                    SchemeHandlerFactory = new CefSharpSchemeHandlerFactory(),
                    IsSecure = true // treated with the same security rules as those applied to "https" URLs

                });
                Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
```

```javascript
    <script type="text/javascript">
      function save() {
          return editor.value;
      }
    </script>
```

实例项目在[-> HERE <-](https://github.com/mingong/CefSharpDemo)

保存功能就没有写了, 要么可以把内容序列化到GET 参数里, 但是这样不能处理大文本.
