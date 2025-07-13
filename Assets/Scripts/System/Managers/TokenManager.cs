using System.Collections.Generic;
using UnityEngine;

public class TokenManager
{
    PlayerManager playerManager; 

    Dictionary<Vector2Int, Token> GridToToken;
    Dictionary<Token, Vector2Int> TokenToGrid; 

    Dictionary<int, Token> PlayerIDToKingToken; 

    public void Init(PlayerManager playerManager)
    {
        this.playerManager = playerManager;

        GridToToken = new Dictionary<Vector2Int, Token>();
        TokenToGrid = new Dictionary<Token, Vector2Int>();
        PlayerIDToKingToken = new Dictionary<int, Token>();
    }

    public void PlaceTokenTo(Token token, Vector2Int to)
    {
        AddToken(to, token);
    }
    public void MoveTokenTo(Token token, Vector2Int to)
    {
        RemoveToken(token);
        PlaceTokenTo(token, to); 
    }
    public void AddToken(Vector2Int gridPosition, Token token)
    {
        GridToToken[gridPosition] = token;
        TokenToGrid[token] = gridPosition; 
    }
    public void AddKingToken(int playerID, Token token)
    {
        PlayerIDToKingToken[playerID] = token; 
    }
    public void RemoveToken(Token token)
    {
        Vector2Int gridPosition = TokenToGrid[token]; 
        GridToToken.Remove(gridPosition);
        TokenToGrid.Remove(token);
    }
    public Token GetTokenFrom(Vector2Int gridPosition)
    {
        if (GridToToken.TryGetValue(gridPosition, out Token target))
        {
            return target;
        }

        return null;
    }
    public Vector2Int GetGridPositionOfToken(Token token)
    {
        if(TokenToGrid.TryGetValue(token, out Vector2Int gridPosition))
            return gridPosition;

        return -Vector2Int.one; 
    }
    public bool IsTokenAtGridPosition(Vector2Int gridPosition)
    {
        if(GridToToken.TryGetValue(gridPosition, out Token token))
            return true;

        return false; 
    }
    public bool TryGetTokenFrom(Vector2Int gridPosition, out Token token)
    {
        if(GridToToken.TryGetValue(gridPosition, out token))
            return true;
        return false; 
    }
    public bool TryGetKingTokenFrom(int playerID, out Token token)
    {
        if(PlayerIDToKingToken.TryGetValue(playerID, out token))
        {
            return true; 
        }

        return false; 
    }
    public bool IsMyToken(Token token)
    {
        return token.OwnerPlayerID == playerManager.LocalPlayerData.PlayerID;
    }
    public void DestroyToken(Token token)
    {
        RemoveToken(token);
        GameObject.Destroy(token.gameObject); 
    }
}
