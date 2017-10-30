using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingDownRoot : MonoBehaviour {

	[SerializeField]
	GameObject[] DUSTS = null;
	[SerializeField]
	int DUSTS_AMOUNT = 20;
	[HideInInspector]
	public GameObject[] dusts;
	public GameObject air_guard;
	[SerializeField]
	bool IS_SPWANING_DUSTS = false;
	public Dust.DUST_FROMATION_TYPE FROMATION_TYPE = Dust.DUST_FROMATION_TYPE.SIMPLE_RANDOM;

	void Awake() {
		dusts = new GameObject[DUSTS_AMOUNT];
	}

	// Use this for initialization
	void Start () {
		if (IS_SPWANING_DUSTS) {
			GenerateAirGuard ();
			SpawnNewDusts ();
		}
	}

	// Update is called once per frame
	void Update () {

	}

	void SpawnNewDusts() {
		if (DUSTS == null || DUSTS.Length <= 0)
			return;
		for (int i = 0; i < dusts.Length; i++) {
			int dust_index = Random.Range (0, DUSTS.Length);
			dusts[i] = Dust.SpawnDust (this.gameObject, DUSTS [dust_index]);
		}
	}

	public GameObject GenerateAirGuard() {
		air_guard = new GameObject ();
		air_guard.name = "Air_Guard";
		air_guard.transform.position = this.transform.position;
		air_guard.transform.rotation = this.transform.rotation;
		air_guard.transform.parent = this.transform;
		return air_guard;
	}
	

	public void migrationDusts(GameObject targetRoot) {
		SetDeactiveColor ();
		if (targetRoot.GetComponent<BreakingDownRoot> ().air_guard == null) {
			targetRoot.GetComponent<BreakingDownRoot> ().GenerateAirGuard ();
		}

		for (int i = 0; i < dusts.Length; i++) {
			if (dusts [i] == null)
				continue;
			dusts [i].GetComponent<Dust> ().Departure (targetRoot);
			targetRoot.GetComponent<BreakingDownRoot> ().dusts [i] = dusts [i];
			dusts [i] = null;
		}
	}

	public void ReleaseDusts() {
		
	}

	public void SetActiveColor() {
		Material material = GetComponent<Renderer> ().material;
		material.color = new Color(1f, 0f, 0f, 0.2f);
	}

	public void SetDeactiveColor() {
		Material material = GetComponent<Renderer> ().material;
		material.color = new Color(1f, 1f, 1f, 0.2f);
	}

	public void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag != "Projectile")
			return;
		Vector3 impactPoint = collision.gameObject.transform.position;
		foreach (GameObject dust in dusts) {
			if (dust == null)
				continue;
			
			float distance = Vector3.Distance (impactPoint, dust.transform.position);
			if (distance < 0.3f) {
				dust.GetComponent<Rigidbody> ().useGravity = true;
				dust.GetComponent<SphereCollider> ().enabled = true;
				dust.transform.parent = null;
				dust.GetComponent<Dust> ().SetQueueReversePosition (3.0f);
			}
			if (distance <= 1f) {
				dust.GetComponent<Dust> ().SetQueueIllusion (distance);
			}
		}
	}
}
