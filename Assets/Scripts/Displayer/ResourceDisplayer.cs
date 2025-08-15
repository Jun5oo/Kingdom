using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplayer : MonoBehaviour
{
    const int MAX_COIN = 3; 
    
    [SerializeField] Transform localLayout;
    [SerializeField] Transform remoteLayout;
    [SerializeField] GameObject slotPrefab;

    Dictionary<Race, Sprite> abilitySpriteDictionary; 

    AbilityResourceSystem abilityResourceSystem;
    PlayerManager playerManager;
    SpriteLoader spriteLoader;

    List<Image> localSlots;
    List<Image> remoteSlots;

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
        
        if(!abilitySpriteDictionary.ContainsKey(localRace)) 
            abilitySpriteDictionary[localRace] = localCoin; 

        if(remoteRace != localRace)
        {
            Sprite remoteCoin = await spriteLoader.LoadSpriteAsync($"coin_{remoteRace.ToString().ToLower()}");
            if (!abilitySpriteDictionary.ContainsKey(remoteRace))
                abilitySpriteDictionary[remoteRace] = remoteCoin; 
        }

        PreWarm(localLayout, localSlots, localRace); 
        PreWarm(remoteLayout, remoteSlots, remoteRace);
    }

    public void PreWarm(Transform parent, List<Image> slots, Race race)
    {
        int count = parent.childCount; 

        for(int i=0; i<count; i++)
            Destroy(parent.GetChild(i).gameObject);
        
        slots.Clear(); 

        for(int i=0; i<MAX_COIN; i++)
        {
            var slot = Instantiate(slotPrefab, parent); 
            if(!slot.TryGetComponent<Image>(out Image img))
            {
                Debug.LogError($"{slotPrefab}에서 Image Component를 찾을 수 없습니다.");
                return; 
            }

            img.sprite = abilitySpriteDictionary[race];
            img.gameObject.SetActive(false);
            slots.Add(img); 
        }
    }

    public void OnUpdateLayout(int playerID, int resourceCount)
    {
        bool isLocal = playerManager.Local.PlayerID == playerID;

        resourceCount = Mathf.Clamp(resourceCount, 0, MAX_COIN);

        if (isLocal)
        {
            for (int i=0; i<MAX_COIN; i++)
            {
                if (resourceCount > i)
                    localSlots[i].gameObject.SetActive(true);
                else
                    localSlots[i].gameObject.SetActive(false);
            }
        }

        else
        {
            for (int i=0; i<MAX_COIN; i++)
            {
                if (resourceCount > i)
                    remoteSlots[i].gameObject.SetActive(true);
                else
                    remoteSlots[i].gameObject.SetActive(false);
            }
        }
    }
}
