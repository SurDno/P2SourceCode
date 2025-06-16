using System;

[Flags]
public enum FlockChildTypeEnum
{
  None = 1,
  Crow = 2,
  Pigeon = 4,
  Seagul = 8,
  Sparrow = 16, // 0x00000010
  Vulture = 32, // 0x00000020
}
