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
    public async UniTask<Token> CreateToken(UnitCardData unitData, int playerID, CardData sourceObject = null, List<UnitCardData> sourceObjects = null)
    {
        GameObject prefab = tokenPrefab; 
        prefab.SetActive(false); 

        GameObject tokenObject = GameObject.Instantiate(prefab);

        if (tokenObject.TryGetComponent<Token>(out Token token))
            token.Init(unitData, playerID, sourceObject, sourceObjects);

        VisualTexture textures = await textureLoader.LoadAllTextures(unitData);
        
        if (token.TryGetComponent<TokenView>(out TokenView tokenView))
            tokenView.Init(textures, unitData.GetCP(unitData.Level), unitData.GetMovement(unitData.Level));

        if (tokenView == null)
            Debug.Log("TokenView 컴포넌트를 찾을 수 없습니다.");

        tokenObject.SetActive(true); 
        return token; 
    }
}
