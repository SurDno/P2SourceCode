using Cofe.Meta;
using Engine.Source.Services.Consoles;

[Initialisable]
public class InputSettingsData : ScriptableObjectInstance<InputSettingsData>
{
  [Header("Player")]
  [GetSetConsoleCommand("walk_speed")]
  public float WalkSpeed;
  [GetSetConsoleCommand("walk_back_speed")]
  public float WalkBackSpeed;
  [GetSetConsoleCommand("run_speed")]
  public float RunSpeed;
  [GetSetConsoleCommand("run_back_speed")]
  public float RunBackSpeed;
  [GetSetConsoleCommand("sneak_speed")]
  public float SneakSpeed;
  [GetSetConsoleCommand("jump_speed")]
  public float JumpSpeed;
  [GetSetConsoleCommand("gravity_multiplier")]
  public float GravityMultiplier;
  [Header("Player fight speeds")]
  [GetSetConsoleCommand("fight_forward_speed")]
  public float FightForwardSpeed;
  [GetSetConsoleCommand("fight_backward_speed")]
  public float FightBackwardSpeed;
  [GetSetConsoleCommand("fight_strafe_speed")]
  public float FightStrafeSpeed;
  [Header("Player fight sneak speeds")]
  [GetSetConsoleCommand("fight_forward_sneak_speed")]
  public float FightForwardSneakSpeed;
  [GetSetConsoleCommand("fight_backward_sneak_speed")]
  public float FightBackwardSneakSpeed;
  [GetSetConsoleCommand("fight_strafe_sneak_speed")]
  public float FightStrafeSneakSpeed;
  [Header("Player fight run speeds")]
  [GetSetConsoleCommand("fight_forward_run_speed")]
  public float FightForwardRunSpeed;
  [GetSetConsoleCommand("fight_backward_run_speed")]
  public float FightBackwardRunSpeed;
  [GetSetConsoleCommand("fight_strafe_run_speed")]
  public float FightStrafeRunSpeed;
}
