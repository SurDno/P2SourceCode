using System.Collections.Generic;
using Engine.Source.Connections;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCSoundBankDescription", menuName = "Pathologic2/NPC SoundBank Description", order = 101)]
public class NPCSoundBankDescription : ScriptableObject
{
  public AudioClip[] RagdollFallSounds;
  [Tooltip("Проигрываются при попадании по NPC")]
  public AudioClip[] HittedVocalSounds;
  public AudioClip[] HittedLowStaminaVocalSounds;
  public AudioClip[] HittedDodgeVocalSounds;
  public AudioClip[] StaggerVocalSounds;
  public AudioClip[] StaggerNonVocalSounds;
  [Tooltip("Отходы")]
  public AudioClip[] StrafeLeftSounds;
  public AudioClip[] StrafeRightSounds;
  public AudioClip[] StepBackSounds;
  [Tooltip("Удар попал в быстрый блок")]
  public AudioClip[] QuickBlockHittedSounds;
  [Tooltip("Удар попал в стационарный блок")]
  public AudioClip[] BlockHittedSounds;
  [Tooltip("Удар попал в лицо (по открытому телу)")]
  public AudioClip[] FaceHittedSounds;
  [Tooltip("Удар ножом попал в тело")]
  public AudioClip[] StabSounds;
  [Tooltip("Удар ножом попал в блок")]
  public AudioClip[] StabBlockSounds;
  [Tooltip("Попадание пули")]
  public AudioClip[] BulletHitSounds;
  public List<NPCSoundBankCrySettings> CombatCries;
  public List<NPCSoundBankDialogObject> CivilTalks;
  public NPCSoundBankDialogRoleEnum DialogRole;
  public LipSyncObjectSerializable Poem;
}
