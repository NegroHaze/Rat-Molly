using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    
    public Material HighLightedMaterial;
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
        print("high");
        var tempArray = new List<Material>();
        for (int i = 0; i < mesh.GetComponent<Renderer>().materials.Length; i++)
        {
            tempArray.Add(this.HighLightedMaterial);
        }
        print("ici");
        mesh.GetComponent<Renderer>().materials = tempArray.ToArray();
    }

    public void RemoveHighLight()
    {
        print("remove");
        mesh.GetComponent<Renderer>().materials = this.DefaultMaterials;
    }

    public void setOutlined()
    {
        var tempArray = new List<Material>();
        for (int i = 0; i < mesh.GetComponent<Renderer>().materials.Length; i++)
        {
            tempArray.Add(this.outlined);
            print(tempArray[i]);
        }

        print("la");
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
