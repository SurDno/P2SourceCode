// Decompiled with JetBrains decompiler
// Type: RootMotion.Dynamics.PuppetMasterSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace RootMotion.Dynamics
{
  [AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/PuppetMaster Settings")]
  public class PuppetMasterSettings : Singleton<PuppetMasterSettings>
  {
    [Header("Optimizations")]
    public PuppetMasterSettings.PuppetUpdateLimit kinematicCollidersUpdateLimit = new PuppetMasterSettings.PuppetUpdateLimit();
    public PuppetMasterSettings.PuppetUpdateLimit freeUpdateLimit = new PuppetMasterSettings.PuppetUpdateLimit();
    public PuppetMasterSettings.PuppetUpdateLimit fixedUpdateLimit = new PuppetMasterSettings.PuppetUpdateLimit();
    public bool collisionStayMessages = true;
    public bool collisionExitMessages = true;
    public float activePuppetCollisionThresholdMlp = 0.0f;
    private List<PuppetMaster> _puppets = new List<PuppetMaster>();

    public int currentlyActivePuppets { get; private set; }

    public int currentlyKinematicPuppets { get; private set; }

    public int currentlyDisabledPuppets { get; private set; }

    public List<PuppetMaster> puppets => this._puppets;

    public void Register(PuppetMaster puppetMaster)
    {
      if (this._puppets.Contains(puppetMaster))
        return;
      this._puppets.Add(puppetMaster);
    }

    public void Unregister(PuppetMaster puppetMaster) => this._puppets.Remove(puppetMaster);

    public bool UpdateMoveToTarget(PuppetMaster puppetMaster)
    {
      return this.kinematicCollidersUpdateLimit.Update(this._puppets, puppetMaster);
    }

    public bool UpdateFree(PuppetMaster puppetMaster)
    {
      return this.freeUpdateLimit.Update(this._puppets, puppetMaster);
    }

    public bool UpdateFixed(PuppetMaster puppetMaster)
    {
      return this.fixedUpdateLimit.Update(this._puppets, puppetMaster);
    }

    private void Update()
    {
      this.currentlyActivePuppets = 0;
      this.currentlyKinematicPuppets = 0;
      this.currentlyDisabledPuppets = 0;
      foreach (PuppetMaster puppet in this._puppets)
      {
        if (puppet.isActive && puppet.isActiveAndEnabled)
          ++this.currentlyActivePuppets;
        if (puppet.mode == PuppetMaster.Mode.Kinematic)
          ++this.currentlyKinematicPuppets;
        if (puppet.mode == PuppetMaster.Mode.Disabled && !puppet.isActive || !puppet.isActiveAndEnabled)
          ++this.currentlyDisabledPuppets;
      }
      this.freeUpdateLimit.Step(this._puppets.Count);
      this.kinematicCollidersUpdateLimit.Step(this._puppets.Count);
    }

    private void FixedUpdate() => this.fixedUpdateLimit.Step(this._puppets.Count);

    [Serializable]
    public class PuppetUpdateLimit
    {
      [Range(1f, 100f)]
      public int puppetsPerFrame;
      private int index;

      public PuppetUpdateLimit() => this.puppetsPerFrame = 100;

      public void Step(int puppetCount)
      {
        this.index += this.puppetsPerFrame;
        if (this.index < puppetCount)
          return;
        this.index -= puppetCount;
      }

      public bool Update(List<PuppetMaster> puppets, PuppetMaster puppetMaster)
      {
        if (this.puppetsPerFrame >= puppets.Count)
          return true;
        if (this.index >= puppets.Count)
          return false;
        for (int index1 = 0; index1 < this.puppetsPerFrame; ++index1)
        {
          int index2 = this.index + index1;
          if (index2 >= puppets.Count)
            index2 -= puppets.Count;
          if ((UnityEngine.Object) puppets[index2] == (UnityEngine.Object) puppetMaster)
            return true;
        }
        return false;
      }
    }
  }
}
