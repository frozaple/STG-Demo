using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public abstract class LoopBehaviour : PlayableBehaviour
{
    public float loopGap;

    protected float loopTime;
    protected int loopCount;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        loopTime = 0;
        loopCount = 0;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        loopTime += info.deltaTime;
        if (loopTime > 0 || loopCount == 0)
        {
            if (loopTime > loopCount * loopGap)
            {
                OnLoopCompleteOnce();
                loopCount++;
            }
        }
    }

    protected abstract void OnLoopCompleteOnce();
}
