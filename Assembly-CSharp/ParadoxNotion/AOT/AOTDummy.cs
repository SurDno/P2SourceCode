using Engine.Common.Commons;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;
using Engine.Common.DateTime;
using Engine.Source.Connections;
using Engine.Source.Effects.Values;
using Engine.Source.Services.CameraServices;
using Engine.Source.Services.Inputs;
using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.Framework;
using UnityEngine;

namespace ParadoxNotion.AOT
{
  public class AOTDummy
  {
    public object o = (object) null;

    public void FlowCanvas_ValueHandler_Delegate()
    {
    }

    public void FlowCanvas_FlowNode_AddValueInput_1()
    {
      FlowNode flowNode = (FlowNode) null;
      flowNode.AddValueInput<BlockTypeEnum>((string) this.o, (string) this.o);
      flowNode.AddValueInput<IntensityParameter<Color>>((string) this.o, (string) this.o);
      flowNode.AddValueInput<UiEffectType>((string) this.o, (string) this.o);
      flowNode.AddValueInput<ActionEnum>((string) this.o, (string) this.o);
      flowNode.AddValueInput<DiseasedStateEnum>((string) this.o, (string) this.o);
      flowNode.AddValueInput<WeaponKind>((string) this.o, (string) this.o);
      flowNode.AddValueInput<ParameterNameEnum>((string) this.o, (string) this.o);
      flowNode.AddValueInput<BuildingEnum>((string) this.o, (string) this.o);
      flowNode.AddValueInput<RegionEnum>((string) this.o, (string) this.o);
      flowNode.AddValueInput<TimesOfDay>((string) this.o, (string) this.o);
      flowNode.AddValueInput<IEntitySerializable>((string) this.o, (string) this.o);
      flowNode.AddValueInput<LipSyncObjectSerializable>((string) this.o, (string) this.o);
      flowNode.AddValueInput<AbilityValueNameEnum>((string) this.o, (string) this.o);
      flowNode.AddValueInput<CameraKindEnum>((string) this.o, (string) this.o);
      flowNode.AddValueInput<GameActionType>((string) this.o, (string) this.o);
      flowNode.AddValueInput<bool>((string) this.o, (string) this.o);
      flowNode.AddValueInput<int>((string) this.o, (string) this.o);
      flowNode.AddValueInput<float>((string) this.o, (string) this.o);
      flowNode.AddValueInput<Bounds>((string) this.o, (string) this.o);
      flowNode.AddValueInput<Color>((string) this.o, (string) this.o);
      flowNode.AddValueInput<ContactPoint>((string) this.o, (string) this.o);
      flowNode.AddValueInput<Keyframe>((string) this.o, (string) this.o);
      flowNode.AddValueInput<LayerMask>((string) this.o, (string) this.o);
      flowNode.AddValueInput<Quaternion>((string) this.o, (string) this.o);
      flowNode.AddValueInput<RaycastHit>((string) this.o, (string) this.o);
      flowNode.AddValueInput<Rect>((string) this.o, (string) this.o);
      flowNode.AddValueInput<Space>((string) this.o, (string) this.o);
      flowNode.AddValueInput<Vector2>((string) this.o, (string) this.o);
      flowNode.AddValueInput<Vector3>((string) this.o, (string) this.o);
      flowNode.AddValueInput<Vector4>((string) this.o, (string) this.o);
    }

    public void FlowCanvas_FlowNode_AddValueOutput_2()
    {
      FlowNode flowNode = (FlowNode) null;
      flowNode.AddValueOutput<BlockTypeEnum>((string) this.o, (ValueHandler<BlockTypeEnum>) this.o, (string) this.o);
      flowNode.AddValueOutput<IntensityParameter<Color>>((string) this.o, (ValueHandler<IntensityParameter<Color>>) this.o, (string) this.o);
      flowNode.AddValueOutput<UiEffectType>((string) this.o, (ValueHandler<UiEffectType>) this.o, (string) this.o);
      flowNode.AddValueOutput<ActionEnum>((string) this.o, (ValueHandler<ActionEnum>) this.o, (string) this.o);
      flowNode.AddValueOutput<DiseasedStateEnum>((string) this.o, (ValueHandler<DiseasedStateEnum>) this.o, (string) this.o);
      flowNode.AddValueOutput<WeaponKind>((string) this.o, (ValueHandler<WeaponKind>) this.o, (string) this.o);
      flowNode.AddValueOutput<ParameterNameEnum>((string) this.o, (ValueHandler<ParameterNameEnum>) this.o, (string) this.o);
      flowNode.AddValueOutput<BuildingEnum>((string) this.o, (ValueHandler<BuildingEnum>) this.o, (string) this.o);
      flowNode.AddValueOutput<RegionEnum>((string) this.o, (ValueHandler<RegionEnum>) this.o, (string) this.o);
      flowNode.AddValueOutput<TimesOfDay>((string) this.o, (ValueHandler<TimesOfDay>) this.o, (string) this.o);
      flowNode.AddValueOutput<IEntitySerializable>((string) this.o, (ValueHandler<IEntitySerializable>) this.o, (string) this.o);
      flowNode.AddValueOutput<LipSyncObjectSerializable>((string) this.o, (ValueHandler<LipSyncObjectSerializable>) this.o, (string) this.o);
      flowNode.AddValueOutput<AbilityValueNameEnum>((string) this.o, (ValueHandler<AbilityValueNameEnum>) this.o, (string) this.o);
      flowNode.AddValueOutput<CameraKindEnum>((string) this.o, (ValueHandler<CameraKindEnum>) this.o, (string) this.o);
      flowNode.AddValueOutput<GameActionType>((string) this.o, (ValueHandler<GameActionType>) this.o, (string) this.o);
      flowNode.AddValueOutput<bool>((string) this.o, (ValueHandler<bool>) this.o, (string) this.o);
      flowNode.AddValueOutput<int>((string) this.o, (ValueHandler<int>) this.o, (string) this.o);
      flowNode.AddValueOutput<float>((string) this.o, (ValueHandler<float>) this.o, (string) this.o);
      flowNode.AddValueOutput<Bounds>((string) this.o, (ValueHandler<Bounds>) this.o, (string) this.o);
      flowNode.AddValueOutput<Color>((string) this.o, (ValueHandler<Color>) this.o, (string) this.o);
      flowNode.AddValueOutput<ContactPoint>((string) this.o, (ValueHandler<ContactPoint>) this.o, (string) this.o);
      flowNode.AddValueOutput<Keyframe>((string) this.o, (ValueHandler<Keyframe>) this.o, (string) this.o);
      flowNode.AddValueOutput<LayerMask>((string) this.o, (ValueHandler<LayerMask>) this.o, (string) this.o);
      flowNode.AddValueOutput<Quaternion>((string) this.o, (ValueHandler<Quaternion>) this.o, (string) this.o);
      flowNode.AddValueOutput<RaycastHit>((string) this.o, (ValueHandler<RaycastHit>) this.o, (string) this.o);
      flowNode.AddValueOutput<Rect>((string) this.o, (ValueHandler<Rect>) this.o, (string) this.o);
      flowNode.AddValueOutput<Space>((string) this.o, (ValueHandler<Space>) this.o, (string) this.o);
      flowNode.AddValueOutput<Vector2>((string) this.o, (ValueHandler<Vector2>) this.o, (string) this.o);
      flowNode.AddValueOutput<Vector3>((string) this.o, (ValueHandler<Vector3>) this.o, (string) this.o);
      flowNode.AddValueOutput<Vector4>((string) this.o, (ValueHandler<Vector4>) this.o, (string) this.o);
    }

    public void CustomSpoof()
    {
    }

    public class FlowCanvas_BinderConnection_BlockTypeEnum : BinderConnection<BlockTypeEnum>
    {
    }

    public class FlowCanvas_BinderConnection_IntensityParameter_UnityEngine_Color_ : 
      BinderConnection<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_BinderConnection_UiEffectType : BinderConnection<UiEffectType>
    {
    }

    public class FlowCanvas_BinderConnection_Engine_Common_Commons_ActionEnum : 
      BinderConnection<ActionEnum>
    {
    }

    public class FlowCanvas_BinderConnection_Engine_Common_Commons_DiseasedStateEnum : 
      BinderConnection<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_BinderConnection_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      BinderConnection<WeaponKind>
    {
    }

    public class FlowCanvas_BinderConnection_Engine_Common_Components_Parameters_ParameterNameEnum : 
      BinderConnection<ParameterNameEnum>
    {
    }

    public class FlowCanvas_BinderConnection_Engine_Common_Components_Regions_BuildingEnum : 
      BinderConnection<BuildingEnum>
    {
    }

    public class FlowCanvas_BinderConnection_Engine_Common_Components_Regions_RegionEnum : 
      BinderConnection<RegionEnum>
    {
    }

    public class FlowCanvas_BinderConnection_Engine_Common_DateTime_TimesOfDay : 
      BinderConnection<TimesOfDay>
    {
    }

    public class FlowCanvas_BinderConnection_Engine_Source_Connections_IEntitySerializable : 
      BinderConnection<IEntitySerializable>
    {
    }

    public class FlowCanvas_BinderConnection_Engine_Source_Connections_LipSyncObjectSerializable : 
      BinderConnection<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_BinderConnection_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      BinderConnection<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_BinderConnection_Engine_Source_Services_CameraServices_CameraKindEnum : 
      BinderConnection<CameraKindEnum>
    {
    }

    public class FlowCanvas_BinderConnection_Engine_Source_Services_Inputs_GameActionType : 
      BinderConnection<GameActionType>
    {
    }

    public class FlowCanvas_BinderConnection_bool : BinderConnection<bool>
    {
    }

    public class FlowCanvas_BinderConnection_int : BinderConnection<int>
    {
    }

    public class FlowCanvas_BinderConnection_float : BinderConnection<float>
    {
    }

    public class FlowCanvas_BinderConnection_UnityEngine_Bounds : BinderConnection<Bounds>
    {
    }

    public class FlowCanvas_BinderConnection_UnityEngine_Color : BinderConnection<Color>
    {
    }

    public class FlowCanvas_BinderConnection_UnityEngine_ContactPoint : 
      BinderConnection<ContactPoint>
    {
    }

    public class FlowCanvas_BinderConnection_UnityEngine_Keyframe : BinderConnection<Keyframe>
    {
    }

    public class FlowCanvas_BinderConnection_UnityEngine_LayerMask : BinderConnection<LayerMask>
    {
    }

    public class FlowCanvas_BinderConnection_UnityEngine_Quaternion : BinderConnection<Quaternion>
    {
    }

    public class FlowCanvas_BinderConnection_UnityEngine_RaycastHit : BinderConnection<RaycastHit>
    {
    }

