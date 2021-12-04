using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptAsh : MonoBehaviour
{
    Vector3 gogo;


    // Start is called before the first frame update
    void Start()
    {
        gogo.x = -.005f;
        gogo.y = 0.01f;
        gogo.z = 0f;
    }

    // Update is called once per frame
    // Bounces around
    void Update()
    {
        this.transform.position += gogo;
        Vector3 me = this.transform.position;
        float deg = Mathf.Atan( (6.4f - me.y) / (0f - me.x) ) * Mathf.Rad2Deg;
        

        //left and right edges
        if (me.x < -5.5426+.55426 || me.x > 5.5426-.55426) gogo.x *= -1;
        
        else
        {
            //hyp
            float v = Mathf.Sqrt((gogo.x * gogo.x) + (gogo.y * gogo.y));

            //if the degrees from [0,64] to ball are less than 30 or greater than 30
            if (deg > -30 && deg < 0)
            {
                deg = Mathf.Atan(gogo.y / gogo.x) * Mathf.Rad2Deg;
                deg = 60 + (60-deg);

                //new direction
                deg = (180 + deg) * Mathf.Deg2Rad;
                gogo.x = Mathf.Cos(deg) * v;
                gogo.y = Mathf.Sin(deg) * v;

            }
            else if (deg < 30 && deg > 0)
            {
                deg = Mathf.Atan(gogo.y / gogo.x) * Mathf.Rad2Deg;
                deg = -60 + (-60 - deg);

                deg = deg * Mathf.Deg2Rad;
                gogo.x = Mathf.Cos(deg) * v;
                gogo.y = Mathf.Sin(deg) * v;
            }
            else
            {
                deg = Mathf.Atan( (-6.4f - me.y) / (0f - me.x) ) * Mathf.Rad2Deg;
                if (deg > -30 && deg <= 0)
                {
                    deg = Mathf.Atan(gogo.y / gogo.x) * Mathf.Rad2Deg;
                    deg = 60 + (60 - deg);

                    //new direction
                    deg = deg * Mathf.Deg2Rad;
                    gogo.x = Mathf.Cos(deg) * v;
                    gogo.y = Mathf.Sin(deg) * v;
                }
                else if (deg < 30 && deg >= 0)
                {
                    deg = Mathf.Atan(gogo.y / gogo.x) * Mathf.Rad2Deg;
                    deg = -60 + (-60 + deg);

                    //new direction
                    deg = (180 + deg) * Mathf.Deg2Rad;
                    gogo.x = Mathf.Cos(deg) * v;
                    gogo.y = Mathf.Sin(deg) * v;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject wall = col.gameObject;
        gogo.x *= -1;

        float wallX = wall.transform.position.x;
        float wallY = wall.transform.position.y;

        float myX = this.transform.position.x;
        float myY = this.transform.position.y;

        //represents the vector between the colliders
        float nX = (wallX - myX);
        float nY = (wallY - myY);

        //velocity
        float v = Mathf.Sqrt((gogo.x * gogo.x) + (gogo.y * gogo.y));

        //deg is the original angle; dd is the collision angle
        float deg = -1 * (Mathf.Atan(gogo.y / gogo.x));
        float dd = Mathf.Atan(nY / nX);

        //absolute values used for +/- directions
        float absX = Mathf.Abs(gogo.x)/gogo.x;
        float absY = Mathf.Abs(gogo.y)/gogo.y;

        //reposition outside the collision
        Vector3 newV;
        newV.x = (float)(wallX + (Mathf.Cos(dd) * .64 * absX));
        newV.y = (float)(wallY + (Mathf.Sin(dd) * .64 * absY));
        newV.z = 0f;
        this.transform.position = newV;

        //new angle is twice the difference between the collision angle
        //it's twice because once to equal it, and once more to reflect to the other side
        deg -= 2 * (deg - dd);


        //new direction
        gogo.x = Mathf.Cos(deg) * v * absX;
        gogo.y = Mathf.Sin(deg) * v * absY;
        Debug.Log("gogo " + gogo.x + "," + gogo.y);

    }
}
