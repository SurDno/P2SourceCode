using UnityEngine;

namespace Engine.Source.Utility
{
  public static class ColorPreset
  {
    public static readonly Color White = Color.white;
    public static readonly Color Black = Color.black;
    public static readonly Color Gray = Color.gray;
    public static readonly Color Red = Color.red;
    public static readonly Color Green = Color.green;
    public static readonly Color Blue = Color.blue;
    public static readonly Color Magenta = Color.magenta;
    public static readonly Color Cyan = Color.cyan;
    public static readonly Color Yellow = Color.yellow;
    public static readonly Color LightGray = new(0.7f, 0.7f, 0.7f);
    public static readonly Color DarkRed = new(0.7f, 0.0f, 0.0f);
    public static readonly Color DarkGreen = new(0.0f, 0.7f, 0.0f);
    public static readonly Color DarkBlue = new(0.0f, 0.0f, 0.7f);
    public static readonly Color DarkMagenta = new(0.7f, 0.0f, 0.7f);
    public static readonly Color DarkCyan = new(0.0f, 0.7f, 0.7f);
    public static readonly Color DarkYellow = new(0.7f, 0.7f, 0.0f);
    public static readonly Color Orange = new(1f, 0.5f, 0.0f);
  }
}
