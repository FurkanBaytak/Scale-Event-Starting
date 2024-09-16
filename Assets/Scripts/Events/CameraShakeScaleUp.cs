using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraShakeScaleUpEvent : BaseEvent
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    private float originalAmplitudeGain;
    private float originalFrequencyGain;

    public CameraShakeScaleUpEvent()
    {
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise != null)
            {
                originalAmplitudeGain = noise.m_AmplitudeGain;
                originalFrequencyGain = noise.m_FrequencyGain;
            }
        }
    }

    public override void StartEvent()
    {
        Debug.Log("Camera Shake Scale Up Event Started");
        if (noise != null)
        {
            virtualCamera.StartCoroutine(SmoothShakeTransition(originalAmplitudeGain, originalFrequencyGain, 3f, 3f, 1f));
        }
    }

    public override void StopEvent()
    {
        Debug.Log("Camera Shake Scale Up Event Stopped");
        if (noise != null)
        {
            virtualCamera.StartCoroutine(SmoothShakeTransition(3f, 3f, originalAmplitudeGain, originalFrequencyGain, 1f));
        }
    }

    private IEnumerator SmoothShakeTransition(float startAmplitude, float startFrequency, float endAmplitude, float endFrequency, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            noise.m_AmplitudeGain = Mathf.Lerp(startAmplitude, endAmplitude, elapsedTime / duration);
            noise.m_FrequencyGain = Mathf.Lerp(startFrequency, endFrequency, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        noise.m_AmplitudeGain = endAmplitude;
        noise.m_FrequencyGain = endFrequency;
    }

    public override string GetName()
    {
        return "Camera Shake Scale Up";
    }

    public override float GetEventDuration()
    {
        return 5;
    }
}
