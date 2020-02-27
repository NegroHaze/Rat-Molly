using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolObject : MonoBehaviour
{
    public GameObject p_Sphere;
    public GameObject p_Cube;

    public GameObject Bomb;

    public AudioClip boom;

    private bool explosion = false;

    public GameObject parent;

    private void Awake()
    {
        foreach (var go in GameObject.FindGameObjectsWithTag("WillGrab"))
        {
            go.GetComponent<Rigidbody>().useGravity = false;
            go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
    void Start()
    {
    }

    void Update()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        print(collision.transform.gameObject.name);
        switch (collision.transform.gameObject.name)
        {
            case "bombe_bas":
                if (!explosion)
                {
                    explosion = true;
                }
                else
                {
                    break;
                }
                GetComponent<Tremblement>().enabled = true;
                var bombAudio = Bomb.GetComponent<AudioSource>();
                bombAudio.Stop();
                bombAudio.loop = false;
                bombAudio.clip = boom;
                bombAudio.volume = 0.5f;
                bombAudio.Play();

                foreach (GameObject go in GameObject.FindGameObjectsWithTag("WillGrab"))
                {
                    this.CloneGrabable(go);
                    go.GetComponent<Rigidbody>().useGravity = true;
                    go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    go.GetComponent<Rigidbody>().AddForce(go.transform.up * 3 - go.transform.forward * 2, ForceMode.Impulse);
                    go.tag = "Grabable";
                }
                break;
        }
    }



    void CloneGrabable(GameObject Object)
    {
        var objectScript = Object.GetComponent<GrabableObject>();
        if (!objectScript.dirty)
        {
            var clonedObject = Instantiate(Object);
            clonedObject.transform.SetParent(parent.transform);
            InputControl.clonedObjects.Add(clonedObject);
            clonedObject.transform.position = Object.transform.position;
            clonedObject.transform.localScale = Object.transform.localScale;
            clonedObject.transform.rotation = Object.transform.rotation;
            clonedObject.tag = "Untagged";
            clonedObject.GetComponent<GrabableObject>().setOutlined();
            Destroy(clonedObject.GetComponent<Rigidbody>());
            Destroy(clonedObject.GetComponent<GrabableObject>());
            clonedObject.GetComponent<MeshCollider>().isTrigger = true;
            Object.GetComponent<GrabableObject>().dirty = true;
        }
    }

}
