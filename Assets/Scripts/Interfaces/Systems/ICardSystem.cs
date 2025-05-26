public interface ICardSystem 
{
    public void DrawCard(int playerID);
    public void Init(IGridSystem gridSystem, IUISystem uiSystem, ISelectionSystem selectionSystem, IActionSystem actionSystem);
}