    public class FlowCanvas_BinderConnection_UnityEngine_Rect : BinderConnection<Rect>
    {
    }

    public class FlowCanvas_BinderConnection_UnityEngine_Space : BinderConnection<Space>
    {
    }

    public class FlowCanvas_BinderConnection_UnityEngine_Vector2 : BinderConnection<Vector2>
    {
    }

    public class FlowCanvas_BinderConnection_UnityEngine_Vector3 : BinderConnection<Vector3>
    {
    }

    public class FlowCanvas_BinderConnection_UnityEngine_Vector4 : BinderConnection<Vector4>
    {
    }

    public class FlowCanvas_ValueInput_BlockTypeEnum : ValueInput<BlockTypeEnum>
    {
    }

    public class FlowCanvas_ValueInput_IntensityParameter_UnityEngine_Color_ : 
      ValueInput<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_ValueInput_UiEffectType : ValueInput<UiEffectType>
    {
    }

    public class FlowCanvas_ValueInput_Engine_Common_Commons_ActionEnum : ValueInput<ActionEnum>
    {
    }

    public class FlowCanvas_ValueInput_Engine_Common_Commons_DiseasedStateEnum : 
      ValueInput<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_ValueInput_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      ValueInput<WeaponKind>
    {
    }

    public class FlowCanvas_ValueInput_Engine_Common_Components_Parameters_ParameterNameEnum : 
      ValueInput<ParameterNameEnum>
    {
    }

    public class FlowCanvas_ValueInput_Engine_Common_Components_Regions_BuildingEnum : 
      ValueInput<BuildingEnum>
    {
    }

    public class FlowCanvas_ValueInput_Engine_Common_Components_Regions_RegionEnum : 
      ValueInput<RegionEnum>
    {
    }

    public class FlowCanvas_ValueInput_Engine_Common_DateTime_TimesOfDay : ValueInput<TimesOfDay>
    {
    }

    public class FlowCanvas_ValueInput_Engine_Source_Connections_IEntitySerializable : 
      ValueInput<IEntitySerializable>
    {
    }

    public class FlowCanvas_ValueInput_Engine_Source_Connections_LipSyncObjectSerializable : 
      ValueInput<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_ValueInput_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      ValueInput<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_ValueInput_Engine_Source_Services_CameraServices_CameraKindEnum : 
      ValueInput<CameraKindEnum>
    {
    }

    public class FlowCanvas_ValueInput_Engine_Source_Services_Inputs_GameActionType : 
      ValueInput<GameActionType>
    {
    }

    public class FlowCanvas_ValueInput_bool : ValueInput<bool>
    {
    }

    public class FlowCanvas_ValueInput_int : ValueInput<int>
    {
    }

    public class FlowCanvas_ValueInput_float : ValueInput<float>
    {
    }

    public class FlowCanvas_ValueInput_UnityEngine_Bounds : ValueInput<Bounds>
    {
    }

    public class FlowCanvas_ValueInput_UnityEngine_Color : ValueInput<Color>
    {
    }

    public class FlowCanvas_ValueInput_UnityEngine_ContactPoint : ValueInput<ContactPoint>
    {
    }

    public class FlowCanvas_ValueInput_UnityEngine_Keyframe : ValueInput<Keyframe>
    {
    }

    public class FlowCanvas_ValueInput_UnityEngine_LayerMask : ValueInput<LayerMask>
    {
    }

    public class FlowCanvas_ValueInput_UnityEngine_Quaternion : ValueInput<Quaternion>
    {
    }

    public class FlowCanvas_ValueInput_UnityEngine_RaycastHit : ValueInput<RaycastHit>
    {
    }

    public class FlowCanvas_ValueInput_UnityEngine_Rect : ValueInput<Rect>
    {
    }

    public class FlowCanvas_ValueInput_UnityEngine_Space : ValueInput<Space>
    {
    }

    public class FlowCanvas_ValueInput_UnityEngine_Vector2 : ValueInput<Vector2>
    {
    }

    public class FlowCanvas_ValueInput_UnityEngine_Vector3 : ValueInput<Vector3>
    {
    }

    public class FlowCanvas_ValueInput_UnityEngine_Vector4 : ValueInput<Vector4>
    {
    }

    public class FlowCanvas_ValueOutput_BlockTypeEnum : ValueOutput<BlockTypeEnum>
    {
    }

    public class FlowCanvas_ValueOutput_IntensityParameter_UnityEngine_Color_ : 
      ValueOutput<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_ValueOutput_UiEffectType : ValueOutput<UiEffectType>
    {
    }

    public class FlowCanvas_ValueOutput_Engine_Common_Commons_ActionEnum : ValueOutput<ActionEnum>
    {
    }

    public class FlowCanvas_ValueOutput_Engine_Common_Commons_DiseasedStateEnum : 
      ValueOutput<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_ValueOutput_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      ValueOutput<WeaponKind>
    {
    }

    public class FlowCanvas_ValueOutput_Engine_Common_Components_Parameters_ParameterNameEnum : 
      ValueOutput<ParameterNameEnum>
    {
    }

    public class FlowCanvas_ValueOutput_Engine_Common_Components_Regions_BuildingEnum : 
      ValueOutput<BuildingEnum>
    {
    }

    public class FlowCanvas_ValueOutput_Engine_Common_Components_Regions_RegionEnum : 
      ValueOutput<RegionEnum>
    {
    }

    public class FlowCanvas_ValueOutput_Engine_Common_DateTime_TimesOfDay : ValueOutput<TimesOfDay>
    {
    }

    public class FlowCanvas_ValueOutput_Engine_Source_Connections_IEntitySerializable : 
      ValueOutput<IEntitySerializable>
    {
    }

    public class FlowCanvas_ValueOutput_Engine_Source_Connections_LipSyncObjectSerializable : 
      ValueOutput<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_ValueOutput_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      ValueOutput<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_ValueOutput_Engine_Source_Services_CameraServices_CameraKindEnum : 
      ValueOutput<CameraKindEnum>
    {
    }

    public class FlowCanvas_ValueOutput_Engine_Source_Services_Inputs_GameActionType : 
      ValueOutput<GameActionType>
    {
    }

    public class FlowCanvas_ValueOutput_bool : ValueOutput<bool>
    {
    }

    public class FlowCanvas_ValueOutput_int : ValueOutput<int>
    {
    }

    public class FlowCanvas_ValueOutput_float : ValueOutput<float>
    {
    }

    public class FlowCanvas_ValueOutput_UnityEngine_Bounds : ValueOutput<Bounds>
    {
    }

    public class FlowCanvas_ValueOutput_UnityEngine_Color : ValueOutput<Color>
    {
    }

    public class FlowCanvas_ValueOutput_UnityEngine_ContactPoint : ValueOutput<ContactPoint>
    {
    }

    public class FlowCanvas_ValueOutput_UnityEngine_Keyframe : ValueOutput<Keyframe>
    {
    }

    public class FlowCanvas_ValueOutput_UnityEngine_LayerMask : ValueOutput<LayerMask>
    {
    }

    public class FlowCanvas_ValueOutput_UnityEngine_Quaternion : ValueOutput<Quaternion>
    {
    }

    public class FlowCanvas_ValueOutput_UnityEngine_RaycastHit : ValueOutput<RaycastHit>
    {
    }

    public class FlowCanvas_ValueOutput_UnityEngine_Rect : ValueOutput<Rect>
    {
    }

    public class FlowCanvas_ValueOutput_UnityEngine_Space : ValueOutput<Space>
    {
    }

    public class FlowCanvas_ValueOutput_UnityEngine_Vector2 : ValueOutput<Vector2>
    {
    }

    public class FlowCanvas_ValueOutput_UnityEngine_Vector3 : ValueOutput<Vector3>
    {
    }

    public class FlowCanvas_ValueOutput_UnityEngine_Vector4 : ValueOutput<Vector4>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_BlockTypeEnum : AddListItem<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_IntensityParameter_UnityEngine_Color_ : 
      AddListItem<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UiEffectType : AddListItem<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_Engine_Common_Commons_ActionEnum : 
      AddListItem<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_Engine_Common_Commons_DiseasedStateEnum : 
      AddListItem<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      AddListItem<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_Engine_Common_Components_Parameters_ParameterNameEnum : 
      AddListItem<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_Engine_Common_Components_Regions_BuildingEnum : 
      AddListItem<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_Engine_Common_Components_Regions_RegionEnum : 
      AddListItem<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_Engine_Common_DateTime_TimesOfDay : 
      AddListItem<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_Engine_Source_Connections_IEntitySerializable : 
      AddListItem<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_Engine_Source_Connections_LipSyncObjectSerializable : 
      AddListItem<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      AddListItem<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_Engine_Source_Services_CameraServices_CameraKindEnum : 
      AddListItem<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_Engine_Source_Services_Inputs_GameActionType : 
      AddListItem<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_bool : AddListItem<bool>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_int : AddListItem<int>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_float : AddListItem<float>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UnityEngine_Bounds : AddListItem<Bounds>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UnityEngine_Color : AddListItem<Color>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UnityEngine_ContactPoint : AddListItem<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UnityEngine_Keyframe : AddListItem<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UnityEngine_LayerMask : AddListItem<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UnityEngine_Quaternion : AddListItem<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UnityEngine_RaycastHit : AddListItem<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UnityEngine_Rect : AddListItem<Rect>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UnityEngine_Space : AddListItem<Space>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UnityEngine_Vector2 : AddListItem<Vector2>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UnityEngine_Vector3 : AddListItem<Vector3>
    {
    }

    public class FlowCanvas_Nodes_AddListItem_UnityEngine_Vector4 : AddListItem<Vector4>
    {
    }

    public class FlowCanvas_Nodes_Cache_BlockTypeEnum : Cache<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_Cache_IntensityParameter_UnityEngine_Color_ : 
      Cache<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_Cache_UiEffectType : Cache<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_Cache_Engine_Common_Commons_ActionEnum : Cache<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_Cache_Engine_Common_Commons_DiseasedStateEnum : 
      Cache<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_Cache_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      Cache<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_Cache_Engine_Common_Components_Parameters_ParameterNameEnum : 
      Cache<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_Cache_Engine_Common_Components_Regions_BuildingEnum : 
      Cache<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_Cache_Engine_Common_Components_Regions_RegionEnum : 
      Cache<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_Cache_Engine_Common_DateTime_TimesOfDay : Cache<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_Cache_Engine_Source_Connections_IEntitySerializable : 
      Cache<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_Cache_Engine_Source_Connections_LipSyncObjectSerializable : 
      Cache<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_Cache_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      Cache<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_Cache_Engine_Source_Services_CameraServices_CameraKindEnum : 
      Cache<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_Cache_Engine_Source_Services_Inputs_GameActionType : 
      Cache<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_Cache_bool : Cache<bool>
    {
    }

