using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptAsh : MonoBehaviour
{
    Vector3 gogo;


    // Start is called before the first frame update
    void Start()
    {
        gogo.x = .01f;
        gogo.y = 0f;
        gogo.z = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += gogo;
    }
}
