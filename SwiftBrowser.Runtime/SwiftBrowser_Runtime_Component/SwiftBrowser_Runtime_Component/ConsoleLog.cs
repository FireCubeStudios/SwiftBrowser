using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
namespace SwiftBrowser_Runtime_Component
{
	[AllowForWeb]
	public sealed class ConsoleLog
	{
		public static string LogString { get; set; }
		public void setLogCombination(string Log)
		{
			LogString = Log;
		}
	}
}
