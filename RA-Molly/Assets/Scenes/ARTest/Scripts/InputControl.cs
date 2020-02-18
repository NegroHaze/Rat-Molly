using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputControl : MonoBehaviour
{

    private GameObject _rightHand;
    private GameObject _hand;
    private GameObject _head;
    private GameObject _grabedObject;

    private GameObject _highLightedObject;


    private Vector3 _objectPosition;
    private float _rotationSpeed = 5f;

    public Material Outlined;

    private float _verticalSpeed = 5f;
    private float _horizontalSpeed = 5f;

    void Start()
    {
        Cursor.visible = false;
        this._rightHand = GameObject.Find("RightHand");
        this._hand = GameObject.Find("Hand");
        this._head = GameObject.Find("Head");
    }

    void Update()
    {
        if (XRDevice.isPresent && !OVRInput.Get(OVRInput.Button.One))
        {
            this._hand.transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);

        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            this._hand.GetComponent<LineRenderer>().enabled = true;
            this._rightHand.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * this._horizontalSpeed, 0f, 0f));
            this._hand.transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X") * this._verticalSpeed, 0f));
        }
        else if ((Input.GetKey(KeyCode.LeftAlt) || OVRInput.Get(OVRInput.Button.One)) && this._grabedObject)
        {
            if (XRDevice.isPresent)
            {
                this._grabedObject.transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
            }
            else
            {
                this._grabedObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * _rotationSpeed, -Input.GetAxis("Mouse X") * _rotationSpeed, 0f), Space.World);
            }

        }
        else
        {
            this._head.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * this._horizontalSpeed, 0f, 0f));
            this.transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X") * this._verticalSpeed, 0f));
            this._hand.GetComponent<LineRenderer>().enabled = false;
        }

        this.ManageController();
    }

    void ManageController()
    {
        Ray ray = new Ray(this._hand.transform.position, this._hand.transform.forward);
        RaycastHit hit;

        Physics.Raycast(ray, out hit);

        if (hit.collider)
        {
            if (hit.transform.gameObject.tag == "Grabable" && hit.transform.gameObject != this._grabedObject)
            {
                if (this._highLightedObject != null && this._highLightedObject != hit.transform.gameObject)
                {
                    print("removing highLight");
                    this._highLightedObject.GetComponent<GrabableObject>().RemoveHighLight();
                }
                if (this._highLightedObject != hit.transform.gameObject)
                {
                    print("highLight");
                    this._highLightedObject = hit.transform.gameObject;
                    this._highLightedObject.transform.gameObject.GetComponent<GrabableObject>().HighLight();
                }
            }
            else if (this._highLightedObject != null && this._highLightedObject != hit.transform.gameObject)
            {
                this._highLightedObject.GetComponent<GrabableObject>().RemoveHighLight();
                this._highLightedObject = null;
            }

            // If user click on the trigger
            if ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTrackedRemote) || Input.GetMouseButton(0)) && !this._grabedObject)
            {
                if (hit.transform.gameObject.tag == "Grabable" && hit.transform.gameObject != this._grabedObject)
                {
                    print("Grab");
                    this.GrabObject(hit.transform.gameObject);
                }
            }

            else if ((OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTrackedRemote) || !Input.GetMouseButton(0)) && this._grabedObject)
            {
                print("Release");
                this.ReleaseObject();
            }
        }
    }

    void GrabObject(GameObject gameObject)
    {
        this._grabedObject = gameObject;
        this.CloneGrabable(this._grabedObject);
        gameObject.transform.SetParent(this._hand.transform);
        var grabedRigidBody = this._grabedObject.GetComponent<Rigidbody>();
        var boxCollider = this._grabedObject.GetComponent<BoxCollider>();
        boxCollider.isTrigger = false;
        grabedRigidBody.velocity = Vector3.zero;
        grabedRigidBody.constraints = RigidbodyConstraints.FreezeAll;
        grabedRigidBody.angularVelocity = Vector3.zero;
        grabedRigidBody.useGravity = false;
        this._grabedObject.transform.position = this._hand.transform.position + this._hand.transform.forward * 2;
    }

    //void LaunchObject()
    //{
    //    var tempElem = this._grabedObject;
    //    this._grabedObject = null;
    //    var grabedRigidBody = tempElem.GetComponent<Rigidbody>();
    //    grabedRigidBody.useGravity = true;
    //    tempElem.transform.SetParent(null);
    //    grabedRigidBody.AddForce(this._hand.transform.forward, ForceMode.Impulse);
    //    var boxCollider = tempElem.GetComponent<BoxCollider>();
    //    boxCollider.isTrigger = false;
    //}

    void ReleaseObject()
    {
        var tempElem = this._grabedObject;
        this._grabedObject = null;
        var grabedRigidBody = tempElem.GetComponent<Rigidbody>();
        grabedRigidBody.constraints = RigidbodyConstraints.None;
        grabedRigidBody.useGravity = true;
        tempElem.transform.SetParent(null);
        var boxCollider = tempElem.GetComponent<BoxCollider>();
        boxCollider.isTrigger = false;
    }

    void CloneGrabable(GameObject Object)
    {
        var objectScript = Object.GetComponent<GrabableObject>();
        if (!objectScript.dirty)
        {
            var clonedObject = Instantiate(Object);
            clonedObject.tag = "Untagged";
            clonedObject.GetComponent<GrabableObject>().setOutlined();
            Destroy(clonedObject.GetComponent<Rigidbody>());
            Destroy(clonedObject.GetComponent<GrabableObject>());
            clonedObject.GetComponent<BoxCollider>().isTrigger = true;
            Object.GetComponent<GrabableObject>().dirty = true;
        }
    }
}
