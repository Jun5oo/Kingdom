using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 행동 포인트를 코인 오브젝트의 머티리얼 텍스처(filled/depleted)로 표시하는 컴포넌트.
/// TurnSystem.onTurnStarted와 ActionResourceSystem.onActionResourceChanged를 구독하여 갱신한다.
/// </summary>
public class ActionCoinDisplayer : MonoBehaviour
{
    [SerializeField] List<GameObject> actionCoins; // 코인 오브젝트 목록 (인덱스 = AP 수)
    List<Renderer> renderers;

    [SerializeField] Sprite depleted; // 소진된 코인 텍스처
    [SerializeField] Sprite filled;   // 충전된 코인 텍스처

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
        foreach (var gameObject in actionCoins)
        {
            var r = gameObject.GetComponent<Renderer>();
            renderers.Add(r);
        }

        turnSystem.onTurnStarted -= OnUpdate;
        turnSystem.onTurnStarted += OnUpdate;

        actionResourceSystem.onActionResourceChanged -= OnUpdate;
        actionResourceSystem.onActionResourceChanged += OnUpdate;
    }

    /// <summary>
    /// 현재 AP에 따라 코인 머티리얼을 갱신한다.
    /// count 이상이면 전부 filled, 미만이면 count 인덱스의 코인을 depleted로 전환한다.
    /// </summary>
    void OnUpdate(int currentPlayerID)
    {
        int count = actionResourceSystem.GetCurrentResources(currentPlayerID);

        if (count >= renderers.Count)
        {
            foreach (var renderer in renderers)
                renderer.material.SetTexture("_BaseMap", filled.texture);
        }

        else
            renderers[count].material.SetTexture("_BaseMap", depleted.texture);
    }
}
