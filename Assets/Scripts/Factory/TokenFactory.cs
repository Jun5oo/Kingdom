using Cysharp.Threading.Tasks;
using UnityEngine;

public class TokenFactory
{
    GameObject tokenPrefab;
    GameObject kingTokenPrefab;
    TextureLoader textureLoader; 


    public void Init(GameObject tokenPrefab, GameObject kingTokenPrefab)
    {
        this.tokenPrefab = tokenPrefab;
        this.kingTokenPrefab = kingTokenPrefab;

        textureLoader = ServiceLocator.Get<TextureLoader>(); 
    }
    public async UniTask<Token> CreateToken(UnitCardData unitData, int playerID)
    {
        GameObject prefab = tokenPrefab;

        if (unitData.IsKing)
            prefab = kingTokenPrefab;

        if(prefab == null)
            Debug.Log("No prefab found"); 

        GameObject tokenObject = GameObject.Instantiate(prefab); 
        
        if (tokenObject.TryGetComponent<Token>(out Token token))
            token.Init(unitData, playerID);

        if(tokenObject == null)
            Debug.Log("tokenObject == null");

        if (token == null)
            Debug.Log("token not found"); 

        Debug.Log("try loading all textures"); 
        VisualTexture textures = await textureLoader.LoadAllTextures(unitData);
        Debug.Log("all texture loading complete");

        if (token.TryGetComponent<TokenView>(out TokenView tokenView))
            tokenView.Init(textures, unitData.CP, unitData.Movement);

        return token; 
    }
}
