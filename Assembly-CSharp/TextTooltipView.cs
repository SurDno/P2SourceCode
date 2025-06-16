// Decompiled with JetBrains decompiler
// Type: TextTooltipView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public abstract class TextTooltipView : MonoBehaviour
{
  public static TextTooltipView Current { get; protected set; }

  public abstract void Show(Vector2 screenPosition, string text);

  public abstract void Hide();
}