    public class FlowCanvas_Nodes_Cache_int : Cache<int>
    {
    }

    public class FlowCanvas_Nodes_Cache_float : Cache<float>
    {
    }

    public class FlowCanvas_Nodes_Cache_UnityEngine_Bounds : Cache<Bounds>
    {
    }

    public class FlowCanvas_Nodes_Cache_UnityEngine_Color : Cache<Color>
    {
    }

    public class FlowCanvas_Nodes_Cache_UnityEngine_ContactPoint : Cache<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_Cache_UnityEngine_Keyframe : Cache<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_Cache_UnityEngine_LayerMask : Cache<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_Cache_UnityEngine_Quaternion : Cache<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_Cache_UnityEngine_RaycastHit : Cache<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_Cache_UnityEngine_Rect : Cache<Rect>
    {
    }

    public class FlowCanvas_Nodes_Cache_UnityEngine_Space : Cache<Space>
    {
    }

    public class FlowCanvas_Nodes_Cache_UnityEngine_Vector2 : Cache<Vector2>
    {
    }

    public class FlowCanvas_Nodes_Cache_UnityEngine_Vector3 : Cache<Vector3>
    {
    }

    public class FlowCanvas_Nodes_Cache_UnityEngine_Vector4 : Cache<Vector4>
    {
    }

    public class FlowCanvas_Nodes_CastTo_BlockTypeEnum : CastTo<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_CastTo_IntensityParameter_UnityEngine_Color_ : 
      CastTo<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UiEffectType : CastTo<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_CastTo_Engine_Common_Commons_ActionEnum : CastTo<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_CastTo_Engine_Common_Commons_DiseasedStateEnum : 
      CastTo<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_CastTo_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      CastTo<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_CastTo_Engine_Common_Components_Parameters_ParameterNameEnum : 
      CastTo<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_CastTo_Engine_Common_Components_Regions_BuildingEnum : 
      CastTo<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_CastTo_Engine_Common_Components_Regions_RegionEnum : 
      CastTo<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_CastTo_Engine_Common_DateTime_TimesOfDay : CastTo<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_CastTo_Engine_Source_Connections_IEntitySerializable : 
      CastTo<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_CastTo_Engine_Source_Connections_LipSyncObjectSerializable : 
      CastTo<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_CastTo_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      CastTo<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_CastTo_Engine_Source_Services_CameraServices_CameraKindEnum : 
      CastTo<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_CastTo_Engine_Source_Services_Inputs_GameActionType : 
      CastTo<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_CastTo_bool : CastTo<bool>
    {
    }

    public class FlowCanvas_Nodes_CastTo_int : CastTo<int>
    {
    }

    public class FlowCanvas_Nodes_CastTo_float : CastTo<float>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UnityEngine_Bounds : CastTo<Bounds>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UnityEngine_Color : CastTo<Color>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UnityEngine_ContactPoint : CastTo<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UnityEngine_Keyframe : CastTo<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UnityEngine_LayerMask : CastTo<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UnityEngine_Quaternion : CastTo<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UnityEngine_RaycastHit : CastTo<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UnityEngine_Rect : CastTo<Rect>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UnityEngine_Space : CastTo<Space>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UnityEngine_Vector2 : CastTo<Vector2>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UnityEngine_Vector3 : CastTo<Vector3>
    {
    }

    public class FlowCanvas_Nodes_CastTo_UnityEngine_Vector4 : CastTo<Vector4>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_BlockTypeEnum : CodeEvent<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_IntensityParameter_UnityEngine_Color_ : 
      CodeEvent<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UiEffectType : CodeEvent<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_Engine_Common_Commons_ActionEnum : CodeEvent<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_Engine_Common_Commons_DiseasedStateEnum : 
      CodeEvent<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      CodeEvent<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_Engine_Common_Components_Parameters_ParameterNameEnum : 
      CodeEvent<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_Engine_Common_Components_Regions_BuildingEnum : 
      CodeEvent<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_Engine_Common_Components_Regions_RegionEnum : 
      CodeEvent<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_Engine_Common_DateTime_TimesOfDay : CodeEvent<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_Engine_Source_Connections_IEntitySerializable : 
      CodeEvent<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_Engine_Source_Connections_LipSyncObjectSerializable : 
      CodeEvent<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      CodeEvent<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_Engine_Source_Services_CameraServices_CameraKindEnum : 
      CodeEvent<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_Engine_Source_Services_Inputs_GameActionType : 
      CodeEvent<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_bool : CodeEvent<bool>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_int : CodeEvent<int>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_float : CodeEvent<float>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UnityEngine_Bounds : CodeEvent<Bounds>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UnityEngine_Color : CodeEvent<Color>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UnityEngine_ContactPoint : CodeEvent<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UnityEngine_Keyframe : CodeEvent<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UnityEngine_LayerMask : CodeEvent<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UnityEngine_Quaternion : CodeEvent<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UnityEngine_RaycastHit : CodeEvent<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UnityEngine_Rect : CodeEvent<Rect>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UnityEngine_Space : CodeEvent<Space>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UnityEngine_Vector2 : CodeEvent<Vector2>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UnityEngine_Vector3 : CodeEvent<Vector3>
    {
    }

    public class FlowCanvas_Nodes_CodeEvent_UnityEngine_Vector4 : CodeEvent<Vector4>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_BlockTypeEnum : CreateCollection<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_IntensityParameter_UnityEngine_Color_ : 
      CreateCollection<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UiEffectType : CreateCollection<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_Engine_Common_Commons_ActionEnum : 
      CreateCollection<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_Engine_Common_Commons_DiseasedStateEnum : 
      CreateCollection<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      CreateCollection<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_Engine_Common_Components_Parameters_ParameterNameEnum : 
      CreateCollection<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_Engine_Common_Components_Regions_BuildingEnum : 
      CreateCollection<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_Engine_Common_Components_Regions_RegionEnum : 
      CreateCollection<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_Engine_Common_DateTime_TimesOfDay : 
      CreateCollection<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_Engine_Source_Connections_IEntitySerializable : 
      CreateCollection<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_Engine_Source_Connections_LipSyncObjectSerializable : 
      CreateCollection<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      CreateCollection<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_Engine_Source_Services_CameraServices_CameraKindEnum : 
      CreateCollection<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_Engine_Source_Services_Inputs_GameActionType : 
      CreateCollection<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_bool : CreateCollection<bool>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_int : CreateCollection<int>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_float : CreateCollection<float>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UnityEngine_Bounds : CreateCollection<Bounds>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UnityEngine_Color : CreateCollection<Color>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UnityEngine_ContactPoint : 
      CreateCollection<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UnityEngine_Keyframe : CreateCollection<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UnityEngine_LayerMask : 
      CreateCollection<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UnityEngine_Quaternion : 
      CreateCollection<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UnityEngine_RaycastHit : 
      CreateCollection<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UnityEngine_Rect : CreateCollection<Rect>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UnityEngine_Space : CreateCollection<Space>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UnityEngine_Vector2 : CreateCollection<Vector2>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UnityEngine_Vector3 : CreateCollection<Vector3>
    {
    }

    public class FlowCanvas_Nodes_CreateCollection_UnityEngine_Vector4 : CreateCollection<Vector4>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_BlockTypeEnum : CustomEvent<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_IntensityParameter_UnityEngine_Color_ : 
      CustomEvent<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UiEffectType : CustomEvent<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_Engine_Common_Commons_ActionEnum : 
      CustomEvent<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_Engine_Common_Commons_DiseasedStateEnum : 
      CustomEvent<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      CustomEvent<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_Engine_Common_Components_Parameters_ParameterNameEnum : 
      CustomEvent<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_Engine_Common_Components_Regions_BuildingEnum : 
      CustomEvent<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_Engine_Common_Components_Regions_RegionEnum : 
      CustomEvent<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_Engine_Common_DateTime_TimesOfDay : 
      CustomEvent<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_Engine_Source_Connections_IEntitySerializable : 
      CustomEvent<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_Engine_Source_Connections_LipSyncObjectSerializable : 
      CustomEvent<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      CustomEvent<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_Engine_Source_Services_CameraServices_CameraKindEnum : 
      CustomEvent<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_Engine_Source_Services_Inputs_GameActionType : 
      CustomEvent<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_bool : CustomEvent<bool>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_int : CustomEvent<int>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_float : CustomEvent<float>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UnityEngine_Bounds : CustomEvent<Bounds>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UnityEngine_Color : CustomEvent<Color>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UnityEngine_ContactPoint : CustomEvent<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UnityEngine_Keyframe : CustomEvent<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UnityEngine_LayerMask : CustomEvent<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UnityEngine_Quaternion : CustomEvent<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UnityEngine_RaycastHit : CustomEvent<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UnityEngine_Rect : CustomEvent<Rect>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UnityEngine_Space : CustomEvent<Space>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UnityEngine_Vector2 : CustomEvent<Vector2>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UnityEngine_Vector3 : CustomEvent<Vector3>
    {
    }

    public class FlowCanvas_Nodes_CustomEvent_UnityEngine_Vector4 : CustomEvent<Vector4>
    {
    }

    public class FlowCanvas_Nodes_ForEach_BlockTypeEnum : ForEach<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_ForEach_IntensityParameter_UnityEngine_Color_ : 
      ForEach<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UiEffectType : ForEach<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_ForEach_Engine_Common_Commons_ActionEnum : ForEach<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_ForEach_Engine_Common_Commons_DiseasedStateEnum : 
      ForEach<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_ForEach_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      ForEach<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_ForEach_Engine_Common_Components_Parameters_ParameterNameEnum : 
      ForEach<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_ForEach_Engine_Common_Components_Regions_BuildingEnum : 
      ForEach<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_ForEach_Engine_Common_Components_Regions_RegionEnum : 
      ForEach<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_ForEach_Engine_Common_DateTime_TimesOfDay : ForEach<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_ForEach_Engine_Source_Connections_IEntitySerializable : 
      ForEach<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_ForEach_Engine_Source_Connections_LipSyncObjectSerializable : 
      ForEach<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_ForEach_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      ForEach<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_ForEach_Engine_Source_Services_CameraServices_CameraKindEnum : 
      ForEach<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_ForEach_Engine_Source_Services_Inputs_GameActionType : 
      ForEach<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_ForEach_bool : ForEach<bool>
    {
    }

    public class FlowCanvas_Nodes_ForEach_int : ForEach<int>
    {
    }

