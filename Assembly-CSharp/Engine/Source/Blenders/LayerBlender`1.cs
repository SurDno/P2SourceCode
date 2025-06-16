// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blenders.LayerBlender`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Blenders;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Blenders
{
  public abstract class LayerBlender<T> : EngineObject, ILayerBlender<T> where T : class, IObject, IBlendable<T>
  {
    [Inspected]
    private List<ILayerBlenderItem<T>> items = new List<ILayerBlenderItem<T>>();
    [Inspected]
    private T current;
    [Inspected]
    private bool invalidate;
    private OpacityBlendOperation opacityBlendOperation = new OpacityBlendOperation();
    private T empty;
    private T tmp;

    public T Current
    {
      get
      {
        if (this.invalidate)
        {
          this.Compute();
          this.invalidate = false;
        }
        return this.current;
      }
    }

    public IEnumerable<ILayerBlenderItem<T>> Items
    {
      get => (IEnumerable<ILayerBlenderItem<T>>) this.items;
    }

    public event Action<ILayerBlender<T>> OnChanged;

    public LayerBlender()
    {
      this.current = ServiceLocator.GetService<IFactory>().Create<T>();
      this.empty = ServiceLocator.GetService<IFactory>().Create<T>();
      this.tmp = ServiceLocator.GetService<IFactory>().Create<T>();
    }

    public void AddItem(ILayerBlenderItem<T> item)
    {
      item.OnChanged += new Action<ILayerBlenderItem<T>>(this.ItemOnChanged);
      this.items.Add(item);
      this.invalidate = true;
      Action<ILayerBlender<T>> onChanged = this.OnChanged;
      if (onChanged == null)
        return;
      onChanged((ILayerBlender<T>) this);
    }

    public void RemoveItem(ILayerBlenderItem<T> item)
    {
      item.OnChanged -= new Action<ILayerBlenderItem<T>>(this.ItemOnChanged);
      this.items.Remove(item);
      this.invalidate = true;
      Action<ILayerBlender<T>> onChanged = this.OnChanged;
      if (onChanged == null)
        return;
      onChanged((ILayerBlender<T>) this);
    }

    private void ItemOnChanged(ILayerBlenderItem<T> item)
    {
      this.invalidate = true;
      Action<ILayerBlender<T>> onChanged = this.OnChanged;
      if (onChanged == null)
        return;
      onChanged((ILayerBlender<T>) this);
    }

    private void Compute()
    {
      ((ICopyable) (object) this.empty).CopyTo((object) this.current);
      foreach (ILayerBlenderItem<T> layerBlenderItem in this.items)
      {
        ISmoothBlender<T> blender = layerBlenderItem.Blender;
        if (blender != null)
        {
          this.opacityBlendOperation.Opacity = layerBlenderItem.Opacity;
          ((ICopyable) (object) this.current).CopyTo((object) this.tmp);
          this.current.Blend(this.tmp, blender.Current, (IPureBlendOperation) this.opacityBlendOperation);
        }
      }
    }
  }
}
