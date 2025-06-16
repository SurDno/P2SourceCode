// Decompiled with JetBrains decompiler
// Type: IntroData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using BehaviorDesigner.Runtime;
using UnityEngine;

#nullable disable
public class IntroData : ScriptableObjectInstance<IntroData>
{
  [Header("Suok Circle")]
  public ExternalBehaviorTree SuokSubAI_WaitForUnholster;
  public ExternalBehaviorTree SuokSubAI_WaitForPunch;
  public ExternalBehaviorTree SuokSubAI_WaitForUppercut;
  public ExternalBehaviorTree SuokSubAI_WaitForLowStamina;
  public ExternalBehaviorTree SuokSubAI_BlockExample;
  public ExternalBehaviorTree SuokSubAI_RuinBlockExample;
  public ExternalBehaviorTree SuokSubAI_WaitForStaminaGrows;
  public ExternalBehaviorTree SuokSubAI_WaitForFreeFight;
  public ExternalBehaviorTree SuokSubAI_FreeFight;
  public ExternalBehaviorTree SuokSubAI_FinishedYouWin;
  public ExternalBehaviorTree SuokSubAI_FinishedYouLoose;
  public ExternalBehaviorTree SuokSubAI_FinishedIdle;
  public ExternalBehaviorTree SuokSubAI_Ragdoll;
  [Header("Blueprints")]
  public GameObject SuokNotificationBlueprint;
  public GameObject SuokVoicesBlueprint;
}
