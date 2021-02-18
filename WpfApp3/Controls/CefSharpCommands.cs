// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.


using System.Windows.Input;

namespace CefSharp.Wpf.Example.Controls
{
    public static class CefSharpCommands
    {
        public static RoutedUICommand Close = new RoutedUICommand("Close", "Close", typeof(CefSharpCommands));
        public static RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(CefSharpCommands));
        /*
        ** CsCallJs1
        */
        public static RoutedUICommand CsCallJs1 = new RoutedUICommand("CsCallJs1", "CsCallJs1", typeof(CefSharpCommands));
        /*
        ** CsCallJs2
        */
        public static RoutedUICommand CsCallJs2 = new RoutedUICommand("CsCallJs2", "CsCallJs2", typeof(CefSharpCommands));
        public static RoutedUICommand DevTools = new RoutedUICommand("DevTools", "DevTools", typeof(CefSharpCommands));
        /*
        ** JsCallCs
        */
        public static RoutedUICommand JsCallCs = new RoutedUICommand("JsCallCs", "JsCallCs", typeof(CefSharpCommands));
        public static RoutedUICommand About = new RoutedUICommand("About", "About", typeof(CefSharpCommands));
    }
}
