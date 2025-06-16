// Decompiled with JetBrains decompiler
// Type: TerrainDetailDensities
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(menuName = "Utility/Terrain Detail Densities")]
public class TerrainDetailDensities : ScriptableObject
{
  [Range(0.0f, 1f)]
  public float[] Densities;
}
