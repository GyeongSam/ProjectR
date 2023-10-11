using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleToIslandTrampoline : MonoBehaviour
{
    [SerializeField]
    float bounceForce = 5;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.transform.parent.GetComponent<Rigidbody>();

        Vector3 triggerPoint = other.ClosestPoint(transform.position);
        
        float finalBounceForce = bounceForce;

        float distance = Vector3.Distance(triggerPoint, transform.position);

        if (distance < 1.0f) finalBounceForce += 60;
        else if (distance < 1.5f) finalBounceForce += 7;
       
        rb.AddForce(new Vector3(0.85f, 0.9f, 0) * 60, ForceMode.Impulse);
        
    }
}
