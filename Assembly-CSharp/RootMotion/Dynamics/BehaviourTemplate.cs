using UnityEngine;

namespace RootMotion.Dynamics
{
  [AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Behaviours/BehaviourTemplate")]
  public class BehaviourTemplate : BehaviourBase
  {
    public SubBehaviourCOM centerOfMass;
    public LayerMask groundLayers;
    public BehaviourBase.PuppetEvent onLoseBalance;
    public float loseBalanceAngle = 60f;

    protected override void OnInitiate()
    {
      this.centerOfMass.Initiate((BehaviourBase) this, this.groundLayers);
    }

    protected override void OnActivate()
    {
    }

    public override void OnReactivate()
    {
    }

    protected override void OnDeactivate()
    {
    }

    protected override void OnFixedUpdate()
    {
      if ((double) this.centerOfMass.angle <= (double) this.loseBalanceAngle)
        return;
      this.onLoseBalance.Trigger(this.puppetMaster);
    }

    protected override void OnLateUpdate()
    {
    }

    protected override void OnMuscleHitBehaviour(MuscleHit hit)
    {
      if (this.enabled)
        ;
    }

    protected override void OnMuscleCollisionBehaviour(MuscleCollision m)
    {
      if (this.enabled)
        ;
    }
  }
}
