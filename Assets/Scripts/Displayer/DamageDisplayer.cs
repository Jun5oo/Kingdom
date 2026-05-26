using UnityEngine;

/// <summary>
/// 데미지 숫자 팝업을 풀에서 꺼내 표시하는 클래스.
/// Play() 완료 콜백으로 팝업을 자동 반납한다.
/// </summary>
public class DamageDisplayer
{
    PoolManager poolManager;

    public DamageDisplayer()
    {
        poolManager = ServiceLocator.Get<PoolManager>();
    }

    /// <summary>
    /// baseObject의 월드 위치에 damage 값 팝업을 표시한다.
    /// 팝업 애니메이션 완료 시 풀에 자동 반납된다.
    /// </summary>
    public void Display(int damage, BaseObject baseObject)
    {
        DamagePopup damagePopup = poolManager.Pop<DamagePopup>();

        damagePopup.transform.position = Camera.main.WorldToScreenPoint(baseObject.transform.position);
        damagePopup.Init(damage);
        damagePopup.Play(() => poolManager.Push<DamagePopup>(damagePopup));
    }
}
