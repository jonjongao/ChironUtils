using System;

[Flags]
public enum JointMask
{
    None = 0,
    All = Hip | UpperBody | RightArm | LeftArm |
        LowerBody | RightLeg | LeftLeg,
    Hip = 1 << 0,
    UpperBody = 1 << 1,
    RightArm = 1 << 2,
    LeftArm = 1 << 3,
    LowerBody = 1 << 4,
    RightLeg = 1 << 5,
    LeftLeg = 1 << 6
}