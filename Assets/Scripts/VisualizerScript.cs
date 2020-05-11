using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class VisualizerScript : MonoBehaviour
{
    public float minHeight = 15.0f;
    public float maxHeight = 425.0f;
    public float updateSenstivity = 0.5f;
    public Color visualizerColor = Color.gray;
    [Space(15)]
    public AudioClip audioClip;
    public bool loop = true;
    [Space(15), Range (64, 8192)]
    public int visualizerSimples = 64;


    public VisualizerObjectScript[] visualizerObjects;
    AudioSource m_audioSource;
    // Start is called before the first frame update
    void Start()
    {
        visualizerObjects = GetComponentsInChildren<VisualizerObjectScript>();

        if (!audioClip)
            return;

        m_audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
        m_audioSource.loop = loop;
        m_audioSource.clip = audioClip;
        m_audioSource.Play();

    }

    // Update is called once per frame
    void Update(){
        float[] spectrumData = m_audioSource.GetSpectrumData(visualizerSimples, 0, FFTWindow.Rectangular);


        for (int i = 0; i < visualizerObjects.Length; i++) {
            Vector2 newSize = visualizerObjects[i].GetComponent<RectTransform>().rect.size;

            newSize.y = Mathf.Clamp (Mathf.Lerp(newSize.y, minHeight  + (spectrumData[i] * (maxHeight - minHeight) * 5.0f), updateSenstivity), minHeight, maxHeight);
            visualizerObjects[i].GetComponent<RectTransform>().sizeDelta = newSize;

            visualizerObjects[i].GetComponent<Image>().color = visualizerColor;
        }
            

        

        }

    }
