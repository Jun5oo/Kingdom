using System;

namespace Refactor
{
    [Flags]
    public enum EventReason
    {
        None = 0,
        Battle = 1 << 0,
        Effect = 1 << 1,
        Rule = 1 << 2,
        Destroy = 1 << 3
    }
}
