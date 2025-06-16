using UnityEngine;

namespace RootMotion.Dynamics
{
  [AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Muscle Collision Broadcaster")]
  public class MuscleCollisionBroadcaster : MonoBehaviour
  {
    [SerializeField]
    [HideInInspector]
    public PuppetMaster puppetMaster;
    [SerializeField]
    [HideInInspector]
    public int muscleIndex;
    private const string onMuscleHit = "OnMuscleHit";
    private const string onMuscleCollision = "OnMuscleCollision";
    private const string onMuscleCollisionExit = "OnMuscleCollisionExit";
    private MuscleCollisionBroadcaster otherBroadcaster;

    public void Hit(float unPin, Vector3 force, Vector3 position)
    {
      if (!this.enabled)
        return;
      foreach (BehaviourBase behaviour in this.puppetMaster.behaviours)
        behaviour.OnMuscleHit(new MuscleHit(this.muscleIndex, unPin, force, position));
    }

    private void OnCollisionEnter(Collision collision)
    {
      if (!this.enabled || (Object) this.puppetMaster == (Object) null || (Object) collision.collider.transform.root == (Object) this.transform.root)
        return;
      foreach (BehaviourBase behaviour in this.puppetMaster.behaviours)
        behaviour.OnMuscleCollision(new MuscleCollision(this.muscleIndex, collision));
    }

    private void OnCollisionStay(Collision collision)
    {
      if (!this.enabled || (Object) this.puppetMaster == (Object) null || (Object) Singleton<PuppetMasterSettings>.instance != (Object) null && !Singleton<PuppetMasterSettings>.instance.collisionStayMessages || (Object) collision.collider.transform.root == (Object) this.transform.root)
        return;
      foreach (BehaviourBase behaviour in this.puppetMaster.behaviours)
        behaviour.OnMuscleCollision(new MuscleCollision(this.muscleIndex, collision, true));
    }

    private void OnCollisionExit(Collision collision)
    {
      if (!this.enabled || (Object) this.puppetMaster == (Object) null || (Object) Singleton<PuppetMasterSettings>.instance != (Object) null && !Singleton<PuppetMasterSettings>.instance.collisionExitMessages || (Object) collision.collider.transform.root == (Object) this.transform.root)
        return;
      foreach (BehaviourBase behaviour in this.puppetMaster.behaviours)
        behaviour.OnMuscleCollisionExit(new MuscleCollision(this.muscleIndex, collision));
    }
  }
}
