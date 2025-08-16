using UnityEngine;

public interface IResourceSystem
{
    public void Init(); 

    public void Add(int playerID, int amount);
    public void Consume(int playerID, int cost);
    public bool IsEnoughResources(int playerID, int cost);
    public int GetCurrentResources(int playerID); 
}
