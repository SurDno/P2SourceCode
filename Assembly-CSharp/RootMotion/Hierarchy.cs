using System;

namespace RootMotion
{
  public class Hierarchy
  {
    public static bool HierarchyIsValid(Transform[] bones)
    {
      for (int index = 1; index < bones.Length; ++index)
      {
        if (!IsAncestor(bones[index], bones[index - 1]))
          return false;
      }
      return true;
    }

    public static UnityEngine.Object ContainsDuplicate(UnityEngine.Object[] objects)
    {
      for (int index1 = 0; index1 < objects.Length; ++index1)
      {
        for (int index2 = 0; index2 < objects.Length; ++index2)
        {
          if (index1 != index2 && objects[index1] == objects[index2])
            return objects[index1];
        }
      }
      return (UnityEngine.Object) null;
    }

    public static bool IsAncestor(Transform transform, Transform ancestor)
    {
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null || (UnityEngine.Object) ancestor == (UnityEngine.Object) null)
        return true;
      if ((UnityEngine.Object) transform.parent == (UnityEngine.Object) null)
        return false;
      return (UnityEngine.Object) transform.parent == (UnityEngine.Object) ancestor || IsAncestor(transform.parent, ancestor);
    }

    public static bool ContainsChild(Transform transform, Transform child)
    {
      if ((UnityEngine.Object) transform == (UnityEngine.Object) child)
        return true;
      foreach (UnityEngine.Object componentsInChild in transform.GetComponentsInChildren<Transform>())
      {
        if (componentsInChild == (UnityEngine.Object) child)
          return true;
      }
      return false;
    }

    public static void AddAncestors(Transform transform, Transform blocker, ref Transform[] array)
    {
      if (!((UnityEngine.Object) transform.parent != (UnityEngine.Object) null) || !((UnityEngine.Object) transform.parent != (UnityEngine.Object) blocker))
        return;
      if (transform.parent.position != transform.position && transform.parent.position != blocker.position)
      {
        Array.Resize(ref array, array.Length + 1);
        array[array.Length - 1] = transform.parent;
      }
      AddAncestors(transform.parent, blocker, ref array);
    }

    public static Transform GetAncestor(Transform transform, int minChildCount)
    {
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null || !((UnityEngine.Object) transform.parent != (UnityEngine.Object) null))
        return (Transform) null;
      return transform.parent.childCount >= minChildCount ? transform.parent : GetAncestor(transform.parent, minChildCount);
    }

    public static Transform GetFirstCommonAncestor(Transform t1, Transform t2)
    {
      if ((UnityEngine.Object) t1 == (UnityEngine.Object) null || (UnityEngine.Object) t2 == (UnityEngine.Object) null || (UnityEngine.Object) t1.parent == (UnityEngine.Object) null || (UnityEngine.Object) t2.parent == (UnityEngine.Object) null)
        return (Transform) null;
      return IsAncestor(t2, t1.parent) ? t1.parent : GetFirstCommonAncestor(t1.parent, t2);
    }

    public static Transform GetFirstCommonAncestor(Transform[] transforms)
    {
      if (transforms == null)
      {
        Debug.LogWarning((object) "Transforms is null.");
        return (Transform) null;
      }
      if (transforms.Length == 0)
      {
        Debug.LogWarning((object) "Transforms.Length is 0.");
        return (Transform) null;
      }
      for (int index = 0; index < transforms.Length; ++index)
      {
        if ((UnityEngine.Object) transforms[index] == (UnityEngine.Object) null)
          return (Transform) null;
        if (IsCommonAncestor(transforms[index], transforms))
          return transforms[index];
      }
      return GetFirstCommonAncestorRecursive(transforms[0], transforms);
    }

    public static Transform GetFirstCommonAncestorRecursive(
      Transform transform,
      Transform[] transforms)
    {
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "Transform is null.");
        return (Transform) null;
      }
      if (transforms == null)
      {
        Debug.LogWarning((object) "Transforms is null.");
        return (Transform) null;
      }
      if (transforms.Length == 0)
      {
        Debug.LogWarning((object) "Transforms.Length is 0.");
        return (Transform) null;
      }
      if (IsCommonAncestor(transform, transforms))
        return transform;
      return (UnityEngine.Object) transform.parent == (UnityEngine.Object) null ? (Transform) null : GetFirstCommonAncestorRecursive(transform.parent, transforms);
    }

    public static bool IsCommonAncestor(Transform transform, Transform[] transforms)
    {
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "Transform is null.");
        return false;
      }
      for (int index = 0; index < transforms.Length; ++index)
      {
        if ((UnityEngine.Object) transforms[index] == (UnityEngine.Object) null)
        {
          Debug.Log((object) ("Transforms[" + index + "] is null."));
          return false;
        }
        if (!IsAncestor(transforms[index], transform) && (UnityEngine.Object) transforms[index] != (UnityEngine.Object) transform)
          return false;
      }
      return true;
    }
  }
}
