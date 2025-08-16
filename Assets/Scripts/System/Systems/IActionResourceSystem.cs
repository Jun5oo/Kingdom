using UnityEngine;

public interface IActionResourceSystem : IResourceSystem
{
    public int GetMaxResources(int playerID);
    public int IncreaseMaxResources(int playerID, int amount);
    public void ResetResources(int playerID);
}
