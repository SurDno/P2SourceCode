using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Maps;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components.Maps
{
  [Factory(typeof (IMapPlaceholder))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class MapPlaceholder : EngineObject, IMapPlaceholder, IObject
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected MapPlaceholderKind kind = MapPlaceholderKind.Building;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> mainSprite;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> hoverSprite;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> normalSprite;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> shadowSprite;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool alphaRaycast = false;
    [DataReadProxy(Name = "NPCSprite")]
    [DataWriteProxy(Name = "NPCSprite")]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> npcSprite;
    private bool cached;
    private Sprite cachedMainSprite;
    private Sprite cachedHoverSprite;
    private Sprite cachedNormalSprite;
    private Sprite cachedShadowSprite;

    public MapPlaceholderKind Kind => kind;

    public Sprite MainSprite => GetSprite(ref mainSprite, ref cachedMainSprite);

    public Sprite HoverSprite => GetSprite(ref hoverSprite, ref cachedHoverSprite);

    public Sprite NormalSprite
    {
      get => GetSprite(ref normalSprite, ref cachedNormalSprite);
    }

    public Sprite ShadowSprite
    {
      get => GetSprite(ref shadowSprite, ref cachedShadowSprite);
    }

    public bool AlphaRaycast => alphaRaycast;

    public Sprite NPCSprite => npcSprite.Value;

    public Sprite GetSprite(ref UnitySubAsset<Sprite> resource, ref Sprite cache)
    {
      if (!Application.isPlaying)
        return resource.Value;
      if (!cached)
      {
        cachedMainSprite = mainSprite.Value;
        cachedHoverSprite = hoverSprite.Value;
        cachedNormalSprite = normalSprite.Value;
        cachedShadowSprite = shadowSprite.Value;
        cached = true;
      }
      return cache;
    }
  }
}
