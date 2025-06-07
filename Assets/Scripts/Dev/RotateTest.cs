using DG.Tweening;
using UnityEngine;

public class RotateTest : MonoBehaviour
{
    [SerializeField] Transform target; 
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 distance = target.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(-Vector3.up, distance); 

            this.transform.DORotateQuaternion(rot, 0.1f); 
        }
    }
}
