using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeChangeColor : MonoBehaviour
{
    public Color[] colors;
    public AudioClip fxClip;

    private Material material;
    protected AudioSource myAS;

    private int index = 0;

    void Start()
    {
        material = GetComponent<Renderer>().material;
        myAS = GetComponent<AudioSource>();
    }

    public void InteractionTriggered()
    {
        material.color = colors[index++];
        if (index >= colors.Length) { index = 0; }
        if (fxClip != null){ myAS.PlayOneShot(fxClip); }
    }
}
