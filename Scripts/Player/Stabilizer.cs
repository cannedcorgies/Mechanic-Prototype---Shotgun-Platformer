////    CREDITS ////
//
//  ruzhim on stack overflow
//      - https://stackoverflow.com/questions/58419942/stabilize-hovercraft-rigidbody-upright-using-torque
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabilizer : MonoBehaviour {

    public bool activated;
    
    private Rigidbody rb;

    public float dampenFactor = 1f;         // for shakiness in balancing
    public float adjustFactor = 0.7f;       // for how upright it maintains

    void Start() {

        rb = gameObject.GetComponent<Rigidbody>();

    }

    void FixedUpdate() {    
        Quaternion deltaQuat = Quaternion.FromToRotation(rb.transform.up, Vector3.up);

        Vector3 axis;
        float angle;
        deltaQuat.ToAngleAxis(out angle, out axis);
        
        rb.AddTorque(-rb.angularVelocity * dampenFactor, ForceMode.Acceleration);
        
        rb.AddTorque(axis.normalized * angle * adjustFactor, ForceMode.Acceleration);
    }
    
}
