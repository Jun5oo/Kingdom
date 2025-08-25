using UnityEngine;
using UnityEngine.SceneManagement;

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
