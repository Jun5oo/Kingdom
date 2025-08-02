using Cysharp.Threading.Tasks;
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
    public async UniTask<Token> CreateToken(UnitCardData unitData, int playerID)
    {
        GameObject prefab = tokenPrefab; 
        prefab.SetActive(false); 

        if(prefab == null)
            Debug.Log("No prefab found"); 

        GameObject tokenObject = GameObject.Instantiate(prefab);

        if (tokenObject == null)
            Debug.Log("Cannot Instantiate TokenPrefab");

        if (tokenObject.TryGetComponent<Token>(out Token token))
            token.Init(unitData, playerID);

        if (token == null)
            Debug.Log("Cannot found Token Component in TokenPrefab"); 

        VisualTexture textures = await textureLoader.LoadAllTextures(unitData);
        Debug.Log("TokenTextureLoad Complete");
        
        if (token.TryGetComponent<TokenView>(out TokenView tokenView))
            tokenView.Init(textures, unitData.CP, unitData.Movement);

        if (tokenView == null)
            Debug.Log("Cannot found TokenView Component in TokenPrefab");

        tokenObject.SetActive(true); 
        return token; 
    }
}
