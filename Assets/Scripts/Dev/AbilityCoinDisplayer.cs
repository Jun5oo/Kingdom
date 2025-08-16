using TMPro;
using UnityEngine;

public class AbilityCoinDisplayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    AbilityResourceSystem abilitySystem;

    [SerializeField] bool IsLocal;

    int playerID = -1; 

    void Start()
    {
        PlayerManager playerManager = ServiceLocator.Get<PlayerManager>();    
        abilitySystem = ServiceLocator.Get<AbilityResourceSystem>();

        if (IsLocal)
            playerID = playerManager.Local.PlayerID; 
        else 
            playerID = playerManager.Remote.PlayerID;

    }

    private void Update()
    {
        if (abilitySystem != null)
            text.text = abilitySystem.GetCurrentResources(playerID).ToString(); 
    }

}
