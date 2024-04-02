namespace RSCG_WhatIAmDoing_Common;

[Flags]
public enum AccumulatedStateMethod
{
    None = 0,
    Started=1,
    Finished=2,
    HasFinishedWithResult = 4,
    HasFinishedNoResult=8,
    RaiseException=16,

}
