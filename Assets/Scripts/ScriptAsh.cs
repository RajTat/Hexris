using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptAsh : MonoBehaviour
{
    Vector3 gogo;


    // Start is called before the first frame update
    void Start()
    {
        gogo.x = -.0040f;
        gogo.y = -.0050f;
        gogo.z = 0.0000f;
    }

    // Update is called once per frame
    // Bounces around
    void Update()
    {
        this.transform.position += gogo;
        Vector3 me = this.transform.position;

        //left and right edges
        if (me.x < -5.5426 + .55426 || me.x > 5.5426 - .55426)
        {
            gogo.x *= -1;
            return; //mutually exclusive to a left or right bound bounce
        }

        //find the angle bounds
        float boundY = 6.4f - .64f - me.y;
        float boundX = 0f - me.x;
        float deg = Mathf.Atan(boundY / boundX) * Mathf.Rad2Deg;

        //positive atan deg is actually 180 degrees off if y is negative
        if (deg > 0 && me.y < 6.4f - .64f) deg += 180;
        //negative atan deg is actually 180 degrees off if y is positive
        else if (deg < 0 && me.y > 6.4f - .64f) deg += 180;
        while (deg < 0) deg += 360;

        //hyp
        float v = Mathf.Sqrt((gogo.x * gogo.x) + (gogo.y * gogo.y));

        //if the degrees from [0,64] to ball is greater than -30
        if (deg > 330 && deg < 360)
        {
            //un-collide
            backUp();

            //reuse the deg var - switch to keeping track of the degrees of the ball's motion
            deg = Mathf.Atan(gogo.y / gogo.x) * Mathf.Rad2Deg;
            if (deg > 0 && gogo.y < 0) deg += 180;
            else if (deg < 0 && gogo.y > 0) deg += 180;

            //reverse direction
            deg += 180;

            //keep degrees between 0-360 deg
            while (deg < 0) deg += 360;
            while (deg > 360) deg -= 360;

            //mirror around 240 deg
            deg = 240 + (240 - deg);

            //new direction
            deg = deg * Mathf.Deg2Rad;
            gogo.x = Mathf.Cos(deg) * v;
            gogo.y = Mathf.Sin(deg) * v;

        }
        //if the degrees from [0,64] to ball is less than 210 deg
        else if (deg < 210)
        {
            backUp();

            deg = Mathf.Atan(gogo.y / gogo.x) * Mathf.Rad2Deg;
            if (deg > 0 && gogo.y < 0) deg += 180;
            else if (deg < 0 && gogo.y > 0) deg += 180;

            //reverse direction
            deg += 180;

            //keep degrees between 0-360 deg
            while (deg < 0) deg += 360;
            while (deg > 360) deg -= 360;

            //mirror around 300 deg
            deg = 300 + (300 - deg);

            //new direction
            deg = deg * Mathf.Deg2Rad;
            gogo.x = Mathf.Cos(deg) * v;
            gogo.y = Mathf.Sin(deg) * v;
        }
        //next check the bounds from point [0,-6.4]
        else
        {
            boundY = -6.4f + .64f - me.y;
            boundX = 0f - me.x;
            deg = Mathf.Atan(boundY / boundX) * Mathf.Rad2Deg;

            //adjust to represent 1-360 degrees
            if (deg > 0 && me.y < -6.4f + .64f) deg += 180;
            else if (deg < 0 && me.y > -6.4f + .64f) deg += 180;
            while (deg < 0) deg += 360;

            if (deg > 150)
            {
                //puts the object back where it was
                backUp();

                //find the degrees of the angle of motion
                //converting everything to actual degrees for debugging purpose
                deg = Mathf.Atan(gogo.y / gogo.x) * Mathf.Rad2Deg;

                //reverse direction
                //Atan could be 180 deg off if x and y were both negative
                deg += gogo.y < 0 && gogo.x < 0 ? 0 : 180;

                //keep degrees between 0-360
                while (deg < 0) deg += 360;
                while (deg > 360) deg -= 360;

                //mirror around 60 deg
                deg = 60 + (60 - deg);

                //new direction
                deg = deg * Mathf.Deg2Rad; //convert back to Radians for Trig functions
                gogo.x = Mathf.Cos(deg) * v;
                gogo.y = Mathf.Sin(deg) * v;
            }
            else if (deg < 30)
            {
                backUp();

                deg = Mathf.Atan(gogo.y / gogo.x) * Mathf.Rad2Deg;
                if (deg > 0 && gogo.y < 0) deg += 180;
                else if (deg < 0 && gogo.y > 0) deg += 180;

                //reverse direction
                deg += 180;

                //keep degrees between 0-360
                while (deg < 0) deg += 360;
                while (deg > 360) deg -= 360;

                //mirror around 120 deg
                deg = 120 + (120 - deg);

                //new direction
                deg = deg * Mathf.Deg2Rad; //convert back to Radians for Trig function
                gogo.x = Mathf.Cos(deg) * v;
                gogo.y = Mathf.Sin(deg) * v;
            }
        }
    } //end of void Update()

    //need to replace with a function that places the object with perfect collision
    void backUp()
    {
        Vector3 pos = this.transform.position;
        pos.x -= gogo.x;
        pos.y -= gogo.y;
        this.transform.position = pos;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject wall = col.gameObject;
		if (wall.tag != "myblock") return;

		//extract x and y coordinates for easy reference
        float wallX = wall.transform.position.x;
        float wallY = wall.transform.position.y;

        float myX   = this.transform.position.x;
        float myY   = this.transform.position.y;

        //represents the vector between the colliders
        float nX = (myX - wallX);
        float nY = (myY - wallY);

        //velocity
        float v = Mathf.Sqrt((gogo.x * gogo.x) + (gogo.y * gogo.y));

        //deg is the original angle; dd is the collision angle
        float deg = Mathf.Atan(gogo.y / gogo.x);
        float dd  = Mathf.Atan(    nY / nX    );

		//distinguish between x/y and -x/-y because Atan doesnt
		if (gogo.x < 0) deg += 3.14159f;
        if (    nX < 0)  dd += 3.14159f;
		deg = 3.14159f + deg; //reverse direction
		deg = fixRadians(deg);
		dd  = fixRadians(dd);

        //absolute values used for +/- directions
        //always +/- 1
        //float absX = gogo.x == 0 ? 1f : Mathf.Abs(gogo.x)/gogo.x;
        //float absY = gogo.y == 0 ? 1f : Mathf.Abs(gogo.y)/gogo.y; 

        //reposition outside the collision
        Vector3 newV;
        newV.x = wallX + (Mathf.Cos(dd) * .64f);
        newV.y = wallY + (Mathf.Sin(dd) * .64f);
        newV.z = 0f;
        this.transform.position = newV;

        //new angle is twice the difference between the collision angle
        //it's twice because once to equal it, and once more to reflect to the other side
        deg -= 2 * (deg - dd);

        //new direction
        gogo.x = Mathf.Cos(deg) * v;
        gogo.y = Mathf.Sin(deg) * v;

		GameObject.Find("Main Controller").GetComponent<MainController>().getAutograph(wall);

    } // end of oncollicionenter2d

	//keep deg between 0-2pi r
	private static float fixRadians(float r) {
		while (r < 0) r += 3.14159f * 2;
		while (r >= 3.14159f * 2) r -= 3.14159f * 2;
		return r;
	}
}
