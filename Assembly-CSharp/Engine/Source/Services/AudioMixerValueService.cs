using System.Collections.Generic;
using Engine.Common;
using Engine.Source.Blueprints.Sounds;
using Engine.Source.Commons;
using Engine.Source.Settings.External;
using Inspectors;

namespace Engine.Source.Services
{
  [RuntimeService(typeof (AudioMixerValueService))]
  public class AudioMixerValueService : 
    IInitialisable,
    IUpdateItem<AudioMixerValueService.ItemInfo>,
    IUpdatable
  {
    [Inspected]
    private List<ItemInfo> items = new List<ItemInfo>();
    [Inspected]
    private ReduceUpdateProxy<ItemInfo> updater;

    public void Initialise()
    {
      updater = new ReduceUpdateProxy<ItemInfo>(items, this, ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.BlueprintSoundsDelay);
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Terminate()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }

    public void AddNode(AudioMixerValueNode node)
    {
      AudioMixerGroup mixer = node.Mixer;
      if ((UnityEngine.Object) mixer == (UnityEngine.Object) null)
      {
        Debug.LogError((object) ("Mixer not found, owner : " + node.graph.agent.GetInfo()));
      }
      else
      {
        string str = mixer.name + "   [" + node.Name + "]";
        ItemInfo itemInfo1 = null;
        foreach (ItemInfo itemInfo2 in items)
        {
          if (itemInfo2.Name == str)
          {
            itemInfo1 = itemInfo2;
            break;
          }
        }
        if (itemInfo1 == null)
        {
          itemInfo1 = new ItemInfo();
          itemInfo1.Name = str;
          if (!node.Mixer.audioMixer.GetFloat(node.Name, out itemInfo1.DefaultValue))
          {
            Debug.LogError((object) ("Parameter not found : " + node.Name + " , graph : " + node.Agent.GetInfo()));
            node.Failed = true;
          }
          items.Add(itemInfo1);
        }
        itemInfo1.Nodes.Add(node);
      }
    }

    public void RemoveNode(AudioMixerValueNode node)
    {
      AudioMixerGroup mixer = node.Mixer;
      if ((UnityEngine.Object) mixer == (UnityEngine.Object) null)
        return;
      string str = mixer.name + "   [" + node.Name + "]";
      ItemInfo itemInfo1 = null;
      foreach (ItemInfo itemInfo2 in items)
      {
        if (itemInfo2.Name == str)
        {
          itemInfo1 = itemInfo2;
          break;
        }
      }
      if (itemInfo1 == null)
      {
        Debug.LogError((object) ("Value not found, owner : " + node.Agent.GetInfo()));
      }
      else
      {
        itemInfo1.Nodes.Remove(node);
        if (itemInfo1.Nodes.Count != 0 || node.Mixer.audioMixer.SetFloat(node.Name, itemInfo1.DefaultValue))
          return;
        Debug.LogError((object) ("Parameter not found : " + node.Name + " , graph : " + node.Agent.GetInfo()));
        node.Failed = true;
      }
    }

    public void ComputeUpdate() => updater.Update();

    public void ComputeUpdateItem(ItemInfo item)
    {
      List<AudioMixerValueNode> nodes = item.Nodes;
      if (nodes.Count == 0)
        return;
      AudioMixerValueNode audioMixerValueNode1 = nodes[0];
      float t = audioMixerValueNode1.Value;
      for (int index = 1; index < nodes.Count; ++index)
      {
        AudioMixerValueNode audioMixerValueNode2 = nodes[index];
        float num = audioMixerValueNode2.Value;
        if (num > (double) t)
        {
          audioMixerValueNode1 = audioMixerValueNode2;
          t = num;
        }
      }
      if (audioMixerValueNode1.Failed)
        return;
      float num1 = Mathf.LerpUnclamped(audioMixerValueNode1.MinValue, audioMixerValueNode1.MaxValue, t);
      string name = audioMixerValueNode1.Name;
      if (audioMixerValueNode1.Mixer.audioMixer.SetFloat(name, num1))
        return;
      Debug.LogError((object) ("Parameter not found : " + name + " , graph : " + audioMixerValueNode1.Agent.GetInfo()));
      audioMixerValueNode1.Failed = true;
    }

    public class ItemInfo
    {
      [Inspected(Header = true)]
      public string Name;
      [Inspected]
      public float DefaultValue;
      [Inspected]
      public List<AudioMixerValueNode> Nodes = new List<AudioMixerValueNode>();

      [Inspected(Header = true)]
      public int Count => Nodes.Count;
    }
  }
}
