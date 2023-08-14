using System.Collections;
using UnityEngine;

public class RotationSnapper : MonoBehaviour
{
    private Coroutine snapCoroutine = null;

    public void InitSnapCoroutine(Quaternion targetRotation, float timeToComplete, System.Action OnSnapFinished)
    {
        snapCoroutine = StartCoroutine(RotateCoroutine(targetRotation, timeToComplete, OnSnapFinished));
    }

    public void StopSnapCoroutine()
    {
        if (snapCoroutine != null) StopCoroutine(snapCoroutine);
    }

    private IEnumerator RotateCoroutine(Quaternion targetRotation, float timeToComplete, System.Action OnSnapFinished)
    {
        Quaternion startingRotation = transform.rotation;

        float elapsedTime = 0;

        while (elapsedTime < timeToComplete)
        {
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / timeToComplete);
            yield return null;

            elapsedTime += Time.deltaTime;
        }
        transform.rotation = targetRotation;

        OnSnapFinished?.Invoke(); // ApplyConfiguration(targetAngle);
    }
}
