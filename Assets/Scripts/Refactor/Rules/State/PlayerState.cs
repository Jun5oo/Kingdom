namespace Refactor
{
    public sealed class PlayerState
    {
        public int Id { get; }
        public int ActionResource { get; internal set; }
        public int AbilityResource { get; internal set; }

        public PlayerState(int id)
        {
            Id = id;
        }
    }
}
