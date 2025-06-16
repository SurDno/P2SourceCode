// Decompiled with JetBrains decompiler
// Type: PlagueZoneSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[CreateAssetMenu(fileName = "New Settings", menuName = "Plague/Zone Settings")]
public class PlagueZoneSettings : ScriptableObject
{
  [Tooltip("Высота над основным слоем с линейно уменьшающейся плотностью")]
  public float falloffHeight;
  public PlagueZoneSettings.LODLevel[] lodLevels;
  [Tooltip("Высота над землей слоя с полной плотностью")]
  public float mainHeight;
  [Tooltip("Максимальный наклон столбов")]
  public float maxSkew;

  public void CheckValues()
  {
    if ((double) this.mainHeight < 0.10000000149011612)
      this.mainHeight = 0.1f;
    if ((double) this.falloffHeight < 0.10000000149011612)
      this.falloffHeight = 0.1f;
    for (int index = 0; index < this.lodLevels.Length; ++index)
    {
      if ((double) this.lodLevels[index].countPerAre < 0.0)
        this.lodLevels[index].countPerAre = 0.0f;
      if ((double) this.lodLevels[index].shaftRadius < 0.10000000149011612)
        this.lodLevels[index].shaftRadius = 0.1f;
    }
  }

  [Serializable]
  public class LODLevel
  {
    [Tooltip("Количество столбов на 100 кв. м")]
    public float countPerAre;
    [Tooltip("Не вырезать из видимого меша преграды (дома и т.п.)")]
    public bool ignoreColliders;
    [Tooltip("Граница перехода на этот LOD для объекта «размером» 100 м")]
    public float lodThreshold;
    public Material material;
    [Tooltip("Радиус столбов")]
    public float shaftRadius;
  }
}
