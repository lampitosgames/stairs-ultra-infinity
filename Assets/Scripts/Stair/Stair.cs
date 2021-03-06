﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair : MonoBehaviour {
	public static float stairSlopeDeg = 35f;
	public static float stairLength = 8f;
	public static float stairHeight = 1f;

	public Direction dir;
	public SRotation rot;

	public StairBounds bounds;

	// Use this for initialization
	void Start() {
		bounds = new StairBounds(gameObject);
	}
	
	// Update is called once per frame
	void Update() {
		dir = bounds.Dir;
		rot = bounds.Rot;
	}

	//Snap to the nearest platform
	public void SnapToNearest() {

	}
}
