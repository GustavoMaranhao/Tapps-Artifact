using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptObjectFade : MonoBehaviour {
	[HideInInspector]
	public bool bStartDissolving = false;
	[HideInInspector]
	public bool bFadeOut = true;

	[SerializeField] 
	private float fadePerSecond = 2.5f;

	private Material material;
	private int cont = 0;

	void Start() {
		material = GetComponent<Renderer>().material;
	}

	private void Update() {
		if (bStartDissolving) {
			material.color = new Color (material.color.r, material.color.g, material.color.b, material.color.a - (fadePerSecond * Time.deltaTime));

			cont++;
			if (bFadeOut && ((material.color.a <= 0) || (cont >= 100))) {
				gameObject.GetComponent<Collider>().enabled = false;
				bStartDissolving = false;
			}
		}
	}
}
