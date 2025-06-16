// Decompiled with JetBrains decompiler
// Type: SoundPropagation.SPAudioListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
        this.Check();
        return this.cell;
      }
    }

    private void Check()
    {
      int frameCount = Time.frameCount;
      if (this.lastFrame == frameCount)
        return;
      this.position = this.transform.position;
      this.direction = this.transform.forward * -this.Directionality;
      this.cell = SPCell.Find(this.position, this.LayerMask);
      this.lastFrame = frameCount;
    }

    public Vector3 Direction
    {
      get
      {
        this.Check();
        return this.direction;
      }
    }

    private void OnDisable()
    {
      this.cell = (SPCell) null;
      if (!((Object) SPAudioListener.Instance == (Object) this))
        return;
      SPAudioListener.Instance = (SPAudioListener) null;
    }

    private void OnEnable()
    {
      if (!((Object) SPAudioListener.Instance == (Object) null))
        return;
      SPAudioListener.Instance = this;
    }

    public Vector3 Position
    {
      get
      {
        this.Check();
        return this.position;
      }
    }
  }
}
