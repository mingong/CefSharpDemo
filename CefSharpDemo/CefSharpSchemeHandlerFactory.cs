// "Copyright © 2010-2017 The CefSharp Authors. All rights reserved."
//
// "Use of this source code is governed by a BSD-style license that can be found in the LICENSE file."

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
            // "Notes:"
            // "- The 'host' portion is entirely ignored by this scheme handler."
            // "- if you register a ISchemeHandlerFactory for http/https schemes you should also specify a domain name"
            // "- Avoid doing lots of processing in this method as it will affect performance."
            // "- Use the Default ResourceHandler implementation"

            if (schemeName == SchemeName)
            {
                return new CefSharpSchemeHandler();
            }

            return null;
        }
    }
}
//