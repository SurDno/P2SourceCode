using System;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Audio;
using Engine.Source.Components.BoundCharacters;

public class BoundCharacterStateChangeView : MonoBehaviour
{
  [SerializeField]
  private SpriteView portrait;
  [SerializeField]
  private StringView nameView;
  [SerializeField]
  private HideableView female;
  [SerializeField]
  private HideableView male;
  [SerializeField]
  private FloatView stat;
  [SerializeField]
  private RollAnimation roll;
  [SerializeField]
  private HideableView hadImmunity;
  [SerializeField]
  private HideableView hadInfection;
  [SerializeField]
  private HideableView medicated;
  [SerializeField]
  private EventView notInfected;
  [SerializeField]
  private EventView infected;
  [SerializeField]
  private EventView survived;
  [SerializeField]
  private EventView died;
  [SerializeField]
  private FloatView opacity;
  [SerializeField]
  private float fadeTime = 1f;
  [SerializeField]
  private float delayWithoutRoll = 2f;
  [SerializeField]
  private float resultDuration = 2f;
  private BoundCharacterComponent character;
  private float duration;
  private float time;
  private bool revealed;

  public event Action FinishEvent;

  public void ContinueWithoutRoll() => this.Invoke("Reveal", delayWithoutRoll);

  private float FakeRoll(float min, float max)
  {
    float num = UnityEngine.Random.value;
    float t = (float) (0.10000000149011612 + num * num * 0.89999997615814209);
    return Mathf.LerpUnclamped(min, max, t);
  }

  public void Finish()
  {
    Action finishEvent = FinishEvent;
    if (finishEvent == null)
      return;
    finishEvent();
  }

  public void ImmunityRoll(bool success)
  {
    hadImmunity.Visible = true;
    roll.Set(FakeRoll(character.PreRollStatValue, success ? 0.0f : 1f), success);
    roll.FinishEvent += Reveal;
  }

  public void InfectionRoll(bool success)
  {
    hadInfection.Visible = true;
    roll.Set(FakeRoll(character.PreRollStatValue, success ? 1f : 0.0f), success);
    roll.FinishEvent += Reveal;
  }

  public void Reveal()
  {
    revealed = true;
    portrait.SetValue(BoundCharacterUtility.StateLargeSprite(character, character.BoundHealthState.Value), false);
    time = roll.Duration;
    if (character.PreRollHealthState == BoundHealthStateEnum.Diseased && character.PreRollHealthState == BoundHealthStateEnum.Diseased)
      stat.FloatValue = character.Infection.Value;
    if (character.BoundHealthState.Value == BoundHealthStateEnum.Dead)
      died.Invoke();
    else if (character.BoundHealthState.Value == BoundHealthStateEnum.Diseased)
    {
      if (character.PreRollHealthState == BoundHealthStateEnum.Diseased)
        survived.Invoke();
      else
        infected.Invoke();
    }
    else
    {
      if (character.BoundHealthState.Value != BoundHealthStateEnum.Normal || character.PreRollHealthState != BoundHealthStateEnum.Danger)
        return;
      notInfected.Invoke();
    }
  }

  public void Show(BoundCharacterComponent character)
  {
    duration = resultDuration + roll.Duration;
    time = 0.0f;
    opacity.FloatValue = 0.0f;
    this.character = character;
    portrait.SetValue(BoundCharacterUtility.StateLargeSprite(character, character.PreRollHealthState), true);
    Gender gender = BoundCharacterUtility.GetGender(character);
    female.Visible = gender == Gender.Female;
    male.Visible = gender == Gender.Male;
    nameView.StringValue = ServiceLocator.GetService<LocalizationService>().GetText(character.Name);
    medicated.Visible = character.PreRollMedicated;
    stat.FloatValue = character.PreRollStatValue;
    stat.SkipAnimation();
    if (character.BoundHealthState.Value == BoundHealthStateEnum.Dead)
    {
      if (character.PreRollHealthState == BoundHealthStateEnum.Danger)
        ImmunityRoll(false);
      else if (character.PreRollHealthState == BoundHealthStateEnum.Diseased)
        InfectionRoll(false);
      else
        ContinueWithoutRoll();
    }
    else if (character.BoundHealthState.Value == BoundHealthStateEnum.Diseased)
    {
      if (character.PreRollHealthState == BoundHealthStateEnum.Danger)
        ImmunityRoll(false);
      else if (character.PreRollHealthState == BoundHealthStateEnum.Diseased)
        InfectionRoll(true);
      else
        ContinueWithoutRoll();
    }
    else if (character.BoundHealthState.Value == BoundHealthStateEnum.Normal)
    {
      if (character.PreRollHealthState == BoundHealthStateEnum.Danger)
        ImmunityRoll(true);
      else
        ContinueWithoutRoll();
    }
    else
      ContinueWithoutRoll();
    character.PreRollStateStored = false;
  }

  public void Skip()
  {
    if (!revealed)
      roll.Skip();
    else
      Finish();
  }

  private void Update()
  {
    if (time == (double) duration)
      return;
    time += Time.deltaTime;
    if (time > (double) duration)
      time = duration;
    opacity.FloatValue = SoundUtility.ComputeFade(time, duration, fadeTime);
    if (time != (double) duration)
      return;
    Finish();
  }
}
