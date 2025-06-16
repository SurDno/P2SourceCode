using System.Collections.Generic;
using Engine.Common.Components.AttackerPlayer;

namespace Engine.Behaviours.Components
{
  [DisallowMultipleComponent]
  public class PivotPlayer : MonoBehaviour
  {
    public GameObject MoveRoot;
    [Tooltip("Геометрия рук, которая будет скрываться")]
    public GameObject HandsGeometry;
    [Tooltip("Геометрия рукавов, которая будет скрываться")]
    public GameObject SleevesGeometry;
    [Header("Revolver")]
    [Tooltip("Cам объект револьвера (базовый в иерархии револьвера)")]
    public Transform RevolverBone;
    [Tooltip("Cам объект револьвера (базовый в иерархии револьвера)")]
    public GameObject RevolverGeometry;
    [Tooltip("А это куда его нужно поставить")]
    public GameObject RevolverAnchor;
    public ParticleBurster RevolverFirePS;
    public Transform RevolverBullet;
    public Transform[] RevolverBullets;
    public Transform RevolverCylinder;
    [Header("Rifle")]
    [Tooltip("Cам объект винтовки (базовый в иерархии винтовки)")]
    public GameObject RifleGeometry;
    public ParticleBurster RifleShot;
    public GameObject RifleAmmo;
    [Header("Shotgun")]
    [Tooltip("Cам объект ружья (базовый в иерархии ружья)")]
    public GameObject ShotgunGeometry;
    public ParticleBurster ShotgunShot;
    public GameObject ShotgunAmmo;
    public Transform[] ShotgunAmmos;
    [Header("Knife")]
    public GameObject KnifeGeometry;
    [Header("Scalpel")]
    public GameObject ScalpelGeometry;
    public List<GameObject> ScalpelCustomGeometry;
    [Header("Lockpick")]
    public GameObject LockpickGeometry;
    [Header("Visir")]
    [Tooltip("Cам объект визира (базовый в иерархии револьвера)")]
    public GameObject VisirBone;
    [Tooltip("Cам объект визира (базовый в иерархии револьвера)")]
    public GameObject VisirGeometry;
    [Tooltip("А это куда его нужно поставить")]
    public GameObject VisirAnchor;
    [Header("Flashlight")]
    public GameObject FlashlightGeometry;
    public GameObject FlashlightPhysics;
    public GameObject FlashlightEffect;
    public GameObject FlashlightFire;
    public GameObject FlashlightLight;
    public Joint FlashlightJoint;
    [Header("Camera")]
    [Tooltip("Кость, которая врашается управлением, чтобы правильно развернут камеру.")]
    public Transform CameraControllingBone;
    [Tooltip("Кость, которая анимируется при движении камеры во время анимаций")]
    public Transform AnimatedCameraBone;
    private Animator animator;

    public bool HandsGeometryVisible
    {
      set
      {
        HandsGeometry?.SetActive(value);
        SleevesGeometry?.SetActive(value);
      }
    }

    public bool KnifeGeometryVisible
    {
      set => KnifeGeometry?.SetActive(value);
    }

    public bool ScalpelGeometryVisible
    {
      set => ScalpelGeometry?.SetActive(value);
    }

    public void SetScalpelCustomGeometryVisible(int kind)
    {
      foreach (GameObject gameObject in ScalpelCustomGeometry)
        gameObject.SetActive(false);
      if (kind >= ScalpelCustomGeometry.Count)
        return;
      ScalpelCustomGeometry[kind]?.SetActive(true);
    }

    public bool LockpickGeometryVisible
    {
      set => LockpickGeometry?.SetActive(value);
    }

    public bool FlashlightGeometryVisible
    {
      set
      {
        FlashlightGeometry?.SetActive(value);
        FlashlightPhysics?.SetActive(value);
      }
    }

    public bool RevolverGeometryVisible
    {
      set
      {
        RevolverGeometry?.SetActive(value);
        RevolverBullet.gameObject.SetActive(value);
      }
    }

    public bool RifleGeometryVisible
    {
      set => RifleGeometry?.SetActive(value);
    }

    public bool ShotgunGeometryVisible
    {
      set => ShotgunGeometry?.SetActive(value);
    }

    public bool VisirGeometryVisible
    {
      set => VisirGeometry?.SetActive(value);
    }

    private void Awake()
    {
      animator = this.GetComponent<Animator>();
      if ((Object) animator == (Object) null)
      {
        Debug.LogErrorFormat("{0} doesn't contain {1} unity component.", (object) this.gameObject.name, (object) typeof (Animator).Name);
      }
      else
      {
        if ((Object) VisirBone != (Object) null && (Object) VisirAnchor != (Object) null)
        {
          VisirBone.transform.parent = VisirAnchor.transform;
          VisirBone.transform.localPosition = Vector3.zero;
          VisirBone.transform.localRotation = Quaternion.identity;
        }
        HandsGeometryVisible = false;
        KnifeGeometryVisible = false;
        ScalpelGeometryVisible = false;
        LockpickGeometryVisible = false;
        RevolverGeometryVisible = false;
        VisirGeometryVisible = false;
        FlashlightGeometryVisible = false;
        FlashlightLight.SetActive(false);
      }
    }

    public void ApplyWeaponTransform(WeaponKind weaponKind)
    {
      switch (weaponKind)
      {
        case WeaponKind.Revolver:
          if (!((Object) RevolverBone != (Object) null) || !((Object) RevolverAnchor != (Object) null))
            break;
          RevolverBone.transform.position = RevolverAnchor.transform.position;
          RevolverBone.transform.rotation = RevolverAnchor.transform.rotation;
          break;
        case WeaponKind.Visir:
          if ((Object) VisirBone != (Object) null && (Object) VisirAnchor != (Object) null)
          {
            VisirBone.transform.localPosition = Vector3.zero;
            VisirBone.transform.localRotation = Quaternion.identity;
          }
          break;
      }
    }

    public void MoveFlashlightToJointAnchorPosition()
    {
      Rigidbody component1 = FlashlightGeometry.GetComponent<Rigidbody>();
      foreach (CharacterJoint component2 in FlashlightJoint.gameObject.GetComponents<CharacterJoint>())
      {
        if (!((Object) component2.connectedBody != (Object) component1))
        {
          component2.transform.TransformPoint(component2.anchor);
          component1.transform.TransformPoint(component2.connectedAnchor);
          component2.axis = component2.transform.InverseTransformDirection(-Vector3.up);
          component2.swingAxis = component2.transform.InverseTransformDirection(-Vector3.up);
          break;
        }
      }
    }
  }
}
