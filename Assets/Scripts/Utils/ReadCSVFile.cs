#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ReadCSVFile
{
    #region PATH 
    private const string CARD_CSV_PATH = "Assets/CSV/CardData.csv";
    private const string SAVE_PATH = "Assets/ScriptableObjects/Cards";
    #endregion 

    #region CARD_HEADER 
    const string CARD_ID = "CardID";
    const string CARD_NAME = "Name";
    const string CARD_DESCRIPTION = "Description";

    const string CARD_RACE = "Race";
    const string CARD_TAG = "Tag";

    const string MAX_LEVEL = "MaxLevel";

    const string LEVEL1_CP = "Level1CP";
    const string LEVEL2_CP = "Level2CP";
    const string LEVEL3_CP = "Level3CP";

    const string CARD_ATTACKTYPE1 = "AttackType1";
    const string CARD_ATTACKRANGE1 = "AttackRange1";
    const string CARD_ATTACKTYPE2 = "AttackType2";
    const string CARD_ATTACKRANGE2 = "AttackRange2";
    const string CARD_ATTACKTYPE3 = "AttackType3";
    const string CARD_ATTACKRANGE3 = "AttackRange3";

    const string CARD_MOVETYPE1 = "MoveType1";
    const string CARD_MOVERANGE1 = "MoveRange1";
    const string CARD_MOVETYPE2 = "MoveType2";
    const string CARD_MOVERANGE2 = "MoveRange2";
    const string CARD_MOVETYPE3 = "MoveType3";
    const string CARD_MOVERANGE3 = "MoveRange3";

    const string CARD_ACTION = "Action";
    const string CARD_PASSIVE1 = "Passive1";
    const string CARD_PASSIVE2 = "Passive2"; 

    #endregion

    [MenuItem("Tools/Import Cards from CSV")]
    public static void Import()
    {
        // 카드데이터 CSV 파일 확인 
        if (!File.Exists(CARD_CSV_PATH))
        {
            Debug.LogError($"CSV file not found at {CARD_CSV_PATH}");
            return;
        }

        Dictionary<string, CardData> cardDict = new Dictionary<string, CardData>();

        #region Parsing CardData 
        string[] dataLines = File.ReadAllLines(CARD_CSV_PATH);

        if (dataLines.Length <= 1)
        {
            Debug.LogWarning($"{CARD_CSV_PATH} : CSV file is empty or missing headers.");
            return;
        }

        string[] dataHeaders = dataLines[0].Split(',');

        for (int i = 1; i < dataLines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(dataLines[i]))
                continue;

            string[] values = dataLines[i].Split(',');

            string id = GetValue(dataHeaders, values, CARD_ID);
            string name = GetValue(dataHeaders, values, CARD_NAME);
            string description = GetValue(dataHeaders, values, CARD_DESCRIPTION);

            CardData card = ScriptableObject.CreateInstance<CardData>();

            card.ID = id;
            card.CardName = name;
            card.Description = description;
            card.Race = ParseEnum<Race>(GetValue(dataHeaders, values, CARD_RACE), Race.None);
            card.Tag = ParseEnum<UnitTag>(GetValue(dataHeaders, values, CARD_TAG), UnitTag.Normal);

            card.MaxLevel = int.Parse(GetValue(dataHeaders, values, MAX_LEVEL));

            card.CP.Add(int.Parse(GetValue(dataHeaders, values, LEVEL1_CP)));
            card.CP.Add(int.Parse(GetValue(dataHeaders, values, LEVEL2_CP)));
            card.CP.Add(int.Parse(GetValue(dataHeaders, values, LEVEL3_CP)));

            card.AttackType.Add(ParseEnum<RangeType>(GetValue(dataHeaders, values, CARD_ATTACKTYPE1)));
            card.AttackType.Add(ParseEnum<RangeType>(GetValue(dataHeaders, values, CARD_ATTACKTYPE2)));
            card.AttackType.Add(ParseEnum<RangeType>(GetValue(dataHeaders, values, CARD_ATTACKTYPE3)));

            card.AttackRange.Add(int.Parse(GetValue(dataHeaders, values, CARD_ATTACKRANGE1)));
            card.AttackRange.Add(int.Parse(GetValue(dataHeaders, values, CARD_ATTACKRANGE2)));
            card.AttackRange.Add(int.Parse(GetValue(dataHeaders, values, CARD_ATTACKRANGE3)));

            card.MoveType.Add(ParseEnum<RangeType>(GetValue(dataHeaders, values, CARD_MOVETYPE1)));
            card.MoveType.Add(ParseEnum<RangeType>(GetValue(dataHeaders, values, CARD_MOVETYPE2)));
            card.MoveType.Add(ParseEnum<RangeType>(GetValue(dataHeaders, values, CARD_MOVETYPE3)));

            card.MoveRange.Add(int.Parse(GetValue(dataHeaders, values, CARD_MOVERANGE1)));
            card.MoveRange.Add(int.Parse(GetValue(dataHeaders, values, CARD_MOVERANGE2)));
            card.MoveRange.Add(int.Parse(GetValue(dataHeaders, values, CARD_MOVERANGE3)));

            card.Action.Add(ParseEnum<ActionType>(GetValue(dataHeaders, values, CARD_ACTION))); 

            card.Passive.Add(ParseEnum<PassiveType>(GetValue(dataHeaders, values, CARD_PASSIVE1)));
            card.Passive.Add(ParseEnum<PassiveType>(GetValue(dataHeaders, values, CARD_PASSIVE2)));

            string path = $"{SAVE_PATH}/{card.ID}.asset";
            Directory.CreateDirectory(SAVE_PATH);
            AssetDatabase.CreateAsset(card, path);

            if (!cardDict.ContainsKey(card.ID))
                cardDict.Add(card.ID, card);
        }

        AssetDatabase.SaveAssets();
        #endregion
    }

    private static string GetValue(string[] headers, string[] values, string key)
    {
        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i].Trim() == key && i < values.Length)
                return values[i].Trim();
        }

        return "";
    }

    private static T ParseEnum<T>(string input, T defaultValue = default) where T : struct
    {
        if (System.Enum.TryParse<T>(input, true, out var result))
            return result;

        Debug.LogWarning($"Invalid enum value: {input} for {typeof(T).Name}");
        return defaultValue;
    }

    private static List<string> SplitCSVLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        var cur = new System.Text.StringBuilder();

        foreach (char c in line)
        {
            if (c == '"') inQuotes = !inQuotes;
            else if (c == ',' && !inQuotes)
            {
                result.Add(cur.ToString().Trim());
                cur.Clear();
            }
            else cur.Append(c);
        }
        result.Add(cur.ToString().Trim());
        return result;
    }
}

#endif