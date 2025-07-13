using UnityEngine;

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
