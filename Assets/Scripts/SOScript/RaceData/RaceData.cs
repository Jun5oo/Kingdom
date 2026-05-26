using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 진영(Race) UI 표시용 ScriptableObject. Title 씬 진영 선택 화면에서 이름과 이미지를 제공한다.
/// </summary>
[CreateAssetMenu(menuName = "Game/RaceData")]
public class RaceData : ScriptableObject
{
    public Race raceType;
    public string raceName;
    public Texture2D raceImage; 
}
