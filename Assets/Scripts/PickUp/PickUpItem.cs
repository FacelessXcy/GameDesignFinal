using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PickUpItem : MonoBehaviour
{
    public bool UseGravityAtBegin;
    public float rotatingSpeed = 360f;
    public AudioClip pickupClip;
    public UnityAction<PlayerWeaponController> onPick;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private MeshCollider[] _meshColliders;
    private bool _rotate;
    private bool _canPickup;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _meshColliders = GetComponentsInChildren<MeshCollider>();
        _collider.isTrigger = true;
        _canPickup = false;
        if (UseGravityAtBegin)
        {
            _rigidbody.isKinematic = false;
            StartCoroutine(StartRotate());
        }
        else
        {
            ActivePickUp();
        }

    }

    private void Update()
    {
        if (_rotate)
        {
            transform.Rotate(Vector3.up, rotatingSpeed * Time.deltaTime, Space.Self);
        }
    }

    IEnumerator StartRotate()
    {
        yield return new WaitForSeconds(2.0f);
        ActivePickUp();
    }

    private void ActivePickUp()
    {
        _canPickup = true;
        transform.up = Vector3.up;
        transform.position += Vector3.up;
        _rigidbody.isKinematic = true;
        foreach (MeshCollider meshCollider in _meshColliders)
        {
            meshCollider.isTrigger = true;
        }
        _rotate = true;
        gameObject.layer = LayerMask.NameToLayer("PickUp");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canPickup)
        {
            return;
        }
        PlayerWeaponController weaponController = other.GetComponent<PlayerWeaponController>();
        
        if (weaponController != null)
        {
            if (pickupClip!=null)
            {
                weaponController.GetComponent<AudioSource>().PlayOneShot(pickupClip);
            }
            if (onPick != null)
            {
                _canPickup = false;
                onPick.Invoke(weaponController);
            }
        }
        Destroy(gameObject);
    }
    
}
