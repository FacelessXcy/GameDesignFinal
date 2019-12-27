using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.GetComponent<Health>()!=null)
        {
            other.collider.GetComponent<Health>().Kill();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Health>()!=null)
        {
            other.GetComponent<Health>().Kill();
        }
    }
}
