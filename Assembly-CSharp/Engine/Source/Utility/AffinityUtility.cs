rt("Affinity")]
    private static extern bool SetAffinity(long value);

    public static void ComputeAffinity()
    {
      try
      {
        Debug.Log((object) "ComputeAffinity , try compute");
        int affinity = ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.Affinity;
        switch (affinity)
        {
          case -1:
            Debug.Log((object) "ComputeAffinity , not requred");
            break;
          case 0:
            Debug.Log((object) "ComputeAffinity , try disable ht");
            int numberOfCores = SystemInfoUtility.NumberOfCores;
            int logicalProcessors1 = SystemInfoUtility.NumberOfLogicalProcessors;
            Debug.Log((object) ("ComputeAffinity , NumberOfCores : " + numberOfCores));
            Debug.Log((object) ("ComputeAffinity , NumberOfLogicalProcessors : " + logicalProcessors1));
            if (ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MinAffinityCount > logicalProcessors1)
            {
              Debug.Log((object) ("ComputeAffinity , min : " + (object) ExternalSettingsInstance<ExternalOptimizationSettings>.Instance.MinAffinityCount + " > current : " + logicalProcessors1));
              break;
            }
            long num1 = 0;
            if (!AffinityUtility.GetAffinity(ref num1))
            {
              Debug.LogError((object) "ComputeAffinity , Error invoke : GetAffinity");
              break;
            }
            Debug.Log((object) ("ComputeAffinity , current value : " + AffinityUtility.ToBinary(num1, logicalProcessors1)));
            if (numberOfCores == logicalProcessors1)
            {
              Debug.Log((object) "ComputeAffinity , no ht detected");
              break;
            }
            if (numberOfCores * 2 == logicalProcessors1)
            {
              Debug.Log((object) "ComputeAffinity , try compute x2");
              for (long index = 0; index < numberOfCores; ++index)
              {
                long num2 = index * 2L + 1L;
                num1 &= ~(1 << (int) num2);
              }
              if (!AffinityUtility.SetAffinity(num1))
              {
                Debug.LogError((object) "ComputeAffinity , Error invoke : SetAffinity");
                break;
              }
              if (!AffinityUtility.GetAffinity(ref num1))
              {
                Debug.LogError((object) "ComputeAffinity , Error invoke : GetAffinity");
                break;
              }
              Debug.Log((object) ("ComputeAffinity , new value : " + AffinityUtility.ToBinary(num1, logicalProcessors1)));
              break;
            }
            Debug.Log((object) "ComputeAffinity , is not support");
            break;
          default:
            if (affinity > 0)
            {
              Debug.Log((object) ("ComputeAffinity , try set value : " + affinity));
              int logicalProcessors2 = SystemInfoUtility.NumberOfLogicalProcessors;
              long num3 = 0;
              if (!AffinityUtility.GetAffinity(ref num3))
              {
                Debug.LogError((object) "ComputeAffinity , Error invoke : GetAffinity");
                break;
              }
              Debug.Log((object) ("ComputeAffinity , current value : " + AffinityUtility.ToBinary(num3, logicalProcessors2)));
              long num4 = affinity;
              if (!AffinityUtility.SetAffinity(num4))
              {
                Debug.LogError((object) "ComputeAffinity , Error invoke : SetAffinity");
                break;
              }
              if (!AffinityUtility.GetAffinity(ref num4))
              {
                Debug.LogError((object) "ComputeAffinity , Error invoke : GetAffinity");
                break;
              }
              Debug.Log((object) ("ComputeAffinity , new value : " + AffinityUtility.ToBinary(num4, logicalProcessors2)));
              break;
            }
            Debug.Log((object) "ComputeAffinity , wrong value");
            break;
        }
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
      }
    }

    public static string ToBinary(long value, int count)
    {
      return "0b" + Convert.ToString(value, 2).PadLeft(count, '0');
    }
  }
}
