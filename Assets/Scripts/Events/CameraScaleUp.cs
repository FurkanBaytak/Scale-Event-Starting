using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraScaleUp : BaseEvent
{
    private CinemachineVirtualCamera virtualCamera;
    private float originalOrthoSize;
    private float targetOrthoSize = 13.5f;
    private float duration = 5f;

    public CameraScaleUp()
    {
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        if (virtualCamera != null)
        {
            originalOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera not found!");
        }
    }

    public override void StartEvent()
    {
        Debug.Log("Camera Scale Up Event Started");
        if (virtualCamera != null)
        {
            CoroutineRunner.Instance.StartCoroutine(ScaleCamera(targetOrthoSize));
        }
    }

    public override void StopEvent()
    {
        Debug.Log("Camera Scale Up Event Stopped");

        if (virtualCamera != null)
        {
            CoroutineRunner.Instance.StartCoroutine(ScaleCamera(originalOrthoSize));
        }
    }

    private IEnumerator ScaleCamera(float targetSize)
    {
        float startSize = virtualCamera.m_Lens.OrthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = targetSize;
    }

    public override string GetName()
    {
        return "Camera Scale Up";
    }

    public override float GetEventDuration()
    {
        return duration;
    }
}
