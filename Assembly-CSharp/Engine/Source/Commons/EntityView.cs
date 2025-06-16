// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.EntityView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons
{
  public class EntityView
  {
    [Inspected]
    public GameObject GameObject;

    [Inspected]
    public Vector3 Position { get; set; }

    [Inspected]
    public Quaternion Rotation { get; set; } = Quaternion.identity;

    public event Action OnGameObjectChangedEvent;

    public void InvokeEvent()
    {
      if (this.OnGameObjectChangedEvent == null)
        return;
      foreach (Delegate invocation in this.OnGameObjectChangedEvent.GetInvocationList())
      {
        try
        {
          invocation.DynamicInvoke((object[]) null);
        }
        catch (Exception ex)
        {
          Debug.LogError((object) ("Error invoke listener, target : " + invocation.Target.GetInfo() + " , owner : " + this.GetInfo() + " , ex : " + (object) ex));
        }
      }
    }
  }
}
