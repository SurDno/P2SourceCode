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
    private KeyboardShortcut[] _keyboardShortcuts;
    [SerializeField]
    private KeyboardShortcut[] _newKeyboardShortcuts = new KeyboardShortcut[0];
    [SerializeField]
    private bool _keyboardModifierControl = true;
    [SerializeField]
    private bool _keyboardModifierAlt;
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
        if (_instance == null)
          _instance = GetOrCreateInstance();
        if (_instance._keyboardShortcuts != null && _instance._keyboardShortcuts.Length != 0)
          _instance.UpgradeKeyboardShortcuts();
        return _instance;
      }
    }

    private void UpgradeKeyboardShortcuts()
    {
      Debug.Log("[SRDebugger] Upgrading Settings format");
      List<KeyboardShortcut> keyboardShortcutList = new List<KeyboardShortcut>();
      for (int index = 0; index < _keyboardShortcuts.Length; ++index)
      {
        KeyboardShortcut keyboardShortcut = _keyboardShortcuts[index];
        keyboardShortcutList.Add(new KeyboardShortcut {
          Action = keyboardShortcut.Action,
          Key = keyboardShortcut.Key,
          Alt = _keyboardModifierAlt,
          Shift = _keyboardModifierShift,
          Control = _keyboardModifierControl
        });
      }
      _keyboardShortcuts = new KeyboardShortcut[0];
      _newKeyboardShortcuts = keyboardShortcutList.ToArray();
    }

    public bool IsEnabled
    {
      get => _isEnabled;
      set => _isEnabled = value;
    }

    public DefaultTabs DefaultTab
    {
      get => _defaultTab;
      set => _defaultTab = value;
    }

    public bool EnableKeyboardShortcuts
    {
      get => _enableKeyboardShortcuts;
      set => _enableKeyboardShortcuts = value;
    }

    public IList<KeyboardShortcut> KeyboardShortcuts
    {
      get => _newKeyboardShortcuts;
      set => _newKeyboardShortcuts = value.ToArray();
    }

    public bool KeyboardEscapeClose
    {
      get => _keyboardEscapeClose;
      set => _keyboardEscapeClose = value;
    }

    public bool CollapseDuplicateLogEntries
    {
      get => _collapseDuplicateLogEntries;
      set => _collapseDuplicateLogEntries = value;
    }

    public PinAlignment ProfilerAlignment
    {
      get => _profilerAlignment;
      set
      {
        if (value == PinAlignment.CenterRight || value == PinAlignment.CenterLeft || value == PinAlignment.TopCenter || value == PinAlignment.BottomCenter)
        {
          int num = 0;
          if (num < 4)
            _profilerAlignment = (PinAlignment) num;
          else
            _profilerAlignment = PinAlignment.BottomLeft;
        }
        else
        {
          PinAlignment profilerAlignment = _profilerAlignment;
          _profilerAlignment = value;
        }
      }
    }

    public ConsoleAlignment ConsoleAlignment
    {
      get => _consoleAlignment;
      set => _consoleAlignment = value;
    }

    private static Settings GetOrCreateInstance()
    {
      Settings instance = Resources.Load<Settings>("SRDebugger/Settings");
      if (instance == null)
        instance = CreateInstance<Settings>();
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
      ToggleDockedProfiler = 11,
    }

    [Serializable]
    public sealed class KeyboardShortcut
    {
      [SerializeField]
      public ShortcutActions Action;
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
