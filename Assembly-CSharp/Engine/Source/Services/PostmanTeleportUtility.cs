// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.PostmanTeleportUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Engine.Source.Settings;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services
{
  public static class PostmanTeleportUtility
  {
    public static bool IsPointVisibleByPlayer(GameObject playerGameObject, Vector3 point)
    {
      Vector3 to = point - playerGameObject.transform.position;
      if ((double) to.magnitude > 40.0)
        return false;
      to.y = 0.0f;
      return (double) Vector3.Angle(playerGameObject.transform.forward, to) < (double) (InstanceByRequest<GraphicsGameSettings>.Instance.FieldOfView.Value * GameCamera.Instance.Camera.aspect) / 2.0;
    }
  }
}
