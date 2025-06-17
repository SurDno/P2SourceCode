using System;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.Dynamics
{
  [AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/PuppetMaster Settings")]
  public class PuppetMasterSettings : Singleton<PuppetMasterSettings>
  {
    [Header("Optimizations")]
    public PuppetUpdateLimit kinematicCollidersUpdateLimit = new();
    public PuppetUpdateLimit freeUpdateLimit = new();
    public PuppetUpdateLimit fixedUpdateLimit = new();
    public bool collisionStayMessages = true;
    public bool collisionExitMessages = true;
    public float activePuppetCollisionThresholdMlp;
    private List<PuppetMaster> _puppets = [];

    public int currentlyActivePuppets { get; private set; }

    public int currentlyKinematicPuppets { get; private set; }

    public int currentlyDisabledPuppets { get; private set; }

    public List<PuppetMaster> puppets => _puppets;

    public void Register(PuppetMaster puppetMaster)
    {
      if (_puppets.Contains(puppetMaster))
        return;
      _puppets.Add(puppetMaster);
    }

    public void Unregister(PuppetMaster puppetMaster) => _puppets.Remove(puppetMaster);

    public bool UpdateMoveToTarget(PuppetMaster puppetMaster)
    {
      return kinematicCollidersUpdateLimit.Update(_puppets, puppetMaster);
    }

    public bool UpdateFree(PuppetMaster puppetMaster)
    {
      return freeUpdateLimit.Update(_puppets, puppetMaster);
    }

    public bool UpdateFixed(PuppetMaster puppetMaster)
    {
      return fixedUpdateLimit.Update(_puppets, puppetMaster);
    }

    private void Update()
    {
      currentlyActivePuppets = 0;
      currentlyKinematicPuppets = 0;
      currentlyDisabledPuppets = 0;
      foreach (PuppetMaster puppet in _puppets)
      {
        if (puppet.isActive && puppet.isActiveAndEnabled)
          ++currentlyActivePuppets;
        if (puppet.mode == PuppetMaster.Mode.Kinematic)
          ++currentlyKinematicPuppets;
        if (puppet.mode == PuppetMaster.Mode.Disabled && !puppet.isActive || !puppet.isActiveAndEnabled)
          ++currentlyDisabledPuppets;
      }
      freeUpdateLimit.Step(_puppets.Count);
      kinematicCollidersUpdateLimit.Step(_puppets.Count);
    }

    private void FixedUpdate() => fixedUpdateLimit.Step(_puppets.Count);

    [Serializable]
    public class PuppetUpdateLimit
    {
      [Range(1f, 100f)]
      public int puppetsPerFrame = 100;
      private int index;

      public void Step(int puppetCount)
      {
        index += puppetsPerFrame;
        if (index < puppetCount)
          return;
        index -= puppetCount;
      }

      public bool Update(List<PuppetMaster> puppets, PuppetMaster puppetMaster)
      {
        if (puppetsPerFrame >= puppets.Count)
          return true;
        if (index >= puppets.Count)
          return false;
        for (int index1 = 0; index1 < puppetsPerFrame; ++index1)
        {
          int index2 = index + index1;
          if (index2 >= puppets.Count)
            index2 -= puppets.Count;
          if (puppets[index2] == puppetMaster)
            return true;
        }
        return false;
      }
    }
  }
}
