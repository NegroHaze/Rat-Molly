using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AntiAliasingClick : MonoBehaviour
{

    public Material m_clicked;
    public GameObject increaseAa;

    private Material defaultMat;

    // Start is called before the first frame update
    void Start()
    {
        this.defaultMat = increaseAa.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

        //if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        //{
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == increaseAa.gameObject)
            {
                increaseAa.GetComponent<Renderer>().material = m_clicked;
                if (Input.GetMouseButtonDown(0))
                {

                    UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 1.4f;
                    Debug.Log(XRSettings.eyeTextureResolutionScale);
                    Debug.Log("+");
                }
            } else
            {
                if (Input.GetMouseButtonDown(0))
                {

                    UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 0.5f;
                    Debug.Log(XRSettings.eyeTextureResolutionScale);
                    Debug.Log("-");
                }
                increaseAa.GetComponent<Renderer>().material = this.defaultMat;
            }
        }
        //}
    }
}
