// Decompiled with JetBrains decompiler
// Type: UnityEngine.EventSystems.ButtonState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace UnityEngine.EventSystems
{
  public class ButtonState
  {
    private PointerEventData.InputButton m_Button = PointerEventData.InputButton.Left;
    private MouseButtonEventData m_EventData;

    public MouseButtonEventData eventData
    {
      get => this.m_EventData;
      set => this.m_EventData = value;
    }

    public PointerEventData.InputButton button
    {
      get => this.m_Button;
      set => this.m_Button = value;
    }
  }
}
