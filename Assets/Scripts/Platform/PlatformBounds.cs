using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used both in-game and in the editor for positioning objects
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
            ZSize = value.z * 2;
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
        this.trans = platform.GetComponent<Transform>();
        //Get the child collider by looping to look for the platform's mesh object
        for (int i = 0; i < trans.childCount; i++) {
            Transform child = trans.GetChild(i);
            if (child.tag == "platformMesh") {
                col = child.GetComponent<Collider>();
                break;
            }
        }
    }
}