    public class FlowCanvas_Nodes_ForEach_float : ForEach<float>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UnityEngine_Bounds : ForEach<Bounds>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UnityEngine_Color : ForEach<Color>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UnityEngine_ContactPoint : ForEach<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UnityEngine_Keyframe : ForEach<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UnityEngine_LayerMask : ForEach<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UnityEngine_Quaternion : ForEach<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UnityEngine_RaycastHit : ForEach<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UnityEngine_Rect : ForEach<Rect>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UnityEngine_Space : ForEach<Space>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UnityEngine_Vector2 : ForEach<Vector2>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UnityEngine_Vector3 : ForEach<Vector3>
    {
    }

    public class FlowCanvas_Nodes_ForEach_UnityEngine_Vector4 : ForEach<Vector4>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_BlockTypeEnum : GetFirstListItem<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_IntensityParameter_UnityEngine_Color_ : 
      GetFirstListItem<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UiEffectType : GetFirstListItem<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_Engine_Common_Commons_ActionEnum : 
      GetFirstListItem<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_Engine_Common_Commons_DiseasedStateEnum : 
      GetFirstListItem<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      GetFirstListItem<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_Engine_Common_Components_Parameters_ParameterNameEnum : 
      GetFirstListItem<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_Engine_Common_Components_Regions_BuildingEnum : 
      GetFirstListItem<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_Engine_Common_Components_Regions_RegionEnum : 
      GetFirstListItem<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_Engine_Common_DateTime_TimesOfDay : 
      GetFirstListItem<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_Engine_Source_Connections_IEntitySerializable : 
      GetFirstListItem<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_Engine_Source_Connections_LipSyncObjectSerializable : 
      GetFirstListItem<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      GetFirstListItem<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_Engine_Source_Services_CameraServices_CameraKindEnum : 
      GetFirstListItem<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_Engine_Source_Services_Inputs_GameActionType : 
      GetFirstListItem<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_bool : GetFirstListItem<bool>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_int : GetFirstListItem<int>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_float : GetFirstListItem<float>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UnityEngine_Bounds : GetFirstListItem<Bounds>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UnityEngine_Color : GetFirstListItem<Color>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UnityEngine_ContactPoint : 
      GetFirstListItem<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UnityEngine_Keyframe : GetFirstListItem<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UnityEngine_LayerMask : 
      GetFirstListItem<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UnityEngine_Quaternion : 
      GetFirstListItem<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UnityEngine_RaycastHit : 
      GetFirstListItem<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UnityEngine_Rect : GetFirstListItem<Rect>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UnityEngine_Space : GetFirstListItem<Space>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UnityEngine_Vector2 : GetFirstListItem<Vector2>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UnityEngine_Vector3 : GetFirstListItem<Vector3>
    {
    }

    public class FlowCanvas_Nodes_GetFirstListItem_UnityEngine_Vector4 : GetFirstListItem<Vector4>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_BlockTypeEnum : GetLastListItem<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_IntensityParameter_UnityEngine_Color_ : 
      GetLastListItem<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UiEffectType : GetLastListItem<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_Engine_Common_Commons_ActionEnum : 
      GetLastListItem<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_Engine_Common_Commons_DiseasedStateEnum : 
      GetLastListItem<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      GetLastListItem<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_Engine_Common_Components_Parameters_ParameterNameEnum : 
      GetLastListItem<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_Engine_Common_Components_Regions_BuildingEnum : 
      GetLastListItem<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_Engine_Common_Components_Regions_RegionEnum : 
      GetLastListItem<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_Engine_Common_DateTime_TimesOfDay : 
      GetLastListItem<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_Engine_Source_Connections_IEntitySerializable : 
      GetLastListItem<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_Engine_Source_Connections_LipSyncObjectSerializable : 
      GetLastListItem<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      GetLastListItem<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_Engine_Source_Services_CameraServices_CameraKindEnum : 
      GetLastListItem<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_Engine_Source_Services_Inputs_GameActionType : 
      GetLastListItem<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_bool : GetLastListItem<bool>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_int : GetLastListItem<int>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_float : GetLastListItem<float>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UnityEngine_Bounds : GetLastListItem<Bounds>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UnityEngine_Color : GetLastListItem<Color>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UnityEngine_ContactPoint : 
      GetLastListItem<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UnityEngine_Keyframe : GetLastListItem<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UnityEngine_LayerMask : GetLastListItem<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UnityEngine_Quaternion : 
      GetLastListItem<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UnityEngine_RaycastHit : 
      GetLastListItem<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UnityEngine_Rect : GetLastListItem<Rect>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UnityEngine_Space : GetLastListItem<Space>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UnityEngine_Vector2 : GetLastListItem<Vector2>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UnityEngine_Vector3 : GetLastListItem<Vector3>
    {
    }

    public class FlowCanvas_Nodes_GetLastListItem_UnityEngine_Vector4 : GetLastListItem<Vector4>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_BlockTypeEnum : GetListItem<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_IntensityParameter_UnityEngine_Color_ : 
      GetListItem<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UiEffectType : GetListItem<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_Engine_Common_Commons_ActionEnum : 
      GetListItem<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_Engine_Common_Commons_DiseasedStateEnum : 
      GetListItem<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      GetListItem<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_Engine_Common_Components_Parameters_ParameterNameEnum : 
      GetListItem<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_Engine_Common_Components_Regions_BuildingEnum : 
      GetListItem<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_Engine_Common_Components_Regions_RegionEnum : 
      GetListItem<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_Engine_Common_DateTime_TimesOfDay : 
      GetListItem<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_Engine_Source_Connections_IEntitySerializable : 
      GetListItem<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_Engine_Source_Connections_LipSyncObjectSerializable : 
      GetListItem<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      GetListItem<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_Engine_Source_Services_CameraServices_CameraKindEnum : 
      GetListItem<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_Engine_Source_Services_Inputs_GameActionType : 
      GetListItem<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_bool : GetListItem<bool>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_int : GetListItem<int>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_float : GetListItem<float>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UnityEngine_Bounds : GetListItem<Bounds>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UnityEngine_Color : GetListItem<Color>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UnityEngine_ContactPoint : GetListItem<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UnityEngine_Keyframe : GetListItem<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UnityEngine_LayerMask : GetListItem<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UnityEngine_Quaternion : GetListItem<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UnityEngine_RaycastHit : GetListItem<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UnityEngine_Rect : GetListItem<Rect>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UnityEngine_Space : GetListItem<Space>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UnityEngine_Vector2 : GetListItem<Vector2>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UnityEngine_Vector3 : GetListItem<Vector3>
    {
    }

    public class FlowCanvas_Nodes_GetListItem_UnityEngine_Vector4 : GetListItem<Vector4>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_BlockTypeEnum : GetOtherVariable<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_IntensityParameter_UnityEngine_Color_ : 
      GetOtherVariable<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UiEffectType : GetOtherVariable<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_Engine_Common_Commons_ActionEnum : 
      GetOtherVariable<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_Engine_Common_Commons_DiseasedStateEnum : 
      GetOtherVariable<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      GetOtherVariable<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_Engine_Common_Components_Parameters_ParameterNameEnum : 
      GetOtherVariable<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_Engine_Common_Components_Regions_BuildingEnum : 
      GetOtherVariable<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_Engine_Common_Components_Regions_RegionEnum : 
      GetOtherVariable<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_Engine_Common_DateTime_TimesOfDay : 
      GetOtherVariable<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_Engine_Source_Connections_IEntitySerializable : 
      GetOtherVariable<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_Engine_Source_Connections_LipSyncObjectSerializable : 
      GetOtherVariable<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      GetOtherVariable<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_Engine_Source_Services_CameraServices_CameraKindEnum : 
      GetOtherVariable<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_Engine_Source_Services_Inputs_GameActionType : 
      GetOtherVariable<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_bool : GetOtherVariable<bool>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_int : GetOtherVariable<int>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_float : GetOtherVariable<float>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UnityEngine_Bounds : GetOtherVariable<Bounds>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UnityEngine_Color : GetOtherVariable<Color>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UnityEngine_ContactPoint : 
      GetOtherVariable<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UnityEngine_Keyframe : GetOtherVariable<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UnityEngine_LayerMask : 
      GetOtherVariable<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UnityEngine_Quaternion : 
      GetOtherVariable<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UnityEngine_RaycastHit : 
      GetOtherVariable<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UnityEngine_Rect : GetOtherVariable<Rect>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UnityEngine_Space : GetOtherVariable<Space>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UnityEngine_Vector2 : GetOtherVariable<Vector2>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UnityEngine_Vector3 : GetOtherVariable<Vector3>
    {
    }

    public class FlowCanvas_Nodes_GetOtherVariable_UnityEngine_Vector4 : GetOtherVariable<Vector4>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_BlockTypeEnum : GetRandomListItem<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_IntensityParameter_UnityEngine_Color_ : 
      GetRandomListItem<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UiEffectType : GetRandomListItem<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_Engine_Common_Commons_ActionEnum : 
      GetRandomListItem<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_Engine_Common_Commons_DiseasedStateEnum : 
      GetRandomListItem<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      GetRandomListItem<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_Engine_Common_Components_Parameters_ParameterNameEnum : 
      GetRandomListItem<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_Engine_Common_Components_Regions_BuildingEnum : 
      GetRandomListItem<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_Engine_Common_Components_Regions_RegionEnum : 
      GetRandomListItem<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_Engine_Common_DateTime_TimesOfDay : 
      GetRandomListItem<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_Engine_Source_Connections_IEntitySerializable : 
      GetRandomListItem<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_Engine_Source_Connections_LipSyncObjectSerializable : 
      GetRandomListItem<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      GetRandomListItem<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_Engine_Source_Services_CameraServices_CameraKindEnum : 
      GetRandomListItem<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_Engine_Source_Services_Inputs_GameActionType : 
      GetRandomListItem<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_bool : GetRandomListItem<bool>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_int : GetRandomListItem<int>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_float : GetRandomListItem<float>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UnityEngine_Bounds : GetRandomListItem<Bounds>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UnityEngine_Color : GetRandomListItem<Color>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UnityEngine_ContactPoint : 
      GetRandomListItem<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UnityEngine_Keyframe : 
      GetRandomListItem<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UnityEngine_LayerMask : 
      GetRandomListItem<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UnityEngine_Quaternion : 
      GetRandomListItem<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UnityEngine_RaycastHit : 
      GetRandomListItem<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UnityEngine_Rect : GetRandomListItem<Rect>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UnityEngine_Space : GetRandomListItem<Space>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UnityEngine_Vector2 : GetRandomListItem<Vector2>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UnityEngine_Vector3 : GetRandomListItem<Vector3>
    {
    }

