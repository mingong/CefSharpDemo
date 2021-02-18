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
                    /*
                    String statusText = "OK";
                    */
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
                                /*
                                statusText = "OK";
                                statusCode = (int)HttpStatusCode.OK;
                                */
                                var content = File.ReadAllText(path);
                                stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                            }
                            else
                            {
                                mimeType = "text/plain";
                                /*
                                statusText = "404 error";
                                */
                                statusCode = (int)HttpStatusCode.NotFound;
                                stream = new MemoryStream();
                            }
                        }
                        else
                        {
                            mimeType = "text/plain";

                            /*
                            statusText = "404 error";
                            */
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
                            /*
                            statusText = "OK";
                            statusCode = (int)HttpStatusCode.OK;
                            Byte[] bytes = File.ReadAllBytes(file);
                            */
                            var bytes = File.ReadAllBytes(file);
                            stream = new MemoryStream(bytes);
                        }
                        else
                        {
                            /*
                            mimeType = "text/plain";
                            statusText = "404 error";
                            */
                            statusCode = (int)HttpStatusCode.NotFound;
                            stream = new MemoryStream();
                        }
                    }

                    /*
                    if (stream == null)
                    {
                        callback.Cancel();
                    }
                    else
                    { 
                        // Reset the stream position to 0 so the stream can be copied into the underlying unmanaged buffer
                        stream.Position = 0;
                        // Populate the response values - No longer need to implement GetResponseHeaders (unless you need to perform a redirect)
                        ResponseLength = stream.Length;
                        MimeType = "text/html";
                        StatusText = "OK";
                        StatusCode = (int)HttpStatusCode.OK;
                        Stream = stream;

                        callback.Continue();
                    }
                    */
                    // Reset the stream position to 0 so the stream can be copied into the underlying unmanaged buffer
                    stream.Position = 0;
                    // Populate the response values - No longer need to implement GetResponseHeaders (unless you need to perform a redirect)
                    ResponseLength = stream.Length;
                    MimeType = mimeType;
                    /*
                    StatusText = statusText;
                    */
                    StatusCode = statusCode;
                    Stream = stream;

                    callback.Continue();
                }
            });

            return true;
        }
    }
}
