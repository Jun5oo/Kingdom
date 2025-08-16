#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class CSVReader
{
    private const string csvPath = "Assets/CSV/CardData.csv";
    private const string unitSavePath = "Assets/ScriptableObjects/Cards/Units";

    [MenuItem("Tools/Import Cards from CSV")]
    public static void Import()
    {
        if (!File.Exists(csvPath))
        {
            Debug.LogError($"CSV file not found at {csvPath}");
            return;
        }

        string[] lines = File.ReadAllLines(csvPath);
        if (lines.Length <= 1)
        {
            Debug.LogWarning("CSV file is empty or missing headers.");
            return;
        }

        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] values = lines[i].Split(',');

            string cardType = GetValue(headers, values, "CardType");
            string id = GetValue(headers, values, "ID");
            string name = GetValue(headers, values, "Name");
            string description = GetValue(headers, values, "Description");

            if (cardType == "Unit")
            {
                UnitCardData card = ScriptableObject.CreateInstance<UnitCardData>();

                card.ID = int.Parse(id);
                card.Name = name;
                card.Description = description;
                card.Race = ParseEnum<Race>(GetValue(headers, values, "Race"), Race.None);
                card.Tag = ParseEnum<UnitTag>(GetValue(headers, values, "Race"), UnitTag.Normal);
                card.CP = ParseIntList(GetValue(headers, values, "CP"));
                card.AttackRange = ParseVector2IntArray(GetValue(headers, values, "AttackRange"));
                card.MoveRange = ParseVector2IntArray(GetValue(headers, values, "MovementRange"));
                card.Passive = ParseEnumList<PassiveType>(GetValue(headers, values, "Passive"));
                card.Actions = ParseEnumList<ActionType>(GetValue(headers, values, "Actions"));

                string path = $"{unitSavePath}/{card.ID}.asset";
                Directory.CreateDirectory(unitSavePath);
                AssetDatabase.CreateAsset(card, path);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("All cards imported from CSV.");
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

    private static bool ParseBool(string input)
    {
        if (bool.TryParse(input.Trim(), out bool result))
            return result;

        Debug.LogWarning($"Invalid boolean: '{input}', defaulting to false.");
        return false;
    }

    private static List<Vector2Int> ParseVector2IntArray(string input)
    {
        Debug.Log($"Parsing Vector2Int Array: {input}");

        var list = new List<Vector2Int>();
        if (string.IsNullOrWhiteSpace(input)) return list;

        var matches = Regex.Matches(input, @"\(\s*(-?\d+)\s*:\s*(-?\d+)\s*\)");
        var result = new List<Vector2Int>();

        foreach (Match m in matches)
        {
            int x = int.Parse(m.Groups[1].Value);
            int y = int.Parse(m.Groups[2].Value);

            Debug.Log($"{x}, {y}");
            result.Add(new Vector2Int(x, y));
        }

        return result;
    }

    private static T ParseEnum<T>(string input, T defaultValue = default) where T : struct
    {
        if (System.Enum.TryParse<T>(input, true, out var result))
            return result;

        Debug.LogWarning($"Invalid enum value: {input} for {typeof(T).Name}");
        return defaultValue;
    }

    private static List<T> ParseEnumList<T>(string input) where T : struct
    {
        Debug.Log($"Parsing Enum Array: {input}");

        var list = new List<T>();
        if (string.IsNullOrWhiteSpace(input)) return list;

        var parts = input.Split(new[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (var p in parts)
        {
            if (System.Enum.TryParse<T>(p.Trim(), true, out var value))
                list.Add(value);
            else
                Debug.LogWarning($"Invalid enum value: {p} in {typeof(T).Name}");
        }

        return list;
    }

    private static List<int> ParseIntList(string input)
    {
        var list = new List<int>();
        if (string.IsNullOrWhiteSpace(input)) return list;

        var parts = input.Split(':');
        foreach (var p in parts)
        {
            if (int.TryParse(p.Trim(), out int val))
                list.Add(val);
            else
                Debug.LogWarning($"Invalid int value: {p} in CP list");
        }

        return list;
    }
}

#endif
