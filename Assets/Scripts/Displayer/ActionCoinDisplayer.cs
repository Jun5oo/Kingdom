using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionCoinDisplayer : MonoBehaviour
{
    [SerializeField] List<GameObject> actionCoins;
    List<Renderer> renderers; 

    [SerializeField] Sprite depleted;
    [SerializeField] Sprite filled; 

    TurnSystem turnSystem; 
    ActionResourceSystem actionResourceSystem;

    void OnDisable()
    {
        turnSystem.onTurnStarted -= OnUpdate;
        actionResourceSystem.onActionResourceChanged -= OnUpdate;
    }

    void Start()
    {
        turnSystem = ServiceLocator.Get<TurnSystem>(); 
        actionResourceSystem = ServiceLocator.Get<ActionResourceSystem>();

        renderers = new List<Renderer>();
        foreach(var gameObject in actionCoins)
        {
            var r = gameObject.GetComponent<Renderer>();
            renderers.Add(r); 
        }

        turnSystem.onTurnStarted -= OnUpdate; 
        turnSystem.onTurnStarted += OnUpdate;

        actionResourceSystem.onActionResourceChanged -= OnUpdate;
        actionResourceSystem.onActionResourceChanged += OnUpdate;
    }

    void OnUpdate(int currentPlayerID)
    {
        int count = actionResourceSystem.GetCurrentResources(currentPlayerID);

        if(count >= renderers.Count)
        {
            foreach (var renderer in renderers)
                renderer.material.SetTexture("_BaseMap", filled.texture); 
        }

        else 
            renderers[count].material.SetTexture("_BaseMap", depleted.texture);
    }
}
