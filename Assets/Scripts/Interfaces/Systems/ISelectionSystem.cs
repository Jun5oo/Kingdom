public interface ISelectionSystem
{
    public void Init(IGridSystem gridSystem, IActionSystem actionSystem); 
    public void OnEnterSelected(ISelectable selectable);
    public void OnExitSelected(); 
}
