// Decompiled with JetBrains decompiler
// Type: SRDebugger.UI.Tabs.ConsoleTabController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
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
      this._consoleCanvas = this.GetComponent<Canvas>();
      Service.Panel.VisibilityChanged += new Action<IDebugPanelService, bool>(this.PanelOnVisibilityChanged);
      ServiceLocator.GetService<ConsoleService>().OnAddedLine += new Action<string>(this.OnAddedLine);
      ConsoleTabController.instance = this;
    }

    private void OnEnable()
    {
      this.LoadCommands();
      this.StartCoroutine(this.SetFocusCoroutine());
      this.StackTraceInputField.enabled = !Application.isConsolePlatform;
    }

    private void OnDisable() => this.SaveCommands();

    protected void OnDestroy()
    {
      this.SaveCommands();
      ServiceLocator.GetService<ConsoleService>().OnAddedLine -= new Action<string>(this.OnAddedLine);
    }

    private void LoadCommands()
    {
      this.commands.Clear();
      this.commands.AddRange(((IEnumerable<string>) PlayerSettings.Instance.GetString("LastCommandsKey").Split('\n')).Where<string>((Func<string, bool>) (o => !o.IsNullOrEmpty())).Distinct<string>());
      this.ComputeMaxCommands();
    }

    private void SaveCommands()
    {
      string str = "";
      foreach (string command in this.commands)
        str = str + command + "\n";
      PlayerSettings.Instance.SetString("LastCommandsKey", str);
      PlayerSettings.Instance.Save();
    }

    private void OnAddedLine(string value)
    {
      this.CreateItem(value);
      this.StartCoroutine(this.ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
      yield return (object) new WaitForEndOfFrame();
      yield return (object) new WaitForEndOfFrame();
      yield return (object) new WaitForEndOfFrame();
      this.ScrollRect.verticalNormalizedPosition = 0.0f;
    }

    public IEnumerator ClearCoroutine()
    {
      yield return (object) new WaitForEndOfFrame();
      List<GameObject> list = new List<GameObject>();
      foreach (Transform item in (Transform) this.LayoutContainer)
        list.Add(item.gameObject);
      foreach (GameObject item in list)
        UnityEngine.Object.Destroy((UnityEngine.Object) item);
    }

    public IEnumerator SetFocusCoroutine()
    {
      yield return (object) new WaitForEndOfFrame();
      yield return (object) new WaitForEndOfFrame();
      this.StackTraceInputField.ActivateInputField();
    }

    private void ExecuteCommand(string data)
    {
      this.commands.RemoveAll((Predicate<string>) (o => o == data));
      this.commands.Add(data);
      this.ComputeMaxCommands();
      this.commandIndex = -1;
      this.StackTraceInputField.text = "";
      this.StackTraceInputField.ActivateInputField();
      ServiceLocator.GetService<ConsoleService>().ExecuteCommand(data);
    }

    private void ComputeMaxCommands()
    {
      while (this.commands.Count > 32)
        this.commands.RemoveAt(0);
    }

    private void Update()
    {
      this.ComputeHackInput();
      this.ComputeEnter();
      this.ComputeHistory();
      this.ComputeScroll();
    }

    private void ComputeScroll()
    {
      if (Input.GetKeyDown(KeyCode.PageUp))
      {
        this.ScrollRect.verticalNormalizedPosition -= 0.1f;
      }
      else
      {
        if (!Input.GetKeyDown(KeyCode.PageDown))
          return;
        this.ScrollRect.verticalNormalizedPosition += 0.1f;
      }
    }

    private void ComputeHackInput()
    {
      if (this.StackTraceInputField.enabled)
        return;
      if (Input.GetKeyDown(KeyCode.Backspace))
      {
        if (this.StackTraceInputField.text.Length == 0)
          return;
        this.StackTraceInputField.text = this.StackTraceInputField.text.Substring(0, this.StackTraceInputField.text.Length - 1);
      }
      else
      {
        bool flag = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        foreach (KeyCode keyCode in ConsoleTabController.keyCodes)
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
                  break;
                }
                break;
            }
            if (str.Length == 1)
              this.StackTraceInputField.text += str;
          }
        }
      }
    }

    private void ComputeEnter()
    {
      if (!Input.GetKeyDown(KeyCode.Return))
        return;
      string text = this.StackTraceInputField.text;
      if (!text.IsNullOrEmpty())
        this.ExecuteCommand(text);
    }

    private void ComputeHistory()
    {
      if (this.commands.Count == 0)
        return;
      if (this.commandIndex != -1 && this.commands[this.commandIndex] != this.StackTraceInputField.text)
        this.commandIndex = -1;
      if (Input.GetKeyDown(KeyCode.UpArrow))
      {
        if (this.commandIndex == 0)
          return;
        if (this.commandIndex != -1)
          --this.commandIndex;
        else
          this.commandIndex = this.commands.Count - 1;
        this.StackTraceInputField.text = this.commands[this.commandIndex];
      }
      else
      {
        if (!Input.GetKeyDown(KeyCode.DownArrow) || this.commandIndex == -1 || this.commandIndex + 1 >= this.commands.Count)
          return;
        ++this.commandIndex;
        this.StackTraceInputField.text = this.commands[this.commandIndex];
      }
    }

    private void PanelOnVisibilityChanged(IDebugPanelService debugPanelService, bool visible)
    {
      if (!((UnityEngine.Object) this._consoleCanvas != (UnityEngine.Object) null))
        return;
      this._consoleCanvas.enabled = visible;
    }

    private void CreateItem(string data)
    {
      ConsoleEntryView consoleEntryView = SRInstantiate.Instantiate<ConsoleEntryView>(this.ItemPrefab);
      consoleEntryView.SetData(data);
      consoleEntryView.CachedTransform.SetParent((Transform) this.LayoutContainer, false);
    }

    private void Clear() => this.StartCoroutine(this.ClearCoroutine());

    [ConsoleCommand("history")]
    private static string HistoryCommand(string command, ConsoleParameter[] parameters)
    {
      string str = "\nCommand history :\n";
      for (int index = 0; index < ConsoleTabController.instance.commands.Count; ++index)
        str = str + "[" + (object) index + "] " + ConsoleTabController.instance.commands[index] + "\n";
      return str;
    }

    [ConsoleCommand("clear")]
    private static string ClearCommand(string command, ConsoleParameter[] parameters)
    {
      if ((UnityEngine.Object) ConsoleTabController.instance != (UnityEngine.Object) null)
        ConsoleTabController.instance.Clear();
      return "";
    }
  }
}
