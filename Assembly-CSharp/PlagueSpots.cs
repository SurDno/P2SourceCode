// Decompiled with JetBrains decompiler
// Type: PlagueSpots
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Behaviours;
using Inspectors;
using UnityEngine;

#nullable disable
public class PlagueSpots : MonoBehaviour, IValueController
{
  private static MaterialPropertyBlock propertyBlock;
  private static int propertyID;
  private float value;

  [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
  public float Value
  {
    get => this.value;
    set
    {
      this.value = Mathf.Clamp01(value);
      this.ApplyLevel();
    }
  }

  private void ApplyLevel()
  {
    if (PlagueSpots.propertyBlock == null)
    {
      PlagueSpots.propertyBlock = new MaterialPropertyBlock();
      PlagueSpots.propertyID = Shader.PropertyToID("_Level");
    }
    PlagueSpots.propertyBlock.SetFloat(PlagueSpots.propertyID, this.value);
    foreach (Renderer componentsInChild in this.GetComponentsInChildren<MeshRenderer>())
      componentsInChild.SetPropertyBlock(PlagueSpots.propertyBlock);
  }
}
