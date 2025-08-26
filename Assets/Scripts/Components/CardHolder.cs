using System.Collections.Generic;
using UnityEngine;

public class CardHolder : MonoBehaviour
{
    [SerializeField] PlayerConfig config;

    const float LOCAL_Y_ROTATION = 0f;
    const float REMOTE_Y_ROTATION = 180f; 

    [SerializeField] Transform localHolder;
    [SerializeField] Transform remoteHolder;

    [SerializeField] GameObject undeadHolder;
    [SerializeField] GameObject celestialHolder; 

    void Awake()
    {
        Race localRace = config.playerSelected;
        Race remoteRace = config.botSelected;

        Dictionary<Race, GameObject> holderDict = new Dictionary<Race, GameObject>();
        holderDict[Race.Undead] = undeadHolder; 
        holderDict[Race.Celestial] = celestialHolder;

        GameObject localHolderObject = Instantiate(holderDict[localRace], localHolder); 
        localHolderObject.transform.localRotation = Quaternion.Euler(0f, LOCAL_Y_ROTATION, 180f);
        GameObject remoteHolderObject = Instantiate(holderDict[remoteRace], remoteHolder);
        remoteHolderObject.transform.localRotation = Quaternion.Euler(0f, REMOTE_Y_ROTATION, 180f);

    }
}
