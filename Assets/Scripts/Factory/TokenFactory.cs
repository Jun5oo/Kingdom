using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CardData를 바탕으로 Token 오브젝트를 비동기 생성하는 팩토리.
/// King이면 BarStatusPresenter, 일반 유닛이면 NumberStatusPresenter를 사용하여 상태 UI를 구성한다.
/// </summary>
public class TokenFactory
{
    GameObject tokenPrefab;

    TokenTextureLoader textureLoader;
    PrefabLoader prefabLoader;

    /// <summary> 토큰 프리팹을 Addressables로 비동기 로드하여 캐시한다. </summary>
    public async UniTask Init()
    {
        textureLoader = ServiceLocator.Get<TokenTextureLoader>();
        prefabLoader = ServiceLocator.Get<PrefabLoader>();

        tokenPrefab = await prefabLoader.LoadPrefabAsync<Token>();
    }

    /// <summary>
    /// CardData로 Token 오브젝트를 생성한다.
    /// 텍스처와 StatusPresenter를 비동기 로드·생성하고 TokenView를 초기화한다.
    /// </summary>
    public async UniTask<Token> CreateToken(CardData cardData, int playerID, CardData sourceObject = null, List<CardData> sourceObjects = null, int spawnLevel = 1)
    {
        GameObject prefab = tokenPrefab; 

        GameObject tokenObject = GameObject.Instantiate(prefab);
        tokenObject.SetActive(false);

        if (tokenObject.TryGetComponent<Token>(out Token token))
            token.Init(cardData, playerID, sourceObject, sourceObjects, spawnLevel);

        VisualTexture textures = await textureLoader.LoadAllTextures(cardData);

        GameObject statusPrefab = (cardData.Tag != UnitTag.King) ? await prefabLoader.LoadPrefabAsync<NumberStatusPresenter>() : await prefabLoader.LoadPrefabAsync<BarStatusPresenter>(); 

        if(statusPrefab == null)
        {
            Debug.LogError($"{statusPrefab}을 찾을 수 없습니다.");
            return null; 
        }

        if (!token.TryGetComponent<TokenView>(out TokenView tokenView))
        {
            Debug.LogError($"{tokenView}를 찾을 수 없습니다.");
            return null;
        }

        GameObject statusInstance = GameObject.Instantiate(statusPrefab);

        if (!statusInstance.TryGetComponent<StatusPresenter>(out StatusPresenter presenter))
        {
            Debug.LogError($"{presenter}를 찾을 수 없습니다.");
            return null;
        }

        statusInstance.transform.SetParent(tokenView.Canvas.transform, false);
        tokenView.Init(textures, presenter, cardData.CP[token.Level - 1], cardData.MoveRange[token.Level - 1]);

        tokenObject.SetActive(true); 
        
        return token; 
    }
}
