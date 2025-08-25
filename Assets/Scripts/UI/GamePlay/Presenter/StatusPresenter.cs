using UnityEngine;

public abstract class StatusPresenter : MonoBehaviour
{
    public abstract void Init();
    public abstract void SetStatus(int cp, int movement = 1);
    public abstract void OnUpdateCP(int cp);
}
