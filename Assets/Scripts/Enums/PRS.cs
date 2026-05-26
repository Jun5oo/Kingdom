using UnityEngine;

/// <summary>
/// Position, Rotation, Scale을 하나의 구조로 묶은 Transform 상태 스냅샷.
/// BaseMovement.MoveTransform() 호출 시 목표 상태로 전달된다.
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
