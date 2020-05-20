using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Material))]

public class GetColorFree : MonoBehaviour {
    private MusicAnalyzerFree analyzer;
    private Material material;
    public Color basicsColor = Color.grey;
    private Color color;
    private Color newColor;
    public bool isEmissive = false;
    public bool useRed = true;
    public bool useGreen = true;
    public bool useBlue = true;

    // Use this for initialization
    void Start () {
        analyzer = MusicAnalyzerFree.instance;
        material = GetComponent<MeshRenderer>().material;
        material.color = basicsColor;
    }
	
	// Update is called once per frame
	void Update () {
        GetColorUpdate();
        if(isEmissive)
        {
            GetEmissiveColorUpdate();
        } else
        {
            GetNoEmissiveColorUpdate();
        }
	}

    void GetColorUpdate()
    {
        newColor = analyzer.GetColor() + basicsColor;
        if(useRed && newColor.r > 1)
        {
            newColor.r = 1;
        } else if(useRed && newColor.r > 1)
        {
            newColor.r = 0;
        }
        if (useGreen && newColor.g > 1)
        {
            newColor.g = 1;
        }
        else if (useGreen && newColor.g < 0)
        {
            newColor.g = 0;
        }
        if (useBlue && newColor.r > 1)
        {
            newColor.b = 1;
        }
        else if (useBlue && newColor.b < 0)
        {
            newColor.b = 0;
        }

        color = new Color(useRed ? newColor.r : color.r, useGreen ? newColor.g : color.g, useBlue ? newColor.b : color.b);

        material.color = color;
    }

    void GetEmissiveColorUpdate()
    {
        material.SetColor("_EmissionColor", color);
    }

    void GetNoEmissiveColorUpdate()
    {
        material.SetColor("_EmissionColor", Color.black);
    }
    
}
