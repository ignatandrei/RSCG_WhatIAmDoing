﻿namespace RSCG_WhatIAmDoing_Common;

[Flags]
public enum AccumulatedStateMethod
{
    None = 0,
    Started=1,
    Finished=2,
    HasResult=4,
    RaiseException=8,

}
