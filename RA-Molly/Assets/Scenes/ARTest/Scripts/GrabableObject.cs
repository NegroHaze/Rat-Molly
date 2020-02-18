using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    
    public Material HighLightedMaterial;
    public Material outlined;
    private Material[] DefaultMaterials;

    public bool dirty = false;

    public GameObject mesh;

    // Start is called before the first frame update
    void Start()
    {
        this.DefaultMaterials = mesh.GetComponent<Renderer>().materials;
    }

    // Update is called once per frame
    void Update()
    {
        print(mesh.GetComponent<Renderer>().material);
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

}
