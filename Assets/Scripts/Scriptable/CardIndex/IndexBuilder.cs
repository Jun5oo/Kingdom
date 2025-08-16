#if UNITY_EDITOR

using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class IndexBuilder : MonoBehaviour
{
    const string INDEX_PATH = "Assets/ScriptableObjects/Index/CardIndex.asset";
    const string DATA_PATH = "Assets/ScriptableObjects/Cards";
    const string LABEL = "CardData";

    [MenuItem("Tools/CCG/Rebuild CardIndex")]
    public static void Rebuild()
    {
        // AssetDatabase.LoadAssetAtPath: UNITY_EDITOR에서만 사용가능하며, 해당 Path의 첫 번째 에셋만을 반환한다. 
        var index = AssetDatabase.LoadAssetAtPath<CardIndex>(INDEX_PATH);
        if(index == null)
        {
            Debug.LogError("Index ScriptableObject를 찾을 수 없습니다.");
            return; 
        }

        var settings = AddressableAssetSettingsDefaultObject.Settings; 
        if(settings == null)
        {
            Debug.LogError("Addressables 설정이 없습니다.");
            return; 
        }
        if (!settings.GetLabels().Contains(LABEL))
            settings.AddLabel(LABEL);

        // DefaultGroup은 Asset을 Addressables에 등록을 할 때, 따로 Group을 설정하지 않았을 경우 들어가는 Group이다. 
        var groupName = "CardData";
        var group = settings.FindGroup(groupName); 
        if(group == null)
        {
            Debug.LogError($"현재 Addressables setting에서 {groupName} Group을 찾을 수 없습니다.");
            return; 
        }
        // t:typeName (해당 타입의 asset을 모두 찾는다.), 두 번째 매개변수가 없으면 프로젝트 전체에서, 존재하면 해당 폴더에서 탐색 
        // GUID: Global Unique Identifier 

        var guids = AssetDatabase.FindAssets("t:CardData", new[] { DATA_PATH });
        var newEntries = new List<CardIndex.Entry>();
        var seen = new HashSet<int>(); 

        foreach(var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<CardData>(path);
            if (data == null)
                continue;

            // 현재 int Type 
            var id = data.ID;
            if (!seen.Add(id))
            {
                Debug.Log($"중복된 카드{id} 입니다.");
                return; 
            }

            var entry = settings.FindAssetEntry(guid) ?? settings.CreateOrMoveEntry(guid, group);
            var key = $"Data/Cards/{id}";
            entry.address = key;
            entry.SetLabel(LABEL, true); 

            var assetRef = new AssetReferenceT<CardData>(guid);
            newEntries.Add(new CardIndex.Entry { cardID = id, dataReference = assetRef});
        }

        index.entries = newEntries;
        EditorUtility.SetDirty(index);
        AssetDatabase.SaveAssets(); 
    }
}

#endif