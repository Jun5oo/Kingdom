using System.Collections.Generic;
using UnityEngine;

public class TokenManager
{
    PlayerManager playerManager; 

    Dictionary<Vector2Int, Token> gridToken;
    Dictionary<Token, Vector2Int> tokenGrid;

    Dictionary<int, List<Token>> playerToken;
    Dictionary<int, Token> playerKingToken;

    public void Init()
    {
        this.playerManager = ServiceLocator.Get<PlayerManager>();

        gridToken = new Dictionary<Vector2Int, Token>();
        tokenGrid = new Dictionary<Token, Vector2Int>();

        playerToken = new Dictionary<int, List<Token>>();   
        playerKingToken = new Dictionary<int, Token>();
    }

    public void PlaceTokenTo(Token token, Vector2Int to)
    {
        if (IsTokenAtGridPosition(to))
            Debug.LogError("해당 위치에는 토큰이 존재합니다."); 

        AddToken(to, token);
    }
    public void MoveTokenTo(Token token, Vector2Int to)
    {
        RemoveToken(token);
        PlaceTokenTo(token, to); 
    }
    public void AddToken(Vector2Int gridPosition, Token token)
    {
        gridToken[gridPosition] = token;
        tokenGrid[token] = gridPosition;

        if (!playerToken.ContainsKey(token.OwnerID))
            playerToken[token.OwnerID] = new List<Token>();
        
        playerToken[token.OwnerID].Add(token);
    }
    public void AddKingToken(int playerID, Token token)
    {
        playerKingToken[playerID] = token; 
    }
    public void RemoveToken(Token token)
    {
        Vector2Int gridPosition = tokenGrid[token]; 
        gridToken.Remove(gridPosition);
        tokenGrid.Remove(token);
        playerToken[token.OwnerID].Remove(token);
    }
    public void DestroyToken(Token token)
    {
        GameObject.Destroy(token.gameObject);
        RemoveToken(token);
    }
    public Token GetTokenFrom(Vector2Int gridPosition)
    {
        if (gridToken.TryGetValue(gridPosition, out Token target))
        {
            return target;
        }

        return null;
    }
    public Vector2Int GetGridPositionOfToken(Token token)
    {
        if(tokenGrid.TryGetValue(token, out Vector2Int gridPosition))
            return gridPosition;

        return -Vector2Int.one; 
    }
    public bool IsTokenAtGridPosition(Vector2Int gridPosition)
    {
        if(gridToken.TryGetValue(gridPosition, out Token token))
            return true;

        return false; 
    }
    public bool TryGetTokenFrom(Vector2Int gridPosition, out Token token)
    {
        if(gridToken.TryGetValue(gridPosition, out token))
            return true;
        return false; 
    }
    public bool TryGetKingTokenFrom(int playerID, out Token token)
    {
        if(playerKingToken.TryGetValue(playerID, out token))
        {
            return true; 
        }

        return false; 
    }
    public List<Token> GetPlayerToken(int playerID) => playerToken[playerID]; 
}
