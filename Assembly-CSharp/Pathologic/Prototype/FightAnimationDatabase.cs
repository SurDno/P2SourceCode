using System;
using System.Collections.Generic;

namespace Pathologic.Prototype
{
  public static class FightAnimationDatabase
  {
    private static Dictionary<int, FightAnimation> _animations = new Dictionary<int, FightAnimation>();

    public static int FightAnimationCount => _animations.Count;

    static FightAnimationDatabase()
    {
      FightAnimation fightAnimation1 = new FightAnimation();
      fightAnimation1.SpeedScale = 1.2f;
      fightAnimation1.MatchTargetTime = 0.15f;
      fightAnimation1.UnlockTime = 1.40066671f;
      fightAnimation1.AttackPosition = new Vector3(-0.214f, 0.0f, 1.35f);
      fightAnimation1.AttackRotation = Quaternion.AngleAxis(180f, Vector3.up);
      fightAnimation1.HitTimesB.Add(0.8833333f);
      fightAnimation1.HitTimesB.Add(0.9166667f);
      fightAnimation1.HitTimesB.Add(0.95f);
      _animations[0] = fightAnimation1;
      _animations[1] = new FightAnimation {
        SpeedScale = 1.3f,
        MatchTargetTime = 0.3f,
        UnlockTime = 1.16899991f,
        AttackPosition = new Vector3(0.2f, 0.0f, 1f),
        AttackRotation = Quaternion.AngleAxis(180f, Vector3.up)
      };
      FightAnimation fightAnimation2 = new FightAnimation();
      fightAnimation2.SpeedScale = 1f;
      fightAnimation2.MatchTargetTime = 227f * (float) Math.E / 861f;
      fightAnimation2.UnlockTime = 1.295f;
      fightAnimation2.AttackPosition = new Vector3(0.0f, 0.0f, 1.1f);
      fightAnimation2.AttackRotation = Quaternion.AngleAxis(180f, Vector3.up);
      fightAnimation2.HitTimesB.Add(0.8333333f);
      _animations[2] = fightAnimation2;
      Matrix4x4 m = Matrix4x4.TRS(new Vector3(0.3832948f, 0.0f, 1.722896f), Quaternion.Euler(0.17f, 106.544f, -94.325f), new Vector3(1f, 1f, 1f)) * Matrix4x4.Inverse(Matrix4x4.TRS(new Vector3(0.4054076f, 0.0f, -0.4498747f), Quaternion.Euler(-1.515f, -77.57101f, -101.132f), new Vector3(1f, 1f, 1f)));
      FightAnimation fightAnimation3 = new FightAnimation {
        SpeedScale = 1f,
        MatchTargetTime = 227f * (float) Math.E / 861f,
        UnlockTime = 0.0f,
        AttackPosition = GetPosition(m),
        AttackRotation = GetRotation(m)
      };
      fightAnimation3.AttackPosition = new Vector3(0.2f, 0.0f, 1.9f);
      fightAnimation3.AttackRotation = Quaternion.AngleAxis(180f, Vector3.up);
      fightAnimation3.HitTimesB.Add(0.8333333f);
      _animations[1000] = fightAnimation3;
    }

    public static Vector3 GetScale(Matrix4x4 m)
    {
      Vector4 column = m.GetColumn(0);
      double magnitude1 = (double) column.magnitude;
      column = m.GetColumn(1);
      double magnitude2 = (double) column.magnitude;
      column = m.GetColumn(2);
      double magnitude3 = (double) column.magnitude;
      return new Vector3((float) magnitude1, (float) magnitude2, (float) magnitude3);
    }

    public static Vector3 GetPosition(Matrix4x4 m) => new Vector3(m[0, 3], m[1, 3], m[2, 3]);

    public static Quaternion GetRotation(Matrix4x4 m)
    {
      Quaternion rotation = new Quaternion();
      rotation.w = Mathf.Sqrt(Mathf.Max(0.0f, 1f + m[0, 0] + m[1, 1] + m[2, 2])) / 2f;
      rotation.x = Mathf.Sqrt(Mathf.Max(0.0f, 1f + m[0, 0] - m[1, 1] - m[2, 2])) / 2f;
      rotation.y = Mathf.Sqrt(Mathf.Max(0.0f, 1f - m[0, 0] + m[1, 1] - m[2, 2])) / 2f;
      rotation.z = Mathf.Sqrt(Mathf.Max(0.0f, 1f - m[0, 0] - m[1, 1] + m[2, 2])) / 2f;
      rotation.x *= Mathf.Sign(rotation.x * (m[2, 1] - m[1, 2]));
      rotation.y *= Mathf.Sign(rotation.y * (m[0, 2] - m[2, 0]));
      rotation.z *= Mathf.Sign(rotation.z * (m[1, 0] - m[0, 1]));
      return rotation;
    }

    public static FightAnimation Get(int animationType)
    {
      return _animations[animationType];
    }

    public static void FillTransforms(GameObject go)
    {
      foreach (KeyValuePair<int, FightAnimation> animation in _animations)
      {
        FightAnimation fightAnimation = animation.Value;
        GameObject gameObject = new GameObject("AttackTransform" + animation.Key)
        {
          transform = {
            parent = go.transform,
            localPosition = fightAnimation.AttackPosition,
            localRotation = fightAnimation.AttackRotation
          }
        };
      }
    }
  }
}
