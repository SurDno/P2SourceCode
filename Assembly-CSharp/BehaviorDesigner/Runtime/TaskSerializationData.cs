// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.TaskSerializationData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace BehaviorDesigner.Runtime
{
  [Serializable]
  public class TaskSerializationData
  {
    [SerializeField]
    public string XmlData;
    [SerializeField]
    public List<UnityEngine.Object> unityObjects = new List<UnityEngine.Object>();
  }
}
