using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputControl : MonoBehaviour
{

    private GameObject _rightHand;
    private GameObject _hand;
    private GameObject _head;
    public static GameObject grabedObject;
    public static List<GameObject> clonedObjects = new List<GameObject>();

    private GameObject _highLightedObject;


    private Vector3 _objectPosition;
    private float _rotationSpeed = 5f;

    public Material Outlined;

    private float _verticalSpeed = 5f;
    private float _horizontalSpeed = 5f;


    private bool _isCheckPos;
    private bool _isCheckAngles;

    [Range(0.1f, 10f)]
    public float RATIOMAGNETPOSITION = 2f;
    [Range(0.1f, 10f)]
    public float RATIONMAGNETANGLES = 25f;

    void Start()
    {
        Cursor.visible = false;
        this._rightHand = GameObject.Find("RightHand");
        this._hand = GameObject.Find("Hand");
        this._head = GameObject.Find("Head");
    }

    void Update()
    {
        if (XRDevice.isPresent)
        {
            if (!OVRInput.Get(OVRInput.Button.One))
            {
                this._hand.transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
            }
            else if (OVRInput.Get(OVRInput.Button.One) && grabedObject)
            {
                grabedObject.transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                this._hand.GetComponent<LineRenderer>().enabled = true;
                this._rightHand.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * this._horizontalSpeed, 0f, 0f));
                this._hand.transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X") * this._verticalSpeed, 0f));
            }
            else if (Input.GetKey(KeyCode.LeftAlt) && grabedObject)
            {
                grabedObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * _rotationSpeed, -Input.GetAxis("Mouse X") * _rotationSpeed, 0f), Space.World);
            }
            else
            {
                this._head.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * this._horizontalSpeed, 0f, 0f));
                this.transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X") * this._verticalSpeed, 0f));
                this._hand.GetComponent<LineRenderer>().enabled = false;
            }
        }

        this.ManageController();
        this.ManageGrabedPosition();
        if (grabedObject != null && GameObject.Find(grabedObject.name + "(Clone)"))
        {
            print(clonedObjects.Count);
            var clone = clonedObjects.Find(o => o.name == grabedObject.name + "(Clone)");
            if (clone)
            {
                this.CheckPlacement(clone);
            }
        }
    }

    void ManageController()
    {
        Ray ray = new Ray(this._hand.transform.position, this._hand.transform.forward);
        RaycastHit hit;

        Physics.Raycast(ray, out hit);

        if (hit.collider)
        {
            if (hit.transform.gameObject.tag == "Grabable" && hit.transform.gameObject != grabedObject && !grabedObject)
            {
                if (this._highLightedObject != null && this._highLightedObject != hit.transform.gameObject)
                {
                    this._highLightedObject.GetComponent<GrabableObject>().RemoveHighLight();
                }
                if (this._highLightedObject != hit.transform.gameObject)
                {
                    this._highLightedObject = hit.transform.gameObject;
                    this._highLightedObject.transform.gameObject.GetComponent<GrabableObject>().HighLight();
                }
            }
            else if (this._highLightedObject != null && this._highLightedObject != hit.transform.gameObject && !grabedObject)
            {
                this._highLightedObject.GetComponent<GrabableObject>().RemoveHighLight();
                this._highLightedObject = null;
            }


            if (XRDevice.isPresent)
            {
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTrackedRemote) && !grabedObject)
                {
                    if (hit.transform.gameObject.tag == "Grabable" && hit.transform.gameObject != grabedObject)
                    {
                        this.GrabObject(hit.transform.gameObject);
                    }
                }
                else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTrackedRemote) && grabedObject)
                {
                    this.ReleaseObject();
                }
            }
            else
            {
                if (Input.GetMouseButton(0) && !grabedObject)
                {
                    if (hit.transform.gameObject.tag == "Grabable" && hit.transform.gameObject != grabedObject)
                    {
                        this.GrabObject(hit.transform.gameObject);
                    }
                }
                else if (!Input.GetMouseButton(0) && grabedObject)
                {
                    this.ReleaseObject();
                }

            }
        }
    }

    void ManageGrabedPosition()
    {
        if (!grabedObject || OVRInput.Get(OVRInput.Button.One)) { return; }
        if (XRDevice.isPresent)
        {
            Vector2 primaryTouchpad = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
            if (primaryTouchpad.y < 0f)
            {
                grabedObject.GetComponent<GrabableObject>().moveForward(this._hand);
            }
            else if (primaryTouchpad.y > 0f)
            {
                grabedObject.GetComponent<GrabableObject>().moveBackward(this._hand);
            }
        }
        else
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                grabedObject.GetComponent<GrabableObject>().moveForward(this._hand);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                grabedObject.GetComponent<GrabableObject>().moveBackward(this._hand);
            }
        }
    }

    void GrabObject(GameObject gameObject)
    {
        grabedObject = gameObject;
        gameObject.transform.SetParent(this._hand.transform);
        var grabedRigidBody = grabedObject.GetComponent<Rigidbody>();
        var boxCollider = grabedObject.GetComponent<Collider>();
        boxCollider.isTrigger = false;
        grabedRigidBody.velocity = Vector3.zero;
        grabedRigidBody.constraints = RigidbodyConstraints.FreezeAll;
        grabedRigidBody.angularVelocity = Vector3.zero;
        grabedRigidBody.useGravity = false;
        grabedObject.GetComponent<GrabableObject>().initPos = grabedObject.transform.position;
    }

    void ReleaseObject()
    {
        var tempElem = grabedObject;
        Vector3 tempPosClone = Vector3.zero;
        Quaternion tempAngleClone = new Quaternion();
        GameObject clone = null;
        if (GameObject.Find(grabedObject.name + "(Clone)"))
        {
            clone = clonedObjects.Find(o => o.name == grabedObject.name + "(Clone)");

            if (clone)
            {
                tempPosClone = clone.transform.position;
                tempAngleClone = clone.transform.rotation;
            }

        }
        grabedObject = null;
        var grabedRigidBody = tempElem.GetComponent<Rigidbody>();
        grabedRigidBody.constraints = RigidbodyConstraints.None;

        if (tempPosClone != Vector3.zero)
        {
            grabedRigidBody.useGravity = this._isCheckPos || this._isCheckAngles;
        } else
        {
            grabedRigidBody.useGravity = true;
        }

        tempElem.transform.SetParent(null);
        var boxCollider = tempElem.GetComponent<MeshCollider>();
        boxCollider.isTrigger = false;

        if (this._isCheckPos)
        {
            tempElem.tag = "Untagged";
            clonedObjects.Remove(GameObject.Find(tempElem.name + "(Clone)"));
            Destroy(GameObject.Find(tempElem.name + "(Clone)"));
            this.ReplaceAxes(tempPosClone, tempElem);
            this.ReplaceAngles(tempAngleClone, tempElem);
            tempElem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    void CheckPlacement(GameObject clone)
    {
        var grabedPosition = grabedObject.transform.position;
        var clonePosition = clone.transform.position;

        var grabedAngle = grabedObject.transform.rotation.eulerAngles;
        var cloneAngle = clone.transform.rotation.eulerAngles;

        this._isCheckPos = this.CheckAxes(clonePosition, grabedPosition, RATIOMAGNETPOSITION);
        this._isCheckAngles = this.CheckAxes(cloneAngle, grabedAngle, RATIONMAGNETANGLES);
    }

    bool CheckAxes(Vector3 cloneAxes, Vector3 grabAxes, float ratio)
    {
        var checkX = (cloneAxes.x - ratio) < grabAxes.x && grabAxes.x < (cloneAxes.x + ratio);
        var checkY = (cloneAxes.y - ratio) < grabAxes.y && grabAxes.y < (cloneAxes.y + ratio);
        var checkZ = (cloneAxes.z - ratio) < grabAxes.z && grabAxes.z < (cloneAxes.z + ratio);
        return checkX && checkY && checkZ;

    }

    void ReplaceAxes(Vector3 cloneObj, GameObject grabObj)
    {
        grabObj.transform.position = cloneObj;
    }
    void ReplaceAngles(Quaternion cloneObj, GameObject grabObj)
    {
        grabObj.transform.rotation = cloneObj;
    }
}
