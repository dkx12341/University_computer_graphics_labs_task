using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SnakeController : MonoBehaviour
{
    public float turnSpeed = 100f;
    public float forceAmount = 100f;
    public float interval = 1.5f;
    private float timer = 0f;

    private Rigidbody headRb;

    public GameObject bodyPrefab; // white cube prefab
    public float segmentSpacing = 1.1f;

    private List<Rigidbody> segments = new List<Rigidbody>();

    public float springForce = 50f;
    public float damper = 5f;
    public float distance = 10f;

    void Start()
    {
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();

        segments = new List<Rigidbody>(bodies); 
        headRb = segments[0];

        for (int i = 1; i < segments.Count; i++)
        {
            Rigidbody current = segments[i];
            Rigidbody previous = segments[i - 1];

            SpringJoint joint = current.gameObject.AddComponent<SpringJoint>();

            joint.connectedBody = previous;

            joint.spring = springForce;
            joint.damper = damper;
            joint.minDistance = distance/1.3f;
            joint.maxDistance = distance*1.3f;
            joint.enableCollision = true;
        }
    }



    void FixedUpdate()
    {
        float turn = Input.GetAxis("Horizontal");

        headRb.MoveRotation(
            headRb.rotation * Quaternion.Euler(0f, turn * turnSpeed * Time.fixedDeltaTime, 0f)
        );

        timer += Time.fixedDeltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            StartCoroutine(ApplyForceWave());
        }
    }


    public void Grow()
        {
            Rigidbody lastSegment = segments[segments.Count - 1];

            // Spawn new segment behind last
            Vector3 spawnPos = lastSegment.transform.position - lastSegment.transform.forward * segmentSpacing;

            GameObject newSegment = Instantiate(bodyPrefab, spawnPos, lastSegment.transform.rotation);

            Rigidbody rb = newSegment.GetComponent<Rigidbody>();

            // Add joint
            ConfigurableJoint joint = newSegment.AddComponent<ConfigurableJoint>();
            joint.connectedBody = lastSegment;

            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            joint.enableCollision = true;

            segments.Add(rb);
        }

    IEnumerator ApplyForceWave()
    {
        float delayBetweenSegments = 0.1f;

        for (int i = 0; i < segments.Count; i++)
        {
            if (i == 0)
            {
                Vector3 dir = headRb.transform.forward; // lock direction
                segments[i].AddForce(dir * (forceAmount / (1f + i * 0.1f)), ForceMode.Impulse);
                yield return new WaitForSeconds(delayBetweenSegments);
            }
            else
            {

                Rigidbody current = segments[i];
                Rigidbody previous = segments[i - 1];

                Vector3 direction = (previous.position - current.position);

                float dist = direction.magnitude;

                if (dist > 0.001f)
                {
                    Vector3 force =
                        direction.normalized *
                        (forceAmount / (1f + i * 0.1f));

                    current.AddForce(force, ForceMode.Impulse);
                }

                yield return new WaitForSeconds(delayBetweenSegments);
            }
        }
    }
}

