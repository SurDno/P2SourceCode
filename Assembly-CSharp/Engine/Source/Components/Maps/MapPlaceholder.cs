// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Maps.MapPlaceholder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Maps;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components.Maps
{
  [Factory(typeof (IMapPlaceholder))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MapPlaceholder : EngineObject, IMapPlaceholder, IObject
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected MapPlaceholderKind kind = MapPlaceholderKind.Building;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> mainSprite;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> hoverSprite;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> normalSprite;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> shadowSprite;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool alphaRaycast = false;
    [DataReadProxy(MemberEnum.None, Name = "NPCSprite")]
    [DataWriteProxy(MemberEnum.None, Name = "NPCSprite")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> npcSprite;
    private bool cached = false;
    private Sprite cachedMainSprite = (Sprite) null;
    private Sprite cachedHoverSprite = (Sprite) null;
    private Sprite cachedNormalSprite = (Sprite) null;
    private Sprite cachedShadowSprite = (Sprite) null;

    public MapPlaceholderKind Kind => this.kind;

    public Sprite MainSprite => this.GetSprite(ref this.mainSprite, ref this.cachedMainSprite);

    public Sprite HoverSprite => this.GetSprite(ref this.hoverSprite, ref this.cachedHoverSprite);

    public Sprite NormalSprite
    {
      get => this.GetSprite(ref this.normalSprite, ref this.cachedNormalSprite);
    }

    public Sprite ShadowSprite
    {
      get => this.GetSprite(ref this.shadowSprite, ref this.cachedShadowSprite);
    }

    public bool AlphaRaycast => this.alphaRaycast;

    public Sprite NPCSprite => this.npcSprite.Value;

    public Sprite GetSprite(ref UnitySubAsset<Sprite> resource, ref Sprite cache)
    {
      if (!Application.isPlaying)
        return resource.Value;
      if (!this.cached)
      {
        this.cachedMainSprite = this.mainSprite.Value;
        this.cachedHoverSprite = this.hoverSprite.Value;
        this.cachedNormalSprite = this.normalSprite.Value;
        this.cachedShadowSprite = this.shadowSprite.Value;
        this.cached = true;
      }
      return cache;
    }
  }
}
