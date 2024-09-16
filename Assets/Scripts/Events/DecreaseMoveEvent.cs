using UnityEngine;

public class DecreaseMoveEvent : BaseEvent
{

    private float defaultMoveSpeed;

    public DecreaseMoveEvent()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement moveScript = player.GetComponent<PlayerMovement>();
        defaultMoveSpeed = moveScript.moveSpeed;
    }

    public override void StartEvent()
    {
        Debug.Log("Decrease Move Event Started");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement moveScript = player.GetComponent<PlayerMovement>();
        moveScript.moveSpeed = defaultMoveSpeed * 0.75f;
    }
    
    public override void StopEvent()
    {
        Debug.Log("Decrease Move Event Stopped");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement moveScript = player.GetComponent<PlayerMovement>();
        moveScript.moveSpeed = defaultMoveSpeed;
    }
    
    public override string GetName() {
        return "Move Scale Down";
    }
    
    public override float GetEventDuration()
    {
        return 3;
    }
    
}
