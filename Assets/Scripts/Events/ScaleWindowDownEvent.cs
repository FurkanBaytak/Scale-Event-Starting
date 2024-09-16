using UnityEngine;

public class ScaleWindowDownEvent : BaseEvent
{
    private int originalWidth;
    private int originalHeight;
    private bool wasFullScreen;

    public ScaleWindowDownEvent()
    {
        originalWidth = Screen.width;
        originalHeight = Screen.height;
        wasFullScreen = Screen.fullScreen;
    }

    public override void StartEvent()
    {
        Debug.Log("Scale Window Event Started");

        if (wasFullScreen)
        {
            Screen.fullScreen = false;
        }

        Screen.SetResolution(1200, 900, false);
    }

    public override void StopEvent()
    {
        Debug.Log("Scale Window Event Stopped");

        Screen.SetResolution(originalWidth, originalHeight, false);

        if (wasFullScreen)
        {
            Screen.fullScreen = true;
        }
    }

    public override string GetName()
    {
        return "Scale Window";
    }

    public override float GetEventDuration()
    {
        return 5;
    }
}
