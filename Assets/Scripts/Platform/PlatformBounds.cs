using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBounds {
    private Collider col;
    private Transform trans;

    public int sides;

    public Vector3 Center {
        get { return trans.position; }
        set { trans.position = value; }
    }

    public Vector3 HalfWidth {
        get { return col.bounds.extents; }
        set {
            XSize = value.x * 2;
            YSize = value.y * 2;
            ZSize = value.x * 2;
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
            trans.localScale = new Vector3(value / scaleRatio, trans.localScale.y, trans.localScale.z);
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
            trans.localScale = new Vector3(trans.localScale.x, trans.localScale.y, value / scaleRatio);
        }
    }

    public PlatformBounds(GameObject platform, int _sides) {
        this.sides = _sides;
        this.col = platform.GetComponent<Collider>();
        this.trans = platform.GetComponent<Transform>();
    }

    //Returns the position and axis to pivot around
    public Vector3[] SidePivot(Direction dir) {
        Vector3[] posAndAxis = new Vector3[2];
        switch (dir) {
            case Direction.NORTH:
                posAndAxis[0] = Center + trans.forward * HalfWidth.z;
                posAndAxis[1] = trans.forward;
                break;
            case Direction.EAST:
                posAndAxis[0] = Center + trans.right * HalfWidth.x;
                posAndAxis[1] = trans.right;
                break;
            case Direction.SOUTH:
                posAndAxis[0] = Center - trans.forward * HalfWidth.z;
                posAndAxis[1] = -trans.forward;
                break;
            case Direction.WEST:
                posAndAxis[0] = Center - trans.right * HalfWidth.x;
                posAndAxis[1] = -trans.right;
                break;
            default:
                break;
        }
        return posAndAxis;
    }
}
