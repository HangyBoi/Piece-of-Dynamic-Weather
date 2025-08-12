using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script manages the tornado's movement and its physical interaction with objects.
public class TornadoController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The maximum distance the tornado will travel from its starting point.")]
    public float movementRadius = 30f;
    [Tooltip("The speed at which the tornado moves between points.")]
    public float movementSpeed = 5f;
    [Tooltip("How long the tornado waits at a destination before moving again.")]
    public float waitTimeAtDestination = 2f;

    [Header("Physics & Attraction")]
    [Tooltip("Assign the empty 'PullingCenter' child object here.")]
    public Transform pullingCenter;
    [Tooltip("The radius of the tornado's influence. Should match your Sphere Collider.")]
    public float attractionRadius = 20f;
    [Tooltip("The maximum force applied to objects.")]
    public float maxAttractionForce = 50f;
    [Tooltip("The curve controlling force based on distance (Time 0=Center, Time 1=Edge).")]
    public AnimationCurve forceByDistanceCurve;

    // Private variables
    private Vector3 _originalPosition;

    void Start()
    {
        _originalPosition = transform.position;

        if (pullingCenter == null)
        {
            Debug.LogError("Pulling Center transform is not assigned in the TornadoController!");
            this.enabled = false; // Disable the script if not set up
            return;
        }

        StartCoroutine(MovementCoroutine());
    }

    // This coroutine handles moving the tornado to random points.
    private IEnumerator MovementCoroutine()
    {
        while (true) // Loop forever
        {
            Vector3 randomOffset = new Vector3(Random.Range(-movementRadius, movementRadius), 0, Random.Range(-movementRadius, movementRadius));
            Vector3 newPos = _originalPosition + randomOffset;
            float distance = Vector3.Distance(transform.position, newPos);
            float travelTime = distance / movementSpeed;

            iTween.MoveTo(gameObject, iTween.Hash("position", newPos, "time", travelTime, "easetype", iTween.EaseType.easeInOutSine));
            yield return new WaitForSeconds(travelTime + waitTimeAtDestination);
        }
    }

    // Physics calculations should always be in FixedUpdate for reliability.
    void FixedUpdate()
    {
        // Proactively find all colliders within our radius every physics frame.
        // This is much more reliable than OnTriggerEnter/Exit for moving objects.
        Collider[] colliders = Physics.OverlapSphere(pullingCenter.position, attractionRadius);

        foreach (var collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 1. Calculate the object's distance from the vortex center.
                float distance = Vector3.Distance(pullingCenter.position, rb.position);

                // 2. Normalize the distance to a 0-1 value to sample the curve.
                // We invert it, so 1 means you are at the center (max force) and 0 is the outer edge.
                float normalizedDistance = 1f - (distance / attractionRadius);

                // 3. Get the force multiplier from the curve using our normalized distance.
                float forceMultiplier = forceByDistanceCurve.Evaluate(normalizedDistance);

                // 4. Calculate the direction towards the pulling center.
                Vector3 directionToCenter = (pullingCenter.position - rb.position).normalized;

                // 5. Apply the final, calculated force.
                rb.AddForce(directionToCenter * maxAttractionForce * forceMultiplier);
            }
        }
    }

    // Helper to visualize the attraction radius in the editor.
    void OnDrawGizmosSelected()
    {
        if (pullingCenter != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pullingCenter.position, attractionRadius);
        }
    }
}