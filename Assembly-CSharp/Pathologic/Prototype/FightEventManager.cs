// Decompiled with JetBrains decompiler
// Type: Pathologic.Prototype.FightEventManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Pathologic.Prototype
{
  public class FightEventManager : MonoBehaviour
  {
    private List<FightEventManager.SimpleEvent> eventHandlersOnAttackOrReactionEnds = new List<FightEventManager.SimpleEvent>();
    private List<FightEventManager.SimpleEvent> eventHandlersOnAttackOrReactionStarts = new List<FightEventManager.SimpleEvent>();
    private List<FightEventManager.SimpleEvent> eventHandlersOnKlubokFalldownEnd = new List<FightEventManager.SimpleEvent>();
    private List<FightEventManager.SimpleEvent> eventHandlersOnKlubokFalldownStart = new List<FightEventManager.SimpleEvent>();
    private List<FightEventManager.SimpleEvent> eventHandlersOnKlubokHitEnd = new List<FightEventManager.SimpleEvent>();
    private List<FightEventManager.SimpleEvent> eventHandlersOnKlubokIdleEnter = new List<FightEventManager.SimpleEvent>();
    private List<FightEventManager.SimpleEvent> eventHandlersOnKlubokIdleExit = new List<FightEventManager.SimpleEvent>();
    private List<FightEventManager.SimpleEvent> eventHandlersOnKlubokJumpBackTransitionEnd = new List<FightEventManager.SimpleEvent>();
    private List<FightEventManager.SimpleEvent> eventHandlersOnKlubokJumpTransitionEnd = new List<FightEventManager.SimpleEvent>();
    private List<FightEventManager.SimpleEvent> eventHandlersOnLocomotionStepLastCycleEnter = new List<FightEventManager.SimpleEvent>();
    private List<FightEventManager.SimpleEvent> eventHandlersOnLocomotionStepLastCycleExit = new List<FightEventManager.SimpleEvent>();
    private List<FightEventManager.SimpleEvent> eventHandlersOnLocomotionStepsCycleEnter = new List<FightEventManager.SimpleEvent>();
    private List<FightEventManager.SimpleEvent> eventHandlersOnLocomotionStepsCycleExit = new List<FightEventManager.SimpleEvent>();

    public event FightEventManager.SimpleEvent OnLocomotionStepsCycleEnterEvent
    {
      add => this.eventHandlersOnLocomotionStepsCycleEnter.Add(value);
      remove => this.eventHandlersOnLocomotionStepsCycleEnter.Remove(value);
    }

    public event FightEventManager.SimpleEvent OnLocomotionStepsCycleExitEvent
    {
      add => this.eventHandlersOnLocomotionStepsCycleExit.Add(value);
      remove => this.eventHandlersOnLocomotionStepsCycleExit.Remove(value);
    }

    public event FightEventManager.SimpleEvent OnLocomotionStepLastCycleEnterEvent
    {
      add => this.eventHandlersOnLocomotionStepLastCycleEnter.Add(value);
      remove => this.eventHandlersOnLocomotionStepLastCycleEnter.Remove(value);
    }

    public event FightEventManager.SimpleEvent OnLocomotionStepLastCycleExitEvent
    {
      add => this.eventHandlersOnLocomotionStepLastCycleExit.Add(value);
      remove => this.eventHandlersOnLocomotionStepLastCycleExit.Remove(value);
    }

    public event FightEventManager.SimpleEvent OnAttackOrReactionStartsEvent
    {
      add => this.eventHandlersOnAttackOrReactionStarts.Add(value);
      remove => this.eventHandlersOnAttackOrReactionStarts.Remove(value);
    }

    public event FightEventManager.SimpleEvent OnAttackOrReactionEndsEvent
    {
      add => this.eventHandlersOnAttackOrReactionEnds.Add(value);
      remove => this.eventHandlersOnAttackOrReactionEnds.Remove(value);
    }

    public event FightEventManager.SimpleEvent OnKlubokIdleEnterEvent
    {
      add => this.eventHandlersOnKlubokIdleEnter.Add(value);
      remove => this.eventHandlersOnKlubokIdleEnter.Remove(value);
    }

    public event FightEventManager.SimpleEvent OnKlubokIdleExitEvent
    {
      add => this.eventHandlersOnKlubokIdleExit.Add(value);
      remove => this.eventHandlersOnKlubokIdleExit.Remove(value);
    }

    public event FightEventManager.SimpleEvent OnKlubokJumpTransitionEndEvent
    {
      add => this.eventHandlersOnKlubokJumpTransitionEnd.Add(value);
      remove => this.eventHandlersOnKlubokJumpTransitionEnd.Remove(value);
    }

    public event FightEventManager.SimpleEvent OnKlubokJumpBackTransitionEndEvent
    {
      add => this.eventHandlersOnKlubokJumpBackTransitionEnd.Add(value);
      remove => this.eventHandlersOnKlubokJumpBackTransitionEnd.Remove(value);
    }

    public event FightEventManager.SimpleEvent OnKlubokHitEndEvent
    {
      add => this.eventHandlersOnKlubokHitEnd.Add(value);
      remove => this.eventHandlersOnKlubokHitEnd.Remove(value);
    }

    public event FightEventManager.SimpleEvent OnKlubokFalldownStartEvent
    {
      add => this.eventHandlersOnKlubokFalldownStart.Add(value);
      remove => this.eventHandlersOnKlubokFalldownStart.Remove(value);
    }

    public event FightEventManager.SimpleEvent OnKlubokFalldownEndEvent
    {
      add => this.eventHandlersOnKlubokFalldownEnd.Add(value);
      remove => this.eventHandlersOnKlubokFalldownEnd.Remove(value);
    }

    public void OnLocomotionStepsCycleEnter()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnLocomotionStepsCycleEnter))
        simpleEvent();
    }

    public void OnLocomotionStepsCycleExit()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnLocomotionStepsCycleExit))
        simpleEvent();
    }

    public void OnLocomotionStepLastCycleEnter()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnLocomotionStepLastCycleEnter))
        simpleEvent();
    }

    public void OnLocomotionStepLastCycleExit()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnLocomotionStepLastCycleExit))
        simpleEvent();
    }

    public void OnAttackOrReactionStarts()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnAttackOrReactionStarts))
        simpleEvent();
    }

    public void OnAttackOrReactionEnds()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnAttackOrReactionEnds))
        simpleEvent();
    }

    public void OnKlubokIdleEnter()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnKlubokIdleEnter))
        simpleEvent();
    }

    public void OnKlubokIdleExit()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnKlubokIdleExit))
        simpleEvent();
    }

    public void OnKlubokJumpTransitionEnd()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnKlubokJumpTransitionEnd))
        simpleEvent();
    }

    public void OnKlubokJumpBackTransitionEnd()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnKlubokJumpBackTransitionEnd))
        simpleEvent();
    }

    public void OnKlubokHitEnd()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnKlubokHitEnd))
        simpleEvent();
    }

    public void OnKlubokFalldownStart()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnKlubokFalldownStart))
        simpleEvent();
    }

    public void OnKlubokFalldownEnd()
    {
      foreach (FightEventManager.SimpleEvent simpleEvent in new List<FightEventManager.SimpleEvent>((IEnumerable<FightEventManager.SimpleEvent>) this.eventHandlersOnKlubokFalldownEnd))
        simpleEvent();
    }

    public delegate void SimpleEvent();
  }
}
