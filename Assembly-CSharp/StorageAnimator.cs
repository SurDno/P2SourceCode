using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class StorageAnimator : MonoBehaviour, IEntityAttachable
{
  [SerializeField]
  [FormerlySerializedAs("infos")]
  private List<StorageAnimator.ContainerInfo> containers = new List<StorageAnimator.ContainerInfo>();
  private StorageComponent storage;

  public void Attach(IEntity owner)
  {
    this.storage = owner.GetComponent<StorageComponent>();
    if (this.storage == null)
      return;
    this.storage.ChangeInventoryEvent += new Action<IStorageComponent, IInventoryComponent>(this.ChangeInventoryEvent);
    this.ComputeContainers();
  }

  public void Detach()
  {
    if (this.storage == null)
      return;
    this.storage.ChangeInventoryEvent -= new Action<IStorageComponent, IInventoryComponent>(this.ChangeInventoryEvent);
    this.storage = (StorageComponent) null;
  }

  private void ChangeInventoryEvent(IStorageComponent storage, IInventoryComponent container)
  {
    this.ComputeContainers();
  }

  private void ComputeContainers()
  {
    foreach (IInventoryComponent container1 in this.storage.Containers)
    {
      IInventoryComponent container = container1;
      StorageAnimator.ContainerInfo containerInfo = this.containers.FirstOrDefault<StorageAnimator.ContainerInfo>((Func<StorageAnimator.ContainerInfo, bool>) (o =>
      {
        if (container.Owner.TemplateId == o.EntityContainer.Id)
          return true;
        return container.Owner.Template != null && container.Owner.Template.TemplateId == o.EntityContainer.Id;
      }));
      if (containerInfo != null)
      {
        foreach (ContainerAnimator animator in containerInfo.Animators)
        {
          if ((UnityEngine.Object) animator != (UnityEngine.Object) null)
            animator.IsOpened = container.OpenState.Value == ContainerOpenStateEnum.Open;
        }
      }
    }
  }

  [Serializable]
  public class ContainerInfo
  {
    public IEntitySerializable EntityContainer;
    public ContainerAnimator[] Animators = new ContainerAnimator[0];
  }
}