    public class FlowCanvas_Nodes_GetRandomListItem_UnityEngine_Vector4 : GetRandomListItem<Vector4>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_BlockTypeEnum : GetVariable<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_IntensityParameter_UnityEngine_Color_ : 
      GetVariable<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UiEffectType : GetVariable<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_Engine_Common_Commons_ActionEnum : 
      GetVariable<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_Engine_Common_Commons_DiseasedStateEnum : 
      GetVariable<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      GetVariable<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_Engine_Common_Components_Parameters_ParameterNameEnum : 
      GetVariable<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_Engine_Common_Components_Regions_BuildingEnum : 
      GetVariable<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_Engine_Common_Components_Regions_RegionEnum : 
      GetVariable<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_Engine_Common_DateTime_TimesOfDay : 
      GetVariable<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_Engine_Source_Connections_IEntitySerializable : 
      GetVariable<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_Engine_Source_Connections_LipSyncObjectSerializable : 
      GetVariable<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      GetVariable<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_Engine_Source_Services_CameraServices_CameraKindEnum : 
      GetVariable<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_Engine_Source_Services_Inputs_GameActionType : 
      GetVariable<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_bool : GetVariable<bool>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_int : GetVariable<int>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_float : GetVariable<float>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UnityEngine_Bounds : GetVariable<Bounds>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UnityEngine_Color : GetVariable<Color>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UnityEngine_ContactPoint : GetVariable<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UnityEngine_Keyframe : GetVariable<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UnityEngine_LayerMask : GetVariable<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UnityEngine_Quaternion : GetVariable<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UnityEngine_RaycastHit : GetVariable<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UnityEngine_Rect : GetVariable<Rect>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UnityEngine_Space : GetVariable<Space>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UnityEngine_Vector2 : GetVariable<Vector2>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UnityEngine_Vector3 : GetVariable<Vector3>
    {
    }

    public class FlowCanvas_Nodes_GetVariable_UnityEngine_Vector4 : GetVariable<Vector4>
    {
    }

    public class FlowCanvas_Nodes_Identity_BlockTypeEnum : Identity<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_Identity_IntensityParameter_UnityEngine_Color_ : 
      Identity<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_Identity_UiEffectType : Identity<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_Identity_Engine_Common_Commons_ActionEnum : Identity<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_Identity_Engine_Common_Commons_DiseasedStateEnum : 
      Identity<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_Identity_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      Identity<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_Identity_Engine_Common_Components_Parameters_ParameterNameEnum : 
      Identity<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_Identity_Engine_Common_Components_Regions_BuildingEnum : 
      Identity<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_Identity_Engine_Common_Components_Regions_RegionEnum : 
      Identity<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_Identity_Engine_Common_DateTime_TimesOfDay : Identity<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_Identity_Engine_Source_Connections_IEntitySerializable : 
      Identity<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_Identity_Engine_Source_Connections_LipSyncObjectSerializable : 
      Identity<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_Identity_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      Identity<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_Identity_Engine_Source_Services_CameraServices_CameraKindEnum : 
      Identity<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_Identity_Engine_Source_Services_Inputs_GameActionType : 
      Identity<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_Identity_bool : Identity<bool>
    {
    }

    public class FlowCanvas_Nodes_Identity_int : Identity<int>
    {
    }

    public class FlowCanvas_Nodes_Identity_float : Identity<float>
    {
    }

    public class FlowCanvas_Nodes_Identity_UnityEngine_Bounds : Identity<Bounds>
    {
    }

    public class FlowCanvas_Nodes_Identity_UnityEngine_Color : Identity<Color>
    {
    }

    public class FlowCanvas_Nodes_Identity_UnityEngine_ContactPoint : Identity<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_Identity_UnityEngine_Keyframe : Identity<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_Identity_UnityEngine_LayerMask : Identity<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_Identity_UnityEngine_Quaternion : Identity<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_Identity_UnityEngine_RaycastHit : Identity<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_Identity_UnityEngine_Rect : Identity<Rect>
    {
    }

    public class FlowCanvas_Nodes_Identity_UnityEngine_Space : Identity<Space>
    {
    }

    public class FlowCanvas_Nodes_Identity_UnityEngine_Vector2 : Identity<Vector2>
    {
    }

    public class FlowCanvas_Nodes_Identity_UnityEngine_Vector3 : Identity<Vector3>
    {
    }

    public class FlowCanvas_Nodes_Identity_UnityEngine_Vector4 : Identity<Vector4>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_BlockTypeEnum : InsertListItem<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_IntensityParameter_UnityEngine_Color_ : 
      InsertListItem<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UiEffectType : InsertListItem<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_Engine_Common_Commons_ActionEnum : 
      InsertListItem<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_Engine_Common_Commons_DiseasedStateEnum : 
      InsertListItem<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      InsertListItem<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_Engine_Common_Components_Parameters_ParameterNameEnum : 
      InsertListItem<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_Engine_Common_Components_Regions_BuildingEnum : 
      InsertListItem<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_Engine_Common_Components_Regions_RegionEnum : 
      InsertListItem<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_Engine_Common_DateTime_TimesOfDay : 
      InsertListItem<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_Engine_Source_Connections_IEntitySerializable : 
      InsertListItem<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_Engine_Source_Connections_LipSyncObjectSerializable : 
      InsertListItem<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      InsertListItem<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_Engine_Source_Services_CameraServices_CameraKindEnum : 
      InsertListItem<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_Engine_Source_Services_Inputs_GameActionType : 
      InsertListItem<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_bool : InsertListItem<bool>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_int : InsertListItem<int>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_float : InsertListItem<float>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UnityEngine_Bounds : InsertListItem<Bounds>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UnityEngine_Color : InsertListItem<Color>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UnityEngine_ContactPoint : 
      InsertListItem<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UnityEngine_Keyframe : InsertListItem<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UnityEngine_LayerMask : InsertListItem<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UnityEngine_Quaternion : InsertListItem<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UnityEngine_RaycastHit : InsertListItem<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UnityEngine_Rect : InsertListItem<Rect>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UnityEngine_Space : InsertListItem<Space>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UnityEngine_Vector2 : InsertListItem<Vector2>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UnityEngine_Vector3 : InsertListItem<Vector3>
    {
    }

    public class FlowCanvas_Nodes_InsertListItem_UnityEngine_Vector4 : InsertListItem<Vector4>
    {
    }

    public class FlowCanvas_Nodes_PickValue_BlockTypeEnum : PickValue<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_PickValue_IntensityParameter_UnityEngine_Color_ : 
      PickValue<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UiEffectType : PickValue<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_PickValue_Engine_Common_Commons_ActionEnum : PickValue<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_PickValue_Engine_Common_Commons_DiseasedStateEnum : 
      PickValue<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_PickValue_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      PickValue<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_PickValue_Engine_Common_Components_Parameters_ParameterNameEnum : 
      PickValue<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_PickValue_Engine_Common_Components_Regions_BuildingEnum : 
      PickValue<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_PickValue_Engine_Common_Components_Regions_RegionEnum : 
      PickValue<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_PickValue_Engine_Common_DateTime_TimesOfDay : PickValue<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_PickValue_Engine_Source_Connections_IEntitySerializable : 
      PickValue<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_PickValue_Engine_Source_Connections_LipSyncObjectSerializable : 
      PickValue<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_PickValue_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      PickValue<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_PickValue_Engine_Source_Services_CameraServices_CameraKindEnum : 
      PickValue<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_PickValue_Engine_Source_Services_Inputs_GameActionType : 
      PickValue<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_PickValue_bool : PickValue<bool>
    {
    }

    public class FlowCanvas_Nodes_PickValue_int : PickValue<int>
    {
    }

    public class FlowCanvas_Nodes_PickValue_float : PickValue<float>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UnityEngine_Bounds : PickValue<Bounds>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UnityEngine_Color : PickValue<Color>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UnityEngine_ContactPoint : PickValue<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UnityEngine_Keyframe : PickValue<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UnityEngine_LayerMask : PickValue<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UnityEngine_Quaternion : PickValue<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UnityEngine_RaycastHit : PickValue<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UnityEngine_Rect : PickValue<Rect>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UnityEngine_Space : PickValue<Space>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UnityEngine_Vector2 : PickValue<Vector2>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UnityEngine_Vector3 : PickValue<Vector3>
    {
    }

    public class FlowCanvas_Nodes_PickValue_UnityEngine_Vector4 : PickValue<Vector4>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_BlockTypeEnum : RelayValueInput<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_IntensityParameter_UnityEngine_Color_ : 
      RelayValueInput<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UiEffectType : RelayValueInput<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_Engine_Common_Commons_ActionEnum : 
      RelayValueInput<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_Engine_Common_Commons_DiseasedStateEnum : 
      RelayValueInput<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      RelayValueInput<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_Engine_Common_Components_Parameters_ParameterNameEnum : 
      RelayValueInput<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_Engine_Common_Components_Regions_BuildingEnum : 
      RelayValueInput<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_Engine_Common_Components_Regions_RegionEnum : 
      RelayValueInput<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_Engine_Common_DateTime_TimesOfDay : 
      RelayValueInput<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_Engine_Source_Connections_IEntitySerializable : 
      RelayValueInput<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_Engine_Source_Connections_LipSyncObjectSerializable : 
      RelayValueInput<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      RelayValueInput<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_Engine_Source_Services_CameraServices_CameraKindEnum : 
      RelayValueInput<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_Engine_Source_Services_Inputs_GameActionType : 
      RelayValueInput<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_bool : RelayValueInput<bool>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_int : RelayValueInput<int>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_float : RelayValueInput<float>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UnityEngine_Bounds : RelayValueInput<Bounds>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UnityEngine_Color : RelayValueInput<Color>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UnityEngine_ContactPoint : 
      RelayValueInput<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UnityEngine_Keyframe : RelayValueInput<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UnityEngine_LayerMask : RelayValueInput<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UnityEngine_Quaternion : 
      RelayValueInput<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UnityEngine_RaycastHit : 
      RelayValueInput<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UnityEngine_Rect : RelayValueInput<Rect>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UnityEngine_Space : RelayValueInput<Space>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UnityEngine_Vector2 : RelayValueInput<Vector2>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UnityEngine_Vector3 : RelayValueInput<Vector3>
    {
    }

