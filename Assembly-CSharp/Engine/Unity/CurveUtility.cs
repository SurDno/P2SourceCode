﻿using UnityEngine;

namespace Engine.Unity
{
  public static class CurveUtility
  {
    public static bool Bool(AnimationCurve curve, float elapsed)
    {
      return curve.Evaluate(elapsed) > 0.0;
    }

    public static float Float(AnimationCurve curve, float elapsed) => curve.Evaluate(elapsed);

    public static int Int(AnimationCurve curve, float elapsed) => (int) curve.Evaluate(elapsed);

    public static AnimationCurve Invert(AnimationCurve main)
    {
      AnimationCurve animationCurve = new AnimationCurve();
      Keyframe[] keys = main.keys;
      if (keys.Length < 2)
        return null;
      Keyframe key1 = main.keys[keys.Length - 1];
      for (float time = 0.0f; time < (double) key1.time; time += 0.01f)
      {
        Keyframe key2 = new Keyframe();
        key2.time = main.Evaluate(time);
        key2.value = time;
        animationCurve.AddKey(key2);
        if (animationCurve.length > 1)
        {
          int index1 = animationCurve.keys.Length - 2;
          Keyframe key3 = animationCurve.keys[index1] with
          {
            tangentMode = 21
          };
          key3.outTangent = (float) ((key3.value - (double) key2.value) / (key3.time - (double) key2.time));
          animationCurve.MoveKey(index1, key3);
          int index2 = animationCurve.keys.Length - 1;
          Keyframe key4 = animationCurve.keys[index2] with
          {
            tangentMode = 21
          };
          key4.inTangent = (float) ((key4.value - (double) key3.value) / (key4.time - (double) key3.time));
          animationCurve.MoveKey(index2, key4);
        }
      }
      return animationCurve;
    }
  }
}
