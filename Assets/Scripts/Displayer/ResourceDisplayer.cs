using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 어빌리티 코인 개수를 슬롯 이미지의 활성/비활성으로 표시하는 컴포넌트.
/// 종족별 코인 스프라이트를 Addressables에서 비동기 로드하고,
/// AbilityResourceSystem.onAbilityCountChanged 이벤트를 구독하여 실시간으로 갱신한다.
/// </summary>
public class ResourceDisplayer : MonoBehaviour
{
    const int MAX_COIN = 3;

    [SerializeField] Transform localLayout;
    [SerializeField] Transform remoteLayout;
    [SerializeField] GameObject slotPrefab;

    Dictionary<Race, Sprite> abilitySpriteDictionary; // 종족 → 코인 스프라이트

    AbilityResourceSystem abilityResourceSystem;
    PlayerManager playerManager;
    SpriteLoader spriteLoader;

    List<Image> localSlots;  // 로컬 플레이어 코인 슬롯 이미지 목록
    List<Image> remoteSlots; // 원격 플레이어 코인 슬롯 이미지 목록

    void Start()
    {
        abilityResourceSystem = ServiceLocator.Get<AbilityResourceSystem>();
        abilityResourceSystem.onAbilityCountChanged += OnUpdateLayout; 

        playerManager = ServiceLocator.Get<PlayerManager>();    

        spriteLoader = ServiceLocator.Get<SpriteLoader>();

        abilitySpriteDictionary = new Dictionary<Race, Sprite>();
        localSlots = new List<Image>(); 
        remoteSlots = new List<Image>();

        InitAsync().Forget(); 
    }

    /// <summary> 종족별 코인 스프라이트를 비동기 로드하고 슬롯을 MAX_COIN 수만큼 사전 생성한다. </summary>
    async UniTask InitAsync()
    {
        Race localRace = playerManager.Local.Race;
        Race remoteRace = playerManager.Remote.Race;

        if (spriteLoader == null)
        {
            Debug.LogError($"{this}: SpriteLoader를 찾을 수 없습니다.");
            return;
        }

        Sprite localCoin = await spriteLoader.LoadSpriteAsync($"coin_{localRace.ToString().ToLower()}");

        if (!abilitySpriteDictionary.ContainsKey(localRace))
            abilitySpriteDictionary[localRace] = localCoin;

        if (remoteRace != localRace)
        {
            Sprite remoteCoin = await spriteLoader.LoadSpriteAsync($"coin_{remoteRace.ToString().ToLower()}");
            if (!abilitySpriteDictionary.ContainsKey(remoteRace))
                abilitySpriteDictionary[remoteRace] = remoteCoin;
        }

        PreWarm(localLayout, localSlots, localRace);
        PreWarm(remoteLayout, remoteSlots, remoteRace);
    }

    /// <summary> 기존 자식 오브젝트를 정리하고 MAX_COIN 개의 슬롯을 비활성 상태로 생성한다. </summary>
    public void PreWarm(Transform parent, List<Image> slots, Race race)
    {
        int count = parent.childCount;

        for (int i = 0; i < count; i++)
            Destroy(parent.GetChild(i).gameObject);

        slots.Clear();

        for (int i = 0; i < MAX_COIN; i++)
        {
            var slot = Instantiate(slotPrefab, parent);
            if (!slot.TryGetComponent<Image>(out Image img))
            {
                Debug.LogError($"{slotPrefab}에서 Image Component를 찾을 수 없습니다.");
                return;
            }

            img.sprite = abilitySpriteDictionary[race];
            img.gameObject.SetActive(false);
            slots.Add(img);
        }
    }

    /// <summary>
    /// 어빌리티 코인 수가 변경될 때 호출된다.
    /// resourceCount만큼 슬롯을 활성화하고 나머지는 비활성화한다.
    /// </summary>
    public void OnUpdateLayout(int playerID, int resourceCount)
    {
        bool isLocal = playerManager.Local.PlayerID == playerID;

        resourceCount = Mathf.Clamp(resourceCount, 0, MAX_COIN);

        if (isLocal)
        {
            for (int i = 0; i < MAX_COIN; i++)
            {
                if (resourceCount > i)
                    localSlots[i].gameObject.SetActive(true);
                else
                    localSlots[i].gameObject.SetActive(false);
            }
        }

        else
        {
            for (int i = 0; i < MAX_COIN; i++)
            {
                if (resourceCount > i)
                    remoteSlots[i].gameObject.SetActive(true);
                else
                    remoteSlots[i].gameObject.SetActive(false);
            }
        }
    }
}
