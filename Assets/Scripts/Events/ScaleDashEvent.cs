using UnityEngine;

public class ScaleDashEvent : BaseEvent
{

    private float defaultDashSize;
    private float defaultDashRate;

    public ScaleDashEvent()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement moveScript = player.GetComponent<PlayerMovement>();
        defaultDashSize = moveScript.dashSize;
        defaultDashRate = moveScript.dashRate;
    }

    public override void StartEvent()
    {
        Debug.Log("Dash Scale Event Started");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement moveScript = player.GetComponent<PlayerMovement>();
        moveScript.dashSize = defaultDashSize * 2;
        moveScript.dashRate = defaultDashRate / 2;
    }
    
    public override void StopEvent()
    {
        Debug.Log("Dash Scale Event Stopped");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement moveScript = player.GetComponent<PlayerMovement>();
        moveScript.dashSize = defaultDashSize;
        moveScript.dashRate = defaultDashRate;
    }
    
    public override string GetName() {
        return "Dash Scale";
    }
    
    public override float GetEventDuration()
    {
        return 6;
    }
    
}
