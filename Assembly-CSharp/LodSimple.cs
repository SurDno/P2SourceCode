// Decompiled with JetBrains decompiler
// Type: LodSimple
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class LodSimple : MonoBehaviour
{
  [SerializeField]
  private float disableDistance = 30f;
  [SerializeField]
  private bool disableRenderers = true;
  [SerializeField]
  private bool disableAnimator = true;

  private void OnEnable()
  {
  }

  private void OnDisable()
  {
  }
}
