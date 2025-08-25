[System.Serializable]
public class EffectData
{
    public EffectType effectType;
    public int groupID;
    public string abilityName;
    public string description; 

    public Trigger trigger;
    public Target target;

    public int value;
    public int cost;

    public string parameter;
}
