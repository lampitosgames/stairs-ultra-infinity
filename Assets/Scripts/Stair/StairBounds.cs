﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SRotation {
	UP = 0,
	RIGHT = 1,
	DOWN = 2,
	LEFT = 3
}

public class StairBounds {
	private Collider col;
	private Transform trans;


	//Orientation
	private Direction dir;
	private SRotation rot;

	public Direction Dir { get { return dir; } }

	public SRotation Rot { get { return rot; } }

	public Vector3 Center {
		get { return trans.position; }
		set { trans.position = value; }
	}

	public Vector3 HalfWidth {
		get { return Size / 2f; }
		set {
			XSize = value.x * 2f;
			YSize = value.y * 2f;
			ZSize = value.z * 2f;
		}
	}

	public Vector3 Size {
		get { return getSize(); }
		set { setSize(value); }
	}

	public float XSize {
		get { return getSize().x; }
		set { setSize(new Vector3(value, YSize, ZSize)); }
	}

	public float YSize {
		get { return getSize().y; }
		set { setSize(new Vector3(XSize, value, ZSize)); }
	}

	public float ZSize {
		get { return getSize().z; }
		set { setSize(new Vector3(XSize, YSize, value)); }
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

	public StairBounds(GameObject stair) {
		this.trans = stair.GetComponent<Transform>();
		this.col = stair.GetComponent<Collider>();
	}

	public void RoundRotation() {
		//Get euler angles

	}

	public void SetDirection(Direction newDir) {
		//Set the new direction
		dir = newDir;
		if (newDir == Direction.NORTH) {
			trans.eulerAngles = new Vector3(0f, 0f, 0f);
		} else if (newDir == Direction.EAST) {
			trans.eulerAngles = new Vector3(0f, 90f, 0f);
		} else if (newDir == Direction.SOUTH) {
			trans.eulerAngles = new Vector3(0f, 180f, 0f);
		} else if (newDir == Direction.WEST) {
			trans.eulerAngles = new Vector3(0f, 270f, 0f);
		}
		//Set the rotation
		SetRotation(Rot);
	}

	public void SetRotation(SRotation newRot) {
		rot = newRot;
		//Store the y rotation
		float yRot = 0f;
		yRot += (dir == Direction.EAST) ? 90f : 0f;
		yRot += (dir == Direction.SOUTH) ? 180f : 0f;
		yRot += (dir == Direction.WEST) ? 270f : 0f;

		//Switch based on the new target rotation
		switch (newRot) {
			case SRotation.UP:
				trans.eulerAngles = new Vector3(Stair.stairSlopeDeg, yRot, 0f);
				break;
			case SRotation.DOWN:
				trans.eulerAngles = new Vector3(360f - Stair.stairSlopeDeg, yRot, 0f);
				break;
			case SRotation.RIGHT:
				trans.eulerAngles = new Vector3(0f, yRot + Stair.stairSlopeDeg, 0f);
				break;
			case SRotation.LEFT:
				yRot = (yRot < 1f) ? 360f - Stair.stairSlopeDeg : yRot - Stair.stairSlopeDeg;
				trans.eulerAngles = new Vector3(0f, yRot, 0f);
				break;
		}
	}

	private Vector3 getSize() {
		/* The stair's bounds are tricky since Unity only gives us the AABB bounding volume.
         * To get the stair's size, we need to un-rotate the stair */
		//store old rotation
		Quaternion storedRotation = trans.rotation;
		//Set rotation to identity
		trans.rotation = Quaternion.identity;

		//Get size
		Vector3 stairSize = col.bounds.size;

		//Revert to stored rotation
		trans.rotation = storedRotation;

		return stairSize;
	}

	private void setSize(Vector3 _size) {
		/* The stair's bounds are tricky since Unity only gives us the AABB bounding volume.
         * To get the stair's size, we need to un-rotate the stair */
		//store old rotation
		Quaternion storedRotation = trans.rotation;
		//Set rotation to identity
		trans.rotation = Quaternion.identity;

		//We need to change the local scale of the object to match the global desired size
		//Unity doesn't support modifying global scale directly
		float xScaleRatio = XSize / trans.localScale.x;
		float yScaleRatio = YSize / trans.localScale.y;
		float zScaleRatio = ZSize / trans.localScale.z;
		trans.localScale = new Vector3(_size.x / xScaleRatio, _size.y / yScaleRatio, _size.z / zScaleRatio);

		//Revert to stored rotation
		trans.rotation = storedRotation;
	}
}
