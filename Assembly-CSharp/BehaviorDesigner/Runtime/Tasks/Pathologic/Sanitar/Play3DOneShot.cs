// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.Pathologic.Sanitar.Play3DOneShot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks.Pathologic.Sanitar
{
  [TaskDescription("Play 3d one shot.")]
  [TaskCategory("Pathologic/Audio")]
  [TaskIcon("Pathologic_AudioIcon.png")]
  [Factory]
  [GeneratePartial(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  [FactoryProxy(typeof (Play3DOneShot))]
  public class Play3DOneShot : BehaviorDesigner.Runtime.Tasks.Action, IStub, ISerializeDataWrite, ISerializeDataRead
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public AudioMixerGroup AudioMixerGroup;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public AudioClip Sound;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public AudioRolloffMode RolloffMode = AudioRolloffMode.Linear;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat Volume = (SharedFloat) 1f;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("0 for 2D sound, 1 for 3D sound")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat SpatialBlend = (SharedFloat) 1f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat MinDistance = (SharedFloat) 1f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat MaxDistance = (SharedFloat) 10f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public DetectType DetectType = DetectType.Casual;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public Pivot.AimWeaponType BodyPart = Pivot.AimWeaponType.Head;
    protected DetectableComponent detectable;
    protected IEntity entity;
    private Pivot pivot;

    private void PlayAudioClip()
    {
      SoundUtility.PlayAudioClip3D(this.gameObject.transform, this.Sound, this.AudioMixerGroup, this.Volume.Value, this.MinDistance.Value, this.MaxDistance.Value, true, 0.0f);
    }

    public override TaskStatus OnUpdate()
    {
      if ((UnityEngine.Object) this.Sound == (UnityEngine.Object) null)
        return TaskStatus.Failure;
      this.entity = EntityUtility.GetEntity(this.gameObject);
      if (this.entity == null)
      {
        Debug.LogWarning((object) (this.gameObject.name + " : entity not found, method : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name), (UnityEngine.Object) this.gameObject);
        return TaskStatus.Failure;
      }
      this.detectable = (DetectableComponent) this.entity.GetComponent<IDetectableComponent>();
      if (this.detectable == null)
        Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (IDetectableComponent).Name + " engine component (this is not critical"), (UnityEngine.Object) this.gameObject);
      if ((UnityEngine.Object) this.Sound == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (IDetectableComponent).Name + " engine component"), (UnityEngine.Object) this.gameObject);
        return TaskStatus.Failure;
      }
      this.pivot = this.gameObject.GetComponent<Pivot>();
      if ((UnityEngine.Object) this.pivot == (UnityEngine.Object) null)
      {
        Debug.LogErrorFormat(this.gameObject.name + " doesnt' contain " + typeof (Pivot).Name + " unity component", (object) this.gameObject);
        return TaskStatus.Failure;
      }
      if (this.detectable == null)
        ;
      this.PlayAudioClip();
      return TaskStatus.Success;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteSerialize<NodeData>(writer, "NodeData", this.nodeData);
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.Write(writer, "FriendlyName", this.friendlyName);
      DefaultDataWriteUtility.Write(writer, "Instant", this.instant);
      DefaultDataWriteUtility.Write(writer, "Disabled", this.disabled);
      BehaviorTreeDataWriteUtility.WriteUnity<AudioMixerGroup>(writer, "AudioMixerGroup", this.AudioMixerGroup);
      BehaviorTreeDataWriteUtility.WriteUnity<AudioClip>(writer, "Sound", this.Sound);
      DefaultDataWriteUtility.WriteEnum<AudioRolloffMode>(writer, "RolloffMode", this.RolloffMode);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "Volume", this.Volume);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "SpatialBlend", this.SpatialBlend);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "MinDistance", this.MinDistance);
      BehaviorTreeDataWriteUtility.WriteShared<SharedFloat>(writer, "MaxDistance", this.MaxDistance);
      DefaultDataWriteUtility.WriteEnum<DetectType>(writer, "DetectType", this.DetectType);
      DefaultDataWriteUtility.WriteEnum<Pivot.AimWeaponType>(writer, "BodyPart", this.BodyPart);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.nodeData = DefaultDataReadUtility.ReadSerialize<NodeData>(reader, "NodeData");
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.friendlyName = DefaultDataReadUtility.Read(reader, "FriendlyName", this.friendlyName);
      this.instant = DefaultDataReadUtility.Read(reader, "Instant", this.instant);
      this.disabled = DefaultDataReadUtility.Read(reader, "Disabled", this.disabled);
      this.AudioMixerGroup = BehaviorTreeDataReadUtility.ReadUnity<AudioMixerGroup>(reader, "AudioMixerGroup", this.AudioMixerGroup);
      this.Sound = BehaviorTreeDataReadUtility.ReadUnity<AudioClip>(reader, "Sound", this.Sound);
      this.RolloffMode = DefaultDataReadUtility.ReadEnum<AudioRolloffMode>(reader, "RolloffMode");
      this.Volume = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "Volume", this.Volume);
      this.SpatialBlend = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "SpatialBlend", this.SpatialBlend);
      this.MinDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "MinDistance", this.MinDistance);
      this.MaxDistance = BehaviorTreeDataReadUtility.ReadShared<SharedFloat>(reader, "MaxDistance", this.MaxDistance);
      this.DetectType = DefaultDataReadUtility.ReadEnum<DetectType>(reader, "DetectType");
      this.BodyPart = DefaultDataReadUtility.ReadEnum<Pivot.AimWeaponType>(reader, "BodyPart");
    }
  }
}
