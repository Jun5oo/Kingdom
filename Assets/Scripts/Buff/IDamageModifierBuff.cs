using UnityEngine;

public interface IDamageModifierBuff : IBuff
{
    // 데미지 계산에 영향을 주는 버프 (추후 인터페이스가 아닌 추상 클래스로 변환 가능) 
    public int ModifyDamage(int damage);
}
