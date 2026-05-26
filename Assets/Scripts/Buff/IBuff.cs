using Cysharp.Threading.Tasks;

/// <summary>
/// 토큰에 부착되는 버프의 공통 인터페이스.
/// 지속 시간(duration) 감소와 만료(expired) 여부를 관리한다.
/// </summary>
public interface IBuff
{
    public UniTask OnApply();      // 버프가 토큰에 적용될 때 호출
    public void OnRemove();        // 버프가 제거될 때 호출 (시각 효과 정리 포함)
    public void ReduceDuration();  // 턴 시작 시 지속 시간을 1 감소
    public bool IsStackable();     // 중첩 적용 가능 여부
    public bool IsExpired();       // 지속 시간이 0 이하인지 여부
}
