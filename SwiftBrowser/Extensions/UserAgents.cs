﻿using SwiftBrowser.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace SwiftBrowser.Extensions
{
    public class UserAgents
    {
        public void RequestUserAgent_Click(WebView TheWebView)
        {
            try
            {
                if (WebViewPage.IsMobileSiteEnabled == false)
                {
                    string userAgent = "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.10586";
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(TheWebView.Source.ToString()));
                    httpRequestMessage.Headers.Append("User-Agent", userAgent);
                    TheWebView.NavigateWithHttpRequestMessage(httpRequestMessage);
                    WebViewPage.IsMobileSiteEnabled = true;
                    WebViewPage.UserAgentbuttonControl.Content = "Request desktop site";
                }
                else
                {
                    string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246";
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(TheWebView.Source.ToString()));
                    httpRequestMessage.Headers.Append("User-Agent", userAgent);
                    TheWebView.NavigateWithHttpRequestMessage(httpRequestMessage);
                    WebViewPage.IsMobileSiteEnabled = false;
                    WebViewPage.UserAgentbuttonControl.Content = "Request mobile site";
                }
            }
            catch
            {

            }
        }
    }
}
