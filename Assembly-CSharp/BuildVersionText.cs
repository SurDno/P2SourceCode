// Decompiled with JetBrains decompiler
// Type: BuildVersionText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using System;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
public class BuildVersionText : MonoBehaviour
{
  private void Start()
  {
    InstanceByRequest<LabelService>.Instance.OnInvalidate += new Action(this.OnInvalidate);
    this.OnInvalidate();
  }

  private void OnInvalidate()
  {
    this.GetComponent<Text>().text = InstanceByRequest<LabelService>.Instance.Label;
  }
}
