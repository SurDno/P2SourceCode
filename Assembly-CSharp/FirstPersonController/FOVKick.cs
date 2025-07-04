﻿using System;
using System.Collections;
using UnityEngine;

namespace FirstPersonController
{
  [Serializable]
  public class FOVKick
  {
    public Camera Camera;
    public float FOVIncrease = 3f;
    public AnimationCurve IncreaseCurve;
    [HideInInspector]
    public float originalFov;
    public float TimeToDecrease = 1f;
    public float TimeToIncrease = 1f;

    public void Setup(Camera camera)
    {
      CheckStatus(camera);
      Camera = camera;
      originalFov = camera.fieldOfView;
    }

    private void CheckStatus(Camera camera)
    {
      if (camera == null)
        throw new Exception("FOVKick camera is null, please supply the camera to the constructor");
      if (IncreaseCurve == null)
        throw new Exception("FOVKick Increase curve is null, please define the curve for the field of view kicks");
    }

    public void ChangeCamera(Camera camera) => Camera = camera;

    public IEnumerator FOVKickUp()
    {
      float t = Mathf.Abs((Camera.fieldOfView - originalFov) / FOVIncrease);
      while (t < (double) TimeToIncrease)
      {
        Camera.fieldOfView = originalFov + IncreaseCurve.Evaluate(t / TimeToIncrease) * FOVIncrease;
        t += Time.deltaTime;
        yield return new WaitForEndOfFrame();
      }
    }

    public IEnumerator FOVKickDown()
    {
      float t = Mathf.Abs((Camera.fieldOfView - originalFov) / FOVIncrease);
      while (t > 0.0)
      {
        Camera.fieldOfView = originalFov + IncreaseCurve.Evaluate(t / TimeToDecrease) * FOVIncrease;
        t -= Time.deltaTime;
        yield return new WaitForEndOfFrame();
      }
      Camera.fieldOfView = originalFov;
    }
  }
}
