using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Saves;
using Engine.Source.Services.Saves;
using Engine.Source.Utility;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services
{
  [SaveDepend(typeof (ISimulation))]
  [GameService(typeof (ForcedDialogService), typeof (IForcedDialogService))]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class ForcedDialogService : 
    IInitialisable,
    IUpdatable,
    IForcedDialogService,
    ISavesController,
    IEntityEventsListener
  {
    [StateSaveProxy]
    [StateLoadProxy()]
    [Inspected]
    protected List<ForcedDialogCharacterInfo> characters = new List<ForcedDialogCharacterInfo>();

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
      characters.Clear();
    }

    public void AddForcedDialog(IEntity character, float distance)
    {
      ISpeakingComponent component = character.GetComponent<ISpeakingComponent>();
      if (component == null)
        Debug.LogWarning(character.Name + ": has no speaking component");
      else if (!component.SpeakAvailable)
      {
        Debug.LogWarning(character.Name + ": speak not availiable");
      }
      else
      {
        RemoveForcedDialog(character);
        ForcedDialogCharacterInfo dialogCharacterInfo = ProxyFactory.Create<ForcedDialogCharacterInfo>();
        dialogCharacterInfo.Character = character;
        dialogCharacterInfo.Distance = distance;
        characters.Add(dialogCharacterInfo);
        ComputeAdded(character);
      }
    }

    public void RemoveForcedDialog(IEntity character)
    {
      int index = 0;
      while (index < characters.Count)
      {
        if (characters[index].Character == character)
          characters.RemoveAt(index);
        else
          ++index;
      }
      ((Entity) character).RemoveListener(this);
    }

    private void OnCharacterDisposed(IEntity sender) => RemoveForcedDialog(sender);

    public void ComputeUpdate()
    {
      if (characters.Count == 0 || !PlayerUtility.IsPlayerCanControlling)
        return;
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      foreach (ForcedDialogCharacterInfo character in characters)
      {
        if (TryStartDialog(player, character))
        {
          RemoveForcedDialog(character.Character);
          break;
        }
      }
    }

    private bool TryStartDialog(IEntity player, ForcedDialogCharacterInfo character)
    {
      if (character == null || character.Character == null || !(character.Character is IEntityView) || ((IEntityView) character.Character).GameObject == null || !((IEntityView) character.Character).GameObject.activeSelf || character.Distance != 0.0 && (((IEntityView) character.Character).Position - ((IEntityView) player).Position).magnitude >= (double) character.Distance || !SameIndoorWithPlayer(player, character))
        return false;
      StartDialog(character.Character);
      return true;
    }

    private bool SameIndoorWithPlayer(IEntity player, ForcedDialogCharacterInfo character)
    {
      LocationItemComponent component1 = player.GetComponent<LocationItemComponent>();
      LocationItemComponent component2 = character.Character.GetComponent<LocationItemComponent>();
      return !component1.IsIndoor ? !component2.IsIndoor : component1.LogicLocation == component2.LogicLocation;
    }

    private void StartDialog(IEntity character)
    {
      InteractableComponent component = character.GetComponent<InteractableComponent>();
      if (component != null)
      {
        IEntity player = ServiceLocator.GetService<ISimulation>().Player;
        component.BeginInteract(player, InteractType.Dialog);
      }
      else
        BlueprintServiceUtility.Start(ScriptableObjectInstance<ResourceFromCodeData>.Instance.DialogBlueprint, character, null, character.GetInfo());
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      XmlElement node = element[TypeUtility.GetTypeName(GetType())];
      if (node == null)
      {
        errorHandler.LogError(TypeUtility.GetTypeName(GetType()) + " node not found , context : " + context);
      }
      else
      {
        XmlNodeDataReader reader = new XmlNodeDataReader(node, context);
        ((ISerializeStateLoad) this).StateLoad(reader, GetType());
        yield break;
      }
    }

    public void Unload() => characters.Clear();

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize(writer, TypeUtility.GetTypeName(GetType()), this);
    }

    private void ComputeAdded(IEntity character)
    {
      ((Entity) character).AddListener(this);
    }

    [OnLoaded]
    private void OnLoaded()
    {
      foreach (ForcedDialogCharacterInfo character in characters)
        ComputeAdded(character.Character);
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.DisposeEvent)
        return;
      OnCharacterDisposed(sender);
    }
  }
}
