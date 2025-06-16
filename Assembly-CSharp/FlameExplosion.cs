// Decompiled with JetBrains decompiler
// Type: FlameExplosion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class FlameExplosion : MonoBehaviour
{
  [SerializeField]
  private FlamePatch flamePatchPrefab;
  [SerializeField]
  private float radius;
  [SerializeField]
  private LayerMask collisionLayers;
  [SerializeField]
  private float rayOriginShift;
  [SerializeField]
  private int rayCount;
  [SerializeField]
  private AnimationCurve strengthAnimation;
  [SerializeField]
  private AnimationCurve decalsAnimation;
  private FlamePatch[] patches;
  private float[] strengths;
  private float time;
  private AudioSource audioSource;

  private void Start()
  {
    this.audioSource = this.GetComponent<AudioSource>();
    Vector3 position = this.transform.position;
    Vector3 origin = position - this.transform.forward * this.rayOriginShift;
    List<FlamePatch> flamePatchList = new List<FlamePatch>();
    List<float> floatList = new List<float>();
    for (int index = 0; index < this.rayCount; ++index)
    {
      Vector3 onUnitSphere = Random.onUnitSphere;
      RaycastHit hitInfo;
      if (Physics.Raycast(origin, onUnitSphere, out hitInfo, this.radius + this.rayOriginShift, (int) this.collisionLayers, QueryTriggerInteraction.Ignore))
      {
        float num1 = Vector3.Distance(hitInfo.point, position);
        if ((double) num1 < (double) this.radius)
        {
          FlamePatch flamePatch = Object.Instantiate<FlamePatch>(this.flamePatchPrefab);
          Transform transform = flamePatch.transform;
          transform.SetParent(this.transform, false);
          transform.position = hitInfo.point;
          transform.rotation = Quaternion.LookRotation(hitInfo.normal, Random.onUnitSphere);
          flamePatch.Initialize(0.0f);
          flamePatchList.Add(flamePatch);
          float num2 = num1 / this.radius;
          floatList.Add((float) (-(double) num2 * (double) num2 * 0.5));
        }
      }
    }
    this.patches = flamePatchList.ToArray();
    this.strengths = floatList.ToArray();
    this.time = 0.0f;
  }

  private void Update()
  {
    this.time += Time.deltaTime;
    float num1 = Mathf.Clamp01(this.strengthAnimation.Evaluate(this.time));
    if ((Object) this.audioSource != (Object) null)
      this.audioSource.volume = num1;
    float num2 = Mathf.Clamp01(this.decalsAnimation.Evaluate(this.time));
    for (int index = 0; index < this.patches.Length; ++index)
    {
      this.patches[index].Strength = this.strengths[index] + num1;
      this.patches[index].DecalOpacity = this.strengths[index] + num2;
    }
    if ((double) this.time < (double) this.decalsAnimation.keys[this.decalsAnimation.keys.Length - 1].time)
      return;
    this.gameObject.SetActive(false);
  }
}
