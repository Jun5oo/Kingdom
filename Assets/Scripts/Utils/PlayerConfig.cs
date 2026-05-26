using UnityEngine;

/// <summary>
/// 봇전 진영 선택 정보를 저장하는 ScriptableObject.
/// Title 씬에서 플레이어/봇의 Race를 설정하고 GamePlay 씬으로 전달한다.
/// </summary>
[CreateAssetMenu(menuName = "Game/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    // 봇전 전용 

    public Race playerSelected;
    public Race botSelected; 
}
