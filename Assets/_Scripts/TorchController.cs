﻿using UnityEngine;
using System.Collections;

public class TorchController : MonoBehaviour {
	
	public string waveFunction  = "sin"; // possible values: sin, tri(angle), sqr(square), saw(tooth), inv(verted sawtooth), noise (random)
	public float bas = 0f; // start
	public float amplitude = 1.0f; // amplitude of the wave
	public float phase = 0.0f; // start point inside on wave cycle
	public float frequency  = 0.5f; // cycle frequency per second

	Color originalColor;

	// Use this for initialization
	void Start () {
		originalColor = GetComponent<Light> ().color;
	}
	
	// Update is called once per frame
	void Update () {
		Light light = GetComponent<Light>();
		light.color = originalColor * (EvalWave());
	}

	float EvalWave () {
		float x  = (Time.time + phase)*frequency;
		float y;
		
		x = x - Mathf.Floor(x); // normalized value (0..1)
		
		if (waveFunction=="sin") {
			y = Mathf.Sin(x*2*Mathf.PI);
		}
		else if (waveFunction=="tri") {
			if (x < 0.5f)
				y = 4.0f * x - 1.0f;
			else
				y = -4.0f * x + 3.0f;  
		}    
		else if (waveFunction=="sqr") {
			if (x < 0.5f)
				y = 1.0f;
			else
				y = 0f;  
		}    
		else if (waveFunction=="saw") {
			y = x;
		}    
		else if (waveFunction=="inv") {
			y = 1.0f - x;
		}    
		else if (waveFunction=="noise") {
			y = 1 - (Random.value*2);
		}
		else {
			y = 1.0f;
		}        
		return (y*amplitude)+bas;     
	}
}
