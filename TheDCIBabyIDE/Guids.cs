// Guids.cs
// MUST match guids.h
using System;

namespace KimHaiQuang.TheDCIBabyIDE
{
    static class GuidList
    {
        public const string guidTheDCIBabyIDEPkgString = "3b8df1da-51e0-4a27-b805-6891aaaf805c";
        public const string guidTheDCIBabyIDECmdSetString = "1a55555a-d358-43e2-84bb-9c7c8a602f8a";
        public const string guidToolWindowPersistanceString = "7df8029b-658a-4eb8-81ce-fa45d0dd1def";

        public static readonly Guid guidTheDCIBabyIDECmdSet = new Guid(guidTheDCIBabyIDECmdSetString);
    };
}