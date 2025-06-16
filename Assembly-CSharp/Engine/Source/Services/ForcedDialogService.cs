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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Engine.Source.Services
{
  [SaveDepend(typeof (ISimulation))]
  [GameService(new System.Type[] {typeof (ForcedDialogService), typeof (IForcedDialogService)})]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class ForcedDialogService : 
    IInitialisable,
    IUpdatable,
    IForcedDialogService,
    ISavesController,
    IEntityEventsListener
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    protected List<ForcedDialogCharacterInfo> characters = new List<ForcedDialogCharacterInfo>();

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      this.characters.Clear();
    }

    public void AddForcedDialog(IEntity character, float distance)
    {
      ISpeakingComponent component = character.GetComponent<ISpeakingComponent>();
      if (component == null)
        Debug.LogWarning((object) (character.Name + ": has no speaking component"));
      else if (!component.SpeakAvailable)
      {
        Debug.LogWarning((object) (character.Name + ": speak not availiable"));
      }
      else
      {
        this.RemoveForcedDialog(character);
        ForcedDialogCharacterInfo dialogCharacterInfo = ProxyFactory.Create<ForcedDialogCharacterInfo>();
        dialogCharacterInfo.Character = character;
        dialogCharacterInfo.Distance = distance;
        this.characters.Add(dialogCharacterInfo);
        this.ComputeAdded(character);
      }
    }

    public void RemoveForcedDialog(IEntity character)
    {
      int index = 0;
      while (index < this.characters.Count)
      {
        if (this.characters[index].Character == character)
          this.characters.RemoveAt(index);
        else
          ++index;
      }
      ((Entity) character).RemoveListener((IEntityEventsListener) this);
    }

    private void OnCharacterDisposed(IEntity sender) => this.RemoveForcedDialog(sender);

    public void ComputeUpdate()
    {
      if (this.characters.Count == 0 || !PlayerUtility.IsPlayerCanControlling)
        return;
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      foreach (ForcedDialogCharacterInfo character in this.characters)
      {
        if (this.TryStartDialog(player, character))
        {
          this.RemoveForcedDialog(character.Character);
          break;
        }
      }
    }

    private bool TryStartDialog(IEntity player, ForcedDialogCharacterInfo character)
    {
      if (character == null || character.Character == null || !(character.Character is IEntityView) || (UnityEngine.Object) ((IEntityView) character.Character).GameObject == (UnityEngine.Object) null || !((IEntityView) character.Character).GameObject.activeSelf || (double) character.Distance != 0.0 && (double) (((IEntityView) character.Character).Position - ((IEntityView) player).Position).magnitude >= (double) character.Distance || !this.SameIndoorWithPlayer(player, character))
        return false;
      this.StartDialog(character.Character);
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
        BlueprintServiceUtility.Start(ScriptableObjectInstance<ResourceFromCodeData>.Instance.DialogBlueprint, character, (Action) null, character.GetInfo());
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      XmlElement node = element[TypeUtility.GetTypeName(this.GetType())];
      if (node == null)
      {
        errorHandler.LogError(TypeUtility.GetTypeName(this.GetType()) + " node not found , context : " + context);
      }
      else
      {
        XmlNodeDataReader reader = new XmlNodeDataReader((XmlNode) node, context);
        ((ISerializeStateLoad) this).StateLoad((IDataReader) reader, this.GetType());
        yield break;
      }
    }

    public void Unload() => this.characters.Clear();

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize<ForcedDialogService>(writer, TypeUtility.GetTypeName(this.GetType()), this);
    }

    private void ComputeAdded(IEntity character)
    {
      ((Entity) character).AddListener((IEntityEventsListener) this);
    }

    [Cofe.Serializations.Data.OnLoaded]
    private void OnLoaded()
    {
      foreach (ForcedDialogCharacterInfo character in this.characters)
        this.ComputeAdded(character.Character);
    }

    public void OnEntityEvent(IEntity sender, EntityEvents kind)
    {
      if (kind != EntityEvents.DisposeEvent)
        return;
      this.OnCharacterDisposed(sender);
    }
  }
}
