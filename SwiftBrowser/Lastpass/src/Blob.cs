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

        public byte[] Bytes { get; }
        public int KeyIterationCount { get; }
        public string EncryptedPrivateKey { get; }

        public byte[] MakeEncryptionKey(string username, string password)
        {
            return FetcherHelper.MakeKey(username, password, KeyIterationCount);
        }
    }
}
