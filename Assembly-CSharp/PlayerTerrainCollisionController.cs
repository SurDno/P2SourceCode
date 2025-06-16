using Engine.Behaviours.Components;
using Inspectors;
using UnityEngine;

public class PlayerTerrainCollisionController : MonoBehaviour
{
  [SerializeField]
  private string colliderTag;
  [SerializeField]
  private LayerMask player;
  [SerializeField]
  private LayerMask removeLayer;
  [Inspected]
  private bool entry;
  private bool tmpEntry;
  private bool inited;
  private PlayerMoveController playerMoveController;

  private void Awake()
  {
    playerMoveController = GetComponent<PlayerMoveController>();
    playerMoveController.DisableMovement = true;
  }

  private void OnEnable()
  {
    entry = false;
    inited = false;
  }

  private void OnDisable()
  {
    Physics.IgnoreLayerCollision(player.GetIndex(), removeLayer.GetIndex(), false);
    playerMoveController.DisableMovement = true;
    inited = false;
  }

  private void OnDestroy()
  {
    Physics.IgnoreLayerCollision(player.GetIndex(), removeLayer.GetIndex(), false);
  }

  private void FixedUpdate()
  {
    if (tmpEntry != entry)
    {
      entry = tmpEntry;
      Physics.IgnoreLayerCollision(player.GetIndex(), removeLayer.GetIndex(), entry);
    }
    if (!inited)
    {
      playerMoveController.DisableMovement = false;
      inited = true;
    }
    tmpEntry = false;
  }

  private void OnTriggerStay(Collider other)
  {
    if (!(other.gameObject.tag == colliderTag))
      return;
    tmpEntry = true;
  }
}
