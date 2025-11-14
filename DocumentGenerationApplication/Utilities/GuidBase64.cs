namespace DocumentGenerationApplication.Utilities
{
    using System;

    public static class GuidBase64
    {
        // returns 22-char base64url string (no padding)
        public static string NewId()
        {
            var guid = Guid.NewGuid();
            // 16 bytes
            string b64 = Convert.ToBase64String(guid.ToByteArray()); // 24 chars with '==' padding
                                                                     // convert to base64url and remove padding
            b64 = b64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            return b64; // 22 characters typically
        }
    }

}