    public class FlowCanvas_Nodes_RelayValueInput_UnityEngine_Vector4 : RelayValueInput<Vector4>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_BlockTypeEnum : RelayValueOutput<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_IntensityParameter_UnityEngine_Color_ : 
      RelayValueOutput<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UiEffectType : RelayValueOutput<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_Engine_Common_Commons_ActionEnum : 
      RelayValueOutput<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_Engine_Common_Commons_DiseasedStateEnum : 
      RelayValueOutput<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      RelayValueOutput<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_Engine_Common_Components_Parameters_ParameterNameEnum : 
      RelayValueOutput<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_Engine_Common_Components_Regions_BuildingEnum : 
      RelayValueOutput<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_Engine_Common_Components_Regions_RegionEnum : 
      RelayValueOutput<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_Engine_Common_DateTime_TimesOfDay : 
      RelayValueOutput<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_Engine_Source_Connections_IEntitySerializable : 
      RelayValueOutput<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_Engine_Source_Connections_LipSyncObjectSerializable : 
      RelayValueOutput<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      RelayValueOutput<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_Engine_Source_Services_CameraServices_CameraKindEnum : 
      RelayValueOutput<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_Engine_Source_Services_Inputs_GameActionType : 
      RelayValueOutput<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_bool : RelayValueOutput<bool>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_int : RelayValueOutput<int>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_float : RelayValueOutput<float>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UnityEngine_Bounds : RelayValueOutput<Bounds>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UnityEngine_Color : RelayValueOutput<Color>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UnityEngine_ContactPoint : 
      RelayValueOutput<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UnityEngine_Keyframe : RelayValueOutput<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UnityEngine_LayerMask : 
      RelayValueOutput<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UnityEngine_Quaternion : 
      RelayValueOutput<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UnityEngine_RaycastHit : 
      RelayValueOutput<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UnityEngine_Rect : RelayValueOutput<Rect>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UnityEngine_Space : RelayValueOutput<Space>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UnityEngine_Vector2 : RelayValueOutput<Vector2>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UnityEngine_Vector3 : RelayValueOutput<Vector3>
    {
    }

    public class FlowCanvas_Nodes_RelayValueOutput_UnityEngine_Vector4 : RelayValueOutput<Vector4>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_BlockTypeEnum : RemoveListItem<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_IntensityParameter_UnityEngine_Color_ : 
      RemoveListItem<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UiEffectType : RemoveListItem<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_Engine_Common_Commons_ActionEnum : 
      RemoveListItem<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_Engine_Common_Commons_DiseasedStateEnum : 
      RemoveListItem<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      RemoveListItem<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_Engine_Common_Components_Parameters_ParameterNameEnum : 
      RemoveListItem<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_Engine_Common_Components_Regions_BuildingEnum : 
      RemoveListItem<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_Engine_Common_Components_Regions_RegionEnum : 
      RemoveListItem<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_Engine_Common_DateTime_TimesOfDay : 
      RemoveListItem<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_Engine_Source_Connections_IEntitySerializable : 
      RemoveListItem<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_Engine_Source_Connections_LipSyncObjectSerializable : 
      RemoveListItem<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      RemoveListItem<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_Engine_Source_Services_CameraServices_CameraKindEnum : 
      RemoveListItem<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_Engine_Source_Services_Inputs_GameActionType : 
      RemoveListItem<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_bool : RemoveListItem<bool>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_int : RemoveListItem<int>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_float : RemoveListItem<float>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UnityEngine_Bounds : RemoveListItem<Bounds>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UnityEngine_Color : RemoveListItem<Color>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UnityEngine_ContactPoint : 
      RemoveListItem<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UnityEngine_Keyframe : RemoveListItem<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UnityEngine_LayerMask : RemoveListItem<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UnityEngine_Quaternion : RemoveListItem<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UnityEngine_RaycastHit : RemoveListItem<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UnityEngine_Rect : RemoveListItem<Rect>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UnityEngine_Space : RemoveListItem<Space>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UnityEngine_Vector2 : RemoveListItem<Vector2>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UnityEngine_Vector3 : RemoveListItem<Vector3>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItem_UnityEngine_Vector4 : RemoveListItem<Vector4>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_BlockTypeEnum : RemoveListItemAt<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_IntensityParameter_UnityEngine_Color_ : 
      RemoveListItemAt<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UiEffectType : RemoveListItemAt<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_Engine_Common_Commons_ActionEnum : 
      RemoveListItemAt<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_Engine_Common_Commons_DiseasedStateEnum : 
      RemoveListItemAt<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      RemoveListItemAt<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_Engine_Common_Components_Parameters_ParameterNameEnum : 
      RemoveListItemAt<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_Engine_Common_Components_Regions_BuildingEnum : 
      RemoveListItemAt<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_Engine_Common_Components_Regions_RegionEnum : 
      RemoveListItemAt<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_Engine_Common_DateTime_TimesOfDay : 
      RemoveListItemAt<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_Engine_Source_Connections_IEntitySerializable : 
      RemoveListItemAt<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_Engine_Source_Connections_LipSyncObjectSerializable : 
      RemoveListItemAt<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      RemoveListItemAt<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_Engine_Source_Services_CameraServices_CameraKindEnum : 
      RemoveListItemAt<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_Engine_Source_Services_Inputs_GameActionType : 
      RemoveListItemAt<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_bool : RemoveListItemAt<bool>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_int : RemoveListItemAt<int>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_float : RemoveListItemAt<float>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UnityEngine_Bounds : RemoveListItemAt<Bounds>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UnityEngine_Color : RemoveListItemAt<Color>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UnityEngine_ContactPoint : 
      RemoveListItemAt<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UnityEngine_Keyframe : RemoveListItemAt<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UnityEngine_LayerMask : 
      RemoveListItemAt<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UnityEngine_Quaternion : 
      RemoveListItemAt<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UnityEngine_RaycastHit : 
      RemoveListItemAt<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UnityEngine_Rect : RemoveListItemAt<Rect>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UnityEngine_Space : RemoveListItemAt<Space>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UnityEngine_Vector2 : RemoveListItemAt<Vector2>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UnityEngine_Vector3 : RemoveListItemAt<Vector3>
    {
    }

    public class FlowCanvas_Nodes_RemoveListItemAt_UnityEngine_Vector4 : RemoveListItemAt<Vector4>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_BlockTypeEnum : SendEvent<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_IntensityParameter_UnityEngine_Color_ : 
      SendEvent<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UiEffectType : SendEvent<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_Engine_Common_Commons_ActionEnum : SendEvent<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_Engine_Common_Commons_DiseasedStateEnum : 
      SendEvent<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      SendEvent<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_Engine_Common_Components_Parameters_ParameterNameEnum : 
      SendEvent<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_Engine_Common_Components_Regions_BuildingEnum : 
      SendEvent<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_Engine_Common_Components_Regions_RegionEnum : 
      SendEvent<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_Engine_Common_DateTime_TimesOfDay : SendEvent<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_Engine_Source_Connections_IEntitySerializable : 
      SendEvent<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_Engine_Source_Connections_LipSyncObjectSerializable : 
      SendEvent<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      SendEvent<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_Engine_Source_Services_CameraServices_CameraKindEnum : 
      SendEvent<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_Engine_Source_Services_Inputs_GameActionType : 
      SendEvent<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_bool : SendEvent<bool>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_int : SendEvent<int>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_float : SendEvent<float>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UnityEngine_Bounds : SendEvent<Bounds>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UnityEngine_Color : SendEvent<Color>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UnityEngine_ContactPoint : SendEvent<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UnityEngine_Keyframe : SendEvent<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UnityEngine_LayerMask : SendEvent<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UnityEngine_Quaternion : SendEvent<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UnityEngine_RaycastHit : SendEvent<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UnityEngine_Rect : SendEvent<Rect>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UnityEngine_Space : SendEvent<Space>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UnityEngine_Vector2 : SendEvent<Vector2>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UnityEngine_Vector3 : SendEvent<Vector3>
    {
    }

    public class FlowCanvas_Nodes_SendEvent_UnityEngine_Vector4 : SendEvent<Vector4>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_BlockTypeEnum : SetListItem<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_IntensityParameter_UnityEngine_Color_ : 
      SetListItem<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UiEffectType : SetListItem<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_Engine_Common_Commons_ActionEnum : 
      SetListItem<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_Engine_Common_Commons_DiseasedStateEnum : 
      SetListItem<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      SetListItem<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_Engine_Common_Components_Parameters_ParameterNameEnum : 
      SetListItem<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_Engine_Common_Components_Regions_BuildingEnum : 
      SetListItem<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_Engine_Common_Components_Regions_RegionEnum : 
      SetListItem<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_Engine_Common_DateTime_TimesOfDay : 
      SetListItem<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_Engine_Source_Connections_IEntitySerializable : 
      SetListItem<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_Engine_Source_Connections_LipSyncObjectSerializable : 
      SetListItem<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      SetListItem<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_Engine_Source_Services_CameraServices_CameraKindEnum : 
      SetListItem<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_Engine_Source_Services_Inputs_GameActionType : 
      SetListItem<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_bool : SetListItem<bool>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_int : SetListItem<int>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_float : SetListItem<float>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UnityEngine_Bounds : SetListItem<Bounds>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UnityEngine_Color : SetListItem<Color>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UnityEngine_ContactPoint : SetListItem<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UnityEngine_Keyframe : SetListItem<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UnityEngine_LayerMask : SetListItem<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UnityEngine_Quaternion : SetListItem<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UnityEngine_RaycastHit : SetListItem<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UnityEngine_Rect : SetListItem<Rect>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UnityEngine_Space : SetListItem<Space>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UnityEngine_Vector2 : SetListItem<Vector2>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UnityEngine_Vector3 : SetListItem<Vector3>
    {
    }

    public class FlowCanvas_Nodes_SetListItem_UnityEngine_Vector4 : SetListItem<Vector4>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_BlockTypeEnum : SetOtherVariable<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_IntensityParameter_UnityEngine_Color_ : 
      SetOtherVariable<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UiEffectType : SetOtherVariable<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_Engine_Common_Commons_ActionEnum : 
      SetOtherVariable<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_Engine_Common_Commons_DiseasedStateEnum : 
      SetOtherVariable<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      SetOtherVariable<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_Engine_Common_Components_Parameters_ParameterNameEnum : 
      SetOtherVariable<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_Engine_Common_Components_Regions_BuildingEnum : 
      SetOtherVariable<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_Engine_Common_Components_Regions_RegionEnum : 
      SetOtherVariable<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_Engine_Common_DateTime_TimesOfDay : 
      SetOtherVariable<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_Engine_Source_Connections_IEntitySerializable : 
      SetOtherVariable<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_Engine_Source_Connections_LipSyncObjectSerializable : 
      SetOtherVariable<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      SetOtherVariable<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_Engine_Source_Services_CameraServices_CameraKindEnum : 
      SetOtherVariable<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_Engine_Source_Services_Inputs_GameActionType : 
      SetOtherVariable<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_bool : SetOtherVariable<bool>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_int : SetOtherVariable<int>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_float : SetOtherVariable<float>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UnityEngine_Bounds : SetOtherVariable<Bounds>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UnityEngine_Color : SetOtherVariable<Color>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UnityEngine_ContactPoint : 
      SetOtherVariable<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UnityEngine_Keyframe : SetOtherVariable<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UnityEngine_LayerMask : 
      SetOtherVariable<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UnityEngine_Quaternion : 
      SetOtherVariable<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UnityEngine_RaycastHit : 
      SetOtherVariable<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UnityEngine_Rect : SetOtherVariable<Rect>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UnityEngine_Space : SetOtherVariable<Space>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UnityEngine_Vector2 : SetOtherVariable<Vector2>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UnityEngine_Vector3 : SetOtherVariable<Vector3>
    {
    }

