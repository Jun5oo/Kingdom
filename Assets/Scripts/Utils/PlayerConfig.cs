using UnityEngine;

[CreateAssetMenu(menuName = "Game/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    // 봇전 전용 

    public Race playerSelected;
    public Race botSelected; 
}
