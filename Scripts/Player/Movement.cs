using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public bool activated;

    private Rigidbody rb;
    private Stabilizer stab;

    public LayerMask ground;
    public bool onGround;
    public float collisionRadius = 0.25f;
    public Vector3 bottomOffset;

    public bool jumped = false;
    public float jumpPower;
    public float jumpDistance_scale;

    [SerializeField] private bool sprinting = false;
    [SerializeField] private bool canSprint = true;
    [SerializeField] private float stamina = 100f;
    public float stamina_max = 100f;
    public float sprint_coolDownTime = 10f;

    [SerializeField] private float dir;
    [SerializeField] private float speed;
    [SerializeField] private float speed_max;

    public float groundSpeed_def_max;
    public float groundSpeed_def;
    public float groundDampen_def = 1;
    public float groundAdjust_def = 3;

    public float groundSpeed_sprint_max;
    public float groundSpeed_sprint;
    public float groundDampen_sprint;
    public float groundAdjust_sprint;

    public float groundSpeed_tired_max;
    public float groundSpeed_tired;
    public float groundDampen_tired;
    public float groundAdjust_tired;
 

    // Start is called before the first frame update
    void Start()
    {

        name = gameObject.name;

        Debug.Log("=Hello from " + name);
        Debug.Log(" - my default ground speed is " + groundSpeed_def);
        Debug.Log("     - my MAX default ground speed is " + groundSpeed_def_max);

        rb = gameObject.GetComponent<Rigidbody>();
        stab = gameObject.GetComponent<Stabilizer>();

        speed = groundSpeed_def;
        speed_max = groundSpeed_def_max;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        onGround = GroundCheck();

    //// MOVEMENT - do "if grounded"
        if (onGround) {

            var dir = Input.GetAxis("Horizontal") * speed;     // get the direction of movement
            rb.AddForce(transform.right * dir);                      // move in that direction
        
        }

    //// JUMP
        if (Input.GetAxis("Vertical") > 0 && onGround) {

            Debug.Log("JUMPED");

            rb.AddForce(transform.up * jumpPower);
            rb.AddForce(transform.right * dir * jumpDistance_scale);

        }

    //// VELOCITY CAP
        if(rb.velocity.magnitude > speed_max)
        {
               rb.velocity = rb.velocity.normalized * speed_max;
        }
    
    //// IF NOT TIRED
        if (canSprint) {

        //// sprint activation
            if (Input.GetKey("space") && stamina > 0) {

                sprinting = true;

            } else { sprinting = false; }

            //// set sprint values
            if (sprinting) {

                ChangeValues(groundSpeed_sprint, groundSpeed_sprint_max, groundDampen_sprint, groundAdjust_sprint);

            } else {

                ChangeValues(groundSpeed_def, groundSpeed_def_max, groundDampen_def, groundAdjust_def);

            }

        //// stamina logic
            if (dir != 0 && sprinting && stamina > 0) {

                Debug.Log("draining...");
                stamina -= 0.5f;

            } else if (stamina < stamina_max) { stamina += 1f; Debug.Log("recovering..."); }

            //// stamina depleted

            if (stamina <= 0) {

                StartCoroutine(SprintCooldown());

            }

        }
        
    }

    bool GroundCheck() {

        if (Physics.OverlapSphere(transform.position + bottomOffset, collisionRadius, ground).Length > 0) {
            return true;
        } else {
            return false;
        }

    }

    void ChangeValues(float speedSet, float maxSpeed, float dampenFactor, float adjustFactor) {

        speed = speedSet;
        speed_max = maxSpeed;
        stab.dampenFactor = dampenFactor;
        stab.adjustFactor = adjustFactor;

    }

    IEnumerator SprintCooldown() {

        canSprint = false;
        ChangeValues(groundSpeed_tired, groundSpeed_tired_max, groundDampen_tired, groundAdjust_tired);
        Debug.Log("tired :(");

        yield return new WaitForSeconds(sprint_coolDownTime);

        canSprint = true;
        stamina = stamina_max;
        Debug.Log(" - back in the game :)");

    }
}
