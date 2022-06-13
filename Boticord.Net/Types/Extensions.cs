using System.Runtime.CompilerServices;
using Boticord.Net.Types.Enums;

namespace Boticord.Net.Types
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool HasBadge(this GuildBadges badges, GuildBadges badge)
        {
            return ((byte) badges & (byte) badge) != 0;
        }
    }
}
