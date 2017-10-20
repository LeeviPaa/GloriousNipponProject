using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTexture : MonoBehaviour
{
	// need attach texture
	[SerializeField]
    private Texture2D valueTexture;
	[SerializeField]
	private int defaultValue = 100;

	private int width = 100;
	private int height = 100;
	private int degit;
	private List<int> eachDigit = new List<int>();

	private void Start() {
		ChangeShowValue(defaultValue);
	}


	void Update()
    {
    }

	// change particle value
	// value [in] change value you need
	public void ChangeShowValue(int value)
	{
		SetupValueInfo(value);
		ChangeNewTexture();
	}

	private void SetupValueInfo(int value)
	{
		degit = 0;
		eachDigit.Clear();
		while (value != 0)
		{
			eachDigit.Add(value % 10);
			value /= 10;
			degit++;
		}
	}

	private void ChangeNewTexture()
	{
		Color[] output = new Color[width * degit * height];
		for (int i = 0; i < degit; ++i) {
			var pixels = GetPixels(eachDigit[degit - i - 1]);
			for (int y = 0; y < width; ++y) {
				for (int x = 0; x < height; ++x) {
					output[degit * width * y + (width * i + x)] = pixels[width * y + x];
				}
			}
		}

		var changedTexture = new Texture2D(width * degit, height, TextureFormat.RGBA32, false);
		changedTexture.filterMode = FilterMode.Point;
		changedTexture.SetPixels(output);
		changedTexture.Apply();

		GetComponent<ParticleSystemRenderer>().material.mainTexture = changedTexture;
	}

    private Color[] GetPixels(int value) {
		var result = new Color[width * height];
        for (int i = 0; i < height; ++i) {
            for (int j = 0; j < width; ++j) {
                Color pixel = valueTexture.GetPixel(j + width * value, i);
                result[i * width + j] = pixel;
            }
        }
        return result;
    }
}
