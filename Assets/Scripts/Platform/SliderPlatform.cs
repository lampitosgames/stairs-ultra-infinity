using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderPlatform : MonoBehaviour {
    public GameObject movePoints;

    public List<GameObject> points;

    Platform myPlatform;

    public float travelSpeed;
    public float platformRadiusDetection;
    public float pointRadiusDetection;

    public bool loop;
    public bool continous;


    int currentPoint;
    int nextPoint;
    bool currentlyMoving;
    bool foward;

    // Use this for initialization
    void Start () {
        foward = true;
        currentlyMoving = false;
        myPlatform = this.GetComponent<Platform>();

        currentPoint = 0;
        nextPoint = 1;

        for (int i = 0; i < movePoints.transform.childCount; i++)
        {
            points.Add(movePoints.transform.GetChild(i).gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if ((myPlatform.active || currentlyMoving) || continous)
        {
            Vector3 dir = Vector3.zero;
            Vector3 distAway = Vector3.zero;

            dir = points[nextPoint].transform.position - points[currentPoint].transform.position;
            distAway = points[nextPoint].transform.position - this.transform.position;

            dir.Normalize();
            transform.position += (dir * travelSpeed) * Time.deltaTime;

            if (distAway.sqrMagnitude <= (Mathf.Pow(platformRadiusDetection, 2.0f) + Mathf.Pow(pointRadiusDetection, 2.0f)))
            {
                if (foward)
                {
                    currentPoint++;
                    nextPoint++;

                    if (currentPoint >= movePoints.transform.childCount - 1)
                    {
                        if (loop)
                        {
                            if (currentPoint == movePoints.transform.childCount)
                            {
                                currentPoint = 0;
                            }
                            else
                            {
                                nextPoint = 0;
                            }
                        }
                        else
                        {
                            foward = foward ? false : true;
                            nextPoint = currentPoint - 1;
                        }
                    }
                    if (!myPlatform.active)
                    {
                        currentlyMoving = false;
                    }
                }
                else
                {
                    currentPoint--;
                    nextPoint--;

                    if (currentPoint <= 0)
                    {
                        if (loop)
                        {   
                            if(currentPoint == -1)
                            {
                                currentPoint = movePoints.transform.childCount - 1;
                            }
                            else
                            {
                                nextPoint = movePoints.transform.childCount - 1;
                            }
                        }
                        else
                        {
                            foward = foward ? false : true;
                            nextPoint = currentPoint + 1;
                        }
                    }
                    if (!myPlatform.active)
                    {
                        currentlyMoving = false;
                    }
                }  
            }
            else
            {
                currentlyMoving = true;
            }
        }
    }
}
