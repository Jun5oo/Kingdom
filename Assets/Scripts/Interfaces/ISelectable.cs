using System;
using UnityEngine;

public interface ISelectable
{
    public void OnSelected();
    public void OnDeselected();
    public bool IsSelectable();
    public BaseObject BaseObject { get; }   
    
    public event Action OnSelectedComplete;  
}
