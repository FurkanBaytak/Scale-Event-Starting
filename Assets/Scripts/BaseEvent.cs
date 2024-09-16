using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEvent
{

    public abstract float GetEventDuration();
    
    public abstract void StartEvent();
    
    public abstract void StopEvent();

    public abstract string GetName();

}
