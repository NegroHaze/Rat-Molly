using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    
    public Material outlined;
    private Material[] DefaultMaterials;

    public Vector3 initPos;

    public bool dirty = false;

    public GameObject mesh;
    
    void Start()
    {
        this.DefaultMaterials = mesh.GetComponent<Renderer>().materials;
    }

    public void HighLight()
    {
        var tempArray = new List<Material>();
        for (int i = 0; i < mesh.GetComponent<Renderer>().materials.Length; i++)
        {
            mesh.GetComponent<Renderer>().materials[i].SetColor("_EmissionColor", Color.gray);
        }
    }

    public void RemoveHighLight()
    {
        var tempArray = new List<Material>();
        for (int i = 0; i < mesh.GetComponent<Renderer>().materials.Length; i++)
        {
            mesh.GetComponent<Renderer>().materials[i].SetColor("_EmissionColor", Color.black);
        }
    }

    public void setOutlined()
    {
        var tempArray = new List<Material>();
        for (int i = 0; i < mesh.GetComponent<Renderer>().materials.Length; i++)
        {
            tempArray.Add(this.outlined);
        }
        
        mesh.GetComponent<Renderer>().materials = tempArray.ToArray();
        mesh.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public void moveBackward(GameObject relativeTo)
    {
        transform.position = transform.position + relativeTo.transform.forward * 0.2f;
    }

    public void moveForward(GameObject relativeTo)
    {   
        transform.position = transform.position + relativeTo.transform.forward * -0.2f;
    }
}
