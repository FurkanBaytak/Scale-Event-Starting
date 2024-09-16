using UnityEngine;

public class DecreasePlayerSizeEvent : BaseEvent
{
    
    public override void StartEvent()
    {
        Debug.Log("Increase Scale Event Started");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    }
    
    public override void StopEvent()
    {
        Debug.Log("Increase Scale Event Stopped");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }
    
    public override string GetName() {
        return "Decrease Scale";
    }
    
    public override float GetEventDuration()
    {
        return 5;
    }
    
}
