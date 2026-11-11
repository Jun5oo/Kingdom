using System;
using System.Collections.Generic;

namespace Refactor
{
    // Submit의 결과. 실패하면 Reason에 사유가 있고 Events는 비어 있다.
    public sealed class SubmitResult
    {
        public bool Succeeded { get; }
        public string Reason { get; }
        public IReadOnlyList<GameEvent> Events { get; }

        private SubmitResult(bool succeeded, string reason, IReadOnlyList<GameEvent> events)
        {
            Succeeded = succeeded;
            Reason = reason;
            Events = events;
        }

        public static SubmitResult Ok(IReadOnlyList<GameEvent> events) => new(true, string.Empty, events);
        public static SubmitResult Fail(string reason) => new(false, reason, Array.Empty<GameEvent>());
    }
}
