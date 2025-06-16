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
    this.playerMoveController = this.GetComponent<PlayerMoveController>();
    this.playerMoveController.DisableMovement = true;
  }

  private void OnEnable()
  {
    this.entry = false;
    this.inited = false;
  }

  private void OnDisable()
  {
    Physics.IgnoreLayerCollision(this.player.GetIndex(), this.removeLayer.GetIndex(), false);
    this.playerMoveController.DisableMovement = true;
    this.inited = false;
  }

  private void OnDestroy()
  {
    Physics.IgnoreLayerCollision(this.player.GetIndex(), this.removeLayer.GetIndex(), false);
  }

  private void FixedUpdate()
  {
    if (this.tmpEntry != this.entry)
    {
      this.entry = this.tmpEntry;
      Physics.IgnoreLayerCollision(this.player.GetIndex(), this.removeLayer.GetIndex(), this.entry);
    }
    if (!this.inited)
    {
      this.playerMoveController.DisableMovement = false;
      this.inited = true;
    }
    this.tmpEntry = false;
  }

  private void OnTriggerStay(Collider other)
  {
    if (!(other.gameObject.tag == this.colliderTag))
      return;
    this.tmpEntry = true;
  }
}
