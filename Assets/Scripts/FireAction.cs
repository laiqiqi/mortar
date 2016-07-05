﻿using UnityEngine;
using System.Collections;

public class FireAction : MonoBehaviour
{
	public Transform SpawnPoint;
	public GameObject MissilePrefab;
	public float FiringForce;

	public Transform TriggerPullStart;
	public Transform TriggerPullEnd;
	public float TriggerPullTime;
	public float TriggerReturnTime;

	bool cooldown;

	public void OnTriggerStay(Collider other)
	{
		if (other.tag == "Controller")
		{
			SteamVR_TrackedObject controller = other.GetComponent<SteamVR_TrackedObject>();
			if (controller != null && controller.index != SteamVR_TrackedObject.EIndex.None)
			{
				if (SteamVR_Controller.Input((int)controller.index).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
				{
					TryToFire();
				}
			}
		}
	}

	void TryToFire()
	{
		// Check cooldown
		if (cooldown) return;
		cooldown = true;

		Debug.Log("Firing");

		StartCoroutine(AnimateTrigger());
		SpawnMissile();
	}

	void SpawnMissile()
	{
		GameObject missile = (GameObject)Instantiate(MissilePrefab, SpawnPoint.position, SpawnPoint.rotation);
		Rigidbody rb = missile.GetComponent<Rigidbody>();
		rb.AddForce(SpawnPoint.forward * FiringForce, ForceMode.Impulse);
	}

	IEnumerator AnimateTrigger()
	{
		yield return StartCoroutine(Tween(TriggerPullStart, TriggerPullEnd, TriggerPullTime));
		yield return StartCoroutine(Tween(TriggerPullEnd, TriggerPullStart, TriggerReturnTime));
		cooldown = false;
	}

	IEnumerator Tween(Transform from, Transform to, float duration)
	{
		var i = 0.0f;
		var rate = 1.0f / duration;
		while (i < 1.0f)
		{
			i += Time.deltaTime * rate;
			transform.position = Vector3.Lerp(from.position, to.position, i);
			yield return null;
		}
	}
}
