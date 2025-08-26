using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class TokenFactory
{
    GameObject tokenPrefab;

    TokenTextureLoader textureLoader;
    PrefabLoader prefabLoader; 

    public async UniTask Init()
    {
        textureLoader = ServiceLocator.Get<TokenTextureLoader>();
        prefabLoader = ServiceLocator.Get<PrefabLoader>();

        tokenPrefab = await prefabLoader.LoadPrefabAsync<Token>(); 
    }
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
