using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class TokenFactory
{
    GameObject tokenPrefab;

    TokenTextureLoader textureLoader;
    PrefabLoader prefabLoader;

    AbilityFactory abilityFactory;

    public async UniTask Init()
    {
        textureLoader = ServiceLocator.Get<TokenTextureLoader>();
        prefabLoader = ServiceLocator.Get<PrefabLoader>();

        abilityFactory = new AbilityFactory(); 

        tokenPrefab = await prefabLoader.LoadPrefabAsync<Token>(); 
    }
    public async UniTask<Token> CreateToken(CardData cardData, int playerID, CardData sourceObject = null, List<CardData> sourceObjects = null, int spawnLevel = 1)
    {
        // Load한 프리팹을 변수에 등록 
        GameObject prefab = tokenPrefab; 

        // 게임 오브젝트 인스턴스화 
        GameObject tokenObject = GameObject.Instantiate(prefab);
        tokenObject.SetActive(false);
        tokenObject.name = cardData.name; 

        // Token Component가 있는지 확인 후 초기화 
        if (tokenObject.TryGetComponent<Token>(out Token token))
        {
            List<Ability> abilities = abilityFactory.CreateAbilityAsync(cardData, token); 
            
            token.Init(cardData, playerID, sourceObject, sourceObjects, spawnLevel, abilities);
        }

        // 토큰 이미지 Load 
        VisualTexture textures = await textureLoader.LoadAllTextures(cardData);
        // 토큰 별 Status 프리팹 로드 
        GameObject statusPrefab = (cardData.Tag != UnitTag.King) ? await prefabLoader.LoadPrefabAsync<NumberStatusPresenter>() : await prefabLoader.LoadPrefabAsync<BarStatusPresenter>(); 

        if(statusPrefab == null)
        {
            Debug.LogError($"{statusPrefab}을 찾을 수 없습니다.");
            return null; 
        }

        // View component 있는지 확인 
        if (!token.TryGetComponent<TokenView>(out TokenView tokenView))
        {
            Debug.LogError($"{tokenView}를 찾을 수 없습니다.");
            return null;
        }
        // Load한 Status 오브젝트 확인 
        GameObject statusInstance = GameObject.Instantiate(statusPrefab);

        if (!statusInstance.TryGetComponent<StatusPresenter>(out StatusPresenter presenter))
        {
            Debug.LogError($"{presenter}를 찾을 수 없습니다.");
            return null;
        }

        // View Component의 WorldCanvas에 Status 붙이기 
        statusInstance.transform.SetParent(tokenView.Canvas.transform, false);
        tokenView.Init(textures, presenter, cardData.CP[token.Level - 1], cardData.MoveRange[token.Level - 1]);

        // 게임 오브젝트 활성화 후 리턴 
        tokenObject.SetActive(true); 
        return token; 
    }
}
