using System.Collections;
using UnityEngine;

public class RotationSnapper : MonoBehaviour
{
    private Coroutine snapCoroutine = null;

    public System.Action OnSnapFinished { get; set; }

    public void InitSnapCoroutine(Quaternion targetRotation, float timeToComplete)
    {
        if (snapCoroutine != null) StopCoroutine(snapCoroutine);

        snapCoroutine = StartCoroutine(RotateCoroutine(targetRotation, timeToComplete));
    }

    public void StopSnapCoroutine()
    {
        if (snapCoroutine != null) StopCoroutine(snapCoroutine);
    }

    private IEnumerator RotateCoroutine(Quaternion targetRotation, float timeToComplete)
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
