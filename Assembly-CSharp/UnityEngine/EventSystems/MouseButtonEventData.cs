// Decompiled with JetBrains decompiler
// Type: UnityEngine.EventSystems.MouseButtonEventData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace UnityEngine.EventSystems
{
  public class MouseButtonEventData
  {
    public PointerEventData.FramePressState buttonState;
    public PointerEventData buttonData;

    public bool PressedThisFrame()
    {
      return this.buttonState == PointerEventData.FramePressState.Pressed || this.buttonState == PointerEventData.FramePressState.PressedAndReleased;
    }

    public bool ReleasedThisFrame()
    {
      return this.buttonState == PointerEventData.FramePressState.Released || this.buttonState == PointerEventData.FramePressState.PressedAndReleased;
    }
  }
}
