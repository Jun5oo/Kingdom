using UnityEngine;

/// <summary>
/// 토큰 상태(CP, 이동력) 표시 추상 클래스. King 토큰은 BarStatusPresenter, 일반 토큰은 NumberStatusPresenter를 사용한다.
/// </summary>
public abstract class StatusPresenter : MonoBehaviour
{
    public abstract void Init();
    public abstract void SetStatus(int cp, int movement = 1);
    public abstract void OnUpdateCP(int cp);
}
