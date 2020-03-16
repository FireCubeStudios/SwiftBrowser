using SwiftBrowser.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SwiftBrowser.Extensions
{
    public class DarkMode
    {
        public async void DarkMode_Click(WebView TheWebView)
        {
            try { 
            string functionString = "var link = document.createElement('link'); link.rel = 'stylesheet'; link.type = 'text/css';  link.href = 'ms-appx-web:///WebFiles/UniversalDarkMode.css'; document.getElementsByTagName('head')[0].appendChild(link); ";
            await TheWebView.InvokeScriptAsync("eval", new string[] { functionString });
        }
            catch
            {

            }
        }
    }
}
