using UnityEngine;

public class ScaleMoveEvent : BaseEvent
{

    private float defaultMoveSpeed;

    public ScaleMoveEvent()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement moveScript = player.GetComponent<PlayerMovement>();
        defaultMoveSpeed = moveScript.moveSpeed;
    }

    public override void StartEvent()
    {
        Debug.Log("Move Scale Event Started");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement moveScript = player.GetComponent<PlayerMovement>();
        moveScript.moveSpeed = defaultMoveSpeed * 2;
    }
    
    public override void StopEvent()
    {
        Debug.Log("Move Scale Event Stopped");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement moveScript = player.GetComponent<PlayerMovement>();
        moveScript.moveSpeed = defaultMoveSpeed;
    }
    
    public override string GetName() {
        return "Move Scale";
    }
    
    public override float GetEventDuration()
    {
        return 3;
    }
    
}
