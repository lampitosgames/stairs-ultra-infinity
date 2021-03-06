﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used both in-game and in the editor for positioning objects
public class PlatformBounds {
	private Collider col;
	private Transform trans;

	private PlatformStairCollider[] stairColliders;
	private PlatformPlayerCollider playerCollider;

	public int sides;

	public Vector3 Center {
		get { return trans.position; }
		set { trans.position = value; }
	}

	public Vector3 HalfWidth {
		get { return col.bounds.extents; }
		set {
			XSize = value.x * 2f;
			YSize = value.y * 2f;
			ZSize = value.z * 2f;
		}
	}

	public Vector3 Size {
		get { return col.bounds.size; }
		set {
			XSize = value.x;
			YSize = value.y;
			ZSize = value.z;
		}
	}

	public float XSize {
		get { return col.bounds.size.x; }
		set {
			//We need to change the local scale of the object to match the global desired size
			//Unity doesn't support modifying global scale directly
			float scaleRatio = XSize / trans.localScale.x;
			trans.localScale = new Vector3(value / scaleRatio, trans.localScale.y, value / scaleRatio);
		}
	}

	public float YSize {
		get { return col.bounds.size.y; }
		set {
			float scaleRatio = YSize / trans.localScale.y;
			trans.localScale = new Vector3(trans.localScale.x, value / scaleRatio, trans.localScale.z);
		}
	}

	public float ZSize {
		get { return col.bounds.size.z; }
		set {
			float scaleRatio = ZSize / trans.localScale.z;
			trans.localScale = new Vector3(value / scaleRatio, trans.localScale.y, value / scaleRatio);
		}
	}

	public Vector3 MinPos {
		get { return Center - HalfWidth; }
		set { Center = value + HalfWidth; }
	}

	public Vector3 MaxPos {
		get { return Center + HalfWidth; }
		set { Center = value - HalfWidth; }
	}

	public float XMin {
		get { return Center.x - HalfWidth.x; }
		set { Center = new Vector3(value + HalfWidth.x, Center.y, Center.z); }
	}

	public float XMax {
		get { return Center.x + HalfWidth.x; }
		set { Center = new Vector3(value - HalfWidth.x, Center.y, Center.z); }
	}

	public float YMin {
		get { return Center.y - HalfWidth.y; }
		set { Center = new Vector3(Center.x, value + HalfWidth.y, Center.z); }
	}

	public float YMax {
		get { return Center.x + HalfWidth.x; }
		set { Center = new Vector3(Center.x, value - HalfWidth.y, Center.z); }
	}

	public float ZMin {
		get { return Center.z - HalfWidth.z; }
		set { Center = new Vector3(Center.x, Center.y, value + HalfWidth.z); }
	}

	public float ZMax {
		get { return Center.z + HalfWidth.z; }
		set { Center = new Vector3(Center.x, Center.y, value - HalfWidth.z); }
	}

	public PlatformBounds(GameObject platform, int _sides) {
		this.sides = _sides;
		this.trans = platform.GetComponent<Transform>();
		//Get the child collider by looping to look for the platform's mesh object
		for (int i = 0; i < trans.childCount; i++) {
			Transform child = trans.GetChild(i);
			if (child.tag == "platformMesh") {
				col = child.GetComponent<Collider>();
				break;
			}
		}

		stairColliders = trans.GetComponentsInChildren<PlatformStairCollider>();
		playerCollider = trans.GetComponentInChildren<PlatformPlayerCollider>();
	}

	public void RepositionChildren(Vector3 newLocalScale) {
		
		playerCollider.transform.localScale = new Vector3(newLocalScale.x, newLocalScale.x * 0.5f, newLocalScale.z);
		playerCollider.transform.position = new Vector3(Center.x,
		                                                Center.y + newLocalScale.y * 0.5f + newLocalScale.x * 0.25f,
		                                                Center.z);
		for (int i = 0; i < stairColliders.Length; i++) {
			Transform colTrans = stairColliders[i].transform;
			switch (stairColliders[i].dir) {
				case Direction.NORTH:
					colTrans.position = new Vector3(Center.x, Center.y, ZMax + 0.5f);
					break;
				case Direction.SOUTH:
					colTrans.position = new Vector3(Center.x, Center.y, ZMin - 0.5f);
					break;
				case Direction.EAST:
					colTrans.position = new Vector3(XMax + 0.5f, Center.y, Center.z);
					break;
				case Direction.WEST:
					colTrans.position = new Vector3(XMin - 0.5f, Center.y, Center.z);
					break;
			}
		}
	}
}
