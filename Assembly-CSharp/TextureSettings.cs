// Decompiled with JetBrains decompiler
// Type: TextureSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[CreateAssetMenu(menuName = "Data/Texture Settings")]
public class TextureSettings : ScriptableObject
{
  [SerializeField]
  private float memoryBudget = 512f;
  [SerializeField]
  private int maxLevelReduction = 2;
  [SerializeField]
  private int maxFileIORequests = 1024;
  [SerializeField]
  private int memoryRequirement = 1024;

  public void Apply()
  {
    QualitySettings.streamingMipmapsMemoryBudget = this.memoryBudget;
    QualitySettings.streamingMipmapsMaxLevelReduction = this.maxLevelReduction;
    QualitySettings.streamingMipmapsMaxFileIORequests = this.maxFileIORequests;
  }

  public bool CheckMemoryRequirement() => this.memoryRequirement <= SystemInfo.graphicsMemorySize;
}
