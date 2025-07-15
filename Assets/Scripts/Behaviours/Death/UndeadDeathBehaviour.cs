using UnityEngine;

public class UndeadDeathBehaviour : IDeathBehaviour
{
    const int GRAVEYARD_CP = 1;
    const int GRAVEYARD_MOVEMENT = 0;
    const string GRAVEYARD_TEXTURE = "Graveyard"; 
    
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
                {
                    if (tokenView.TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
                        Debug.Log("Renderer"); 
                        // renderer.material.SetTexture("_CardArtTexture", texture);
                }
            });
        }
    }
}
