using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Fruit : MonoBehaviour
{
    public Vector3 spawnAreaMin = new Vector3(-200, 1, -200);
    public Vector3 spawnAreaMax = new Vector3(200, 1, 200);

    public float yOffset = 1f; // height above ground

    private void OnTriggerEnter(Collider other)
    {
        // Only react to snake head
        if (!other.CompareTag("SnakeHead"))
        {
            Debug.Log("collision not head");
            return;
        }
        Debug.Log("collision head");
        SnakeController snake = other.GetComponentInParent<SnakeController>();

        if (snake != null)
        {
            snake.Grow();
        }

        Respawn();
    }

    void Respawn()
    {
        Collider col = GetComponent<Collider>();
        col.enabled = false;

        float checkRadius = 0.5f;

        for (int i = 0; i < 10; i++)
        {
            float x = UnityEngine.Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float z = UnityEngine.Random.Range(spawnAreaMin.z, spawnAreaMax.z);

            Vector3 pos = new Vector3(x, yOffset, z);

            if (!Physics.CheckSphere(pos, checkRadius))
            {
                transform.position = pos;
                col.enabled = true;
                return;
            }
        }

        // fallback (always spawn something)
        transform.position = new Vector3(0, yOffset, 0);
        col.enabled = true;
    }
}