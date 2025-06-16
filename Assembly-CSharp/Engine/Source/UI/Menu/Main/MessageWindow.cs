// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Main.MessageWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using Engine.Impl.UI.Menu.Main;
using UnityEngine;

#nullable disable
namespace Engine.Source.UI.Menu.Main
{
  public class MessageWindow : SimpleWindow, IMessageWindow, IWindow
  {
    [SerializeField]
    private StringView textView;

    protected override void RegisterLayer()
    {
      this.RegisterLayer<IMessageWindow>((IMessageWindow) this);
    }

    public void SetMessage(string text) => this.textView.StringValue = text;
  }
}
