using Engine.Common.Components.MessangerStationary;
using Engine.Common.Services;
using Engine.Source.Services;
using UnityEngine;
using UnityEngine.Serialization;

public class Spawnpoint : MonoBehaviour
{
  [SerializeField]
  [FormerlySerializedAs("SpawnpointKind")]
  private SpawnpointKindEnum spawnpointKind;

  public SpawnpointKindEnum SpawnpointKind => spawnpointKind;

  public bool Locked { get; set; }

  private void OnEnable()
  {
    PostmanStaticTeleportService service = ServiceLocator.GetService<PostmanStaticTeleportService>();
    if (service != null)
      service.AddSpawnpoints(this);
    else
      Debug.LogError("PostmanStaticTeleportService not found");
  }

  private void OnDisable()
  {
    ServiceLocator.GetService<PostmanStaticTeleportService>()?.RemoveSpawnpoints(this);
  }
}
