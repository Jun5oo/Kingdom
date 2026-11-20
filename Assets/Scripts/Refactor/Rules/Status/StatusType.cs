namespace Refactor
{
    // C# 확인 지점이 있는 상태 이상 키워드. 동작 이름으로 짓고 카드 이름으로 짓지 않는다(§5.3).
    public enum StatusType
    {
        Shield, // 피해 처리: 0보다 큰 피해 1회를 0으로 만들고 모든 Shield가 사라진다
    }
}
