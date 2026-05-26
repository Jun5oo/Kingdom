using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

/// <summary>
/// 필드 위 모든 토큰의 위치와 소유권을 추적하는 매니저.
/// 그리드 좌표 ↔ 토큰 양방향 조회, 플레이어별 토큰 목록, 왕 토큰 별도 관리를 담당한다.
/// </summary>
public class TokenManager
{
    PlayerManager playerManager;

    Dictionary<Vector2Int, Token> gridToken;  // 그리드 위치 → 토큰
    Dictionary<Token, Vector2Int> tokenGrid;  // 토큰 → 그리드 위치

    Dictionary<int, List<Token>> playerToken; // 플레이어 ID → 토큰 목록
    Dictionary<int, Token> playerKingToken;   // 플레이어 ID → 왕 토큰

    // GetTokens()에서 사용하는 로컬/AI 토큰 목록 (playerToken과 병렬 관리됨)
    List<Token> playerTokens = new List<Token>();
    List<Token> aiTokens = new List<Token>();

    public void Init()
    {
        this.playerManager = ServiceLocator.Get<PlayerManager>();

        gridToken = new Dictionary<Vector2Int, Token>();
        tokenGrid = new Dictionary<Token, Vector2Int>();

        playerToken = new Dictionary<int, List<Token>>();
        playerKingToken = new Dictionary<int, Token>();
    }

    /// <summary> 토큰을 지정 위치에 최초 배치한다. 이미 토큰이 있으면 오류를 출력한다. </summary>
    public void PlaceTokenTo(Token token, Vector2Int to)
    {
        if (IsTokenAtGridPosition(to))
            Debug.LogError("해당 위치에는 토큰이 존재합니다.");

        AddToken(to, token);
    }

    /// <summary> 토큰을 기존 위치에서 제거하고 새 위치에 배치한다. </summary>
    public void MoveTokenTo(Token token, Vector2Int to)
    {
        RemoveToken(token);
        PlaceTokenTo(token, to);
    }

    /// <summary> 토큰을 모든 딕셔너리에 등록한다. </summary>
    public void AddToken(Vector2Int gridPosition, Token token)
    {
        gridToken[gridPosition] = token;
        tokenGrid[token] = gridPosition;
        List<Token> tokens = GetTokens(token.OwnerID);
        tokens.Add(token);

        if (!playerToken.ContainsKey(token.OwnerID))
            playerToken[token.OwnerID] = new List<Token>();

        playerToken[token.OwnerID].Add(token);
    }

    /// <summary> 왕 토큰을 별도 딕셔너리에 등록한다. </summary>
    public void AddKingToken(int playerID, Token token)
    {
        playerKingToken[playerID] = token;
    }

    /// <summary> 토큰을 모든 딕셔너리에서 제거한다. </summary>
    public void RemoveToken(Token token)
    {
        Vector2Int gridPosition = tokenGrid[token];
        gridToken.Remove(gridPosition);
        tokenGrid.Remove(token);
        List<Token> tokens = GetTokens(token.OwnerID);
        tokens.Remove(token);
        playerToken[token.OwnerID].Remove(token);
    }

    /// <summary> 토큰 게임 오브젝트를 파괴하고 등록 정보를 제거한다. </summary>
    public void DestroyToken(Token token)
    {
        GameObject.Destroy(token.gameObject);
        RemoveToken(token);
    }

    /// <summary> 그리드 위치로 토큰을 조회한다. 없으면 null을 반환한다. </summary>
    public Token GetTokenFrom(Vector2Int gridPosition)
    {
        if (gridToken.TryGetValue(gridPosition, out Token target))
            return target;

        return null;
    }

    /// <summary> 토큰의 현재 그리드 위치를 반환한다. 없으면 -Vector2Int.one을 반환한다. </summary>
    public Vector2Int GetGridPositionOfToken(Token token)
    {
        if (tokenGrid.TryGetValue(token, out Vector2Int gridPosition))
            return gridPosition;

        return -Vector2Int.one;
    }

    public bool IsTokenAtGridPosition(Vector2Int gridPosition)
    {
        if (gridToken.TryGetValue(gridPosition, out Token token))
            return true;

        return false;
    }

    public bool TryGetTokenFrom(Vector2Int gridPosition, out Token token)
    {
        if (gridToken.TryGetValue(gridPosition, out token))
            return true;
        return false;
    }

    public bool TryGetKingTokenFrom(int playerID, out Token token)
    {
        if (playerKingToken.TryGetValue(playerID, out token))
            return true;

        return false;
    }

    /// <summary>
    /// 로컬 플레이어면 playerTokens, 아니면 aiTokens 목록을 반환한다.
    /// GetPlayerToken()과 다른 별도 목록임에 주의한다.
    /// </summary>
    public List<Token> GetTokens(int playerId)
    {
        return playerId == playerManager.Local.PlayerID ? playerTokens : aiTokens;
    }

    /// <summary> playerToken 딕셔너리에서 플레이어별 토큰 목록을 반환한다. </summary>
    public List<Token> GetPlayerToken(int playerID) => playerToken[playerID];
}
