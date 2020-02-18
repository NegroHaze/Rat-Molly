using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputControl : MonoBehaviour
{

    private GameObject _rightHand;
    private GameObject _hand;
    private GameObject _head;

    public GameObject test;

    private Vector3 _objectPosition;
    private float _rotationSpeed = 5f;

    private GameObject _grabedObject;

    [SerializeField]
    [Range(1, 10)]
    private float _verticalSpeed = 5f;

    [SerializeField]
    [Range(1, 10)]
    private float _horizontalSpeed = 5f;

    void Start()
    {
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
        if ((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTrackedRemote) || Input.GetMouseButtonDown(0)) && !this._grabedObject)
        {
            print("clicked");
            Ray ray = new Ray(this._hand.transform.position, this._hand.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider)
                {
                    if (hit.transform.gameObject.tag == "Grabable" && hit.transform.gameObject != _grabedObject)
                    {
                        this.GrabObject(hit);
                    }
                }
            }
        }
        else if ((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTrackedRemote) || Input.GetMouseButtonDown(0)) && this._grabedObject)
        {
            this.ReleaseObject();
        }
        else if ((OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y > 0 || Input.GetMouseButton(1)) && this._grabedObject)
        {
            this.LaunchObject();
        }
    }

    void GrabObject(RaycastHit hit)
    {
        hit.transform.SetParent(this._hand.transform);
        this._grabedObject = hit.transform.gameObject;
        var grabedRigidBody = this._grabedObject.GetComponent<Rigidbody>();
        var boxCollider = this._grabedObject.GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        grabedRigidBody.velocity = Vector3.zero;
        grabedRigidBody.angularVelocity = Vector3.zero;
        grabedRigidBody.useGravity = false;
        this._grabedObject.transform.position = this._hand.transform.position + this._hand.transform.forward * 2;
    }

    void LaunchObject()
    {
        var tempElem = this._grabedObject;
        this._grabedObject = null;
        var grabedRigidBody = tempElem.GetComponent<Rigidbody>();
        grabedRigidBody.useGravity = true;
        tempElem.transform.SetParent(null);
        grabedRigidBody.AddForce(this._hand.transform.forward, ForceMode.Impulse);
        var boxCollider = tempElem.GetComponent<BoxCollider>();
        boxCollider.isTrigger = false;
    }

    void ReleaseObject()
    {
        var tempElem = this._grabedObject;
        this._grabedObject = null;
        var grabedRigidBody = tempElem.GetComponent<Rigidbody>();
        grabedRigidBody.useGravity = true;
        tempElem.transform.SetParent(null);
        var boxCollider = tempElem.GetComponent<BoxCollider>();
        boxCollider.isTrigger = false;
    }
}
