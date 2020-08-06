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

        public string Id { get; }
        public string Name { get; }
        public string Username { get; }
        public string Password { get; }
        public string Url { get; }
        public string Group { get; }
    }
}
