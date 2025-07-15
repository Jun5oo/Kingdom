using System;
using UnityEngine;

public enum TokenViewState
{
    Idle,
    Hover,
    Selected,
    Moving
}

public class TokenHover : MonoBehaviour, IHoverable, ISelectable
{
    [SerializeField] Token token;
    [SerializeField] TokenMovement tokenMovement;
    [SerializeField] TokenViewState viewState;

    public Entity Entity { get { return token; } }

    public event Action<Token> OnTokenSelected; 

    public event Action OnSelectedComplete;
    public event Action OnTokenDeselected; 

    Vector3 originPos;
    Quaternion originRotation;
    Vector3 originScale; 

    float selectOffsetY = 0.5f; 


    public void Init()
    {
        tokenMovement.OnTokenMoved -= OnUpdatePRS;
        tokenMovement.OnTokenMoved += OnUpdatePRS;

        tokenMovement.OnTokenMoveComplete -= OnTokenMoveComplete;
        tokenMovement.OnTokenMoveComplete += OnTokenMoveComplete; 

        viewState = TokenViewState.Idle;

        originPos = tokenMovement.PRS.position;
        originRotation = tokenMovement.PRS.rotation; 
        originScale = tokenMovement.PRS.scale;
    }

    public void OnHover()
    {
        if (!IsHoverable())
            return; 

        viewState = TokenViewState.Hover; 
    }
    public void OffHover()
    {
        if (viewState != TokenViewState.Hover)
            return; 

        viewState = TokenViewState.Idle; 
    }
    public bool IsHoverable()
    {
        return viewState == TokenViewState.Idle; 
    }

    // TODO
    // Card와 Token의 경우 구조가 비슷함. 따라서 Entity에서 중복되는 부분을 작성하고 상속받는 식으로 구현하는 부분에 대해 고려. 
    // Hover, Movement, View 또한 EntityHover, EntityMovement, EntityView로 상속받아 공통되는 부분을 줄일 예정. 

    public void OnSelected()
    {
        if (!IsSelectable())
            return; 

        Vector3 position = originPos + (Vector3.up * selectOffsetY); 
        Quaternion rotation = originRotation;
        Vector3 scale = originScale;

        tokenMovement.MoveTransform(new PRS(position, rotation, scale), 0.2f, true, () =>
        {
            viewState = TokenViewState.Selected;
            OnTokenSelected?.Invoke(token); 
            OnSelectedComplete?.Invoke(); 
        }); 
    }
    public void OnDeselected()
    {
        if (viewState != TokenViewState.Selected)
            return;

        Vector3 position = originPos;
        Quaternion rotation = originRotation;
        Vector3 scale = originScale;

        tokenMovement.MoveTransform(new PRS(position, rotation, scale), 0.2f, true, () =>
        {
            viewState = TokenViewState.Idle;
            OnTokenDeselected?.Invoke(); 
        });
    }

    public bool IsSelectable()
    {
        return viewState == TokenViewState.Idle || viewState == TokenViewState.Hover; 
    }

    public void OnUpdatePRS()
    {
        viewState = TokenViewState.Moving; 

        PRS prs = tokenMovement.PRS; 

        originPos = prs.position;
        originRotation = prs.rotation;
        originScale = prs.scale;
    }

    void OnTokenMoveComplete()
    {
        if (viewState == TokenViewState.Moving)
            viewState = TokenViewState.Idle; 
    }
}
