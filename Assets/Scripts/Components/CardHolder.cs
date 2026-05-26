using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerConfig의 진영 정보를 바탕으로 로컬/원격 플레이어의 카드홀더 프리팹을 Awake 시 인스턴스화한다.
/// 원격 홀더는 Y축 180° 회전하여 반대편 시점을 표현한다.
/// </summary>
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
