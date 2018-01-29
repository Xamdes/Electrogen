//https://www.youtube.com/watch?time_continue=1287&v=1i0da_Pg5Jc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField]
    GameController control;

    [SerializeField]
    LaserController laserHitOne;

    [SerializeField]
    LaserController laserHitTwo;

    [SerializeField]
    LaserController[] lasersHit;

    [SerializeField]
    LineRenderer[] lines;

    [SerializeField]
    float suppliedWatts = 0.0f;

    [SerializeField]
    float transmitOutWatts = 0.0f;

    [SerializeField]
    float transmitInWattsOne = 0.0f;

    [SerializeField]
    float transmitInWattsTwo = 0.0f;

    [SerializeField]
    float requiredWatts = 0.0f;

    [SerializeField]
    float ampMultiplier = 1.0f;

    [SerializeField]
    bool isSelected = false;

    [SerializeField]
    bool isHit = false;

    [SerializeField]
    bool isHitOne = false;

    [SerializeField]
    bool isHitTwo = false;

    [SerializeField]
    bool laserOneHitOne = false;

    [SerializeField]
    bool start = false;

    [SerializeField]
    bool finish = false;

    [SerializeField]
    Material notSelected;

    [SerializeField]
    Material whileSelected;

    [SerializeField]
    Material whilePowered;

    [SerializeField]
    LaserType laserType;

    private IEnumerator coroutine;

    private int whatImI;

    [SerializeField]
    private ParticleSystem particles;


    // Use this for initialization
    void Start()
    {
        control = GetComponentInParent<GameController>();
        lines = GetComponentsInChildren<LineRenderer>();
        particles = GetComponentInChildren<ParticleSystem>();

        foreach (LineRenderer a in lines)
        {
            a.enabled = false;
        }

        switch (laserType)
        {
            case LaserType.begin:
                start = true;
                break;
            case LaserType.extend:
                ampMultiplier = 1.0f;
                break;
            case LaserType.split:
                ampMultiplier = 0.5f;
                break;
            case LaserType.finish:
                finish = true;
                break;
            default:
                break;
        }
        particles.Pause(true);
    }

    // Update is called once per frame
    void Update()
    {
        if ((isHitOne || isHitTwo) && !start)
        {
            isHit = true;
            if (transmitOutWatts > 0.0f)
            {
                particles.Play(true);
            }
        }
        else if (!start)
        {
            isHit = false;
            particles.Pause(true);
            particles.Clear(true);
        }
        if (start)
        {
            transmitOutWatts = suppliedWatts;
            particles.Play(true);
        }
        else
        {
            transmitOutWatts = (transmitInWattsOne + transmitInWattsTwo - requiredWatts + suppliedWatts) * ampMultiplier;
        }

        //Finish
        if (finish)
        {
            if (isHit)
            {
                if (transmitOutWatts >= 0.0f)
                {
                    particles.Play(true);
                    control.SetYouWin(true);
                }
                else
                {
                    particles.Pause(true);
                    particles.Clear(true);
                    control.SetYouWin(false);
                }
            }
            else
            {
                control.SetYouWin(false);
            }            
        }


        if (isSelected)
        {
            GetComponent<MeshRenderer>().material = whileSelected;
        }
        else
        {
            GetComponent<MeshRenderer>().material = notSelected;
        }

        //FireLaser
        bool forwardRight = true;
        foreach (LineRenderer a in lines)
        {
            coroutine = FireLaser(a, forwardRight);
            forwardRight = !forwardRight;
            if (transmitOutWatts > 0.0f && (start || isHit))
            {
                StopCoroutine(coroutine);
                StartCoroutine(coroutine);
                if (!isSelected && !start && !finish)
                {
                    GetComponent<MeshRenderer>().material = whilePowered;
                }
            }
            else
            {
                StopCoroutine(coroutine);
                a.enabled = false;
            }
        }
    }

    IEnumerator FireLaser(LineRenderer lineCR, bool forRit)
    {
        lineCR.enabled = true;
        while (transmitOutWatts > 0.0f && (start || isHit))
        {
            Ray ray;
            if (forRit)
            {
                ray = new Ray(transform.position, transform.forward);
            }
            else
            {
                ray = new Ray(transform.position, transform.right);
            }

            RaycastHit hit;

            lineCR.SetPosition(0, ray.origin);
            if (Physics.Raycast(ray, out hit, 100))
            {
                lineCR.SetPosition(1, hit.point);
                if (hit.collider)
                {
                    if (laserHitOne != null)
                    {
                        if (laserHitTwo != null)
                        {

                            laserHitTwo = laserHitTwo.DisableLaser(false);
                        }
                        laserHitTwo = laserHitOne;
                        laserHitOne = laserHitOne.DisableLaser(laserOneHitOne);
                    }
                    //Only care if laser is split
                    if (laserType == LaserType.split && laserHitTwo != null)
                    {
                        laserHitTwo.WasHit(false, transmitOutWatts);
                    }
                    laserHitOne = hit.collider.GetComponent<LaserController>();

                    bool which = true;
                    if (transmitInWattsOne == 0.0f && transmitInWattsTwo > 0.0f)
                    { which = false; }
                    laserOneHitOne = which;
                    laserHitOne.WasHit(which, transmitOutWatts);
                }
            }
            else
            {
                if (laserHitOne != null)
                {
                    laserHitTwo = laserHitOne;
                    laserHitOne = laserHitOne.DisableLaser(laserOneHitOne);
                }
                lineCR.SetPosition(1, ray.GetPoint(100));
            }
            yield return null;
        }

        if (laserHitOne != null)
        {
            laserHitOne = laserHitOne.DisableLaser(laserOneHitOne);
        }
        if (laserHitTwo != null)
        {
            laserHitTwo = laserHitTwo.DisableLaser(false);
        }
        lineCR.enabled = false;
    }

    public void Selected(bool b)
    {
        isSelected = b;
    }

    public void WasHit(bool which, float input)
    {
        if (which)
        {
            isHitOne = true;
            transmitInWattsOne = input;
        }
        else
        {
            isHitTwo = true;
            transmitInWattsTwo = input;
        }
    }

    enum LaserType
    {
        begin = 0,
        finish = 1,
        extend = 2,
        amp = 3,
        split = 4,
    }

    public LaserController DisableLaser(bool b)
    {
        if (b)
        {
            isHitOne = false;
            transmitInWattsOne = 0.0f;
        }
        else
        {
            isHitTwo = false;
            transmitInWattsTwo = 0.0f;
        }
        return null;
    }

    public float GetWatts()
    {
        return transmitOutWatts;
    }

    public float GetMultiplier()
    {
        return ampMultiplier;
    }

    public float GetRequiredWatts()
    {
        return requiredWatts;
    }

    public int GetWattBoxType()
    {
        return (int)laserType;
    }

    public void NullLasers()
    {
        if (laserHitOne != null)
        {
            laserHitOne = laserHitOne.DisableLaser(laserOneHitOne);
        }
        if (laserHitTwo != null)
        {
            laserHitTwo = laserHitTwo.DisableLaser(laserHitTwo);
        }
    }


}
