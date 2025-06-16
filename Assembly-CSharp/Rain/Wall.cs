// Decompiled with JetBrains decompiler
// Type: Rain.Wall
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Rain
{
  [RequireComponent(typeof (MeshRenderer))]
  public class Wall : MonoBehaviour
  {
    public float speed = 1f;
    private MeshRenderer _renderer;
    private Material _material;
    private float _phase = 0.0f;

    private void Start()
    {
      this._renderer = this.GetComponent<MeshRenderer>();
      this._material = this._renderer.material;
    }

    private void LateUpdate()
    {
      RainManager instance = RainManager.Instance;
      if ((Object) instance == (Object) null || (double) instance.actualRainIntensity <= 0.0)
      {
        this._renderer.enabled = false;
      }
      else
      {
        this.transform.position = instance.PlayerPosition;
        this._phase += Time.deltaTime * this.speed;
        this._phase = Mathf.Repeat(this._phase, 1f);
        if ((double) this._phase > 1.0)
          this._phase -= Mathf.Floor(this._phase);
        this._material.mainTextureOffset = new Vector2(0.0f, this._phase);
        this._material.SetVector("_Params", new Vector4(instance.actualWindVector.x, instance.actualWindVector.y, instance.actualRainIntensity, 0.0f));
        this._renderer.enabled = true;
      }
    }
  }
}
