using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RootMotion.FinalIK
{
  [HelpURL("https://www.youtube.com/watch?v=r5jiZnsDH3M")]
  [AddComponentMenu("Scripts/RootMotion.FinalIK/Interaction System/Interaction System")]
  public class InteractionSystem : MonoBehaviour
  {
    [Tooltip("If not empty, only the targets with the specified tag will be used by this Interaction System.")]
    public string targetTag = "";
    [Tooltip("The fade in time of the interaction.")]
    public float fadeInTime = 0.3f;
    [Tooltip("The master speed for all interactions.")]
    public float speed = 1f;
    [Tooltip("If > 0, lerps all the FBBIK channels used by the Interaction System back to their default or initial values when not in interaction.")]
    public float resetToDefaultsSpeed = 1f;
    [Header("Triggering")]
    [Tooltip("The collider that registers OnTriggerEnter and OnTriggerExit events with InteractionTriggers.")]
    [FormerlySerializedAs("collider")]
    public Collider characterCollider;
    [Tooltip("Will be used by Interaction Triggers that need the camera's position. Assign the first person view character camera.")]
    [FormerlySerializedAs("camera")]
    public Transform FPSCamera;
    [Tooltip("The layers that will be raycasted from the camera (along camera.forward). All InteractionTrigger look at target colliders should be included.")]
    public LayerMask camRaycastLayers;
    [Tooltip("Max distance of raycasting from the camera.")]
    public float camRaycastDistance = 1f;
    private List<InteractionTrigger> inContact = new List<InteractionTrigger>();
    private List<int> bestRangeIndexes = new List<int>();
    public InteractionSystem.InteractionDelegate OnInteractionStart;
    public InteractionSystem.InteractionDelegate OnInteractionPause;
    public InteractionSystem.InteractionDelegate OnInteractionPickUp;
    public InteractionSystem.InteractionDelegate OnInteractionResume;
    public InteractionSystem.InteractionDelegate OnInteractionStop;
    public InteractionSystem.InteractionEventDelegate OnInteractionEvent;
    public RaycastHit raycastHit;
    [Space(10f)]
    [Tooltip("Reference to the FBBIK component.")]
    [SerializeField]
    private FullBodyBipedIK fullBody;
    [Tooltip("Handles looking at the interactions.")]
    public InteractionLookAt lookAt = new InteractionLookAt();
    private InteractionEffector[] interactionEffectors = new InteractionEffector[9]
    {
      new InteractionEffector(FullBodyBipedEffector.Body),
      new InteractionEffector(FullBodyBipedEffector.LeftFoot),
      new InteractionEffector(FullBodyBipedEffector.LeftHand),
      new InteractionEffector(FullBodyBipedEffector.LeftShoulder),
      new InteractionEffector(FullBodyBipedEffector.LeftThigh),
      new InteractionEffector(FullBodyBipedEffector.RightFoot),
      new InteractionEffector(FullBodyBipedEffector.RightHand),
      new InteractionEffector(FullBodyBipedEffector.RightShoulder),
      new InteractionEffector(FullBodyBipedEffector.RightThigh)
    };
    private bool initiated;
    private Collider lastCollider;
    private Collider c;

    [ContextMenu("TUTORIAL VIDEO (PART 1: BASICS)")]
    private void OpenTutorial1()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=r5jiZnsDH3M");
    }

    [ContextMenu("TUTORIAL VIDEO (PART 2: PICKING UP...)")]
    private void OpenTutorial2()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=eP9-zycoHLk");
    }

    [ContextMenu("TUTORIAL VIDEO (PART 3: ANIMATION)")]
    private void OpenTutorial3()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=sQfB2RcT1T4&index=14&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6");
    }

    [ContextMenu("TUTORIAL VIDEO (PART 4: TRIGGERS)")]
    private void OpenTutorial4()
    {
      Application.OpenURL("https://www.youtube.com/watch?v=-TDZpNjt2mk&index=15&list=PLVxSIA1OaTOu8Nos3CalXbJ2DrKnntMv6");
    }

    [ContextMenu("Support Group")]
    private void SupportGroup()
    {
      Application.OpenURL("https://groups.google.com/forum/#!forum/final-ik");
    }

    [ContextMenu("Asset Store Thread")]
    private void ASThread()
    {
      Application.OpenURL("http://forum.unity3d.com/threads/final-ik-full-body-ik-aim-look-at-fabrik-ccd-ik-1-0-released.222685/");
    }

    public bool inInteraction
    {
      get
      {
        if (!this.IsValid(true))
          return false;
        for (int index = 0; index < this.interactionEffectors.Length; ++index)
        {
          if (this.interactionEffectors[index].inInteraction && !this.interactionEffectors[index].isPaused)
            return true;
        }
        return false;
      }
    }

    public bool IsInInteraction(FullBodyBipedEffector effectorType)
    {
      if (!this.IsValid(true))
        return false;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
      {
        if (this.interactionEffectors[index].effectorType == effectorType)
          return this.interactionEffectors[index].inInteraction && !this.interactionEffectors[index].isPaused;
      }
      return false;
    }

    public bool IsPaused(FullBodyBipedEffector effectorType)
    {
      if (!this.IsValid(true))
        return false;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
      {
        if (this.interactionEffectors[index].effectorType == effectorType)
          return this.interactionEffectors[index].inInteraction && this.interactionEffectors[index].isPaused;
      }
      return false;
    }

    public bool IsPaused()
    {
      if (!this.IsValid(true))
        return false;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
      {
        if (this.interactionEffectors[index].inInteraction && this.interactionEffectors[index].isPaused)
          return true;
      }
      return false;
    }

    public bool IsInSync()
    {
      if (!this.IsValid(true))
        return false;
      for (int index1 = 0; index1 < this.interactionEffectors.Length; ++index1)
      {
        if (this.interactionEffectors[index1].isPaused)
        {
          for (int index2 = 0; index2 < this.interactionEffectors.Length; ++index2)
          {
            if (index2 != index1 && this.interactionEffectors[index2].inInteraction && !this.interactionEffectors[index2].isPaused)
              return false;
          }
        }
      }
      return true;
    }

    public bool StartInteraction(
      FullBodyBipedEffector effectorType,
      InteractionObject interactionObject,
      bool interrupt)
    {
      if (!this.IsValid(true) || (Object) interactionObject == (Object) null)
        return false;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
      {
        if (this.interactionEffectors[index].effectorType == effectorType)
          return this.interactionEffectors[index].Start(interactionObject, this.targetTag, this.fadeInTime, interrupt);
      }
      return false;
    }

    public bool PauseInteraction(FullBodyBipedEffector effectorType)
    {
      if (!this.IsValid(true))
        return false;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
      {
        if (this.interactionEffectors[index].effectorType == effectorType)
          return this.interactionEffectors[index].Pause();
      }
      return false;
    }

    public bool ResumeInteraction(FullBodyBipedEffector effectorType)
    {
      if (!this.IsValid(true))
        return false;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
      {
        if (this.interactionEffectors[index].effectorType == effectorType)
          return this.interactionEffectors[index].Resume();
      }
      return false;
    }

    public bool StopInteraction(FullBodyBipedEffector effectorType)
    {
      if (!this.IsValid(true))
        return false;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
      {
        if (this.interactionEffectors[index].effectorType == effectorType)
          return this.interactionEffectors[index].Stop();
      }
      return false;
    }

    public void PauseAll()
    {
      if (!this.IsValid(true))
        return;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
        this.interactionEffectors[index].Pause();
    }

    public void ResumeAll()
    {
      if (!this.IsValid(true))
        return;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
        this.interactionEffectors[index].Resume();
    }

    public void StopAll()
    {
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
        this.interactionEffectors[index].Stop();
    }

    public InteractionObject GetInteractionObject(FullBodyBipedEffector effectorType)
    {
      if (!this.IsValid(true))
        return (InteractionObject) null;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
      {
        if (this.interactionEffectors[index].effectorType == effectorType)
          return this.interactionEffectors[index].interactionObject;
      }
      return (InteractionObject) null;
    }

    public float GetProgress(FullBodyBipedEffector effectorType)
    {
      if (!this.IsValid(true))
        return 0.0f;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
      {
        if (this.interactionEffectors[index].effectorType == effectorType)
          return this.interactionEffectors[index].progress;
      }
      return 0.0f;
    }

    public float GetMinActiveProgress()
    {
      if (!this.IsValid(true))
        return 0.0f;
      float minActiveProgress = 1f;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
      {
        if (this.interactionEffectors[index].inInteraction)
        {
          float progress = this.interactionEffectors[index].progress;
          if ((double) progress > 0.0 && (double) progress < (double) minActiveProgress)
            minActiveProgress = progress;
        }
      }
      return minActiveProgress;
    }

    public bool TriggerInteraction(int index, bool interrupt)
    {
      if (!this.IsValid(true) || !this.TriggerIndexIsValid(index))
        return false;
      bool flag = true;
      InteractionTrigger.Range range = this.triggersInRange[index].ranges[this.bestRangeIndexes[index]];
      for (int index1 = 0; index1 < range.interactions.Length; ++index1)
      {
        for (int index2 = 0; index2 < range.interactions[index1].effectors.Length; ++index2)
        {
          if (!this.StartInteraction(range.interactions[index1].effectors[index2], range.interactions[index1].interactionObject, interrupt))
            flag = false;
        }
      }
      return flag;
    }

    public bool TriggerInteraction(
      int index,
      bool interrupt,
      out InteractionObject interactionObject)
    {
      interactionObject = (InteractionObject) null;
      if (!this.IsValid(true) || !this.TriggerIndexIsValid(index))
        return false;
      bool flag = true;
      InteractionTrigger.Range range = this.triggersInRange[index].ranges[this.bestRangeIndexes[index]];
      for (int index1 = 0; index1 < range.interactions.Length; ++index1)
      {
        for (int index2 = 0; index2 < range.interactions[index1].effectors.Length; ++index2)
        {
          interactionObject = range.interactions[index1].interactionObject;
          if (!this.StartInteraction(range.interactions[index1].effectors[index2], interactionObject, interrupt))
            flag = false;
        }
      }
      return flag;
    }

    public bool TriggerInteraction(
      int index,
      bool interrupt,
      out InteractionTarget interactionTarget)
    {
      interactionTarget = (InteractionTarget) null;
      if (!this.IsValid(true) || !this.TriggerIndexIsValid(index))
        return false;
      bool flag = true;
      InteractionTrigger.Range range = this.triggersInRange[index].ranges[this.bestRangeIndexes[index]];
      for (int index1 = 0; index1 < range.interactions.Length; ++index1)
      {
        for (int index2 = 0; index2 < range.interactions[index1].effectors.Length; ++index2)
        {
          InteractionObject interactionObject = range.interactions[index1].interactionObject;
          Transform target = interactionObject.GetTarget(range.interactions[index1].effectors[index2], this.tag);
          if ((Object) target != (Object) null)
            interactionTarget = target.GetComponent<InteractionTarget>();
          if (!this.StartInteraction(range.interactions[index1].effectors[index2], interactionObject, interrupt))
            flag = false;
        }
      }
      return flag;
    }

    public InteractionTrigger.Range GetClosestInteractionRange()
    {
      if (!this.IsValid(true))
        return (InteractionTrigger.Range) null;
      int closestTriggerIndex = this.GetClosestTriggerIndex();
      return closestTriggerIndex < 0 || closestTriggerIndex >= this.triggersInRange.Count ? (InteractionTrigger.Range) null : this.triggersInRange[closestTriggerIndex].ranges[this.bestRangeIndexes[closestTriggerIndex]];
    }

    public InteractionObject GetClosestInteractionObjectInRange()
    {
      return this.GetClosestInteractionRange()?.interactions[0].interactionObject;
    }

    public InteractionTarget GetClosestInteractionTargetInRange()
    {
      InteractionTrigger.Range interactionRange = this.GetClosestInteractionRange();
      return interactionRange?.interactions[0].interactionObject.GetTarget(interactionRange.interactions[0].effectors[0], this);
    }

    public InteractionObject[] GetClosestInteractionObjectsInRange()
    {
      InteractionTrigger.Range interactionRange = this.GetClosestInteractionRange();
      if (interactionRange == null)
        return new InteractionObject[0];
      InteractionObject[] interactionObjectsInRange = new InteractionObject[interactionRange.interactions.Length];
      for (int index = 0; index < interactionRange.interactions.Length; ++index)
        interactionObjectsInRange[index] = interactionRange.interactions[index].interactionObject;
      return interactionObjectsInRange;
    }

    public InteractionTarget[] GetClosestInteractionTargetsInRange()
    {
      InteractionTrigger.Range interactionRange = this.GetClosestInteractionRange();
      if (interactionRange == null)
        return new InteractionTarget[0];
      List<InteractionTarget> interactionTargetList = new List<InteractionTarget>();
      foreach (InteractionTrigger.Range.Interaction interaction in interactionRange.interactions)
      {
        foreach (FullBodyBipedEffector effector in interaction.effectors)
          interactionTargetList.Add(interaction.interactionObject.GetTarget(effector, this));
      }
      return interactionTargetList.ToArray();
    }

    public bool TriggerEffectorsReady(int index)
    {
      if (!this.IsValid(true) || !this.TriggerIndexIsValid(index))
        return false;
      for (int index1 = 0; index1 < this.triggersInRange[index].ranges.Length; ++index1)
      {
        InteractionTrigger.Range range = this.triggersInRange[index].ranges[index1];
        for (int index2 = 0; index2 < range.interactions.Length; ++index2)
        {
          for (int index3 = 0; index3 < range.interactions[index2].effectors.Length; ++index3)
          {
            if (this.IsInInteraction(range.interactions[index2].effectors[index3]))
              return false;
          }
        }
        for (int index4 = 0; index4 < range.interactions.Length; ++index4)
        {
          for (int index5 = 0; index5 < range.interactions[index4].effectors.Length; ++index5)
          {
            if (this.IsPaused(range.interactions[index4].effectors[index5]))
            {
              for (int index6 = 0; index6 < range.interactions[index4].effectors.Length; ++index6)
              {
                if (index6 != index5 && !this.IsPaused(range.interactions[index4].effectors[index6]))
                  return false;
              }
            }
          }
        }
      }
      return true;
    }

    public InteractionTrigger.Range GetTriggerRange(int index)
    {
      if (!this.IsValid(true))
        return (InteractionTrigger.Range) null;
      if (index >= 0 && index < this.bestRangeIndexes.Count)
        return this.triggersInRange[index].ranges[this.bestRangeIndexes[index]];
      Warning.Log("Index out of range.", this.transform);
      return (InteractionTrigger.Range) null;
    }

    public int GetClosestTriggerIndex()
    {
      if (!this.IsValid(true) || this.triggersInRange.Count == 0)
        return -1;
      if (this.triggersInRange.Count == 1)
        return 0;
      int closestTriggerIndex = -1;
      float num1 = float.PositiveInfinity;
      for (int index = 0; index < this.triggersInRange.Count; ++index)
      {
        if ((Object) this.triggersInRange[index] != (Object) null)
        {
          float num2 = Vector3.SqrMagnitude(this.triggersInRange[index].transform.position - this.transform.position);
          if ((double) num2 < (double) num1)
          {
            closestTriggerIndex = index;
            num1 = num2;
          }
        }
      }
      return closestTriggerIndex;
    }

    public FullBodyBipedIK ik
    {
      get => this.fullBody;
      set => this.fullBody = value;
    }

    public List<InteractionTrigger> triggersInRange { get; private set; }

    protected virtual void Start()
    {
      if ((Object) this.fullBody == (Object) null)
        this.fullBody = this.GetComponent<FullBodyBipedIK>();
      if ((Object) this.fullBody == (Object) null)
      {
        Warning.Log("InteractionSystem can not find a FullBodyBipedIK component", this.transform);
      }
      else
      {
        IKSolverFullBodyBiped solver1 = this.fullBody.solver;
        solver1.OnPreUpdate = solver1.OnPreUpdate + new IKSolver.UpdateDelegate(this.OnPreFBBIK);
        IKSolverFullBodyBiped solver2 = this.fullBody.solver;
        solver2.OnPostUpdate = solver2.OnPostUpdate + new IKSolver.UpdateDelegate(this.OnPostFBBIK);
        IKSolverFullBodyBiped solver3 = this.fullBody.solver;
        solver3.OnFixTransforms = solver3.OnFixTransforms + new IKSolver.UpdateDelegate(this.OnFixTransforms);
        this.OnInteractionStart += new InteractionSystem.InteractionDelegate(this.LookAtInteraction);
        this.OnInteractionPause += new InteractionSystem.InteractionDelegate(this.InteractionPause);
        this.OnInteractionResume += new InteractionSystem.InteractionDelegate(this.InteractionResume);
        this.OnInteractionStop += new InteractionSystem.InteractionDelegate(this.InteractionStop);
        foreach (InteractionEffector interactionEffector in this.interactionEffectors)
          interactionEffector.Initiate(this);
        this.triggersInRange = new List<InteractionTrigger>();
        this.c = this.GetComponent<Collider>();
        this.UpdateTriggerEventBroadcasting();
        this.initiated = true;
      }
    }

    private void InteractionPause(
      FullBodyBipedEffector effector,
      InteractionObject interactionObject)
    {
      this.lookAt.isPaused = true;
    }

    private void InteractionResume(
      FullBodyBipedEffector effector,
      InteractionObject interactionObject)
    {
      this.lookAt.isPaused = false;
    }

    private void InteractionStop(
      FullBodyBipedEffector effector,
      InteractionObject interactionObject)
    {
      this.lookAt.isPaused = false;
    }

    private void LookAtInteraction(
      FullBodyBipedEffector effector,
      InteractionObject interactionObject)
    {
      this.lookAt.Look(interactionObject.lookAtTarget, Time.time + interactionObject.length * 0.5f);
    }

    public void OnTriggerEnter(Collider c)
    {
      if ((Object) this.fullBody == (Object) null)
        return;
      InteractionTrigger component = c.GetComponent<InteractionTrigger>();
      if ((Object) component == (Object) null || this.inContact.Contains(component))
        return;
      this.inContact.Add(component);
    }

    public void OnTriggerExit(Collider c)
    {
      if ((Object) this.fullBody == (Object) null)
        return;
      InteractionTrigger component = c.GetComponent<InteractionTrigger>();
      if ((Object) component == (Object) null)
        return;
      this.inContact.Remove(component);
    }

    private bool ContactIsInRange(int index, out int bestRangeIndex)
    {
      bestRangeIndex = -1;
      if (!this.IsValid(true))
        return false;
      if (index < 0 || index >= this.inContact.Count)
      {
        Warning.Log("Index out of range.", this.transform);
        return false;
      }
      if ((Object) this.inContact[index] == (Object) null)
      {
        Warning.Log("The InteractionTrigger in the list 'inContact' has been destroyed", this.transform);
        return false;
      }
      bestRangeIndex = this.inContact[index].GetBestRangeIndex(this.transform, this.FPSCamera, this.raycastHit);
      return bestRangeIndex != -1;
    }

    private void OnDrawGizmosSelected()
    {
      if (Application.isPlaying)
        return;
      if ((Object) this.fullBody == (Object) null)
        this.fullBody = this.GetComponent<FullBodyBipedIK>();
      if (!((Object) this.characterCollider == (Object) null))
        return;
      this.characterCollider = this.GetComponent<Collider>();
    }

    private void Update()
    {
      if ((Object) this.fullBody == (Object) null)
        return;
      this.UpdateTriggerEventBroadcasting();
      this.Raycasting();
      this.triggersInRange.Clear();
      this.bestRangeIndexes.Clear();
      for (int index = 0; index < this.inContact.Count; ++index)
      {
        int bestRangeIndex = -1;
        if ((Object) this.inContact[index] != (Object) null && this.inContact[index].gameObject.activeInHierarchy && this.inContact[index].enabled && this.ContactIsInRange(index, out bestRangeIndex))
        {
          this.triggersInRange.Add(this.inContact[index]);
          this.bestRangeIndexes.Add(bestRangeIndex);
        }
      }
      this.lookAt.Update();
    }

    private void Raycasting()
    {
      if ((int) this.camRaycastLayers == -1 || (Object) this.FPSCamera == (Object) null)
        return;
      Physics.Raycast(this.FPSCamera.position, this.FPSCamera.forward, out this.raycastHit, this.camRaycastDistance, (int) this.camRaycastLayers);
    }

    private void UpdateTriggerEventBroadcasting()
    {
      if ((Object) this.characterCollider == (Object) null)
        this.characterCollider = this.c;
      if ((Object) this.characterCollider != (Object) null && (Object) this.characterCollider != (Object) this.c)
      {
        if ((Object) this.characterCollider.GetComponent<TriggerEventBroadcaster>() == (Object) null)
          this.characterCollider.gameObject.AddComponent<TriggerEventBroadcaster>().target = this.gameObject;
        if ((Object) this.lastCollider != (Object) null && (Object) this.lastCollider != (Object) this.c && (Object) this.lastCollider != (Object) this.characterCollider)
        {
          TriggerEventBroadcaster component = this.lastCollider.GetComponent<TriggerEventBroadcaster>();
          if ((Object) component != (Object) null)
            Object.Destroy((Object) component);
        }
      }
      this.lastCollider = this.characterCollider;
    }

    private void UpdateEffectors()
    {
      if ((Object) this.fullBody == (Object) null)
        return;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
        this.interactionEffectors[index].Update(this.transform, this.speed);
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
        this.interactionEffectors[index].ResetToDefaults(this.resetToDefaultsSpeed * this.speed);
    }

    private void OnPreFBBIK()
    {
      if (!this.enabled || (Object) this.fullBody == (Object) null || !this.fullBody.enabled)
        return;
      this.lookAt.SolveSpine();
      this.UpdateEffectors();
    }

    private void OnPostFBBIK()
    {
      if (!this.enabled || (Object) this.fullBody == (Object) null || !this.fullBody.enabled)
        return;
      for (int index = 0; index < this.interactionEffectors.Length; ++index)
        this.interactionEffectors[index].OnPostFBBIK();
      this.lookAt.SolveHead();
    }

    private void OnFixTransforms() => this.lookAt.OnFixTransforms();

    private void OnDestroy()
    {
      if ((Object) this.fullBody == (Object) null)
        return;
      IKSolverFullBodyBiped solver1 = this.fullBody.solver;
      solver1.OnPreUpdate = solver1.OnPreUpdate - new IKSolver.UpdateDelegate(this.OnPreFBBIK);
      IKSolverFullBodyBiped solver2 = this.fullBody.solver;
      solver2.OnPostUpdate = solver2.OnPostUpdate - new IKSolver.UpdateDelegate(this.OnPostFBBIK);
      IKSolverFullBodyBiped solver3 = this.fullBody.solver;
      solver3.OnFixTransforms = solver3.OnFixTransforms - new IKSolver.UpdateDelegate(this.OnFixTransforms);
      this.OnInteractionStart -= new InteractionSystem.InteractionDelegate(this.LookAtInteraction);
      this.OnInteractionPause -= new InteractionSystem.InteractionDelegate(this.InteractionPause);
      this.OnInteractionResume -= new InteractionSystem.InteractionDelegate(this.InteractionResume);
      this.OnInteractionStop -= new InteractionSystem.InteractionDelegate(this.InteractionStop);
    }

    private bool IsValid(bool log)
    {
      if ((Object) this.fullBody == (Object) null)
      {
        if (log)
          Warning.Log("FBBIK is null. Will not update the InteractionSystem", this.transform);
        return false;
      }
      if (this.initiated)
        return true;
      if (log)
        Warning.Log("The InteractionSystem has not been initiated yet.", this.transform);
      return false;
    }

    private bool TriggerIndexIsValid(int index)
    {
      if (index < 0 || index >= this.triggersInRange.Count)
      {
        Warning.Log("Index out of range.", this.transform);
        return false;
      }
      if (!((Object) this.triggersInRange[index] == (Object) null))
        return true;
      Warning.Log("The InteractionTrigger in the list 'inContact' has been destroyed", this.transform);
      return false;
    }

    [ContextMenu("User Manual")]
    private void OpenUserManual()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/page10.html");
    }

    [ContextMenu("Scrpt Reference")]
    private void OpenScriptReference()
    {
      Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_interaction_system.html");
    }

    public delegate void InteractionDelegate(
      FullBodyBipedEffector effectorType,
      InteractionObject interactionObject);

    public delegate void InteractionEventDelegate(
      FullBodyBipedEffector effectorType,
      InteractionObject interactionObject,
      InteractionObject.InteractionEvent interactionEvent);
  }
}
