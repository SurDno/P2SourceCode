using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Blenders;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;

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
        if (invalidate)
        {
          Compute();
          invalidate = false;
        }
        return current;
      }
    }

    public IEnumerable<ILayerBlenderItem<T>> Items
    {
      get => items;
    }

    public event Action<ILayerBlender<T>> OnChanged;

    public LayerBlender()
    {
      current = ServiceLocator.GetService<IFactory>().Create<T>();
      empty = ServiceLocator.GetService<IFactory>().Create<T>();
      tmp = ServiceLocator.GetService<IFactory>().Create<T>();
    }

    public void AddItem(ILayerBlenderItem<T> item)
    {
      item.OnChanged += ItemOnChanged;
      items.Add(item);
      invalidate = true;
      Action<ILayerBlender<T>> onChanged = OnChanged;
      if (onChanged == null)
        return;
      onChanged(this);
    }

    public void RemoveItem(ILayerBlenderItem<T> item)
    {
      item.OnChanged -= ItemOnChanged;
      items.Remove(item);
      invalidate = true;
      Action<ILayerBlender<T>> onChanged = OnChanged;
      if (onChanged == null)
        return;
      onChanged(this);
    }

    private void ItemOnChanged(ILayerBlenderItem<T> item)
    {
      invalidate = true;
      Action<ILayerBlender<T>> onChanged = OnChanged;
      if (onChanged == null)
        return;
      onChanged(this);
    }

    private void Compute()
    {
      ((ICopyable) empty).CopyTo(current);
      foreach (ILayerBlenderItem<T> layerBlenderItem in items)
      {
        ISmoothBlender<T> blender = layerBlenderItem.Blender;
        if (blender != null)
        {
          opacityBlendOperation.Opacity = layerBlenderItem.Opacity;
          ((ICopyable) current).CopyTo(tmp);
          current.Blend(tmp, blender.Current, opacityBlendOperation);
        }
      }
    }
  }
}
