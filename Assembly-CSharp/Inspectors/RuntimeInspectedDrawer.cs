using System;
using System.Collections.Generic;
using System.Linq;

namespace Inspectors
{
  public class RuntimeInspectedDrawer : IInspectedDrawer, IContextMenu
  {
    public static readonly RuntimeInspectedDrawer Instance = new RuntimeInspectedDrawer();
    private const int indentSize = 16;
    private const int labelWidth = 300;
    private List<ContextMenuItem> menuItems = new List<ContextMenuItem>();
    private GUISkin skin;
    private GUIStyle foldout;
    private GUIStyle popup;

    public GUISkin Skin
    {
      get
      {
        if ((UnityEngine.Object) skin == (UnityEngine.Object) null)
        {
          skin = Resources.Load<GUISkin>("EditorSkin");
          if ((UnityEngine.Object) skin == (UnityEngine.Object) null)
            skin = GUI.skin;
          foldout = ((IEnumerable<GUIStyle>) skin.customStyles).FirstOrDefault((Func<GUIStyle, bool>) (o => o.name == "Foldout"));
          popup = ((IEnumerable<GUIStyle>) skin.customStyles).FirstOrDefault((Func<GUIStyle, bool>) (o => o.name == "MiniPopup"));
        }
        return skin;
      }
    }

    public int IndentLevel { get; set; }

    public float IndentSize => 16f;

    public bool BoolField(string name, bool value)
    {
      int indentLevel = IndentLevel;
      IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      GUILayout.Label(name, GUILayout.Width(300f));
      value = GUILayout.Toggle(value, "");
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      IndentLevel = indentLevel;
      return value;
    }

    public Enum EnumPopup(string name, Enum value, IExpandedProvider context)
    {
      int indentLevel = IndentLevel;
      IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      GUILayout.Label(name, GUILayout.Width(300f));
      bool flag = context.GetExpanded(context.DeepName + name);
      if (GUILayout.Button(value.ToString(), popup))
      {
        flag = !flag;
        context.SetExpanded(context.DeepName + name, flag);
      }
      GUILayout.EndHorizontal();
      IndentLevel = indentLevel;
      if (flag)
      {
        Enum @enum = value;
        value = DrawEnum(value);
        if (@enum != value)
          context.SetExpanded(context.DeepName + name, false);
      }
      return value;
    }

    public Enum EnumSortedPopup(string name, Enum value, IExpandedProvider context)
    {
      return EnumPopup(name, value, context);
    }

    public Enum EnumMaskPopup(string name, Enum value, IExpandedProvider context)
    {
      return EnumPopup(name, value, context);
    }