    public class FlowCanvas_Nodes_SetOtherVariable_UnityEngine_Vector4 : SetOtherVariable<Vector4>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_BlockTypeEnum : SetVariable<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_IntensityParameter_UnityEngine_Color_ : 
      SetVariable<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UiEffectType : SetVariable<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_Engine_Common_Commons_ActionEnum : 
      SetVariable<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_Engine_Common_Commons_DiseasedStateEnum : 
      SetVariable<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      SetVariable<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_Engine_Common_Components_Parameters_ParameterNameEnum : 
      SetVariable<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_Engine_Common_Components_Regions_BuildingEnum : 
      SetVariable<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_Engine_Common_Components_Regions_RegionEnum : 
      SetVariable<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_Engine_Common_DateTime_TimesOfDay : 
      SetVariable<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_Engine_Source_Connections_IEntitySerializable : 
      SetVariable<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_Engine_Source_Connections_LipSyncObjectSerializable : 
      SetVariable<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      SetVariable<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_Engine_Source_Services_CameraServices_CameraKindEnum : 
      SetVariable<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_Engine_Source_Services_Inputs_GameActionType : 
      SetVariable<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_bool : SetVariable<bool>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_int : SetVariable<int>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_float : SetVariable<float>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UnityEngine_Bounds : SetVariable<Bounds>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UnityEngine_Color : SetVariable<Color>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UnityEngine_ContactPoint : SetVariable<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UnityEngine_Keyframe : SetVariable<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UnityEngine_LayerMask : SetVariable<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UnityEngine_Quaternion : SetVariable<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UnityEngine_RaycastHit : SetVariable<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UnityEngine_Rect : SetVariable<Rect>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UnityEngine_Space : SetVariable<Space>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UnityEngine_Vector2 : SetVariable<Vector2>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UnityEngine_Vector3 : SetVariable<Vector3>
    {
    }

    public class FlowCanvas_Nodes_SetVariable_UnityEngine_Vector4 : SetVariable<Vector4>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_BlockTypeEnum : ShuffleList<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_IntensityParameter_UnityEngine_Color_ : 
      ShuffleList<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UiEffectType : ShuffleList<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_Engine_Common_Commons_ActionEnum : 
      ShuffleList<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_Engine_Common_Commons_DiseasedStateEnum : 
      ShuffleList<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      ShuffleList<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_Engine_Common_Components_Parameters_ParameterNameEnum : 
      ShuffleList<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_Engine_Common_Components_Regions_BuildingEnum : 
      ShuffleList<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_Engine_Common_Components_Regions_RegionEnum : 
      ShuffleList<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_Engine_Common_DateTime_TimesOfDay : 
      ShuffleList<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_Engine_Source_Connections_IEntitySerializable : 
      ShuffleList<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_Engine_Source_Connections_LipSyncObjectSerializable : 
      ShuffleList<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      ShuffleList<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_Engine_Source_Services_CameraServices_CameraKindEnum : 
      ShuffleList<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_Engine_Source_Services_Inputs_GameActionType : 
      ShuffleList<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_bool : ShuffleList<bool>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_int : ShuffleList<int>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_float : ShuffleList<float>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UnityEngine_Bounds : ShuffleList<Bounds>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UnityEngine_Color : ShuffleList<Color>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UnityEngine_ContactPoint : ShuffleList<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UnityEngine_Keyframe : ShuffleList<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UnityEngine_LayerMask : ShuffleList<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UnityEngine_Quaternion : ShuffleList<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UnityEngine_RaycastHit : ShuffleList<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UnityEngine_Rect : ShuffleList<Rect>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UnityEngine_Space : ShuffleList<Space>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UnityEngine_Vector2 : ShuffleList<Vector2>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UnityEngine_Vector3 : ShuffleList<Vector3>
    {
    }

    public class FlowCanvas_Nodes_ShuffleList_UnityEngine_Vector4 : ShuffleList<Vector4>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_BlockTypeEnum : StaticCodeEvent<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_IntensityParameter_UnityEngine_Color_ : 
      StaticCodeEvent<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UiEffectType : StaticCodeEvent<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_Engine_Common_Commons_ActionEnum : 
      StaticCodeEvent<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_Engine_Common_Commons_DiseasedStateEnum : 
      StaticCodeEvent<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      StaticCodeEvent<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_Engine_Common_Components_Parameters_ParameterNameEnum : 
      StaticCodeEvent<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_Engine_Common_Components_Regions_BuildingEnum : 
      StaticCodeEvent<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_Engine_Common_Components_Regions_RegionEnum : 
      StaticCodeEvent<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_Engine_Common_DateTime_TimesOfDay : 
      StaticCodeEvent<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_Engine_Source_Connections_IEntitySerializable : 
      StaticCodeEvent<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_Engine_Source_Connections_LipSyncObjectSerializable : 
      StaticCodeEvent<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      StaticCodeEvent<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_Engine_Source_Services_CameraServices_CameraKindEnum : 
      StaticCodeEvent<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_Engine_Source_Services_Inputs_GameActionType : 
      StaticCodeEvent<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_bool : StaticCodeEvent<bool>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_int : StaticCodeEvent<int>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_float : StaticCodeEvent<float>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UnityEngine_Bounds : StaticCodeEvent<Bounds>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UnityEngine_Color : StaticCodeEvent<Color>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UnityEngine_ContactPoint : 
      StaticCodeEvent<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UnityEngine_Keyframe : StaticCodeEvent<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UnityEngine_LayerMask : StaticCodeEvent<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UnityEngine_Quaternion : 
      StaticCodeEvent<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UnityEngine_RaycastHit : 
      StaticCodeEvent<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UnityEngine_Rect : StaticCodeEvent<Rect>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UnityEngine_Space : StaticCodeEvent<Space>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UnityEngine_Vector2 : StaticCodeEvent<Vector2>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UnityEngine_Vector3 : StaticCodeEvent<Vector3>
    {
    }

    public class FlowCanvas_Nodes_StaticCodeEvent_UnityEngine_Vector4 : StaticCodeEvent<Vector4>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_BlockTypeEnum : SwitchValue<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_IntensityParameter_UnityEngine_Color_ : 
      SwitchValue<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UiEffectType : SwitchValue<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_Engine_Common_Commons_ActionEnum : 
      SwitchValue<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_Engine_Common_Commons_DiseasedStateEnum : 
      SwitchValue<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      SwitchValue<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_Engine_Common_Components_Parameters_ParameterNameEnum : 
      SwitchValue<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_Engine_Common_Components_Regions_BuildingEnum : 
      SwitchValue<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_Engine_Common_Components_Regions_RegionEnum : 
      SwitchValue<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_Engine_Common_DateTime_TimesOfDay : 
      SwitchValue<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_Engine_Source_Connections_IEntitySerializable : 
      SwitchValue<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_Engine_Source_Connections_LipSyncObjectSerializable : 
      SwitchValue<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      SwitchValue<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_Engine_Source_Services_CameraServices_CameraKindEnum : 
      SwitchValue<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_Engine_Source_Services_Inputs_GameActionType : 
      SwitchValue<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_bool : SwitchValue<bool>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_int : SwitchValue<int>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_float : SwitchValue<float>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UnityEngine_Bounds : SwitchValue<Bounds>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UnityEngine_Color : SwitchValue<Color>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UnityEngine_ContactPoint : SwitchValue<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UnityEngine_Keyframe : SwitchValue<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UnityEngine_LayerMask : SwitchValue<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UnityEngine_Quaternion : SwitchValue<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UnityEngine_RaycastHit : SwitchValue<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UnityEngine_Rect : SwitchValue<Rect>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UnityEngine_Space : SwitchValue<Space>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UnityEngine_Vector2 : SwitchValue<Vector2>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UnityEngine_Vector3 : SwitchValue<Vector3>
    {
    }

    public class FlowCanvas_Nodes_SwitchValue_UnityEngine_Vector4 : SwitchValue<Vector4>
    {
    }

    public class FlowCanvas_Nodes_ToArray_BlockTypeEnum : ToArray<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_ToArray_IntensityParameter_UnityEngine_Color_ : 
      ToArray<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UiEffectType : ToArray<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_ToArray_Engine_Common_Commons_ActionEnum : ToArray<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_ToArray_Engine_Common_Commons_DiseasedStateEnum : 
      ToArray<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_ToArray_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      ToArray<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_ToArray_Engine_Common_Components_Parameters_ParameterNameEnum : 
      ToArray<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_ToArray_Engine_Common_Components_Regions_BuildingEnum : 
      ToArray<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_ToArray_Engine_Common_Components_Regions_RegionEnum : 
      ToArray<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_ToArray_Engine_Common_DateTime_TimesOfDay : ToArray<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_ToArray_Engine_Source_Connections_IEntitySerializable : 
      ToArray<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_ToArray_Engine_Source_Connections_LipSyncObjectSerializable : 
      ToArray<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_ToArray_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      ToArray<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_ToArray_Engine_Source_Services_CameraServices_CameraKindEnum : 
      ToArray<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_ToArray_Engine_Source_Services_Inputs_GameActionType : 
      ToArray<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_ToArray_bool : ToArray<bool>
    {
    }

    public class FlowCanvas_Nodes_ToArray_int : ToArray<int>
    {
    }

    public class FlowCanvas_Nodes_ToArray_float : ToArray<float>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UnityEngine_Bounds : ToArray<Bounds>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UnityEngine_Color : ToArray<Color>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UnityEngine_ContactPoint : ToArray<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UnityEngine_Keyframe : ToArray<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UnityEngine_LayerMask : ToArray<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UnityEngine_Quaternion : ToArray<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UnityEngine_RaycastHit : ToArray<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UnityEngine_Rect : ToArray<Rect>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UnityEngine_Space : ToArray<Space>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UnityEngine_Vector2 : ToArray<Vector2>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UnityEngine_Vector3 : ToArray<Vector3>
    {
    }

    public class FlowCanvas_Nodes_ToArray_UnityEngine_Vector4 : ToArray<Vector4>
    {
    }

    public class FlowCanvas_Nodes_ToList_BlockTypeEnum : ToList<BlockTypeEnum>
    {
    }

    public class FlowCanvas_Nodes_ToList_IntensityParameter_UnityEngine_Color_ : 
      ToList<IntensityParameter<Color>>
    {
    }

    public class FlowCanvas_Nodes_ToList_UiEffectType : ToList<UiEffectType>
    {
    }

    public class FlowCanvas_Nodes_ToList_Engine_Common_Commons_ActionEnum : ToList<ActionEnum>
    {
    }

