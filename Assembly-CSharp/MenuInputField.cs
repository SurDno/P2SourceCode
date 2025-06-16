// Decompiled with JetBrains decompiler
// Type: MenuInputField
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
public class MenuInputField : MonoBehaviour
{
  [SerializeField]
  private InputField inputField;
  [SerializeField]
  private StringView messageView;
  [SerializeField]
  private StringView placeholderView;

  public event Action<string> SendEvent;

  private void Awake()
  {
    this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
  }

  public void ClearValue() => this.inputField.text = string.Empty;

  private void OnEnable()
  {
    this.ClearValue();
    this.SetMessage(string.Empty);
  }

  private void OnEndEdit(string value)
  {
    if (!Input.GetKey(KeyCode.Return) && !Input.GetKey(KeyCode.KeypadEnter))
      return;
    Action<string> sendEvent = this.SendEvent;
    if (sendEvent == null)
      return;
    sendEvent(value);
  }

  public void SetPlaceholder(string value)
  {
    if (!((UnityEngine.Object) this.placeholderView != (UnityEngine.Object) null))
      return;
    this.placeholderView.StringValue = value;
  }

  public void SetMessage(string value)
  {
    if (!((UnityEngine.Object) this.messageView != (UnityEngine.Object) null))
      return;
    this.messageView.StringValue = value;
  }
}
