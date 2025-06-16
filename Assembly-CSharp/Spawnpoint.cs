// Decompiled with JetBrains decompiler
// Type: Spawnpoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.MessangerStationary;
using Engine.Common.Services;
using Engine.Source.Services;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class Spawnpoint : MonoBehaviour
{
  [SerializeField]
  [FormerlySerializedAs("SpawnpointKind")]
  private SpawnpointKindEnum spawnpointKind;

  public SpawnpointKindEnum SpawnpointKind => this.spawnpointKind;

  public bool Locked { get; set; }

  private void OnEnable()
  {
    PostmanStaticTeleportService service = ServiceLocator.GetService<PostmanStaticTeleportService>();
    if (service != null)
      service.AddSpawnpoints(this);
    else
      Debug.LogError((object) "PostmanStaticTeleportService not found");
  }

  private void OnDisable()
  {
    ServiceLocator.GetService<PostmanStaticTeleportService>()?.RemoveSpawnpoints(this);
  }
}
