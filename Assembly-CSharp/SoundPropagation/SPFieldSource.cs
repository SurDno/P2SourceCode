using System.Collections.Generic;
using UnityEngine;

namespace SoundPropagation
{
  [RequireComponent(typeof (AudioSource))]
  public class SPFieldSource : MonoBehaviour
  {
    private static Dictionary<SPFieldSource, SPFieldSource> sources;
    private List<SPFieldPoint> points;
    private AudioSource audioSource;

    private void LateUpdate() => UpdatePosition();

    private void UpdatePosition()
    {
      if (SPAudioListener.Instance == null)
      {
        audioSource.mute = true;
      }
      else
      {
        Vector3 position1 = SPAudioListener.Instance.Position;
        float maxDistance = audioSource.maxDistance;
        float num1 = maxDistance * maxDistance;
        Vector3 zero = Vector3.zero;
        float num2 = 0.0f;
        float num3 = maxDistance;
        for (int index = 0; index < points.Count; ++index)
        {
          Vector3 position2 = points[index].Position;
          if (position1.x + (double) maxDistance > position2.x && position1.x - (double) maxDistance < position2.x && position1.z + (double) maxDistance > position2.z && position1.z - (double) maxDistance < position2.z)
          {
            Vector3 vector3 = position2 - position1;
            float sqrMagnitude = vector3.sqrMagnitude;
            if (sqrMagnitude < (double) num1)
            {
              float num4 = Mathf.Sqrt(sqrMagnitude);
              float num5 = (float) (1.0 - num4 / (double) maxDistance);
              float num6 = num5 * num5;
              zero += vector3 / num4 * num6;
              num2 += num6;
              if (num4 < (double) num3)
                num3 = num4;
            }
          }
        }
        if (num2 > 0.0)
        {
          float magnitude = zero.magnitude;
          if (magnitude == 0.0)
          {
            audioSource.spread = 360f;
            transform.position = new Vector3(position1.x, position1.y + num3, position1.z);
          }
          else
          {
            audioSource.spread = (float) ((1.0 - magnitude / (double) num2) * 360.0);
            transform.position = position1 + zero * (num3 / magnitude);
          }
          audioSource.mute = false;
        }
        else
          audioSource.mute = true;
      }
    }

    public static void AddPoint(SPFieldSource prefab, SPFieldPoint point)
    {
      SPFieldSource spFieldSource = null;
      if (sources == null)
        sources = new Dictionary<SPFieldSource, SPFieldSource>();
      else
        sources.TryGetValue(prefab, out spFieldSource);
      if (spFieldSource == null)
      {
        GameObject group = UnityFactory.GetOrCreateGroup("[Sounds]");
        spFieldSource = Instantiate(prefab, group.transform, false);
        spFieldSource.name = prefab.name;
        spFieldSource.audioSource = spFieldSource.GetComponent<AudioSource>();
        spFieldSource.audioSource.dopplerLevel = 0.0f;
        spFieldSource.points = new List<SPFieldPoint> {
          point
        };
        spFieldSource.UpdatePosition();
        sources.Add(prefab, spFieldSource);
      }
      else
        spFieldSource.points.Add(point);
    }

    public static void RemovePoint(SPFieldSource prefab, SPFieldPoint point)
    {
      SPFieldSource source = sources[prefab];
      source.points.Remove(point);
      if (source.points.Count != 0)
        return;
      sources.Remove(prefab);
      Destroy(source.gameObject);
    }
  }
}
