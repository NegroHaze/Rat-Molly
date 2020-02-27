using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tremblement : MonoBehaviour
{
    public Transform camTransform;
    public float shakeDuration = 0f; //4
    public float shakeAmount = 0.7f; //0.7
    public float decreaseFactor = 1.0f; //4
    Vector3 startPos;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        startPos = camTransform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            camTransform.localPosition = startPos + Random.insideUnitSphere * shakeAmount * shakeDuration;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            camTransform.localPosition = startPos;
        }
    }
}
