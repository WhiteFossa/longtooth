using System.Collections.Generic;

namespace longtooth.Common.Implementations.Extensions
{
    /// <summary>
    /// Common extensions
    /// </summary>
    public static class CommonExtensions
    {
        public static int FindFirstSubarray(this List<byte> haystack, List<byte> needle)
        {
            for (var haystackIndex = 0; haystackIndex <= haystack.Count - needle.Count; haystackIndex ++)
            {
                var found = true;
                for (var needleIndex = 0; needleIndex < needle.Count; needleIndex ++)
                {
                    if (haystack[haystackIndex + needleIndex] != needle[needleIndex])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    return haystackIndex;
                }
            }

            return -1;
        }
    }
}
