using UnityEngine;

namespace SoundPropagation
{
  public class SPAudioListener : MonoBehaviour
  {
    public static SPAudioListener Instance;
    public LayerMask LayerMask;
    [Range(0.0f, 1f)]
    public float Directionality;
    private Vector3 position;
    private Vector3 direction;
    private SPCell cell;
    private int lastFrame = -1;

    public SPCell Cell
    {
      get
      {
        Check();
        return cell;
      }
    }

    private void Check()
    {
      int frameCount = Time.frameCount;
      if (lastFrame == frameCount)
        return;
      position = transform.position;
      direction = transform.forward * -Directionality;
      cell = SPCell.Find(position, LayerMask);
      lastFrame = frameCount;
    }

    public Vector3 Direction
    {
      get
      {
        Check();
        return direction;
      }
    }

    private void OnDisable()
    {
      cell = null;
      if (!(Instance == this))
        return;
      Instance = null;
    }

    private void OnEnable()
    {
      if (!(Instance == null))
        return;
      Instance = this;
    }

    public Vector3 Position
    {
      get
      {
        Check();
        return position;
      }
    }
  }
}
