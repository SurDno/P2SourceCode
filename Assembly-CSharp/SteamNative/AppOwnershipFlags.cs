// Decompiled with JetBrains decompiler
// Type: SteamNative.AppOwnershipFlags
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SteamNative
{
  internal enum AppOwnershipFlags
  {
    None = 0,
    OwnsLicense = 1,
    FreeLicense = 2,
    RegionRestricted = 4,
    LowViolence = 8,
    InvalidPlatform = 16, // 0x00000010
    SharedLicense = 32, // 0x00000020
    FreeWeekend = 64, // 0x00000040
    RetailLicense = 128, // 0x00000080
    LicenseLocked = 256, // 0x00000100
    LicensePending = 512, // 0x00000200
    LicenseExpired = 1024, // 0x00000400
    LicensePermanent = 2048, // 0x00000800
    LicenseRecurring = 4096, // 0x00001000
    LicenseCanceled = 8192, // 0x00002000
    AutoGrant = 16384, // 0x00004000
    PendingGift = 32768, // 0x00008000
    RentalNotActivated = 65536, // 0x00010000
    Rental = 131072, // 0x00020000
    SiteLicense = 262144, // 0x00040000
  }
}
