using System;
using System.Collections;
using System.Xml;
using UnityEngine;

[Serializable]
public class BonePose
{
  public string boneName = "";
  public Transform m_bone = (Transform) null;
  public float tx = 0.0f;
  public float ty = 0.0f;
  public float tz = 0.0f;
  public float sx = 1f;
  public float sy = 1f;
  public float sz = 1f;
  public float rx = 0.0f;
  public float ry = 0.0f;
  public float rz = 0.0f;

  public void InitializeBone(Transform bx)
  {
    this.boneName = bx.gameObject.name;
    this.m_bone = bx;
    Vector3 localPosition = this.m_bone.localPosition;
    this.tx = localPosition.x;
    this.ty = localPosition.y;
    this.tz = localPosition.z;
    Vector3 localEulerAngles = this.m_bone.localEulerAngles;
    this.rx = localEulerAngles.x;
    this.ry = localEulerAngles.y;
    this.rz = localEulerAngles.z;
    this.sx = 1f;
    this.sy = 1f;
    this.sz = 1f;
  }

  public bool IsSamePose(BonePose bn, float tolerance)
  {
    float num1 = Mathf.Abs(this.tx - bn.tx);
    float num2 = Mathf.Abs(this.ty - bn.ty);
    float num3 = Mathf.Abs(this.tz - bn.tz);
    float num4 = Mathf.Abs(this.rx - bn.rx);
    float num5 = Mathf.Abs(this.ry - bn.ry);
    float num6 = Mathf.Abs(this.rz - bn.rz);
    float num7 = Mathf.Abs(this.sx - bn.sx);
    float num8 = Mathf.Abs(this.sy - bn.sy);
    float num9 = Mathf.Abs(this.sz - bn.sz);
    return (double) num1 < (double) tolerance && (double) num2 < (double) tolerance && (double) num3 < (double) tolerance && (double) num4 < (double) tolerance && (double) num5 < (double) tolerance && (double) num6 < (double) tolerance && (double) num7 < (double) tolerance && (double) num8 < (double) tolerance && (double) num9 < (double) tolerance;
  }

  public void ReadBonePose(XmlTextReader reader, bool flipX, float scale)
  {
    bool flag = false;
    while (!flag && reader.Read())
    {
      if (reader.Name == "bone" && reader.NodeType == XmlNodeType.EndElement)
        flag = true;
      else if (reader.Name == "label")
        this.boneName = XMLUtils.ReadXMLInnerText(reader, "label");
      else if (reader.Name == "tx")
      {
        this.tx = -XMLUtils.ReadXMLInnerTextFloat(reader, "tx");
        if (flipX)
          this.tx = -this.tx;
      }
      else if (reader.Name == "ty")
        this.ty = XMLUtils.ReadXMLInnerTextFloat(reader, "ty");
      else if (reader.Name == "tz")
        this.tz = XMLUtils.ReadXMLInnerTextFloat(reader, "tz");
      else if (reader.Name == "sx")
        this.sx = XMLUtils.ReadXMLInnerTextFloat(reader, "sx");
      else if (reader.Name == "sy")
        this.sy = XMLUtils.ReadXMLInnerTextFloat(reader, "sy");
      else if (reader.Name == "sz")
        this.sz = XMLUtils.ReadXMLInnerTextFloat(reader, "sz");
      else if (reader.Name == "rx")
        this.rx = XMLUtils.ReadXMLInnerTextFloat(reader, "rx");
      else if (reader.Name == "ry")
        this.ry = XMLUtils.ReadXMLInnerTextFloat(reader, "ry");
      else if (reader.Name == "rz")
        this.rz = XMLUtils.ReadXMLInnerTextFloat(reader, "rz");
    }
    this.tx *= scale;
    this.ty *= scale;
    this.tz *= scale;
  }

  public void ResetToThisPose(BonePose bnIn)
  {
    this.tx = bnIn.tx;
    this.ty = bnIn.ty;
    this.tz = bnIn.tz;
    this.rx = bnIn.rx;
    this.ry = bnIn.ry;
    this.rz = bnIn.rz;
    this.sz = bnIn.sx;
    this.sy = bnIn.sy;
    this.sz = bnIn.sz;
  }

