// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Main.SmallLoading
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.UI.Menu.Main
{
  public class SmallLoading : MonoBehaviour
  {
    private static bool _showBackground = false;
    private static Action<bool> onBackground;
    [SerializeField]
    private GameObject background;

    public static bool showBackground
    {
      get => SmallLoading._showBackground;
      set
      {
        SmallLoading._showBackground = value;
        Action<bool> onBackground = SmallLoading.onBackground;
        if (onBackground == null)
          return;
        onBackground(value);
      }
    }

    private void Awake()
    {
      SmallLoading.onBackground += new Action<bool>(this.OnBackground);
      this.OnBackground(SmallLoading._showBackground);
    }

    private void OnDestroy() => SmallLoading.onBackground -= new Action<bool>(this.OnBackground);

    private void OnBackground(bool active) => this.background.SetActive(active);
  }
}
