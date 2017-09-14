using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptColorBlink : MonoBehaviour {
	[HideInInspector]
	public bool bColorOverflowing = false;
	[HideInInspector]
	public bool bStopBlinking = false;

	public float FadeDuration = 1f;
	public Color ColorStart = Color.gray;
	public Color ColorEnd = Color.white;

	private Color startColor;
	private Color endColor;
	private float lastColorChangeTime;

	private Material material;

	private int addedColors = 0;

	void Start()
	{
		material = GetComponent<Renderer>().material;
		startColor = ColorStart;
		endColor = ColorEnd;
	}

	void Update()
	{		
		if (!bStopBlinking) {
			var ratio = (Time.time - lastColorChangeTime) / FadeDuration;
			ratio = Mathf.Clamp01 (ratio);
			material.color = Color.Lerp (startColor, endColor, ratio);

			if (ratio == 1f) {
				lastColorChangeTime = Time.time;

				// Switch colors
				var temp = startColor;
				startColor = endColor;
				endColor = temp;
			}
		} else {
			if (material.color != startColor)
				material.color = startColor;
		}
	}

	public void AddColor(Color toAdd){
		addedColors++;

		startColor.r = (startColor.r + toAdd.r) / addedColors;
		startColor.g = (startColor.g + toAdd.g) / addedColors;
		startColor.b = (startColor.b + toAdd.b) / addedColors;

		endColor.r = (endColor.r + toAdd.r) / addedColors;
		endColor.g = (endColor.g + toAdd.g) / addedColors;
		endColor.b = (endColor.b + toAdd.b) / addedColors;

		//In mobile the treshold is much lower
		#if UNITY_EDITOR || UNITY_STANDALONE
			//if (startColor == endColor)
			//	bColorOverflowing = true;
		#endif
	}

	public void ChangeColors(Color starting, Color ending){
		startColor = starting;
		endColor = ending;
	}
}
