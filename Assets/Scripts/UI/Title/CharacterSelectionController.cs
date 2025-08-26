using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] List<RaceData> raceList;

    [SerializeField] RawImage raceImage; 
    [SerializeField] TextMeshProUGUI raceName;

    [SerializeField] int currentIdx;

    void Awake()
    {
        if (raceList == null)
        {
            Debug.LogWarning($"진영 데이터를 찾을 수 없습니다.");
            return; 
        }

        currentIdx = 0; 

        OnUpdate(); 
    }

    void OnUpdate()
    {
        raceImage.texture = raceList[currentIdx].raceImage; 
        raceName.text = raceList[currentIdx].raceName;
    }

    public void Prev()
    {
        currentIdx = (currentIdx - 1 + raceList.Count   ) % raceList.Count;
        OnUpdate();
    }
    public void Next()
    {
        currentIdx = (currentIdx + 1) % raceList.Count;
        OnUpdate();
    }
    public Race GetCurrentRaceData() => raceList[currentIdx].raceType; 
}
