using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/ColorSpriteDB")]
public class ColorSpriteDB : ScriptableObject
{
    public List<ColorEntry> entries;
    public static ColorSpriteDB instance;

    public static void Init(ColorSpriteDB db)
    {
        instance = db;
    }

    public static Sprite Get(ColorType type)
    {
        var entry = instance.entries.Find(e => e.type == type);

        if (entry == null)
        {
            Debug.LogError($"Missing sprite for color {type}");
            return null;
        }

        return entry.sprite;
    }
}

[System.Serializable]
public class ColorEntry
{
    public ColorType type;
    public Sprite sprite;
}
public static class ColorConfig
{
    public static Color GetColor(ColorType type)
    {
        return type switch
        {
            ColorType.Red => Color.red,
            ColorType.Blue => Color.blue,
            ColorType.Green => Color.green,
            ColorType.Yellow => Color.yellow,
            ColorType.Pink => new Color(1.0f, 0.5f, 0.7f),
            ColorType.Purple => new Color(0.6f, 0.4f, 0.8f),
            ColorType.Orange => new Color(1.0f, 0.6f, 0.2f),
            ColorType.Cyan => Color.cyan,
            _ => Color.white
        };
    }
}

