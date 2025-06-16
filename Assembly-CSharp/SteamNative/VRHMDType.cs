namespace SteamNative
{
  internal enum VRHMDType
  {
    None = -1, // 0xFFFFFFFF
    Unknown = 0,
    HTC_Dev = 1,
    HTC_VivePre = 2,
    HTC_Vive = 3,
    HTC_Unknown = 20, // 0x00000014
    Oculus_DK1 = 21, // 0x00000015
    Oculus_DK2 = 22, // 0x00000016
    Oculus_Rift = 23, // 0x00000017
    Oculus_Unknown = 40, // 0x00000028
  }
}
