// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Weather.Lightning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace Engine.Behaviours.Weather
{
  [RequireComponent(typeof (Animator))]
  [RequireComponent(typeof (AudioSource))]
  [RequireComponent(typeof (Light))]
  public class Lightning : MonoBehaviour
  {
    private const float soundSpeed = 331f;
    private static int index;
    private Animator animator;
    [SerializeField]
    [FormerlySerializedAs("_Audios")]
    private AudioClip[] audios;
    private AudioSource audioSource;
    private float distance;
    private float elapsed;
    private int flashType;
    private bool isEnded;
    private Light light;
    private float[,] lightningFlash = new float[8, 30]
    {
      {
        0.0f,
        30f,
        190f,
        110f,
        75f,
        50f,
        30f,
        15f,
        200f,
        40f,
        185f,
        25f,
        45f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f
      },
      {
        0.0f,
        60f,
        5f,
        0.0f,
        50f,
        10f,
        0.0f,
        0.0f,
        0.0f,
        (float) byte.MaxValue,
        180f,
        105f,
        100f,
        115f,
        230f,
        120f,
        50f,
        220f,
        170f,
        0.0f,
        145f,
        0.0f,
        45f,
        60f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f
      },
      {
        0.0f,
        25f,
        15f,
        10f,
        200f,
        110f,
        80f,
        65f,
        50f,
        70f,
        245f,
        135f,
        110f,
        245f,
        55f,
        80f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f
      },
      {
        0.0f,
        10f,
        15f,
        20f,
        130f,
        55f,
        20f,
        180f,
        45f,
        75f,
        50f,
        45f,
        90f,
        200f,
        180f,
        110f,
        40f,
        80f,
        245f,
        20f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f
      },
      {
        0.0f,
        (float) byte.MaxValue,
        (float) byte.MaxValue,
        40f,
        (float) byte.MaxValue,
        245f,
        85f,
        35f,
        20f,
        35f,
        30f,
        0.0f,
        45f,
        55f,
        220f,
        85f,
        5f,
        0.0f,
        95f,
        40f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f
      },
      {
        0.0f,
        45f,
        55f,
        220f,
        85f,
        5f,
        0.0f,
        95f,
        40f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f
      },
      {
        0.0f,
        (float) byte.MaxValue,
        (float) byte.MaxValue,
        40f,
        (float) byte.MaxValue,
        245f,
        85f,
        35f,
        20f,
        35f,
        30f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f
      },
      {
        0.0f,
        (float) byte.MaxValue,
        180f,
        105f,
        100f,
        115f,
        230f,
        120f,
        50f,
        220f,
        170f,
        0.0f,
        145f,
        0.0f,
        45f,
        60f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f,
        0.0f
      }
    };

    public void Start()
    {
      this.animator = this.gameObject.GetComponent<Animator>();
      if ((Object) this.animator == (Object) null)
        throw null;
      this.light = this.gameObject.GetComponent<Light>();
      if ((Object) this.light == (Object) null)
        throw null;
      this.audioSource = this.gameObject.GetComponent<AudioSource>();
      if ((Object) this.audioSource == (Object) null)
        throw null;
      UnityEngine.Camera main = UnityEngine.Camera.main;
      if ((Object) main == (Object) null)
        throw null;
      this.distance = (main.transform.position - this.transform.position).magnitude;
      float delay = this.distance / 331f;
      this.audioSource.clip = this.audios[Random.Range(0, this.audios.Length)];
      this.audioSource.PlayDelayed(delay);
      this.flashType = Random.Range(0, this.lightningFlash.GetLength(0));
      this.gameObject.name = "Lightning_" + (object) Lightning.index;
      ++Lightning.index;
    }

    private void Update()
    {
      if (this.isEnded)
      {
        this.gameObject.SetActive(false);
        Object.Destroy((Object) this.gameObject);
      }
      else
      {
        bool flag1 = false;
        float num = 0.0333f;
        if ((double) this.elapsed > (double) ((float) this.lightningFlash.GetLength(1) * num))
        {
          flag1 = true;
          this.light.intensity = 0.0f;
        }
        else
          this.light.intensity = this.lightningFlash[this.flashType, (int) ((double) this.elapsed / (double) num)] / (float) byte.MaxValue;
        bool flag2 = this.animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("End");
        if (flag1 & flag2 && !this.audioSource.isPlaying)
          this.isEnded = true;
        this.elapsed += Time.deltaTime;
        UnityEngine.Camera main = UnityEngine.Camera.main;
        if ((Object) main == (Object) null)
          return;
        this.gameObject.transform.LookAt(this.gameObject.transform.position + main.transform.rotation * Vector3.back, main.transform.rotation * Vector3.up);
      }
    }
  }
}
