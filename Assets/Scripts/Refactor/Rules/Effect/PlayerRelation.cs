using System;

namespace Refactor
{
    // 효과 주인을 기준으로 본 플레이어.
    public enum PlayerRelation
    {
        Any,
        Self,
        Opponent,
    }

    internal static class PlayerRelationExtensions
    {
        // playerId가 효과 주인(ownerId) 기준으로 relation에 해당하는지.
        public static bool Matches(this PlayerRelation relation, int? playerId, int ownerId) => relation switch
        {
            PlayerRelation.Any => true,
            PlayerRelation.Self => playerId == ownerId,
            PlayerRelation.Opponent => playerId.HasValue && playerId != ownerId,
            _ => throw new ArgumentOutOfRangeException(nameof(relation)),
        };
    }
}
