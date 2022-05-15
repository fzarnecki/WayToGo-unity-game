using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBouncer : MonoBehaviour
{
    // [SerializeField] private float minSpeed = 1f;
    private float maxSpeed = 2f;
    private float touchSpeedBoost = 2f;
    private float zSpeed = 0.4f;

    private float minRotation = 1f;
    private float maxRotation = 3f;
    private float touchRotationBoost = 2f;

    [SerializeField] private Rigidbody rgb;


    private float zUpperBound = 10f;
    private float zLowerBound = 0f;

    /***/

    void Start()
    {
        rgb.velocity = RandomVelocity();
        rgb.angularVelocity = RandomAngularVelocity();
    }
    
    void FixedUpdate()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x < -0.1)
        {
            // Debug.Log("I am left of the camera's view.");
            rgb.velocity = Vector3.Reflect(rgb.velocity, Vector3.right);
            rgb.angularVelocity = RandomAngularVelocity();
        }

        if (pos.x > 1.1)
        {
            // Debug.Log("I am right of the camera's view.");
            rgb.velocity = Vector3.Reflect(rgb.velocity, Vector3.left);
            rgb.angularVelocity = RandomAngularVelocity();
        }

        if (pos.y < -0.1)
        {
            // Debug.Log("I am below the camera's view.");
            rgb.velocity = Vector3.Reflect(rgb.velocity, Vector3.down);
            rgb.angularVelocity = RandomAngularVelocity();
        }

        if (pos.y > 1.1)
        {
            // Debug.Log("I am above the camera's view.");
            rgb.velocity = Vector3.Reflect(rgb.velocity, Vector3.up);
            rgb.angularVelocity = RandomAngularVelocity();
        }

        if (transform.position.z > zUpperBound)
        {
            // Debug.Log("Upper z bound." + transform.name);
            rgb.velocity = Vector3.Reflect(rgb.velocity, Vector3.forward);
        }

        if (transform.position.z < zLowerBound)
        {
            // Debug.Log("Lower z bound." + transform.name);
            rgb.velocity = Vector3.Reflect(rgb.velocity, Vector3.back);
        }
    }

    private Vector3 RandomVelocity() {
        var vel = new Vector3(Random.Range(-maxSpeed, maxSpeed), Random.Range(-maxSpeed, maxSpeed), Random.Range(-zSpeed, zSpeed));
        //if (Random.Range(0, 2) == 0)
        //    vel *= -1;
        return vel;
    }

    private Vector3 RandomAngularVelocity() {
        return new Vector3(Random.Range(minRotation, maxRotation), Random.Range(minRotation, maxRotation), Random.Range(minRotation, maxRotation));
    }

    public void HandleTouch() {
        // velocity boost
        //if ((Mathf.Abs(rgb.velocity.x * 2f) <= maxSpeed) && (Mathf.Abs(rgb.velocity.y * 2f) <= maxSpeed) && (Mathf.Abs(rgb.velocity.z * 2f) <= maxSpeed)) 
        //    rgb.velocity *= 2f;
        var rand = Random.Range(0, 2);
        if (rand == 0)
            rgb.velocity *= touchSpeedBoost;
        else
            rgb.velocity *= -touchSpeedBoost;
        rgb.velocity = new Vector3(Mathf.Clamp(rgb.velocity.x, -maxSpeed, maxSpeed), Mathf.Clamp(rgb.velocity.y, -maxSpeed, maxSpeed), Mathf.Clamp(rgb.velocity.z, -maxSpeed, maxSpeed));
        
        // angular velocity boost
        rand = Random.Range(0, 2);
        if (rand == 0)
            rgb.angularVelocity *= touchRotationBoost;
        else
            rgb.angularVelocity *= -touchRotationBoost;
    }
}
