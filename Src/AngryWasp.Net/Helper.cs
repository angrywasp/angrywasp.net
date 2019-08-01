using System;
using AngryWasp.Logger;

namespace AngryWasp.Net
{
    public static class Helper
    {
        public static ulong GenerateRandomID()
        {
            uint a = (uint)(Math.Abs(Guid.NewGuid().GetHashCode()));
            uint b = (uint)(Math.Abs(Guid.NewGuid().GetHashCode()));

            ulong result = ((ulong)a << 32) + b;
            Log.Instance.Write($"Random ID - {result}");
            return result;
        }
    }
}