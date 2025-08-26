using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Game/RaceData")]
public class RaceData : ScriptableObject
{
    public Race raceType;
    public string raceName;
    public Texture2D raceImage; 
}
