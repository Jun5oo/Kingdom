using Cysharp.Threading.Tasks;

public interface IBuff 
{
    // 버프 인터페이스 
    public UniTask OnApply();
    public void OnRemove(); 
    public void ReduceDuration();
    public bool IsStackable(); 
    public bool IsExpired();
}
