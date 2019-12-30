using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Projectile : MonoBehaviour
{
	[HideInInspector]
	public GameObject source;
	public bool useGrav;
	public bool isMonsterProjectile;
	public GameObject explosionPrefab;

	public float force;
	public float despawnTime;
	
	public float explosionRadius = 50.0f;
	public float explosionPower = 250.0f;
	public LayerMask colliderLayerMask;
	
	private Collider[] _colliders;
	private int _damage;
	private EnemyControllerBase _controllerBase;
	void Start () {
		if (useGrav) {
			GetComponent<Rigidbody> ().isKinematic = true;
		}
		GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * force);
		StartCoroutine (DestroyTimer ());
		transform.localScale *= 4;
	}

	void FixedUpdate(){
		if(GetComponent<Rigidbody>().velocity != Vector3.zero)
			transform.LookAt(
				transform.position+
				GetComponent<Rigidbody>().velocity);
	}

	IEnumerator DestroyTimer () {
		yield return new WaitForSeconds (despawnTime);
		Destroy (gameObject);
	}
	
	void OnCollisionEnter (Collision collision)
	{
		if (isMonsterProjectile)
		{
			_controllerBase =
				source.GetComponent<EnemyControllerBase>();
			_damage = _controllerBase.atkDamage;
			
		}
		else
		{
			_damage = source.GetComponent<Weapon>().damage;
		}
		
		if ((colliderLayerMask&(int)Mathf.Pow(2,collision.gameObject
		.layer))!=Mathf.Pow(2,collision.gameObject.layer))
		{
			return;
		}
		if (isMonsterProjectile)
		{
			if (_controllerBase.GetCurrentExplosion()==null)
			{
				_controllerBase.AddCurrentHaveExplosion(Instantiate(explosionPrefab, collision.contacts[0].point,
					Quaternion.FromToRotation(Vector3.up,collision
						.contacts[0].normal)));
				_controllerBase.GetCurrentExplosion()
					.GetComponent<Explosion>().isMonsterExplosion = true;
			}
			_controllerBase.GetCurrentExplosion().transform.position =
				collision.contacts[0].point;
			_controllerBase.GetCurrentExplosion().transform.rotation =
				Quaternion.LookRotation(collision.contacts[0].normal);
			_controllerBase.GetCurrentExplosion().SetActive(true);
		}
		else
		{
			Instantiate(explosionPrefab, collision.contacts[0].point,
				Quaternion.FromToRotation(Vector3.up,collision
				.contacts[0].normal));
		}
		_colliders = Physics.OverlapSphere(transform.position, explosionRadius,colliderLayerMask);
		foreach (Collider hit in _colliders) 
		{
			if (hit.GetComponent<Damageable>()==null)
			{
				continue;
			}
			Rigidbody rb = hit.GetComponent<Rigidbody> ();
			if (rb != null)
				rb.AddExplosionForce (explosionPower, transform.position, explosionRadius, 3.0f);
			Damageable damageable = hit.GetComponentInParent<Damageable>();
			if (damageable!=null)
			{
				damageable.GetDamage(_damage, source);
			}
		}
		Destroy(this.gameObject);

	}
}
