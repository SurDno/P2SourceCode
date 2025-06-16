// Decompiled with JetBrains decompiler
// Type: SoundPropagation.SPFieldSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace SoundPropagation
{
  [RequireComponent(typeof (AudioSource))]
  public class SPFieldSource : MonoBehaviour
  {
    private static Dictionary<SPFieldSource, SPFieldSource> sources;
    private List<SPFieldPoint> points;
    private AudioSource audioSource;

    private void LateUpdate() => this.UpdatePosition();

    private void UpdatePosition()
    {
      if ((Object) SPAudioListener.Instance == (Object) null)
      {
        this.audioSource.mute = true;
      }
      else
      {
        Vector3 position1 = SPAudioListener.Instance.Position;
        float maxDistance = this.audioSource.maxDistance;
        float num1 = maxDistance * maxDistance;
        Vector3 zero = Vector3.zero;
        float num2 = 0.0f;
        float num3 = maxDistance;
        for (int index = 0; index < this.points.Count; ++index)
        {
          Vector3 position2 = this.points[index].Position;
          if ((double) position1.x + (double) maxDistance > (double) position2.x && (double) position1.x - (double) maxDistance < (double) position2.x && (double) position1.z + (double) maxDistance > (double) position2.z && (double) position1.z - (double) maxDistance < (double) position2.z)
          {
            Vector3 vector3 = position2 - position1;
            float sqrMagnitude = vector3.sqrMagnitude;
            if ((double) sqrMagnitude < (double) num1)
            {
              float num4 = Mathf.Sqrt(sqrMagnitude);
              float num5 = (float) (1.0 - (double) num4 / (double) maxDistance);
              float num6 = num5 * num5;
              zero += vector3 / num4 * num6;
              num2 += num6;
              if ((double) num4 < (double) num3)
                num3 = num4;
            }
          }
        }
        if ((double) num2 > 0.0)
        {
          float magnitude = zero.magnitude;
          if ((double) magnitude == 0.0)
          {
            this.audioSource.spread = 360f;
            this.transform.position = new Vector3(position1.x, position1.y + num3, position1.z);
          }
          else
          {
            this.audioSource.spread = (float) ((1.0 - (double) magnitude / (double) num2) * 360.0);
            this.transform.position = position1 + zero * (num3 / magnitude);
          }
          this.audioSource.mute = false;
        }
        else
          this.audioSource.mute = true;
      }
    }

    public static void AddPoint(SPFieldSource prefab, SPFieldPoint point)
    {
      SPFieldSource spFieldSource = (SPFieldSource) null;
      if (SPFieldSource.sources == null)
        SPFieldSource.sources = new Dictionary<SPFieldSource, SPFieldSource>();
      else
        SPFieldSource.sources.TryGetValue(prefab, out spFieldSource);
      if ((Object) spFieldSource == (Object) null)
      {
        GameObject group = UnityFactory.GetOrCreateGroup("[Sounds]");
        spFieldSource = Object.Instantiate<SPFieldSource>(prefab, group.transform, false);
        spFieldSource.name = prefab.name;
        spFieldSource.audioSource = spFieldSource.GetComponent<AudioSource>();
        spFieldSource.audioSource.dopplerLevel = 0.0f;
        spFieldSource.points = new List<SPFieldPoint>()
        {
          point
        };
        spFieldSource.UpdatePosition();
        SPFieldSource.sources.Add(prefab, spFieldSource);
      }
      else
        spFieldSource.points.Add(point);
    }

    public static void RemovePoint(SPFieldSource prefab, SPFieldPoint point)
    {
      SPFieldSource source = SPFieldSource.sources[prefab];
      source.points.Remove(point);
      if (source.points.Count != 0)
        return;
      SPFieldSource.sources.Remove(prefab);
      Object.Destroy((Object) source.gameObject);
    }
  }
}
