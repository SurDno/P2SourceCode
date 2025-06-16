using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cofe.Meta;
using Cofe.Utility;
using Engine.Common.Services;
using Engine.Source.Services.Consoles;
using Engine.Source.Settings;
using Inspectors;
using SRDebugger.Internal;
using SRDebugger.Services;
using SRDebugger.UI.Controls;
using SRF;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Tabs
{
  [Initialisable]
  public class ConsoleTabController : SRMonoBehaviour
  {
    private static ConsoleTabController instance;
    private Canvas _consoleCanvas;
    private const int maxCommandCount = 32;
    private const string commandsKey = "LastCommandsKey";
    [Inspected]
    private List<string> commands = new List<string>();
    [Inspected]
    private int commandIndex = -1;
    public InputField StackTraceInputField;
    public RectTransform LayoutContainer;
    public ConsoleEntryView ItemPrefab;
    public ScrollRect ScrollRect;
    private static KeyCode[] keyCodes = (KeyCode[]) Enum.GetValues(typeof (KeyCode));

    protected void Start()
    {
      _consoleCanvas = GetComponent<Canvas>();
      Service.Panel.VisibilityChanged += PanelOnVisibilityChanged;
      ServiceLocator.GetService<ConsoleService>().OnAddedLine += OnAddedLine;
      instance = this;
    }

    private void OnEnable()
    {
      LoadCommands();
      StartCoroutine(SetFocusCoroutine());
      StackTraceInputField.enabled = !Application.isConsolePlatform;
    }

    private void OnDisable() => SaveCommands();

    protected void OnDestroy()
    {
      SaveCommands();
      ServiceLocator.GetService<ConsoleService>().OnAddedLine -= OnAddedLine;
    }

    private void LoadCommands()
    {
      commands.Clear();
      commands.AddRange(PlayerSettings.Instance.GetString("LastCommandsKey").Split('\n').Where(o => !o.IsNullOrEmpty()).Distinct());
      ComputeMaxCommands();
    }

    private void SaveCommands()
    {
      string str = "";
      foreach (string command in commands)
        str = str + command + "\n";
      PlayerSettings.Instance.SetString("LastCommandsKey", str);
      PlayerSettings.Instance.Save();
    }

    private void OnAddedLine(string value)
    {
      CreateItem(value);
      StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
      yield return new WaitForEndOfFrame();
      yield return new WaitForEndOfFrame();
      yield return new WaitForEndOfFrame();
      ScrollRect.verticalNormalizedPosition = 0.0f;
    }

    public IEnumerator ClearCoroutine()
    {
      yield return new WaitForEndOfFrame();
      List<GameObject> list = new List<GameObject>();
      foreach (Transform item in LayoutContainer)
        list.Add(item.gameObject);
      foreach (GameObject item in list)
        Destroy(item);
    }

    public IEnumerator SetFocusCoroutine()
    {
      yield return new WaitForEndOfFrame();
      yield return new WaitForEndOfFrame();
      StackTraceInputField.ActivateInputField();
    }

    private void ExecuteCommand(string data)
    {
      commands.RemoveAll(o => o == data);
      commands.Add(data);
      ComputeMaxCommands();
      commandIndex = -1;
      StackTraceInputField.text = "";
      StackTraceInputField.ActivateInputField();
      ServiceLocator.GetService<ConsoleService>().ExecuteCommand(data);
    }

    private void ComputeMaxCommands()
    {
      while (commands.Count > 32)
        commands.RemoveAt(0);
    }

    private void Update()
    {
      ComputeHackInput();
      ComputeEnter();
      ComputeHistory();
      ComputeScroll();
    }

    private void ComputeScroll()
    {
      if (Input.GetKeyDown(KeyCode.PageUp))
      {
        ScrollRect.verticalNormalizedPosition -= 0.1f;
      }
      else
      {
        if (!Input.GetKeyDown(KeyCode.PageDown))
          return;
        ScrollRect.verticalNormalizedPosition += 0.1f;
      }
    }

    private void ComputeHackInput()
    {
      if (StackTraceInputField.enabled)
        return;
      if (Input.GetKeyDown(KeyCode.Backspace))
      {
        if (StackTraceInputField.text.Length == 0)
          return;
        StackTraceInputField.text = StackTraceInputField.text.Substring(0, StackTraceInputField.text.Length - 1);
      }
      else
      {
        bool flag = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        foreach (KeyCode keyCode in keyCodes)
        {
          if (Input.GetKeyDown(keyCode))
          {
            string str;
            switch (keyCode)
            {
              case KeyCode.Space:
                str = " ";
                break;
              case KeyCode.Quote:
                str = flag ? "\"" : "'";
                break;
              case KeyCode.Comma:
                str = flag ? "<" : ",";
                break;
              case KeyCode.Minus:
                str = flag ? "_" : "-";
                break;
              case KeyCode.Period:
                str = flag ? ">" : ".";
                break;
              case KeyCode.Slash:
                str = flag ? "?" : "/";
                break;
              case KeyCode.Alpha0:
                str = flag ? ")" : "0";
                break;
              case KeyCode.Alpha1:
                str = flag ? "!" : "1";
                break;
              case KeyCode.Alpha2:
                str = flag ? "@" : "2";
                break;
              case KeyCode.Alpha3:
                str = flag ? "#" : "3";
                break;
              case KeyCode.Alpha4:
                str = flag ? "$" : "4";
                break;
              case KeyCode.Alpha5:
                str = flag ? "%" : "5";
                break;
              case KeyCode.Alpha6:
                str = flag ? "^" : "6";
                break;
              case KeyCode.Alpha7:
                str = flag ? "&" : "7";
                break;
              case KeyCode.Alpha8:
                str = flag ? "*" : "8";
                break;
              case KeyCode.Alpha9:
                str = flag ? "(" : "9";
                break;
              case KeyCode.Semicolon:
                str = flag ? ":" : ";";
                break;
              case KeyCode.Equals:
                str = flag ? "+" : "=";
                break;
              case KeyCode.LeftBracket:
                str = flag ? "{" : "[";
                break;
              case KeyCode.Backslash:
                str = flag ? "|" : "\\";
                break;
              case KeyCode.RightBracket:
                str = flag ? "}" : "]";
                break;
              case KeyCode.BackQuote:
                str = flag ? "~" : "`";
                break;
              default:
                str = keyCode.ToString();
                if (!flag)
                {
                  str = str.ToLowerInvariant();
                }
                break;
            }
            if (str.Length == 1)
              StackTraceInputField.text += str;
          }
        }
      }
    }

    private void ComputeEnter()
    {
      if (!Input.GetKeyDown(KeyCode.Return))
        return;
      string text = StackTraceInputField.text;
      if (!text.IsNullOrEmpty())
        ExecuteCommand(text);
    }

    private void ComputeHistory()
    {
      if (commands.Count == 0)
        return;
      if (commandIndex != -1 && commands[commandIndex] != StackTraceInputField.text)
        commandIndex = -1;
      if (Input.GetKeyDown(KeyCode.UpArrow))
      {
        if (commandIndex == 0)
          return;
        if (commandIndex != -1)
          --commandIndex;
        else
          commandIndex = commands.Count - 1;
        StackTraceInputField.text = commands[commandIndex];
      }
      else
      {
        if (!Input.GetKeyDown(KeyCode.DownArrow) || commandIndex == -1 || commandIndex + 1 >= commands.Count)
          return;
        ++commandIndex;
        StackTraceInputField.text = commands[commandIndex];
      }
    }

    private void PanelOnVisibilityChanged(IDebugPanelService debugPanelService, bool visible)
    {
      if (!(_consoleCanvas != null))
        return;
      _consoleCanvas.enabled = visible;
    }

    private void CreateItem(string data)
    {
      ConsoleEntryView consoleEntryView = SRInstantiate.Instantiate(ItemPrefab);
      consoleEntryView.SetData(data);
      consoleEntryView.CachedTransform.SetParent(LayoutContainer, false);
    }

    private void Clear() => StartCoroutine(ClearCoroutine());

    [ConsoleCommand("history")]
    private static string HistoryCommand(string command, ConsoleParameter[] parameters)
    {
      string str = "\nCommand history :\n";
      for (int index = 0; index < instance.commands.Count; ++index)
        str = str + "[" + index + "] " + instance.commands[index] + "\n";
      return str;
    }

    [ConsoleCommand("clear")]
    private static string ClearCommand(string command, ConsoleParameter[] parameters)
    {
      if (instance != null)
        instance.Clear();
      return "";
    }
  }
}
