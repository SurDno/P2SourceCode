using BehaviorDesigner.Runtime;
using Engine.Impl.Services;
using Engine.Source.Connections;
using UnityEngine;

public class ResourceFromCodeData : ScriptableObjectInstance<ResourceFromCodeData>
{
  public GameObject DialogCameraPrefab;
  public GameObject DialogRigPrefab;
  public GameObject TradeRigPrefab;
  public DialogLightingProfile DefaultDialogLightingProfile;
  public DialogIndicationView DialogIndicationPrefab;
  public ItemSoundGroup DefaultItemSoundGroup;
  public Texture2D DefaultCursor;
  public IEntitySerializable DropBag;
  public GameObject DialogBlueprint;
  public GameObject ChangeLocationBlueprint;
  public ExternalBehaviorTree POIAI;
  public ExternalBehaviorTree POIAIClosest;
  public ExternalBehaviorTree POIAICrowd;
  public ExternalBehaviorTree POIAIIndoorsCrowd;
  public ExternalBehaviorTree POIAIGroupAnswer;
  public ExternalBehaviorTree POIDialogGoToTarget;
  public ExternalBehaviorTree POIDialogWaitForCharacter;
  public ExternalBehaviorTree POIDialogGoToCharacter;
  public ExternalBehaviorTree POIDialogSpeakToCharacter;
  public ExternalBehaviorTree POIDialogListenToCharacter;
  public ExternalBehaviorTree POIExtraExit;
  public ExternalBehaviorTree[] AdditionalAIToPreload;
  public Material BurnedEffect;
  public GameObject RendererBurn;
  public GameObject JerboaPrefab;
  public SteppeHerbDescription HerbBrownTwyre;
  public SteppeHerbDescription HerbBloodTwyre;
  public SteppeHerbDescription HerbBlackTwyre;
  public SteppeHerbDescription HerbSwevery;
  public GameObject AudioSourceForNpcDialogs;
  public GameObject AudioSourceForNpcCombatReplics;
  public GameObject BottleBomb;
}
