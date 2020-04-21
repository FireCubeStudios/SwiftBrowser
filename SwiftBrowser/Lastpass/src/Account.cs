// Copyright (C) 2013 Dmitry Yakimenko (detunized@gmail.com).
// Licensed under the terms of the MIT license. See LICENCE for details.

namespace LastPass
{
    // TODO: Rename Group to Path since it reflects the actual meaning better.
    public class Account
    {
        public Account(string id, string name, string username, string password, string url, string group)
        {
            Id = id;
            Name = name;
            Username = username;
            Password = password;
            Url = url;
            Group = group;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Url { get; private set; }
        public string Group { get; private set; }
    }
}
