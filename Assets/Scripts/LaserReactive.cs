using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReactive : MonoBehaviour {
    LineRenderer line;

    [SerializeField]
    bool isHit = false;

    [SerializeField]
    float Watts = 0;

    // Use this for initialization
    void Start () {
        line = gameObject.GetComponent<LineRenderer>();
        line.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {
        if (isHit)
        {
            StopCoroutine("FireLaser");
            StartCoroutine("FireLaser");
        }
    }

    public void WasHit(float a)
    {
        isHit = true;
        Watts = a;
    }

    IEnumerator FireLaser()
    {
        line.enabled = true;
        while (isHit)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            line.SetPosition(0, ray.origin);
            if (Physics.Raycast(ray, out hit, 100))
            {
                line.SetPosition(1, hit.point);
                if (hit.collider)
                {
                    hit.collider.GetComponentInChildren<LaserReactive>().WasHit(Watts);
                }
            }
            else
            {
                line.SetPosition(1, ray.GetPoint(100));
            }
            yield return null;
        }

        line.enabled = false;
    }

    void Selected()
    {
        
    }
}
