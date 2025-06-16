// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Services.MessageRouter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
namespace ParadoxNotion.Services
{
  public class MessageRouter : 
    MonoBehaviour,
    IPointerEnterHandler,
    IEventSystemHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler,
    IDragHandler,
    IScrollHandler,
    IUpdateSelectedHandler,
    ISelectHandler,
    IDeselectHandler,
    IMoveHandler,
    ISubmitHandler
  {
    private Dictionary<string, List<object>> listeners = new Dictionary<string, List<object>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public void OnPointerEnter(PointerEventData eventData)
    {
      this.Dispatch(nameof (OnPointerEnter), (object) eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      this.Dispatch(nameof (OnPointerExit), (object) eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      this.Dispatch(nameof (OnPointerDown), (object) eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      this.Dispatch(nameof (OnPointerUp), (object) eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      this.Dispatch(nameof (OnPointerClick), (object) eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
      this.Dispatch(nameof (OnDrag), (object) eventData);
    }

    public void OnDrop(BaseEventData eventData)
    {
      this.Dispatch(nameof (OnDrop), (object) eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
      this.Dispatch(nameof (OnScroll), (object) eventData);
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
      this.Dispatch(nameof (OnUpdateSelected), (object) eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
      this.Dispatch(nameof (OnSelect), (object) eventData);
    }

    public void OnDeselect(BaseEventData eventData)
    {
      this.Dispatch(nameof (OnDeselect), (object) eventData);
    }

    public void OnMove(AxisEventData eventData)
    {
      this.Dispatch(nameof (OnMove), (object) eventData);
    }

    public void OnSubmit(BaseEventData eventData)
    {
      this.Dispatch(nameof (OnSubmit), (object) eventData);
    }

    private void OnAnimatorIK(int layerIndex)
    {
      this.Dispatch(nameof (OnAnimatorIK), (object) layerIndex);
    }

    private void OnBecameInvisible() => this.Dispatch(nameof (OnBecameInvisible), (object) null);

    private void OnBecameVisible() => this.Dispatch(nameof (OnBecameVisible), (object) null);

    private void OnCollisionEnter(Collision collisionInfo)
    {
      this.Dispatch(nameof (OnCollisionEnter), (object) collisionInfo);
    }

    private void OnCollisionExit(Collision collisionInfo)
    {
      this.Dispatch(nameof (OnCollisionExit), (object) collisionInfo);
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
      this.Dispatch(nameof (OnCollisionStay), (object) collisionInfo);
    }

    private void OnTriggerEnter(Collider other)
    {
      this.Dispatch(nameof (OnTriggerEnter), (object) other);
    }

    private void OnTriggerExit(Collider other)
    {
      this.Dispatch(nameof (OnTriggerExit), (object) other);
    }

    private void OnTriggerStay(Collider other)
    {
      this.Dispatch(nameof (OnTriggerStay), (object) other);
    }

    private void OnMouseDown() => this.Dispatch(nameof (OnMouseDown), (object) null);

    private void OnMouseDrag() => this.Dispatch(nameof (OnMouseDrag), (object) null);

    private void OnMouseEnter() => this.Dispatch(nameof (OnMouseEnter), (object) null);

    private void OnMouseExit() => this.Dispatch(nameof (OnMouseExit), (object) null);

    private void OnMouseOver() => this.Dispatch(nameof (OnMouseOver), (object) null);

    private void OnMouseUp() => this.Dispatch(nameof (OnMouseUp), (object) null);

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
      this.Dispatch(nameof (OnControllerColliderHit), (object) hit);
    }

    private void OnParticleCollision(GameObject other)
    {
      this.Dispatch(nameof (OnParticleCollision), (object) other);
    }

    public void OnCustomEvent(EventData eventData)
    {
      this.Dispatch(nameof (OnCustomEvent), (object) eventData);
    }

    public void Register(object target, params string[] messages)
    {
      if (target == null)
        return;
      for (int index = 0; index < messages.Length; ++index)
      {
        if (target.GetType().RTGetMethod(messages[index]) == (MethodInfo) null)
        {
          Debug.LogError((object) string.Format("Type '{0}' does not implement a method named '{1}', for the registered event to use.", (object) target.GetType().FriendlyName(), (object) messages[index]));
        }
        else
        {
          List<object> objectList = (List<object>) null;
          if (!this.listeners.TryGetValue(messages[index], out objectList))
          {
            objectList = new List<object>();
            this.listeners[messages[index]] = objectList;
          }
          if (!objectList.Contains(target))
            objectList.Add(target);
        }
      }
    }

    public void RegisterCallback(string message, Action callback)
    {
      this.Internal_RegisterCallback(message, (Delegate) callback);
    }

    public void RegisterCallback<T>(string message, Action<T> callback)
    {
      this.Internal_RegisterCallback(message, (Delegate) callback);
    }

    private void Internal_RegisterCallback(string message, Delegate callback)
    {
      List<object> objectList = (List<object>) null;
      if (!this.listeners.TryGetValue(message, out objectList))
      {
        objectList = new List<object>();
        this.listeners[message] = objectList;
      }
      if (objectList.Contains((object) callback))
        return;
      objectList.Add((object) callback);
    }

    public void UnRegister(object target)
    {
      if (target == null)
        return;
      foreach (string key in this.listeners.Keys)
      {
        foreach (object obj in this.listeners[key].ToArray())
        {
          if (obj == target)
            this.listeners[key].Remove(target);
          else if (obj is Delegate && (obj as Delegate).Target == target)
            this.listeners[key].Remove(obj);
        }
      }
    }

    public void UnRegister(object target, params string[] messages)
    {
      if (target == null || messages == null)
        return;
      for (int index = 0; index < messages.Length; ++index)
      {
        string message = messages[index];
        if (this.listeners.ContainsKey(message))
        {
          foreach (object obj in this.listeners[message].ToArray())
          {
            if (obj == target)
              this.listeners[message].Remove(target);
            else if (obj is Delegate && (obj as Delegate).Target == target)
              this.listeners[message].Remove(obj);
          }
        }
      }
    }

    public void Dispatch(string message, object arg)
    {
      List<object> objectList;
      if (!this.listeners.TryGetValue(message, out objectList))
        return;
      for (int index = 0; index < objectList.Count; ++index)
      {
        object del = objectList[index];
        if (del != null)
        {
          MethodInfo methodInfo = !(del is Delegate) ? del.GetType().RTGetMethod(message) : (del as Delegate).RTGetDelegateMethodInfo();
          if (!(methodInfo == (MethodInfo) null))
          {
            object[] objArray;
            if (methodInfo.GetParameters().Length != 1)
              objArray = (object[]) null;
            else
              objArray = new object[1]{ arg };
            object[] parameters = objArray;
            if (del is Delegate)
              (del as Delegate).DynamicInvoke(parameters);
            else if (methodInfo.ReturnType == typeof (IEnumerator))
              BlueprintManager.current.StartCoroutine((IEnumerator) methodInfo.Invoke(del, parameters));
            else
              methodInfo.Invoke(del, parameters);
          }
        }
      }
    }
  }
}
