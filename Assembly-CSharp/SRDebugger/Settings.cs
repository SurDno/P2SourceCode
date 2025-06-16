using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SRDebugger
{
  public class Settings : ScriptableObject
  {
    private const string ResourcesPath = "/Settings/Resources/SRDebugger";
    private const string ResourcesName = "Settings";
    private static Settings _instance;
    [SerializeField]
    private bool _isEnabled = true;
    [SerializeField]
    private DefaultTabs _defaultTab = DefaultTabs.SystemInformation;
    [SerializeField]
    private bool _enableKeyboardShortcuts = true;
    [SerializeField]
    private Settings.KeyboardShortcut[] _keyboardShortcuts;
    [SerializeField]
    private Settings.KeyboardShortcut[] _newKeyboardShortcuts = new Settings.KeyboardShortcut[0];
    [SerializeField]
    private bool _keyboardModifierControl = true;
    [SerializeField]
    private bool _keyboardModifierAlt = false;
    [SerializeField]
    private bool _keyboardModifierShift = true;
    [SerializeField]
    private bool _keyboardEscapeClose = true;
    [SerializeField]
    private bool _collapseDuplicateLogEntries = true;
    [SerializeField]
    private PinAlignment _profilerAlignment = PinAlignment.BottomLeft;
    [SerializeField]
    private ConsoleAlignment _consoleAlignment = ConsoleAlignment.Top;

    public static Settings Instance
    {
      get
      {
        if ((UnityEngine.Object) Settings._instance == (UnityEngine.Object) null)
          Settings._instance = Settings.GetOrCreateInstance();
        if (Settings._instance._keyboardShortcuts != null && Settings._instance._keyboardShortcuts.Length != 0)
          Settings._instance.UpgradeKeyboardShortcuts();
        return Settings._instance;
      }
    }

    private void UpgradeKeyboardShortcuts()
    {
      Debug.Log((object) "[SRDebugger] Upgrading Settings format");
      List<Settings.KeyboardShortcut> keyboardShortcutList = new List<Settings.KeyboardShortcut>();
      for (int index = 0; index < this._keyboardShortcuts.Length; ++index)
      {
        Settings.KeyboardShortcut keyboardShortcut = this._keyboardShortcuts[index];
        keyboardShortcutList.Add(new Settings.KeyboardShortcut()
        {
          Action = keyboardShortcut.Action,
          Key = keyboardShortcut.Key,
          Alt = this._keyboardModifierAlt,
          Shift = this._keyboardModifierShift,
          Control = this._keyboardModifierControl
        });
      }
      this._keyboardShortcuts = new Settings.KeyboardShortcut[0];
      this._newKeyboardShortcuts = keyboardShortcutList.ToArray();
    }

    public bool IsEnabled
    {
      get => this._isEnabled;
      set => this._isEnabled = value;
    }

    public DefaultTabs DefaultTab
    {
      get => this._defaultTab;
      set => this._defaultTab = value;
    }

    public bool EnableKeyboardShortcuts
    {
      get => this._enableKeyboardShortcuts;
      set => this._enableKeyboardShortcuts = value;
    }

    public IList<Settings.KeyboardShortcut> KeyboardShortcuts
    {
      get => (IList<Settings.KeyboardShortcut>) this._newKeyboardShortcuts;
      set => this._newKeyboardShortcuts = value.ToArray<Settings.KeyboardShortcut>();
    }

    public bool KeyboardEscapeClose
    {
      get => this._keyboardEscapeClose;
      set => this._keyboardEscapeClose = value;
    }

    public bool CollapseDuplicateLogEntries
    {
      get => this._collapseDuplicateLogEntries;
      set => this._collapseDuplicateLogEntries = value;
    }

    public PinAlignment ProfilerAlignment
    {
      get => this._profilerAlignment;
      set
      {
        if (value == PinAlignment.CenterRight || value == PinAlignment.CenterLeft || value == PinAlignment.TopCenter || value == PinAlignment.BottomCenter)
        {
          int num = 0;
          if (num < 4)
            this._profilerAlignment = (PinAlignment) num;
          else
            this._profilerAlignment = PinAlignment.BottomLeft;
        }
        else
        {
          PinAlignment profilerAlignment = this._profilerAlignment;
          this._profilerAlignment = value;
        }
      }
    }

    public ConsoleAlignment ConsoleAlignment
    {
      get => this._consoleAlignment;
      set => this._consoleAlignment = value;
    }

    private static Settings GetOrCreateInstance()
    {
      Settings instance = Resources.Load<Settings>("SRDebugger/Settings");
      if ((UnityEngine.Object) instance == (UnityEngine.Object) null)
        instance = ScriptableObject.CreateInstance<Settings>();
      return instance;
    }

    public enum ShortcutActions
    {
      None = 0,
      ToggleDebugging = 1,
      OpenSystemInfoTab = 2,
      OpenLoggerTab = 3,
      OpenProfilerTab = 4,
      OpenConsoleTab = 5,
      OpenInspectorTab = 6,
      ClosePanel = 7,
      OpenPanel = 8,
      TogglePanel = 9,
      ToggleDockedProfiler = 11, // 0x0000000B
    }

    [Serializable]
    public sealed class KeyboardShortcut
    {
      [SerializeField]
      public Settings.ShortcutActions Action;
      [SerializeField]
      public bool Alt;
      [SerializeField]
      public bool Control;
      [SerializeField]
      public KeyCode Key;
      [SerializeField]
      public bool Shift;
    }
  }
}
