// Copyright (C) 2013 Dmitry Yakimenko (detunized@gmail.com).
// Licensed under the terms of the MIT license. See LICENCE for details.

using System.ComponentModel;

namespace LastPass
{
    [DesignerCategory("Code")]
    public class WebClient : System.Net.WebClient, IWebClient
    {
    }
}
