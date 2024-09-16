using UnityEngine;

public class GodModeEvent : BaseEvent
{
    
    public override void StartEvent()
    {
        Debug.Log("God Event Started");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement moveScript = player.GetComponent<PlayerMovement>();
        moveScript.isGodMode = true;
    }
    
    public override void StopEvent()
    {
        Debug.Log("God Event Stopped");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement moveScript = player.GetComponent<PlayerMovement>();
        moveScript.isGodMode = false;
    }
    
    public override string GetName() {
        return "God Mode";
    }
    
    public override float GetEventDuration()
    {
        return 3;
    }
    
}
