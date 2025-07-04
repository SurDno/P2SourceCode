﻿using Engine.Source.Commons;
using Engine.Source.Settings;
using UnityEngine;

namespace Engine.Source.Services
{
  public static class PostmanTeleportUtility
  {
    public static bool IsPointVisibleByPlayer(GameObject playerGameObject, Vector3 point)
    {
      Vector3 to = point - playerGameObject.transform.position;
      if (to.magnitude > 40.0)
        return false;
      to.y = 0.0f;
      return Vector3.Angle(playerGameObject.transform.forward, to) < InstanceByRequest<GraphicsGameSettings>.Instance.FieldOfView.Value * GameCamera.Instance.Camera.aspect / 2.0;
    }
  }
}
