using UnityEditor;
using UnityEngine;

public class DefaultDeathBehaviour : IDeathBehaviour
{
    public void OnDeath(Token token, TokenManager tokenManager)
    {
        tokenManager.DestroyToken(token); 
    }
}
