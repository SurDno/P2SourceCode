// Decompiled with JetBrains decompiler
// Type: FocusEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class FocusEffect : MonoBehaviour
{
  private static MaterialPropertyBlock propertyBlock;
  [SerializeField]
  private Renderer[] renderers;
  private bool initialized = false;
  private DialogIndicationView externalEffect;
  private Coroutine disablingCoroutine;

  private void Disable() => this.enabled = false;

  private void OnEnable()
  {
    if (!this.initialized)
    {
      if (this.renderers == null || this.renderers.Length == 0)
        this.renderers = this.GetComponentsInChildren<Renderer>();
      this.initialized = true;
    }
    if (FocusEffect.propertyBlock == null)
    {
      FocusEffect.propertyBlock = new MaterialPropertyBlock();
      FocusEffect.propertyBlock.SetInt("_FocusEffect", 1);
    }
    for (int index = 0; index < this.renderers.Length; ++index)
      this.renderers[index].SetPropertyBlock(FocusEffect.propertyBlock);
    if ((Object) this.externalEffect == (Object) null)
    {
      this.externalEffect = DialogIndicationView.Create(this.transform);
      if ((Object) this.externalEffect != (Object) null)
      {
        Renderer renderer1 = (Renderer) null;
        float num1 = float.MinValue;
        for (int index = 0; index < this.renderers.Length; ++index)
        {
          Renderer renderer2 = this.renderers[index];
          Vector3 extents = renderer2.bounds.extents;
          float num2 = extents.x * extents.y * extents.z;
          if ((double) num2 > (double) num1)
          {
            renderer1 = renderer2;
            num1 = num2;
          }
        }
        if (renderer1 is MeshRenderer)
          this.externalEffect.SetShape((MeshRenderer) renderer1);
        else if (renderer1 is SkinnedMeshRenderer)
          this.externalEffect.SetShape((SkinnedMeshRenderer) renderer1);
      }
    }
    if (!((Object) this.externalEffect != (Object) null))
      return;
    this.externalEffect.SetVisibility(true);
  }

  private void OnDisable()
  {
    for (int index = 0; index < this.renderers.Length; ++index)
    {
      Renderer renderer = this.renderers[index];
      if ((Object) renderer != (Object) null)
        renderer.SetPropertyBlock((MaterialPropertyBlock) null);
      else
        Debug.LogError((object) "render == null, разобраться");
    }
    if (!((Object) this.externalEffect != (Object) null))
      return;
    this.externalEffect.SetVisibility(false);
  }

  public void SetActive(bool value)
  {
    if (value)
    {
      if (this.enabled)
        this.CancelInvoke("Disable");
      else
        this.enabled = true;
    }
    else if (this.enabled)
      this.Invoke("Disable", 1f);
  }
}
