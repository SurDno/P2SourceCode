// Decompiled with JetBrains decompiler
// Type: NGSS_ContactShadowsSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class NGSS_ContactShadowsSource : MonoBehaviourInstance<NGSS_ContactShadowsSource>
{
  public Light Light { get; private set; }

  protected override void Awake()
  {
    this.Light = this.GetComponent<Light>();
    base.Awake();
  }
}
