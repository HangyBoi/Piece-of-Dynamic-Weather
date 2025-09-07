using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementRadius = 30f;
    public float movementSpeed = 5f; // We will control this from the Orchestrator
    public float waitTimeAtDestination = 2f;

    [Header("Physics & Attraction")]
    public Transform pullingCenter;
    public float attractionRadius = 30f;
    public float maxAttractionForce = 50f; // We will control this from the Orchestrator
    public AnimationCurve forceByDistanceCurve;

    private Vector3 _originalPosition;
    private Coroutine _movementCoroutine;

    // Awake is called once when the script instance is being loaded.
    void Awake()
    {
        _originalPosition = transform.position;
        if (pullingCenter == null)
        {
            Debug.LogError("Pulling Center not assigned!", this);
            this.enabled = false;
        }
    }

    // OnEnable is called every time the GameObject is activated. This is the fix.
    void OnEnable()
    {
        // Stop any previous movement coroutine to be safe, then start a new one.
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
        }
        _movementCoroutine = StartCoroutine(MovementCoroutine());
    }

    // OnDisable is called when the GameObject is deactivated.
    void OnDisable()
    {
        // Stop the movement coroutine and any iTween animations when disabled.
        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
        iTween.Stop(gameObject);
    }

    private IEnumerator MovementCoroutine()
    {
        // Move to the starting position smoothly when it first appears.
        iTween.MoveTo(gameObject, iTween.Hash("position", _originalPosition, "time", 2f, "easetype", iTween.EaseType.easeInOutSine));
        yield return new WaitForSeconds(2f);

        while (true) // Loop forever while active
        {
            Vector3 randomOffset = new Vector3(Random.Range(-movementRadius, movementRadius), 0, Random.Range(-movementRadius, movementRadius));
            Vector3 newPos = _originalPosition + randomOffset;
            float distance = Vector3.Distance(transform.position, newPos);

            // Ensure movementSpeed is not zero to avoid division errors
            float travelTime = (movementSpeed > 0.01f) ? (distance / movementSpeed) : float.MaxValue;

            iTween.MoveTo(gameObject, iTween.Hash("position", newPos, "time", travelTime, "easetype", iTween.EaseType.easeInOutSine));
            yield return new WaitForSeconds(travelTime + waitTimeAtDestination);
        }
    }

    void FixedUpdate()
    {
        if (maxAttractionForce <= 0) return; // Don't run physics if force is zero

        Collider[] colliders = Physics.OverlapSphere(pullingCenter.position, attractionRadius);
        foreach (var collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float distance = Vector3.Distance(pullingCenter.position, rb.position);
                float normalizedDistance = 1f - (distance / attractionRadius);
                float forceMultiplier = forceByDistanceCurve.Evaluate(normalizedDistance);
                Vector3 directionToCenter = (pullingCenter.position - rb.position).normalized;
                rb.AddForce(directionToCenter * maxAttractionForce * forceMultiplier);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (pullingCenter != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pullingCenter.position, attractionRadius);
        }
    }
}