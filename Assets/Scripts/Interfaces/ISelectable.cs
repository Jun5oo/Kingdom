using UnityEngine;

public interface ISelectable
{
    public void OnSelected();
    public void OnDeselected();
    public bool IsSelectable(); 
}
