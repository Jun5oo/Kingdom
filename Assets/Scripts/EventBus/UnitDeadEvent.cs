public struct UnitDeadEvent : IGameEvent
{
    public ObjectContext killer;
    public ObjectContext victim; 
}
