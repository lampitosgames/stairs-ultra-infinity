using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderPlatform : MonoBehaviour {
    public GameObject startPoint;
    public GameObject endPoint;

    List<GameObject> points;

    Platform myPlatform;

    public float travelSpeed;
    public float platformRadiusDetection;
    public float pointRadiusDetection;

    int currentPoint;

    bool foward;
    bool currentlyMoving;

    // Use this for initialization
    void Start () {
        foward = true;
        currentlyMoving = false;
        myPlatform = this.GetComponent<Platform>();

        for (int i = 1; i < this.transform.childCount; i++)
        {
            points.Add(this.transform.GetChild(i).gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (myPlatform.active || currentlyMoving)
        {
            Vector3 dir = Vector3.zero;
            Vector3 distAway = Vector3.zero;
            if (foward)
            {
                dir = endPoint.transform.position - startPoint.transform.position;
                distAway = endPoint.transform.position - transform.position;
            }
            else
            {
                dir = startPoint.transform.position - endPoint.transform.position;
                distAway = startPoint.transform.position - transform.position;
            }

            //if (foward)
            //{
            //    dir = points[currentPoint + 1].transform.position - points[currentPoint].transform.position;
            //    distAway = points[currentPoint + 1].transform.position - this.transform.position;
            //}
            //else
            //{
            //    dir = points[currentPoint].transform.position - points[currentPoint + 1].transform.position;
            //    distAway = points[currentPoint].transform.position - transform.position;
            //}

            dir.Normalize();
            transform.position += (dir * travelSpeed) * Time.deltaTime;

            if (distAway.sqrMagnitude <= (Mathf.Pow(platformRadiusDetection, 2.0f) + Mathf.Pow(pointRadiusDetection, 2.0f)))
            {
                foward = foward ? false : true;
                if (!myPlatform.active)
                {
                    currentlyMoving = false;
                }
            }
            else
            {
                currentlyMoving = true;
            }
        }
    }
}
