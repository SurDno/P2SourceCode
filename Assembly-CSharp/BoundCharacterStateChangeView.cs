using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Audio;
using Engine.Source.Components.BoundCharacters;
using System;
using UnityEngine;

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

  public void ContinueWithoutRoll() => this.Invoke("Reveal", this.delayWithoutRoll);

  private float FakeRoll(float min, float max)
  {
    float num = UnityEngine.Random.value;
    float t = (float) (0.10000000149011612 + (double) (num * num) * 0.89999997615814209);
    return Mathf.LerpUnclamped(min, max, t);
  }

  public void Finish()
  {
    Action finishEvent = this.FinishEvent;
    if (finishEvent == null)
      return;
    finishEvent();
  }

  public void ImmunityRoll(bool success)
  {
    this.hadImmunity.Visible = true;
    this.roll.Set(this.FakeRoll(this.character.PreRollStatValue, success ? 0.0f : 1f), success);
    this.roll.FinishEvent += new Action(this.Reveal);
  }

  public void InfectionRoll(bool success)
  {
    this.hadInfection.Visible = true;
    this.roll.Set(this.FakeRoll(this.character.PreRollStatValue, success ? 1f : 0.0f), success);
    this.roll.FinishEvent += new Action(this.Reveal);
  }

  public void Reveal()
  {
    this.revealed = true;
    this.portrait.SetValue(BoundCharacterUtility.StateLargeSprite(this.character, this.character.BoundHealthState.Value), false);
    this.time = this.roll.Duration;
    if (this.character.PreRollHealthState == BoundHealthStateEnum.Diseased && this.character.PreRollHealthState == BoundHealthStateEnum.Diseased)
      this.stat.FloatValue = this.character.Infection.Value;
    if (this.character.BoundHealthState.Value == BoundHealthStateEnum.Dead)
      this.died.Invoke();
    else if (this.character.BoundHealthState.Value == BoundHealthStateEnum.Diseased)
    {
      if (this.character.PreRollHealthState == BoundHealthStateEnum.Diseased)
        this.survived.Invoke();
      else
        this.infected.Invoke();
    }
    else
    {
      if (this.character.BoundHealthState.Value != BoundHealthStateEnum.Normal || this.character.PreRollHealthState != BoundHealthStateEnum.Danger)
        return;
      this.notInfected.Invoke();
    }
  }

  public void Show(BoundCharacterComponent character)
  {
    this.duration = this.resultDuration + this.roll.Duration;
    this.time = 0.0f;
    this.opacity.FloatValue = 0.0f;
    this.character = character;
    this.portrait.SetValue(BoundCharacterUtility.StateLargeSprite(character, character.PreRollHealthState), true);
    Gender gender = BoundCharacterUtility.GetGender(character);
    this.female.Visible = gender == Gender.Female;
    this.male.Visible = gender == Gender.Male;
    this.nameView.StringValue = ServiceLocator.GetService<LocalizationService>().GetText(character.Name);
    this.medicated.Visible = character.PreRollMedicated;
    this.stat.FloatValue = character.PreRollStatValue;
    this.stat.SkipAnimation();
    if (character.BoundHealthState.Value == BoundHealthStateEnum.Dead)
    {
      if (character.PreRollHealthState == BoundHealthStateEnum.Danger)
        this.ImmunityRoll(false);
      else if (character.PreRollHealthState == BoundHealthStateEnum.Diseased)
        this.InfectionRoll(false);
      else
        this.ContinueWithoutRoll();
    }
    else if (character.BoundHealthState.Value == BoundHealthStateEnum.Diseased)
    {
      if (character.PreRollHealthState == BoundHealthStateEnum.Danger)
        this.ImmunityRoll(false);
      else if (character.PreRollHealthState == BoundHealthStateEnum.Diseased)
        this.InfectionRoll(true);
      else
        this.ContinueWithoutRoll();
    }
    else if (character.BoundHealthState.Value == BoundHealthStateEnum.Normal)
    {
      if (character.PreRollHealthState == BoundHealthStateEnum.Danger)
        this.ImmunityRoll(true);
      else
        this.ContinueWithoutRoll();
    }
    else
      this.ContinueWithoutRoll();
    character.PreRollStateStored = false;
  }

  public void Skip()
  {
    if (!this.revealed)
      this.roll.Skip();
    else
      this.Finish();
  }

  private void Update()
  {
    if ((double) this.time == (double) this.duration)
      return;
    this.time += Time.deltaTime;
    if ((double) this.time > (double) this.duration)
      this.time = this.duration;
    this.opacity.FloatValue = SoundUtility.ComputeFade(this.time, this.duration, this.fadeTime);
    if ((double) this.time != (double) this.duration)
      return;
    this.Finish();
  }
}
