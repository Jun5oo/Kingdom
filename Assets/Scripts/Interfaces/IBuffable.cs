/// <summary>
/// 버프를 보유할 수 있는 오브젝트가 구현하는 인터페이스. Token에서 구현된다.
/// </summary>
public interface IBuffable
{
    // 버프를 가질 수 있는 오브젝트 인터페이스
    public void AddBuff(IBuff buff);
    public void RemoveBuff(IBuff buff); 
    public bool CanApply(IBuff buff); 
}
