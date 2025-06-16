using System.Collections.Generic;

namespace Pathologic.Prototype
{
  public class FightEventManager : MonoBehaviour
  {
    private List<SimpleEvent> eventHandlersOnAttackOrReactionEnds = new List<SimpleEvent>();
    private List<SimpleEvent> eventHandlersOnAttackOrReactionStarts = new List<SimpleEvent>();
    private List<SimpleEvent> eventHandlersOnKlubokFalldownEnd = new List<SimpleEvent>();
    private List<SimpleEvent> eventHandlersOnKlubokFalldownStart = new List<SimpleEvent>();
    private List<SimpleEvent> eventHandlersOnKlubokHitEnd = new List<SimpleEvent>();
    private List<SimpleEvent> eventHandlersOnKlubokIdleEnter = new List<SimpleEvent>();
    private List<SimpleEvent> eventHandlersOnKlubokIdleExit = new List<SimpleEvent>();
    private List<SimpleEvent> eventHandlersOnKlubokJumpBackTransitionEnd = new List<SimpleEvent>();
    private List<SimpleEvent> eventHandlersOnKlubokJumpTransitionEnd = new List<SimpleEvent>();
    private List<SimpleEvent> eventHandlersOnLocomotionStepLastCycleEnter = new List<SimpleEvent>();
    private List<SimpleEvent> eventHandlersOnLocomotionStepLastCycleExit = new List<SimpleEvent>();
    private List<SimpleEvent> eventHandlersOnLocomotionStepsCycleEnter = new List<SimpleEvent>();
    private List<SimpleEvent> eventHandlersOnLocomotionStepsCycleExit = new List<SimpleEvent>();

    public event SimpleEvent OnLocomotionStepsCycleEnterEvent
    {
      add => eventHandlersOnLocomotionStepsCycleEnter.Add(value);
      remove => eventHandlersOnLocomotionStepsCycleEnter.Remove(value);
    }

    public event SimpleEvent OnLocomotionStepsCycleExitEvent
    {
      add => eventHandlersOnLocomotionStepsCycleExit.Add(value);
      remove => eventHandlersOnLocomotionStepsCycleExit.Remove(value);
    }

    public event SimpleEvent OnLocomotionStepLastCycleEnterEvent
    {
      add => eventHandlersOnLocomotionStepLastCycleEnter.Add(value);
      remove => eventHandlersOnLocomotionStepLastCycleEnter.Remove(value);
    }

    public event SimpleEvent OnLocomotionStepLastCycleExitEvent
    {
      add => eventHandlersOnLocomotionStepLastCycleExit.Add(value);
      remove => eventHandlersOnLocomotionStepLastCycleExit.Remove(value);
    }

    public event SimpleEvent OnAttackOrReactionStartsEvent
    {
      add => eventHandlersOnAttackOrReactionStarts.Add(value);
      remove => eventHandlersOnAttackOrReactionStarts.Remove(value);
    }

    public event SimpleEvent OnAttackOrReactionEndsEvent
    {
      add => eventHandlersOnAttackOrReactionEnds.Add(value);
      remove => eventHandlersOnAttackOrReactionEnds.Remove(value);
    }

    public event SimpleEvent OnKlubokIdleEnterEvent
    {
      add => eventHandlersOnKlubokIdleEnter.Add(value);
      remove => eventHandlersOnKlubokIdleEnter.Remove(value);
    }

    public event SimpleEvent OnKlubokIdleExitEvent
    {
      add => eventHandlersOnKlubokIdleExit.Add(value);
      remove => eventHandlersOnKlubokIdleExit.Remove(value);
    }

    public event SimpleEvent OnKlubokJumpTransitionEndEvent
    {
      add => eventHandlersOnKlubokJumpTransitionEnd.Add(value);
      remove => eventHandlersOnKlubokJumpTransitionEnd.Remove(value);
    }

    public event SimpleEvent OnKlubokJumpBackTransitionEndEvent
    {
      add => eventHandlersOnKlubokJumpBackTransitionEnd.Add(value);
      remove => eventHandlersOnKlubokJumpBackTransitionEnd.Remove(value);
    }

    public event SimpleEvent OnKlubokHitEndEvent
    {
      add => eventHandlersOnKlubokHitEnd.Add(value);
      remove => eventHandlersOnKlubokHitEnd.Remove(value);
    }

    public event SimpleEvent OnKlubokFalldownStartEvent
    {
      add => eventHandlersOnKlubokFalldownStart.Add(value);
      remove => eventHandlersOnKlubokFalldownStart.Remove(value);
    }

    public event SimpleEvent OnKlubokFalldownEndEvent
    {
      add => eventHandlersOnKlubokFalldownEnd.Add(value);
      remove => eventHandlersOnKlubokFalldownEnd.Remove(value);
    }

    public void OnLocomotionStepsCycleEnter()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnLocomotionStepsCycleEnter))
        simpleEvent();
    }

    public void OnLocomotionStepsCycleExit()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnLocomotionStepsCycleExit))
        simpleEvent();
    }

    public void OnLocomotionStepLastCycleEnter()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnLocomotionStepLastCycleEnter))
        simpleEvent();
    }

    public void OnLocomotionStepLastCycleExit()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnLocomotionStepLastCycleExit))
        simpleEvent();
    }

    public void OnAttackOrReactionStarts()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnAttackOrReactionStarts))
        simpleEvent();
    }

    public void OnAttackOrReactionEnds()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnAttackOrReactionEnds))
        simpleEvent();
    }

    public void OnKlubokIdleEnter()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnKlubokIdleEnter))
        simpleEvent();
    }

    public void OnKlubokIdleExit()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnKlubokIdleExit))
        simpleEvent();
    }

    public void OnKlubokJumpTransitionEnd()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnKlubokJumpTransitionEnd))
        simpleEvent();
    }

    public void OnKlubokJumpBackTransitionEnd()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnKlubokJumpBackTransitionEnd))
        simpleEvent();
    }

    public void OnKlubokHitEnd()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnKlubokHitEnd))
        simpleEvent();
    }

    public void OnKlubokFalldownStart()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnKlubokFalldownStart))
        simpleEvent();
    }

    public void OnKlubokFalldownEnd()
    {
      foreach (SimpleEvent simpleEvent in new List<SimpleEvent>(eventHandlersOnKlubokFalldownEnd))
        simpleEvent();
    }

    public delegate void SimpleEvent();
  }
}
