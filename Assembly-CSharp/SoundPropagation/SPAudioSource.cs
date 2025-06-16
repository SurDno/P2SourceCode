using System.Collections.Generic;
using UnityEngine;

namespace SoundPropagation
{
  [RequireComponent(typeof (AudioSource))]
  public class SPAudioSource : MonoBehaviour
  {
    [SerializeField]
    private float lowpassStrength = 3f;
    [SerializeField]
    private float volumeSupression = 1.5f;

    public Transform Origin { get; set; }

    private void OnValidate()
    {
      if ((double) this.lowpassStrength < 1.0)
        this.lowpassStrength = 1f;
      if ((double) this.volumeSupression >= 1.0)
        return;
      this.volumeSupression = 1f;
    }

    private void LateUpdate()
    {
      if ((Object) this.Origin == (Object) null)
        this.Origin = this.transform.parent;
      if ((Object) this.Origin == (Object) null)
      {
        Debug.LogError((object) ("SPAudioSource : " + this.name + " : Needs parent transform or custom Origin to work. Disabled for now."));
        this.enabled = false;
      }
      else
      {
        SPAudioListener instance = SPAudioListener.Instance;
        if ((Object) instance == (Object) null)
        {
          Debug.LogError((object) ("SPAudioSource : " + this.name + " : Needs active SPAudioListener. Disabled for now."));
          this.enabled = false;
        }
        else
        {
          bool logPathfinding = false;
          Vector3 position1 = this.Origin.position;
          SPCell originCell = SPCell.Find(position1, instance.LayerMask);
          Vector3 position2 = instance.Position;
          Vector3 direction = instance.Direction;
          SPCell cell = instance.Cell;
          Location location = Locator.Main.GetLocation(originCell, position1, Vector3.zero, cell, position2, direction, this.GetComponent<AudioSource>().maxDistance, logPathfinding);
          if (logPathfinding)
            Debug.Log((object) Locator.Main.ActivePathfinder.Log);
          if (location.PathFound)
          {
            Vector3 vector3_1 = location.NearestCorner - position2;
            vector3_1.Normalize();
            Vector3 vector3_2 = vector3_1 * location.PathLength;
            this.transform.position = position2 + vector3_2;
            AudioLowPassFilter audioLowPassFilter = this.GetComponent<AudioLowPassFilter>();
            if ((Object) audioLowPassFilter == (Object) null)
              audioLowPassFilter = this.gameObject.AddComponent<AudioLowPassFilter>();
            if ((double) location.Filtering.Occlusion > 0.0)
            {
              audioLowPassFilter.enabled = true;
              audioLowPassFilter.cutoffFrequency = 22000f / Mathf.Pow(this.lowpassStrength, location.Filtering.Occlusion);
            }
            else
              audioLowPassFilter.enabled = false;
            this.GetComponent<AudioSource>().volume = 1f / Mathf.Pow(this.volumeSupression, location.Filtering.Occlusion);
            this.GetComponent<AudioSource>().mute = false;
          }
          else
          {
            this.transform.position = position1;
            this.GetComponent<AudioSource>().mute = true;
          }
        }
      }
    }

    private void OnDrawGizmosSelected()
    {
      if (!Application.isPlaying)
        return;
      List<PathPoint> path = Locator.Main.path;
      if (path != null)
      {
        for (int index = 0; index < path.Count; ++index)
        {
          Gizmos.color = new Color(0.0f, 0.75f, 1f, 0.75f);
          Gizmos.DrawWireSphere(path[index].Position, 0.05f);
          Gizmos.DrawLine(path[index].Position, path[index].Position + path[index].Direction);
          if (index > 0)
          {
            Gizmos.color = new Color(0.0f, 0.5f, 1f, 0.75f);
            Gizmos.DrawLine(path[index - 1].Position, path[index].Position);
          }
        }
      }
    }
  }
}
