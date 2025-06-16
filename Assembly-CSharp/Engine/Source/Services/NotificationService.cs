// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.NotificationService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Saves;
using Engine.Source.Services.Notifications;
using Engine.Source.Services.Saves;
using Engine.Source.UI;
using Inspectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new System.Type[] {typeof (INotificationService), typeof (NotificationService)})]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class NotificationService : 
    INotificationService,
    IUpdatable,
    IInitialisable,
    ISavesController
  {
    [Inspected]
    private Dictionary<NotificationEnum, NotificationLayerItem> layers = new Dictionary<NotificationEnum, NotificationLayerItem>();
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    protected List<NotificationEnum> blockedTypes = new List<NotificationEnum>();

    public void AddNotify(NotificationEnum type, params object[] values)
    {
      if (this.TypeOrLayerBlocked(type))
        return;
      NotificationLayerItem layer = this.GetLayer(type);
      if (this.IgnoreSimilar(type) && (layer.Notifaction != null && layer.Notifaction.Type == type || layer.Queue.Any<NotificationItem>((Func<NotificationItem, bool>) (o => o.Type == type))))
        return;
      layer.Queue.Add(new NotificationItem()
      {
        Type = type,
        Values = values
      });
    }

    public void BlockType(NotificationEnum type)
    {
      NotificationEnum notificationEnum = type;
      if (this.blockedTypes.Contains(notificationEnum))
        return;
      this.blockedTypes.Add(notificationEnum);
    }

    public void UnblockType(NotificationEnum type) => this.blockedTypes.Remove(type);

    private bool TypeBlocked(NotificationEnum type) => this.blockedTypes.Contains(type);

    public bool TypeOrLayerBlocked(NotificationEnum type)
    {
      return this.TypeBlocked(type) || this.TypeBlocked(this.GetLayerEnum(type));
    }

    private bool IgnoreSimilar(NotificationEnum type)
    {
      switch (this.GetLayerEnum(type))
      {
        case NotificationEnum.Tooltip_Layer:
        case NotificationEnum.Item_Layer:
        case NotificationEnum.MindMap_Layer:
          return false;
        default:
          return true;
      }
    }

    private bool UpdateLayerOnPause(NotificationEnum layer)
    {
      return layer == NotificationEnum.Tooltip_Layer;
    }

    public void RemoveNotify(NotificationEnum type)
    {
      NotificationLayerItem layer = this.GetLayer(type);
      if (layer.Notifaction != null && layer.Notifaction.Type == type)
      {
        layer.Notifaction.Shutdown();
        layer.Notifaction = (INotification) null;
      }
      for (int index = layer.Queue.Count - 1; index >= 0; --index)
      {
        if (layer.Queue[index].Type == type)
          layer.Queue.RemoveAt(index);
      }
    }

    private NotificationEnum GetLayerEnum(NotificationEnum type)
    {
      return (NotificationEnum) ((int) type / 1024 * 1024);
    }

    private NotificationLayerItem GetLayer(NotificationEnum type)
    {
      NotificationEnum layerEnum = this.GetLayerEnum(type);
      NotificationLayerItem layer;
      if (!this.layers.TryGetValue(layerEnum, out layer))
      {
        layer = new NotificationLayerItem();
        this.layers.Add(layerEnum, layer);
      }
      return layer;
    }

    public void Initialise()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    public void ComputeUpdate()
    {
      foreach (KeyValuePair<NotificationEnum, NotificationLayerItem> layer in this.layers)
      {
        if (!InstanceByRequest<EngineApplication>.Instance.IsPaused || this.UpdateLayerOnPause(layer.Key))
        {
          NotificationLayerItem notificationLayerItem = layer.Value;
          if ((UnityEngine.Object) notificationLayerItem.Notifaction != (UnityEngine.Object) null)
          {
            if (notificationLayerItem.Notifaction.Complete)
            {
              notificationLayerItem.Notifaction.Shutdown();
              notificationLayerItem.Notifaction = (INotification) null;
            }
          }
          else if (notificationLayerItem.Queue.Count != 0)
          {
            NotificationItem notificationItem = notificationLayerItem.Queue[0];
            notificationLayerItem.Queue.RemoveAt(0);
            UIService service = ServiceLocator.GetService<UIService>();
            if (service != null)
            {
              IHudWindow hudWindow = service.Get<IHudWindow>();
              if (hudWindow != null)
              {
                INotification notification = hudWindow.Create(notificationItem.Type);
                if (notification != null)
                {
                  notificationLayerItem.Notifaction = notification;
                  notification.Initialise(notificationItem.Type, notificationItem.Values);
                }
              }
            }
          }
        }
      }
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

    public void Unload()
    {
      foreach (KeyValuePair<NotificationEnum, NotificationLayerItem> layer in this.layers)
      {
        NotificationLayerItem notificationLayerItem = layer.Value;
        notificationLayerItem.Notifaction?.Shutdown();
        notificationLayerItem.Queue.Clear();
      }
      this.layers.Clear();
      this.blockedTypes.Clear();
    }

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize<NotificationService>(writer, TypeUtility.GetTypeName(this.GetType()), this);
    }
  }
}
