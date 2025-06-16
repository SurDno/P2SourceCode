// Decompiled with JetBrains decompiler
// Type: GroupPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services;
using UnityEngine;

#nullable disable
public class GroupPoint : MonoBehaviour
{
  private bool registered = false;

  private void OnEnable() => this.RegisterInService();

  private void OnDisable()
  {
    GroupPointsService service = ServiceLocator.GetService<GroupPointsService>();
    if (service == null)
      return;
    this.registered = false;
    service.RemovePoint(this);
  }

  private void OnUpdate()
  {
    if (this.registered)
      return;
    this.RegisterInService();
  }

  private void RegisterInService()
  {
    GroupPointsService service = ServiceLocator.GetService<GroupPointsService>();
    if (service == null)
      return;
    this.registered = true;
    service.AddPoint(this);
  }
}
