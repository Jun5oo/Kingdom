/// <summary>
/// 토큰에 부착되는 패시브 능력의 공통 인터페이스.
/// Activate()에서 이벤트를 구독하고 Deactivate()에서 구독을 해제한다.
/// </summary>
public interface IPassive
{
    public void Activate();   // 패시브 이벤트 구독 시작
    public void Deactivate(); // 패시브 이벤트 구독 해제 (토큰 파괴 시 호출)
}
