using System.Collections.Generic;
using System.IO;

namespace JerboaAnimationInstancing
{
  public class JerboaAnimationManager : MonoBehaviour
  {
    private Dictionary<GameObject, InstanceAnimationInfo> animationInfo;

    private void Awake()
    {
      animationInfo = new Dictionary<GameObject, InstanceAnimationInfo>();
    }

    private void Update()
    {
    }

    public InstanceAnimationInfo FindAnimationInfo(
      JerboaInstancingManager jerboaInstancingManager,
      TextAsset textAsset,
      GameObject prefab,
      JerboaInstance instance)
    {
      Debug.Assert((Object) prefab != (Object) null);
      InstanceAnimationInfo instanceAnimationInfo = null;
      return animationInfo.TryGetValue(prefab, out instanceAnimationInfo) ? instanceAnimationInfo : CreateAnimationInfoFromFile(jerboaInstancingManager, textAsset, prefab);
    }

    private InstanceAnimationInfo CreateAnimationInfoFromFile(
      JerboaInstancingManager jerboaInstancingManager,
      TextAsset textAsset,
      GameObject prefab)
    {
      using (MemoryStream input = new MemoryStream(textAsset.bytes))
      {
        InstanceAnimationInfo animationInfoFromFile = new InstanceAnimationInfo();
        BinaryReader reader = new BinaryReader(input);
        animationInfoFromFile.listAniInfo = ReadAnimationInfo(reader).ToArray();
        animationInfoFromFile.extraBoneInfo = ReadExtraBoneInfo(reader);
        animationInfo.Add(prefab, animationInfoFromFile);
        jerboaInstancingManager.ImportAnimationTexture(prefab.name, reader);
        input.Close();
        return animationInfoFromFile;
      }
    }

    private List<AnimationInfo> ReadAnimationInfo(BinaryReader reader)
    {
      int num1 = reader.ReadInt32();
      List<AnimationInfo> animationInfoList = new List<AnimationInfo>();
      for (int index1 = 0; index1 != num1; ++index1)
      {
        AnimationInfo animationInfo = new AnimationInfo {
          animationName = reader.ReadString()
        };
        animationInfo.animationNameHash = animationInfo.animationName.GetHashCode();
        animationInfo.animationIndex = reader.ReadInt32();
        animationInfo.textureIndex = reader.ReadInt32();
        animationInfo.totalFrame = reader.ReadInt32();
        animationInfo.fps = reader.ReadInt32();
        animationInfo.rootMotion = reader.ReadBoolean();
        animationInfo.wrapMode = (WrapMode) reader.ReadInt32();
        if (animationInfo.rootMotion)
        {
          animationInfo.velocity = new Vector3[animationInfo.totalFrame];
          animationInfo.angularVelocity = new Vector3[animationInfo.totalFrame];
          for (int index2 = 0; index2 != animationInfo.totalFrame; ++index2)
          {
            animationInfo.velocity[index2].x = reader.ReadSingle();
            animationInfo.velocity[index2].y = reader.ReadSingle();
            animationInfo.velocity[index2].z = reader.ReadSingle();
            animationInfo.angularVelocity[index2].x = reader.ReadSingle();
            animationInfo.angularVelocity[index2].y = reader.ReadSingle();
            animationInfo.angularVelocity[index2].z = reader.ReadSingle();
          }
        }
        int num2 = reader.ReadInt32();
        animationInfo.eventList = new List<AnimationEvent>();
        for (int index3 = 0; index3 != num2; ++index3)
          animationInfo.eventList.Add(new AnimationEvent {
            function = reader.ReadString(),
            floatParameter = reader.ReadSingle(),
            intParameter = reader.ReadInt32(),
            stringParameter = reader.ReadString(),
            time = reader.ReadSingle(),
            objectParameter = reader.ReadString()
          });
        animationInfoList.Add(animationInfo);
      }
      animationInfoList.Sort(new ComparerHash());
      return animationInfoList;
    }

    private ExtraBoneInfo ReadExtraBoneInfo(BinaryReader reader)
    {
      ExtraBoneInfo extraBoneInfo = null;
      if (reader.ReadBoolean())
      {
        extraBoneInfo = new ExtraBoneInfo();
        int length = reader.ReadInt32();
        extraBoneInfo.extraBone = new string[length];
        extraBoneInfo.extraBindPose = new Matrix4x4[length];
        for (int index = 0; index != extraBoneInfo.extraBone.Length; ++index)
          extraBoneInfo.extraBone[index] = reader.ReadString();
        for (int index1 = 0; index1 != extraBoneInfo.extraBindPose.Length; ++index1)
        {
          for (int index2 = 0; index2 != 16; ++index2)
            extraBoneInfo.extraBindPose[index1][index2] = reader.ReadSingle();
        }
      }
      return extraBoneInfo;
    }

    public class InstanceAnimationInfo
    {
      public AnimationInfo[] listAniInfo;
      public ExtraBoneInfo extraBoneInfo;
    }
  }
}
