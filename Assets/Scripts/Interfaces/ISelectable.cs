using System;
using UnityEngine;

public interface ISelectable
{
    public void OnSelected();
    public void OnDeselected();
    public bool IsSelectable();
    public Entity Entity { get; }

    public event Action OnSelectedComplete;  
}
