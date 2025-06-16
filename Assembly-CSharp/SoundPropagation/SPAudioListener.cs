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
      position = this.transform.position;
      direction = this.transform.forward * -Directionality;
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
      if (!((Object) Instance == (Object) this))
        return;
      Instance = null;
    }

    private void OnEnable()
    {
      if (!((Object) Instance == (Object) null))
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