    public string ListPopup(string name, string value, string[] values, IExpandedProvider context)
    {
      int indentLevel = IndentLevel;
      IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      GUILayout.Label(name, GUILayout.Width(300f));
      bool flag = context.GetExpanded(context.DeepName + name);
      if (GUILayout.Button(value, popup))
      {
        flag = !flag;
        context.SetExpanded(context.DeepName + name, flag);
      }
      GUILayout.EndHorizontal();
      IndentLevel = indentLevel;
      if (flag)
      {
        string str = value;
        value = DrawList(value, values);
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
      int indentLevel = IndentLevel;
      IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      GUILayout.Space(316f);
      GUILayout.BeginVertical(Skin.box);
      for (int index = 0; index < names.Length; ++index)
      {
        if (GUILayout.Button(names[index]))
          value = (Enum) values.GetValue(index);
      }
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
      IndentLevel = indentLevel;
      return value;
    }

    private string DrawList(string value, string[] values)
    {
      int indentLevel = IndentLevel;
      IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      GUILayout.Space(316f);
      GUILayout.BeginVertical(Skin.box);
      for (int index = 0; index < values.Length; ++index)
      {
        if (GUILayout.Button(values[index]))
          value = values[index];
      }
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
      IndentLevel = indentLevel;
      return value;
    }

    public float FloatField(string name, float value)
    {
      string str = value.ToString();
      string s = TextField(name, str);
      if (str != s)
        float.TryParse(s, out value);
      return value;
    }

    public float SliderField(string name, float value, float min, float max)
    {
      return FloatField(name, value);
    }

    public double DoubleField(string name, double value)
    {
      string str = value.ToString();
      string s = TextField(name, str);
      if (str != s)
        double.TryParse(s, out value);
      return value;
    }

    public bool Foldout(string name, bool value, Action context)
    {
      bool enabled = GUI.enabled;
      GUI.enabled = true;
      int indentLevel = IndentLevel;
      IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      value = GUILayout.Toggle(value, name, foldout);
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      IndentLevel = indentLevel;
      GUI.enabled = enabled;
      return value;
    }

    public int IntField(string name, int value)
    {
      string str = value.ToString();
      string s = TextField(name, str);
      if (str != s)
        int.TryParse(s, out value);
      return value;
    }

    public long LongField(string name, long value)
    {
      string str = value.ToString();
      string s = TextField(name, str);
      if (str != s)
        long.TryParse(s, out value);
      return value;
    }

    public UnityEngine.Object ObjectField(string name, UnityEngine.Object value, Type type)
    {
      TextField(name, value != (UnityEngine.Object) null ? ((object) value).ToString() : "null");
      return value;
    }

    public Rect RectField(string name, Rect value)
    {
      TextField(name, value.ToString());
      return value;
    }

    public Bounds BoundsField(string name, Bounds value)
    {
      TextField(name, value.ToString());
      return value;
    }

    public string MultiTextField(string name, string value) => TextField(name, value);

    public string TextField(string name, string value)
    {
      int indentLevel = IndentLevel;
      IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      GUILayout.Label(name, GUILayout.Width(300f));
      value = GUILayout.TextField(value ?? "");
      GUILayout.EndHorizontal();
      IndentLevel = indentLevel;
      return value;
    }

    public Vector2 Vector2Field(string name, Vector2 value)
    {
      TextField(name, value.ToString());
      return value;
    }

    public Vector3 Vector3Field(string name, Vector3 value)
    {
      TextField(name, value.ToString());
      return value;
    }

    public Vector4 Vector4Field(string name, Vector4 value)
    {
      TextField(name, value.ToString());
      return value;
    }

    public Color ColorField(string name, Color value)
    {
      TextField(name, value.ToString());
      return value;
    }

    public bool ButtonField(string name)
    {
      int indentLevel = IndentLevel;
      IndentLevel = 0;
      GUILayout.BeginHorizontal();
      GUILayout.Space((float) (indentLevel * 16));
      bool flag = GUILayout.Button(name);
      GUILayout.EndHorizontal();
      IndentLevel = indentLevel;
      return flag;
    }

    public IContextMenu CreateMenu()
    {
      menuItems.Clear();
      return this;
    }

    public bool MenuVisible { get; private set; }

    public void DrawContextMenu()
    {
      GUILayout.BeginVertical();
      for (int index = 0; index < menuItems.Count; ++index)
      {
        ContextMenuItem menuItem = menuItems[index];
        if (menuItem.Separator)
          GUILayout.Space(10f);
        else if (ButtonField(menuItem.Name))
        {
          MenuVisible = false;
          Action action = menuItem.Action;
          if (action != null)
            action();
        }
      }
      GUILayout.Space(20f);
      if (ButtonField("Close Menu"))
        MenuVisible = false;
      GUILayout.EndVertical();
    }

    public void AddItem(string name, bool on, Action action)
    {
      menuItems.Add(new ContextMenuItem {
        Name = name,
        Separator = false,
        Action = action
      });
    }

    public void AddSeparator(string name)
    {
      menuItems.Add(new ContextMenuItem {
        Name = name,
        Separator = true,
        Action = null
      });
    }

    public void Show() => MenuVisible = true;

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
