using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 시작 버튼 핸들러. LoadScene() 호출 시 PlayerConfig에 선택된 진영을 저장하고 GamePlay 씬을 로드한다.
/// </summary>
public class GameStart : MonoBehaviour
{
    const string GAMEPLAY_SCENE = "GamePlay";

    [SerializeField] PlayerConfig config;

    [SerializeField] CharacterSelectionController playerController;
    [SerializeField] CharacterSelectionController botController;  

    public void LoadScene()
    {
        config.playerSelected = playerController.GetCurrentRaceData();
        config.botSelected = botController.GetCurrentRaceData(); 

        SceneManager.LoadScene(GAMEPLAY_SCENE); 
    }
}
