[System.Serializable]
public class EffectData
{
    public EffectType effectType;
    public int groupID;
    public string abilityName;
    public string description; 

    public Trigger trigger;
    public Target target;

    public int cost;
    public int value;

    public string position;
    public string reward; 

}
