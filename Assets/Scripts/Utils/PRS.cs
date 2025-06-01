using UnityEngine;

/// <summary>
/// 위치, 회전, 크기
/// </summary>

public class PRS
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale; 

    public PRS(Vector3 pos, Quaternion rot, Vector3 scale) 
    {
        this.position = pos;
        this.rotation = rot;
        this.scale = scale; 
    }
}
