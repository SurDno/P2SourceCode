using System;
using System.Collections;
using System.Xml;
using UnityEngine;

[Serializable]
public class BonePose
{
  public string boneName = "";
  public Transform m_bone;
  public float tx;
  public float ty;
  public float tz;
  public float sx = 1f;
  public float sy = 1f;
  public float sz = 1f;
  public float rx;
  public float ry;
  public float rz;

  public void InitializeBone(Transform bx)
  {
    boneName = bx.gameObject.name;
    m_bone = bx;
    Vector3 localPosition = m_bone.localPosition;
    tx = localPosition.x;
    ty = localPosition.y;
    tz = localPosition.z;
    Vector3 localEulerAngles = m_bone.localEulerAngles;
    rx = localEulerAngles.x;
    ry = localEulerAngles.y;
    rz = localEulerAngles.z;
    sx = 1f;
    sy = 1f;
    sz = 1f;
  }

  public bool IsSamePose(BonePose bn, float tolerance)
  {
    float num1 = Mathf.Abs(tx - bn.tx);
    float num2 = Mathf.Abs(ty - bn.ty);
    float num3 = Mathf.Abs(tz - bn.tz);
    float num4 = Mathf.Abs(rx - bn.rx);
    float num5 = Mathf.Abs(ry - bn.ry);
    float num6 = Mathf.Abs(rz - bn.rz);
    float num7 = Mathf.Abs(sx - bn.sx);
    float num8 = Mathf.Abs(sy - bn.sy);
    float num9 = Mathf.Abs(sz - bn.sz);
    return num1 < (double) tolerance && num2 < (double) tolerance && num3 < (double) tolerance && num4 < (double) tolerance && num5 < (double) tolerance && num6 < (double) tolerance && num7 < (double) tolerance && num8 < (double) tolerance && num9 < (double) tolerance;
  }

  public void ReadBonePose(XmlTextReader reader, bool flipX, float scale)
  {
    bool flag = false;
    while (!flag && reader.Read())
    {
      if (reader.Name == "bone" && reader.NodeType == XmlNodeType.EndElement)
        flag = true;
      else if (reader.Name == "label")
        boneName = XMLUtils.ReadXMLInnerText(reader, "label");
      else if (reader.Name == "tx")
      {
        tx = -XMLUtils.ReadXMLInnerTextFloat(reader, "tx");
        if (flipX)
          tx = -tx;
      }
      else if (reader.Name == "ty")
        ty = XMLUtils.ReadXMLInnerTextFloat(reader, "ty");
      else if (reader.Name == "tz")
        tz = XMLUtils.ReadXMLInnerTextFloat(reader, "tz");
      else if (reader.Name == "sx")
        sx = XMLUtils.ReadXMLInnerTextFloat(reader, "sx");
      else if (reader.Name == "sy")
        sy = XMLUtils.ReadXMLInnerTextFloat(reader, "sy");
      else if (reader.Name == "sz")
        sz = XMLUtils.ReadXMLInnerTextFloat(reader, "sz");
      else if (reader.Name == "rx")
        rx = XMLUtils.ReadXMLInnerTextFloat(reader, "rx");
      else if (reader.Name == "ry")
        ry = XMLUtils.ReadXMLInnerTextFloat(reader, "ry");
      else if (reader.Name == "rz")
        rz = XMLUtils.ReadXMLInnerTextFloat(reader, "rz");
    }
    tx *= scale;
    ty *= scale;
    tz *= scale;
  }

  public void ResetToThisPose(BonePose bnIn)
  {
    tx = bnIn.tx;
    ty = bnIn.ty;
    tz = bnIn.tz;
    rx = bnIn.rx;
    ry = bnIn.ry;
    rz = bnIn.rz;
    sz = bnIn.sx;
    sy = bnIn.sy;
    sz = bnIn.sz;
  }

  public void ConvertToOffsets(BonePose defPos)
  {
    tx -= defPos.tx;
    ty -= defPos.ty;
    tz -= defPos.tz;
    sx -= defPos.sx;
    sy -= defPos.sy;
    sz -= defPos.sz;
    rx = FixRotations(rx, defPos.rx);
    ry = FixRotations(ry, defPos.ry);
    rz = FixRotations(rz, defPos.rz);
    rx -= defPos.rx;
    ry -= defPos.ry;
    rz -= defPos.rz;
  }

  private float FixRotations(float r, float defr)
  {
    if (Mathf.Abs(r - defr) > 180.0)
    {
      if (defr > 0.0)
        r = 360f + r;
      else
        r -= 360f;
    }
    return r;
  }

  public void LoadUnityBone(Transform gObject, Hashtable cache, bool bKeepLocalRotations)
  {
    if (cache.ContainsKey(boneName))
    {
      m_bone = cache[boneName] as Transform;
    }
    else
    {
      m_bone = FindRecursive(gObject, boneName);
      if (m_bone != null)
        cache[boneName] = m_bone;
    }
    if (!(m_bone != null) || !bKeepLocalRotations)
      return;
    rx += m_bone.transform.localEulerAngles.x;
    ry += m_bone.transform.localEulerAngles.y;
    rz += m_bone.transform.localEulerAngles.z;
  }

  public void Print()
  {
    Debug.Log("bone " + boneName + " x,y,z (" + tx + "," + ty + "," + tz + ")");
    Debug.Log("bone " + boneName + " rx,ry,rz (" + rx + "," + ry + "," + rz + ")");
  }

  public void PrintDiff(BonePose def)
  {
    if (tx != (double) def.tx || ty != (double) def.ty || tz != (double) def.tz)
      Debug.Log(boneName + " difference in translate ");
    if (rx == (double) def.rx && ry == (double) def.ry && rz == (double) def.rz)
      return;
    Debug.Log(boneName + " difference in rotation");
  }

  public void ResetToThisTransform()
  {
    if (m_bone == null)
      return;
    m_bone.localPosition = m_bone.localPosition with
    {
      x = tx,
      y = ty,
      z = tz
    };
    m_bone.localEulerAngles = m_bone.localEulerAngles with
    {
      x = rx,
      y = ry,
      z = rz
    };
  }

  public void Deform(float weight, BoneDeformMemento memento)
  {
    if (m_bone == null)
      return;
    Vector3 localPosition = m_bone.localPosition;
    float tx = weight * this.tx;
    float ty = weight * this.ty;
    float tz = weight * this.tz;
    float rx = weight * this.rx;
    float ry = weight * this.ry;
    float rz = weight * this.rz;
    localPosition.x += tx;
    localPosition.y += ty;
    localPosition.z += tz;
    m_bone.localPosition = localPosition;
    Vector3 localEulerAngles = m_bone.localEulerAngles;
    localEulerAngles.x += rx;
    localEulerAngles.y += ry;
    localEulerAngles.z += rz;
    m_bone.localEulerAngles = localEulerAngles;
    memento?.AccumulateDeform(m_bone, tx, ty, tz, rx, ry, rz);
  }

  private Transform FindRecursive(Transform obj, string name)
  {
    Transform recursive1 = obj.Find(name);
    if (recursive1 != null)
      return recursive1;
    foreach (Transform transform in obj)
    {
      Transform recursive2 = FindRecursive(transform, name);
      if (recursive2 != null)
        return recursive2;
    }
    return null;
  }
}
