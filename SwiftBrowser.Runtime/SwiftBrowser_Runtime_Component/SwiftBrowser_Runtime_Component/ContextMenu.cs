using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
namespace SwiftBrowser_Runtime_Component
{
	[AllowForWeb]
	public sealed class ContextMenu
	{
		public static string hrefLink { get; set; }
		public static string SRC { get; set; }
		public void setHREFCombination(string href)
		{
			hrefLink = href;
		}
		public void setSRCCombination(string src)
		{
			SRC = src;
		}
	}
}
