using System.Collections.Generic;
using UnityEngine;

public class UndeadDeathBehaviour : IDeathBehaviour
{
    const int GRAVEYARD_CP = 1;
    const int GRAVEYARD_MOVEMENT = 0;

    List<Vector2Int> GRAVEYARD_MOVEMENTRANGE;
    List<Vector2Int> GRAVEYARD_ATTACKRANGE; 

    const string GRAVEYARD_TEXTURE = "Graveyard"; 
    
    public UndeadDeathBehaviour()
    {
        GRAVEYARD_MOVEMENTRANGE = new List<Vector2Int>();
        GRAVEYARD_ATTACKRANGE = new List<Vector2Int>(); 
    }

    public void OnDeath(Token token, TokenManager tokenManager)
    {
        if (!token.TryEnterGraveyard())
        {
            tokenManager.DestroyToken(token);
            return; 
        }

        if (token.TryGetComponent<TokenMovement>(out TokenMovement tokenMovement))
        {
            tokenMovement.PlayerSpinToss(() =>
            {
                Texture2D texture = Resources.Load<Texture2D>(GRAVEYARD_TEXTURE); 

                if(token.TryGetComponent<TokenView>(out TokenView tokenView))
                    tokenView.SetTokenArt(texture); 

                token.SetTokenStatus(GRAVEYARD_CP, GRAVEYARD_MOVEMENT, GRAVEYARD_ATTACKRANGE, GRAVEYARD_MOVEMENTRANGE);
            });
        }
    }
}
