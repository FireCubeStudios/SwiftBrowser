// Copyright (C) 2013 Dmitry Yakimenko (detunized@gmail.com).
// Licensed under the terms of the MIT license. See LICENCE for details.

namespace LastPass
{
    public class Blob
    {
        public Blob(byte[] bytes, int keyIterationCount, string encryptedPrivateKey)
        {
            Bytes = bytes;
            KeyIterationCount = keyIterationCount;
            EncryptedPrivateKey = encryptedPrivateKey;
        }

        public byte[] MakeEncryptionKey(string username, string password)
        {
            return FetcherHelper.MakeKey(username, password, KeyIterationCount);
        }

        public byte[] Bytes { get; private set; }
        public int KeyIterationCount { get; private set; }
        public string EncryptedPrivateKey { get; private set; }
    }
}
