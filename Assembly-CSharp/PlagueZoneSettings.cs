using System;

[CreateAssetMenu(fileName = "New Settings", menuName = "Plague/Zone Settings")]
public class PlagueZoneSettings : ScriptableObject
{
  [Tooltip("Высота над основным слоем с линейно уменьшающейся плотностью")]
  public float falloffHeight;
  public LODLevel[] lodLevels;
  [Tooltip("Высота над землей слоя с полной плотностью")]
  public float mainHeight;
  [Tooltip("Максимальный наклон столбов")]
  public float maxSkew;

  public void CheckValues()
  {
    if (mainHeight < 0.10000000149011612)
      mainHeight = 0.1f;
    if (falloffHeight < 0.10000000149011612)
      falloffHeight = 0.1f;
    for (int index = 0; index < lodLevels.Length; ++index)
    {
      if (lodLevels[index].countPerAre < 0.0)
        lodLevels[index].countPerAre = 0.0f;
      if (lodLevels[index].shaftRadius < 0.10000000149011612)
        lodLevels[index].shaftRadius = 0.1f;
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
