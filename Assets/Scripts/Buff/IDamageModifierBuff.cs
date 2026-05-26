using UnityEngine;

/// <summary>
/// 데미지 계산에 개입하는 버프 인터페이스.
/// IBuff를 상속하며, ModifyDamage()로 최종 데미지 값을 변환한다.
/// </summary>
public interface IDamageModifierBuff : IBuff
{
    public int ModifyDamage(int damage); // 입력 데미지를 수정하여 실제 적용 데미지를 반환한다
}