  public void ConvertToOffsets(BonePose defPos)
  {
    this.tx -= defPos.tx;
    this.ty -= defPos.ty;
    this.tz -= defPos.tz;
    this.sx -= defPos.sx;
    this.sy -= defPos.sy;
    this.sz -= defPos.sz;
    this.rx = this.FixRotations(this.rx, defPos.rx);
    this.ry = this.FixRotations(this.ry, defPos.ry);
    this.rz = this.FixRotations(this.rz, defPos.rz);
    this.rx -= defPos.rx;
    this.ry -= defPos.ry;
    this.rz -= defPos.rz;
  }

  private float FixRotations(float r, float defr)
  {
    if ((double) Mathf.Abs(r - defr) > 180.0)
    {
      if ((double) defr > 0.0)
        r = 360f + r;
      else
        r -= 360f;
    }
    return r;
  }

  public void LoadUnityBone(Transform gObject, Hashtable cache, bool bKeepLocalRotations)
  {
    if (cache.ContainsKey((object) this.boneName))
    {
      this.m_bone = cache[(object) this.boneName] as Transform;
    }
    else
    {
      this.m_bone = this.FindRecursive(gObject, this.boneName);
      if ((UnityEngine.Object) this.m_bone != (UnityEngine.Object) null)
        cache[(object) this.boneName] = (object) this.m_bone;
    }
    if (!((UnityEngine.Object) this.m_bone != (UnityEngine.Object) null) || !bKeepLocalRotations)
      return;
    this.rx += this.m_bone.transform.localEulerAngles.x;
    this.ry += this.m_bone.transform.localEulerAngles.y;
    this.rz += this.m_bone.transform.localEulerAngles.z;
  }

  public void Print()
  {
    Debug.Log((object) ("bone " + this.boneName + " x,y,z (" + (object) this.tx + "," + (object) this.ty + "," + (object) this.tz + ")"));
    Debug.Log((object) ("bone " + this.boneName + " rx,ry,rz (" + (object) this.rx + "," + (object) this.ry + "," + (object) this.rz + ")"));
  }

  public void PrintDiff(BonePose def)
  {
    if ((double) this.tx != (double) def.tx || (double) this.ty != (double) def.ty || (double) this.tz != (double) def.tz)
      Debug.Log((object) (this.boneName + " difference in translate "));
    if ((double) this.rx == (double) def.rx && (double) this.ry == (double) def.ry && (double) this.rz == (double) def.rz)
      return;
    Debug.Log((object) (this.boneName + " difference in rotation"));
  }

  public void ResetToThisTransform()
  {
    if ((UnityEngine.Object) this.m_bone == (UnityEngine.Object) null)
      return;
    this.m_bone.localPosition = this.m_bone.localPosition with
    {
      x = this.tx,
      y = this.ty,
      z = this.tz
    };
    this.m_bone.localEulerAngles = this.m_bone.localEulerAngles with
    {
      x = this.rx,
      y = this.ry,
      z = this.rz
    };
  }

  public void Deform(float weight, BoneDeformMemento memento)
  {
    if ((UnityEngine.Object) this.m_bone == (UnityEngine.Object) null)
      return;
    Vector3 localPosition = this.m_bone.localPosition;
    float tx = weight * this.tx;
    float ty = weight * this.ty;
    float tz = weight * this.tz;
    float rx = weight * this.rx;
    float ry = weight * this.ry;
    float rz = weight * this.rz;
    localPosition.x += tx;
    localPosition.y += ty;
    localPosition.z += tz;
    this.m_bone.localPosition = localPosition;
    Vector3 localEulerAngles = this.m_bone.localEulerAngles;
    localEulerAngles.x += rx;
    localEulerAngles.y += ry;
    localEulerAngles.z += rz;
    this.m_bone.localEulerAngles = localEulerAngles;
    memento?.AccumulateDeform(this.m_bone, tx, ty, tz, rx, ry, rz);
  }

  private Transform FindRecursive(Transform obj, string name)
  {
    Transform recursive1 = obj.Find(name);
    if ((UnityEngine.Object) recursive1 != (UnityEngine.Object) null)
      return recursive1;
    foreach (Transform transform in obj)
    {
      Transform recursive2 = this.FindRecursive(transform, name);
      if ((UnityEngine.Object) recursive2 != (UnityEngine.Object) null)
        return recursive2;
    }
    return (Transform) null;
  }
}
