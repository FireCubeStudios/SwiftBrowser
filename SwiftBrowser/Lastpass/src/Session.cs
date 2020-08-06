// Copyright (C) 2013 Dmitry Yakimenko (detunized@gmail.com).
// Licensed under the terms of the MIT license. See LICENCE for details.

namespace LastPass
{
    internal class Session
    {
        public Session(string id, int keyIterationCount, string token, string encryptedPrivateKey, Platform platform)
        {
            Id = id;
            KeyIterationCount = keyIterationCount;
            Token = token;
            EncryptedPrivateKey = encryptedPrivateKey;
            Platform = platform;
        }

        public string Id { get; }
        public int KeyIterationCount { get; }
        public string Token { get; }
        public string EncryptedPrivateKey { get; }
        public Platform Platform { get; }
    }
}
