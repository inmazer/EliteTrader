using System;

namespace EliteTrader.Helpers
{
    public static class GuidHelper
    {
        public static Guid ReverseByteSequences(this Guid guid)
        {
            string raw = guid.ToString("N");
            char[] aRaw = raw.ToCharArray();

            //compressed format reverses 11 byte sequences of the original guid
            int[] revs = new[] { 8, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2 };
            int pos = 0;
            for (int i = 0; i < revs.Length; i++)
            {
                Array.Reverse(aRaw, pos, revs[i]);
                pos += revs[i];
            }
            return new Guid(new string(aRaw));
        }
    }
}
