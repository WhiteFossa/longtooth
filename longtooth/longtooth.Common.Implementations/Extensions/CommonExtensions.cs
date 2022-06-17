using System.Collections.Generic;
using System.Linq;

namespace longtooth.Common.Implementations.Extensions
{
    /// <summary>
    /// Common extensions
    /// </summary>
    public static class CommonExtensions
    {
        public static int FindFirstSubarray(this IReadOnlyCollection<byte> haystack, IReadOnlyCollection<byte> needle)
        {
            for (var haystackIndex = 0; haystackIndex <= haystack.Count - needle.Count; haystackIndex ++)
            {
                var found = true;
                for (var needleIndex = 0; needleIndex < needle.Count; needleIndex ++)
                {
                    if (haystack.ElementAt(haystackIndex + needleIndex) != needle.ElementAt(needleIndex))
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
