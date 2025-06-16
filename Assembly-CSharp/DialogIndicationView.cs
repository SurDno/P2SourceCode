// Decompiled with JetBrains decompiler
// Type: DialogIndicationView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class DialogIndicationView : MonoBehaviour
{
  [SerializeField]
  private ParticleSystem particleSystem;

  public static DialogIndicationView Create(Transform parent)
  {
    DialogIndicationView indicationPrefab = ScriptableObjectInstance<ResourceFromCodeData>.Instance.DialogIndicationPrefab;
    return (Object) indicationPrefab == (Object) null ? (DialogIndicationView) null : Object.Instantiate<DialogIndicationView>(indicationPrefab, parent, false);
  }

  public void SetVisibility(bool value)
  {
    if (!((Object) this.particleSystem != (Object) null))
      return;
    this.particleSystem.emission.enabled = value;
  }

  public void SetShape(SkinnedMeshRenderer renderer)
  {
    if (!((Object) this.particleSystem != (Object) null))
      return;
    ParticleSystem.ShapeModule shape = this.particleSystem.shape with
    {
      shapeType = ParticleSystemShapeType.SkinnedMeshRenderer,
      meshShapeType = ParticleSystemMeshShapeType.Vertex,
      skinnedMeshRenderer = renderer
    };
  }

  public void SetShape(MeshRenderer renderer)
  {
    if (!((Object) this.particleSystem != (Object) null))
      return;
    ParticleSystem.ShapeModule shape = this.particleSystem.shape with
    {
      shapeType = ParticleSystemShapeType.MeshRenderer,
      meshShapeType = ParticleSystemMeshShapeType.Triangle,
      meshRenderer = renderer
    };
    if ((double) renderer.transform.lossyScale.x < 0.0)
      this.particleSystem.GetComponent<ParticleSystemRenderer>().minParticleSize /= 100000f;
  }
}
