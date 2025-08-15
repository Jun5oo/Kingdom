using TMPro;
using UnityEngine;

public class ActionCoinDisplayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    ActionResourceSystem actionSystem;

    [SerializeField] bool IsLocal;

    int playerID = -1;

    void Start()
    {
        PlayerManager playerManager = ServiceLocator.Get<PlayerManager>();
        actionSystem = ServiceLocator.Get<ActionResourceSystem>();

        if (IsLocal)
            playerID = playerManager.Local.PlayerID;
        else
            playerID = playerManager.Remote.PlayerID;

    }

    private void Update()
    {
        if (actionSystem != null)
            text.text = actionSystem.GetCurrentResources(playerID).ToString();
    }
}
