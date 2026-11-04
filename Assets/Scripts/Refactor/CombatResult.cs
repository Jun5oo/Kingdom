namespace Refactor
{
    public sealed class CombatResult
    {
        public int AttackerId { get; }
        public int DefenderId { get; }
        public int Damage { get; }
        public int RemainingHP { get; }
        public bool DefenderDied => RemainingHP == 0;

        public CombatResult(int attackerId, int defenderId, int damage, int remainingHP)
        {
            AttackerId = attackerId;
            DefenderId = defenderId;
            Damage = damage;
            RemainingHP = remainingHP;
        }
    }
}
