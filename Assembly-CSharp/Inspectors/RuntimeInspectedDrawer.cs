using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inspectors
{
  public class RuntimeInspectedDrawer : IInspectedDrawer, IContextMenu
  {
    public static readonly RuntimeInspectedDrawer Instance = new RuntimeInspectedDrawer();
    private const int indentSize = 16;
    private const int labelWidth = 300;
    private List<RuntimeInspectedDrawer.ContextMenuItem> menuItems = new List<RuntimeInspectedDrawer.ContextMenuItem>();
    private GUISkin skin;
    private GUIStyle foldout;
    private GUIStyle popup;

    public GUISkin Skin
    {
      get
      {
        if ((UnityEngine.Object) this.skin == (UnityEngine.Object) null)
        {
          this.skin = Resources.Load<GUISkin>("EditorSkin");
          if ((UnityEngine.Object) this.skin == (UnityEngine.Object) null)
            this.skin = GUI.skin;
          this.foldout = ((IEnumerable<GUIStyle>) this.skin.customStyles).FirstOrDefault<GUIStyle>((Func<GUIStyle, bool>) (o => o.name == "Foldout"));
          this.popup = ((IEnumerable<GUIStyle>) this.skin.customStyles).FirstOrDefault<GUIStyle>((Func<GUIStyle, bool>) (o => o.name == "MiniPopup"));
        }
        return this.skin;
      }
    }

    public int IndentLevel { get; set; }

    public float IndentSize => 16f;

    public bool BoolField(string name, bool value)
    {
      int indentLevel = this.IndentLevel;
      this.IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      GUILayout.Label(name, GUILayout.Width(300f));
      value = GUILayout.Toggle(value, "");
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      this.IndentLevel = indentLevel;
      return value;
    }

    public Enum EnumPopup(string name, Enum value, IExpandedProvider context)
    {
      int indentLevel = this.IndentLevel;
      this.IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      GUILayout.Label(name, GUILayout.Width(300f));
      bool flag = context.GetExpanded(context.DeepName + name);
      if (GUILayout.Button(value.ToString(), this.popup))
      {
        flag = !flag;
        context.SetExpanded(context.DeepName + name, flag);
      }
      GUILayout.EndHorizontal();
      this.IndentLevel = indentLevel;
      if (flag)
      {
        Enum @enum = value;
        value = this.DrawEnum(value);
        if (@enum != value)
          context.SetExpanded(context.DeepName + name, false);
      }
      return value;
    }

    public Enum EnumSortedPopup(string name, Enum value, IExpandedProvider context)
    {
      return this.EnumPopup(name, value, context);
    }

    public Enum EnumMaskPopup(string name, Enum value, IExpandedProvider context)
    {
      return this.EnumPopup(name, value, context);
    }

    public string ListPopup(string name, string value, string[] values, IExpandedProvider context)
    {
      int indentLevel = this.IndentLevel;
      this.IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      GUILayout.Label(name, GUILayout.Width(300f));
      bool flag = context.GetExpanded(context.DeepName + name);
      if (GUILayout.Button(value.ToString(), this.popup))
      {
        flag = !flag;
        context.SetExpanded(context.DeepName + name, flag);
      }
      GUILayout.EndHorizontal();
      this.IndentLevel = indentLevel;
      if (flag)
      {
        string str = value;
        value = this.DrawList(value, values);
        if (str != value)
          context.SetExpanded(context.DeepName + name, false);
      }
      return value;
    }

    private Enum DrawEnum(Enum value)
    {
      string[] names = Enum.GetNames(value.GetType());
      Array values = Enum.GetValues(value.GetType());
      if (names.Length != values.Length)
        return value;
      int indentLevel = this.IndentLevel;
      this.IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      GUILayout.Space(316f);
      GUILayout.BeginVertical(this.Skin.box);
      for (int index = 0; index < names.Length; ++index)
      {
        if (GUILayout.Button(names[index]))
          value = (Enum) values.GetValue(index);
      }
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
      this.IndentLevel = indentLevel;
      return value;
    }

    private string DrawList(string value, string[] values)
    {
      int indentLevel = this.IndentLevel;
      this.IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      GUILayout.Space(316f);
      GUILayout.BeginVertical(this.Skin.box);
      for (int index = 0; index < values.Length; ++index)
      {
        if (GUILayout.Button(values[index]))
          value = values[index];
      }
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
      this.IndentLevel = indentLevel;
      return value;
    }

    public float FloatField(string name, float value)
    {
      string str = value.ToString();
      string s = this.TextField(name, str);
      if (str != s)
        float.TryParse(s, out value);
      return value;
    }

    public float SliderField(string name, float value, float min, float max)
    {
      return this.FloatField(name, value);
    }

    public double DoubleField(string name, double value)
    {
      string str = value.ToString();
      string s = this.TextField(name, str);
      if (str != s)
        double.TryParse(s, out value);
      return value;
    }

    public bool Foldout(string name, bool value, Action context)
    {
      bool enabled = GUI.enabled;
      GUI.enabled = true;
      int indentLevel = this.IndentLevel;
      this.IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      value = GUILayout.Toggle(value, name, this.foldout);
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      this.IndentLevel = indentLevel;
      GUI.enabled = enabled;
      return value;
    }

    public int IntField(string name, int value)
    {
      string str = value.ToString();
      string s = this.TextField(name, str);
      if (str != s)
        int.TryParse(s, out value);
      return value;
    }

    public long LongField(string name, long value)
    {
      string str = value.ToString();
      string s = this.TextField(name, str);
      if (str != s)
        long.TryParse(s, out value);
      return value;
    }

    public UnityEngine.Object ObjectField(string name, UnityEngine.Object value, System.Type type)
    {
      this.TextField(name, value != (UnityEngine.Object) null ? ((object) value).ToString() : "null");
      return value;
    }

    public Rect RectField(string name, Rect value)
    {
      this.TextField(name, value.ToString());
      return value;
    }

    public Bounds BoundsField(string name, Bounds value)
    {
      this.TextField(name, value.ToString());
      return value;
    }

    public string MultiTextField(string name, string value) => this.TextField(name, value);

    public string TextField(string name, string value)
    {
      int indentLevel = this.IndentLevel;
      this.IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      GUILayout.Label(name, GUILayout.Width(300f));
      value = GUILayout.TextField(value ?? "");
      GUILayout.EndHorizontal();
      this.IndentLevel = indentLevel;
      return value;
    }

    public Vector2 Vector2Field(string name, Vector2 value)
    {
      this.TextField(name, value.ToString());
      return value;
    }

    public Vector3 Vector3Field(string name, Vector3 value)
    {
      this.TextField(name, value.ToString());
      return value;
    }

    public Vector4 Vector4Field(string name, Vector4 value)
    {
      this.TextField(name, value.ToString());
      return value;
    }

    public Color ColorField(string name, Color value)
    {
      this.TextField(name, value.ToString());
      return value;
    }

    public bool ButtonField(string name)
    {
      int indentLevel = this.IndentLevel;
      this.IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      bool flag = GUILayout.Button(name);
      GUILayout.EndHorizontal();
      this.IndentLevel = indentLevel;
      return flag;
    }

    public IContextMenu CreateMenu()
    {
      this.menuItems.Clear();
      return (IContextMenu) this;
    }

    public bool MenuVisible { get; private set; }

    public void DrawContextMenu()
    {
      GUILayout.BeginVertical();
      for (int index = 0; index < this.menuItems.Count; ++index)
      {
        RuntimeInspectedDrawer.ContextMenuItem menuItem = this.menuItems[index];
        if (menuItem.Separator)
          GUILayout.Space(10f);
        else if (this.ButtonField(menuItem.Name))
        {
          this.MenuVisible = false;
          Action action = menuItem.Action;
          if (action != null)
            action();
        }
      }
      GUILayout.Space(20f);
      if (this.ButtonField("Close Menu"))
        this.MenuVisible = false;
      GUILayout.EndVertical();
    }

    public void AddItem(string name, bool on, Action action)
    {
      this.menuItems.Add(new RuntimeInspectedDrawer.ContextMenuItem()
      {
        Name = name,
        Separator = false,
        Action = action
      });
    }

    public void AddSeparator(string name)
    {
      this.menuItems.Add(new RuntimeInspectedDrawer.ContextMenuItem()
      {
        Name = name,
        Separator = true,
        Action = (Action) null
      });
    }

    public void Show() => this.MenuVisible = true;

    public int BeginBox() => 0;

    public void EndBox(int level)
    {
    }

    public AnimationCurve CurveField(string name, AnimationCurve value) => value;

    private class ContextMenuItem
    {
      public string Name;
      public bool Separator;
      public Action Action;
    }
  }
}
