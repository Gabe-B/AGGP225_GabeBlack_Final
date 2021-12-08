using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3[] points;
    public int pointNum = 0;
    public float tolerance, speed, delayTime;
    public bool auto;

    float delayStart;
    Vector3 currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        if(points.Length >0)
		{
            currentTarget = points[0];
		}

        tolerance = speed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position != currentTarget)
		{
            MovePlatform();
		}
        else
		{
            UpdateTarget();
		}
    }

    void MovePlatform()
	{
        Vector3 heading = currentTarget - gameObject.transform.position;
        gameObject.transform.position += ((heading / heading.magnitude) * speed * Time.deltaTime);

        if(heading.magnitude < tolerance)
		{
            gameObject.transform.position = currentTarget;
            delayStart = Time.time;
		}
	}

    void UpdateTarget()
	{
        if(auto)
		{
            if(delayTime < Time.time - delayStart)
			{
                NextPlatform();
			}
		}
	}

    public void NextPlatform()
	{
        pointNum++;

        if(pointNum >= points.Length)
		{
            pointNum = 0;
		}

        currentTarget = points[pointNum];
	}
}
