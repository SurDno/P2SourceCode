using System;
using System.Reflection;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Behaviours.Components;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Common.Components.Detectors;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Audio;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using UnityEngine;
using UnityEngine.Audio;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic.Sanitar
{
  [TaskDescription("Play 3d one shot.")]
  [TaskCategory("Pathologic/Audio")]
  [TaskIcon("Pathologic_AudioIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (Play3DOneShot))]
  public class Play3DOneShot : Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public AudioMixerGroup AudioMixerGroup;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public AudioClip Sound;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public AudioRolloffMode RolloffMode = AudioRolloffMode.Linear;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat Volume = 1f;
    [Tooltip("0 for 2D sound, 1 for 3D sound")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat SpatialBlend = 1f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat MinDistance = 1f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat MaxDistance = 10f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public DetectType DetectType = DetectType.Casual;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public Pivot.AimWeaponType BodyPart = Pivot.AimWeaponType.Head;
    protected DetectableComponent detectable;
    protected IEntity entity;
    private Pivot pivot;

    private void PlayAudioClip()
    {
      SoundUtility.PlayAudioClip3D(gameObject.transform, Sound, AudioMixerGroup, Volume.Value, MinDistance.Value, MaxDistance.Value, true, 0.0f);
    }

    public override TaskStatus OnUpdate()
    {
      if (Sound == null)
        return TaskStatus.Failure;
      entity = EntityUtility.GetEntity(gameObject);
      if (entity == null)
      {
        Debug.LogWarning(gameObject.name + " : entity not found, method : " + GetType().Name + ":" + MethodBase.GetCurrentMethod().Name, gameObject);
        return TaskStatus.Failure;
      }
      detectable = (DetectableComponent) entity.GetComponent<IDetectableComponent>();
      if (detectable == null)
        Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (IDetectableComponent).Name + " engine component (this is not critical", gameObject);
      if (Sound == null)
      {
        Debug.LogWarning(gameObject.name + ": doesn't contain " + typeof (IDetectableComponent).Name + " engine component", gameObject);
        return TaskStatus.Failure;
      }
      pivot = gameObject.GetComponent<Pivot>();
      if (pivot == null)
      {
        Debug.LogErrorFormat(gameObject.name + " doesnt' contain " + typeof (Pivot).Name + " unity component", gameObject);
        return TaskStatus.Failure;
      }
      if (detectable == null)
        ;
      PlayAudioClip();
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize(writer, "NodeData", nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", disabled);
      BehaviorTreeDataWriteUtility.WriteUnity(writer, "AudioMixerGroup", AudioMixerGroup);
      BehaviorTreeDataWriteUtility.WriteUnity(writer, "Sound", Sound);
      DefaultDataWriteUtility.WriteEnum(writer, "RolloffMode", RolloffMode);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "Volume", Volume);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "SpatialBlend", SpatialBlend);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "MinDistance", MinDistance);
      BehaviorTreeDataWriteUtility.WriteShared(writer, "MaxDistance", MaxDistance);
      DefaultDataWriteUtility.WriteEnum(writer, "DetectType", DetectType);
      DefaultDataWriteUtility.WriteEnum(writer, "BodyPart", BodyPart);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      id = DefaultDataReadUtility.Read(reader, "Id", id);
      friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", friendlyName);
      instant = DefaultDataReadUtility.Read(reader, "Instant", instant);
      disabled = DefaultDataReadUtility.Read(reader, "Disabled", disabled);
      AudioMixerGroup = BehaviorTreeDataReadUtility.ReadUnity(reader, "AudioMixerGroup", AudioMixerGroup);
      Sound = BehaviorTreeDataReadUtility.ReadUnity(reader, "Sound", Sound);
      RolloffMode = DefaultDataReadUtility.ReadEnum<AudioRolloffMode>(reader, "RolloffMode");
      Volume = BehaviorTreeDataReadUtility.ReadShared(reader, "Volume", Volume);
      SpatialBlend = BehaviorTreeDataReadUtility.ReadShared(reader, "SpatialBlend", SpatialBlend);
      MinDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "MinDistance", MinDistance);
      MaxDistance = BehaviorTreeDataReadUtility.ReadShared(reader, "MaxDistance", MaxDistance);
      DetectType = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "DetectType");
      BodyPart = DefaultDataReadUtility.ReadEnum<Pivot.AimWeaponType>(reader, "BodyPart");
    }
  }
}