    public class FlowCanvas_Nodes_ToList_Engine_Common_Commons_DiseasedStateEnum : 
      ToList<DiseasedStateEnum>
    {
    }

    public class FlowCanvas_Nodes_ToList_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      ToList<WeaponKind>
    {
    }

    public class FlowCanvas_Nodes_ToList_Engine_Common_Components_Parameters_ParameterNameEnum : 
      ToList<ParameterNameEnum>
    {
    }

    public class FlowCanvas_Nodes_ToList_Engine_Common_Components_Regions_BuildingEnum : 
      ToList<BuildingEnum>
    {
    }

    public class FlowCanvas_Nodes_ToList_Engine_Common_Components_Regions_RegionEnum : 
      ToList<RegionEnum>
    {
    }

    public class FlowCanvas_Nodes_ToList_Engine_Common_DateTime_TimesOfDay : ToList<TimesOfDay>
    {
    }

    public class FlowCanvas_Nodes_ToList_Engine_Source_Connections_IEntitySerializable : 
      ToList<IEntitySerializable>
    {
    }

    public class FlowCanvas_Nodes_ToList_Engine_Source_Connections_LipSyncObjectSerializable : 
      ToList<LipSyncObjectSerializable>
    {
    }

    public class FlowCanvas_Nodes_ToList_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      ToList<AbilityValueNameEnum>
    {
    }

    public class FlowCanvas_Nodes_ToList_Engine_Source_Services_CameraServices_CameraKindEnum : 
      ToList<CameraKindEnum>
    {
    }

    public class FlowCanvas_Nodes_ToList_Engine_Source_Services_Inputs_GameActionType : 
      ToList<GameActionType>
    {
    }

    public class FlowCanvas_Nodes_ToList_bool : ToList<bool>
    {
    }

    public class FlowCanvas_Nodes_ToList_int : ToList<int>
    {
    }

    public class FlowCanvas_Nodes_ToList_float : ToList<float>
    {
    }

    public class FlowCanvas_Nodes_ToList_UnityEngine_Bounds : ToList<Bounds>
    {
    }

    public class FlowCanvas_Nodes_ToList_UnityEngine_Color : ToList<Color>
    {
    }

    public class FlowCanvas_Nodes_ToList_UnityEngine_ContactPoint : ToList<ContactPoint>
    {
    }

    public class FlowCanvas_Nodes_ToList_UnityEngine_Keyframe : ToList<Keyframe>
    {
    }

    public class FlowCanvas_Nodes_ToList_UnityEngine_LayerMask : ToList<LayerMask>
    {
    }

    public class FlowCanvas_Nodes_ToList_UnityEngine_Quaternion : ToList<Quaternion>
    {
    }

    public class FlowCanvas_Nodes_ToList_UnityEngine_RaycastHit : ToList<RaycastHit>
    {
    }

    public class FlowCanvas_Nodes_ToList_UnityEngine_Rect : ToList<Rect>
    {
    }

    public class FlowCanvas_Nodes_ToList_UnityEngine_Space : ToList<Space>
    {
    }

    public class FlowCanvas_Nodes_ToList_UnityEngine_Vector2 : ToList<Vector2>
    {
    }

    public class FlowCanvas_Nodes_ToList_UnityEngine_Vector3 : ToList<Vector3>
    {
    }

    public class FlowCanvas_Nodes_ToList_UnityEngine_Vector4 : ToList<Vector4>
    {
    }

    public class NodeCanvas_Framework_BBParameter_BlockTypeEnum : BBParameter<BlockTypeEnum>
    {
    }

    public class NodeCanvas_Framework_BBParameter_IntensityParameter_UnityEngine_Color_ : 
      BBParameter<IntensityParameter<Color>>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UiEffectType : BBParameter<UiEffectType>
    {
    }

    public class NodeCanvas_Framework_BBParameter_Engine_Common_Commons_ActionEnum : 
      BBParameter<ActionEnum>
    {
    }

    public class NodeCanvas_Framework_BBParameter_Engine_Common_Commons_DiseasedStateEnum : 
      BBParameter<DiseasedStateEnum>
    {
    }

    public class NodeCanvas_Framework_BBParameter_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      BBParameter<WeaponKind>
    {
    }

    public class NodeCanvas_Framework_BBParameter_Engine_Common_Components_Parameters_ParameterNameEnum : 
      BBParameter<ParameterNameEnum>
    {
    }

    public class NodeCanvas_Framework_BBParameter_Engine_Common_Components_Regions_BuildingEnum : 
      BBParameter<BuildingEnum>
    {
    }

    public class NodeCanvas_Framework_BBParameter_Engine_Common_Components_Regions_RegionEnum : 
      BBParameter<RegionEnum>
    {
    }

    public class NodeCanvas_Framework_BBParameter_Engine_Common_DateTime_TimesOfDay : 
      BBParameter<TimesOfDay>
    {
    }

    public class NodeCanvas_Framework_BBParameter_Engine_Source_Connections_IEntitySerializable : 
      BBParameter<IEntitySerializable>
    {
    }

    public class NodeCanvas_Framework_BBParameter_Engine_Source_Connections_LipSyncObjectSerializable : 
      BBParameter<LipSyncObjectSerializable>
    {
    }

    public class NodeCanvas_Framework_BBParameter_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      BBParameter<AbilityValueNameEnum>
    {
    }

    public class NodeCanvas_Framework_BBParameter_Engine_Source_Services_CameraServices_CameraKindEnum : 
      BBParameter<CameraKindEnum>
    {
    }

    public class NodeCanvas_Framework_BBParameter_Engine_Source_Services_Inputs_GameActionType : 
      BBParameter<GameActionType>
    {
    }

    public class NodeCanvas_Framework_BBParameter_bool : BBParameter<bool>
    {
    }

    public class NodeCanvas_Framework_BBParameter_int : BBParameter<int>
    {
    }

    public class NodeCanvas_Framework_BBParameter_float : BBParameter<float>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UnityEngine_Bounds : BBParameter<Bounds>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UnityEngine_Color : BBParameter<Color>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UnityEngine_ContactPoint : 
      BBParameter<ContactPoint>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UnityEngine_Keyframe : BBParameter<Keyframe>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UnityEngine_LayerMask : BBParameter<LayerMask>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UnityEngine_Quaternion : BBParameter<Quaternion>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UnityEngine_RaycastHit : BBParameter<RaycastHit>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UnityEngine_Rect : BBParameter<Rect>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UnityEngine_Space : BBParameter<Space>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UnityEngine_Vector2 : BBParameter<Vector2>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UnityEngine_Vector3 : BBParameter<Vector3>
    {
    }

    public class NodeCanvas_Framework_BBParameter_UnityEngine_Vector4 : BBParameter<Vector4>
    {
    }

    public class NodeCanvas_Framework_Variable_BlockTypeEnum : Variable<BlockTypeEnum>
    {
    }

    public class NodeCanvas_Framework_Variable_IntensityParameter_UnityEngine_Color_ : 
      Variable<IntensityParameter<Color>>
    {
    }

    public class NodeCanvas_Framework_Variable_UiEffectType : Variable<UiEffectType>
    {
    }

    public class NodeCanvas_Framework_Variable_Engine_Common_Commons_ActionEnum : 
      Variable<ActionEnum>
    {
    }

    public class NodeCanvas_Framework_Variable_Engine_Common_Commons_DiseasedStateEnum : 
      Variable<DiseasedStateEnum>
    {
    }

    public class NodeCanvas_Framework_Variable_Engine_Common_Components_AttackerPlayer_WeaponKind : 
      Variable<WeaponKind>
    {
    }

    public class NodeCanvas_Framework_Variable_Engine_Common_Components_Parameters_ParameterNameEnum : 
      Variable<ParameterNameEnum>
    {
    }

    public class NodeCanvas_Framework_Variable_Engine_Common_Components_Regions_BuildingEnum : 
      Variable<BuildingEnum>
    {
    }

    public class NodeCanvas_Framework_Variable_Engine_Common_Components_Regions_RegionEnum : 
      Variable<RegionEnum>
    {
    }

    public class NodeCanvas_Framework_Variable_Engine_Common_DateTime_TimesOfDay : 
      Variable<TimesOfDay>
    {
    }

    public class NodeCanvas_Framework_Variable_Engine_Source_Connections_IEntitySerializable : 
      Variable<IEntitySerializable>
    {
    }

    public class NodeCanvas_Framework_Variable_Engine_Source_Connections_LipSyncObjectSerializable : 
      Variable<LipSyncObjectSerializable>
    {
    }

    public class NodeCanvas_Framework_Variable_Engine_Source_Effects_Values_AbilityValueNameEnum : 
      Variable<AbilityValueNameEnum>
    {
    }

    public class NodeCanvas_Framework_Variable_Engine_Source_Services_CameraServices_CameraKindEnum : 
      Variable<CameraKindEnum>
    {
    }

    public class NodeCanvas_Framework_Variable_Engine_Source_Services_Inputs_GameActionType : 
      Variable<GameActionType>
    {
    }

    public class NodeCanvas_Framework_Variable_bool : Variable<bool>
    {
    }

    public class NodeCanvas_Framework_Variable_int : Variable<int>
    {
    }

    public class NodeCanvas_Framework_Variable_float : Variable<float>
    {
    }

    public class NodeCanvas_Framework_Variable_UnityEngine_Bounds : Variable<Bounds>
    {
    }

    public class NodeCanvas_Framework_Variable_UnityEngine_Color : Variable<Color>
    {
    }

    public class NodeCanvas_Framework_Variable_UnityEngine_ContactPoint : Variable<ContactPoint>
    {
    }

    public class NodeCanvas_Framework_Variable_UnityEngine_Keyframe : Variable<Keyframe>
    {
    }

    public class NodeCanvas_Framework_Variable_UnityEngine_LayerMask : Variable<LayerMask>
    {
    }

    public class NodeCanvas_Framework_Variable_UnityEngine_Quaternion : Variable<Quaternion>
    {
    }

    public class NodeCanvas_Framework_Variable_UnityEngine_RaycastHit : Variable<RaycastHit>
    {
    }

    public class NodeCanvas_Framework_Variable_UnityEngine_Rect : Variable<Rect>
    {
    }

    public class NodeCanvas_Framework_Variable_UnityEngine_Space : Variable<Space>
    {
    }

    public class NodeCanvas_Framework_Variable_UnityEngine_Vector2 : Variable<Vector2>
    {
    }

    public class NodeCanvas_Framework_Variable_UnityEngine_Vector3 : Variable<Vector3>
    {
    }

    public class NodeCanvas_Framework_Variable_UnityEngine_Vector4 : Variable<Vector4>
    {
    }
  }
}
