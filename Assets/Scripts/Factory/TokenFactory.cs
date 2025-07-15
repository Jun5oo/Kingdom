using UnityEngine;

public class TokenFactory
{
    GameObject tokenPrefab;
    GameObject kingTokenPrefab;
    
    public void Init(GameObject tokenPrefab, GameObject kingTokenPrefab)
    {
        this.tokenPrefab = tokenPrefab;
        this.kingTokenPrefab = kingTokenPrefab;
    }

    public Token CreateToken(UnitCardData unitData, int playerID)
    {
        GameObject prefab = tokenPrefab;

        if (unitData.IsKing)
            prefab = kingTokenPrefab;

        GameObject tokenObject = GameObject.Instantiate(prefab); 
        Token token = tokenObject.GetComponent<Token>();
        
        token.Init(unitData, playerID); 
        
        return token; 
    }
}
