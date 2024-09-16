using UnityEngine;

public class IncreasePlayerSize : BaseEvent
{

    public override void StartEvent()
    {
        Debug.Log("Increase Scale Event Started");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }
    
    public override void StopEvent()
    {
        Debug.Log("Increase Scale Event Stopped");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }
    
    public override string GetName() {
        return "Increase Scale";
    }
    
    public override float GetEventDuration()
    {
        return 3;
    }
    
}
