using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class MusicAnalyzerFree : MonoBehaviour {
    public static MusicAnalyzerFree instance = null;
    private AudioSource audio;
    float[] spectrum = new float[128];
    // Spectrum is multiplayed with volume. To take any values from spectrum, volume should have not zero value
    [SerializeField] [HideInInspector] private float minVolume = 0.005f;
    [SerializeField] private Color color = Color.white;
    public AudioClip Song;

    // Examples of multypliers
    private float red;
    private float green;
    private float blue;
    public float multiplyRed = 8.5f / 7;
    public float multiplyGreen = 1.7f / 5;
    public float multiplyBlue = 2.7f / 5;
    private float spectrumMultiply = 1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Duplicated MusicAnalyzer gameObject was removed.");
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start() {
        audio = GetComponent<AudioSource>();
        audio.clip = Song;
        audio.Play();
        audio.loop = true;

    }
	
	// Update is called once per frame
	void Update () {
        AnalyzeMusic();
        UpdateColor();
	}

    private void AnalyzeMusic()
    {
        if (AudioListener.volume == 0)
        {
            AudioListener.volume = minVolume;
        };

        audio.GetOutputData(spectrum, 0);
        spectrumMultiply = 1 / audio.volume;
    }

    private void UpdateColor()
    {
        red = 0;
        for (int i = 1; i < 3; i++)
        {
            red += spectrum[i];
        }
        red *= multiplyRed * spectrumMultiply;
        if (red < 0) red = 0;
        else if (red > 1) red = 1;

        green = 0;
        for (int i = 5; i < 26; i++)
        {
            green += spectrum[i];
        }
        green *= multiplyGreen * spectrumMultiply;
        if (green < 0) green = 0;
        else if (green > 1) green = 1;

        blue = 0;
        for (int i = 36; i < 128; i++)
        {
            blue += spectrum[i];
        }
        blue *= multiplyBlue * spectrumMultiply;
        if (blue < 0) blue = 0;
        else if (blue > 1) blue = 1;

        color = new Color(red, green, blue);
    }

    public Color GetColor()
    {
        return color;
    }

}
