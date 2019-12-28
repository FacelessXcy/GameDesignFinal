using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateGameStateTrigger : MonoBehaviour
{
    public int nextStateIndex;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SendMessageUpwards("UpdateStoryState",nextStateIndex);
            Destroy(this.gameObject);
        }
    }
}
