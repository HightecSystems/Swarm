using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dust : MonoBehaviour {

	public enum DUST_FROMATION_TYPE {
		SIMPLE_RANDOM,
		MESH_REGARDING,
	};

	public Vector3 relativePosWithRootOriginal;
	public Vector3 relativePosWithRootCurrent;
	private bool isMoving = false;
	private float current_moving_time = 0f;
	private float requred_moving_time;
	[SerializeField]
	float MIGRATION_SPEED = 0.5f;
	private Vector3 original_pos;
	private Vector3 destinationPos;
	public GameObject mParent;
	public GameObject mGrandParent;
	private Color originalColor;
	private Color targetColor;
	private bool isIllusion = false;
	private float current_illusion_time = 0f;
	[SerializeField]
	private float ILLUSION_TIME = 0.2f;

	public static GameObject SpawnDust(GameObject root, GameObject prefab) {
		GameObject air_guard = root.GetComponent<BreakingDownRoot>().air_guard;
		if(air_guard == null) {
			air_guard = root.GetComponent<BreakingDownRoot>().GenerateAirGuard();
		}
		GameObject newDust = GameObject.Instantiate (prefab);
		newDust.GetComponent<Dust> ().setParent (air_guard);
		newDust.GetComponent<Dust> ().setGrandParent (root);
		newDust.GetComponent<Dust> ().SetDustPosition ();
		newDust.GetComponent<Dust> ().SetDustRotation ();

		return newDust;
	}

	public void SetDustPosition(bool isMoveImmediatery = true) {
		DUST_FROMATION_TYPE type = mGrandParent.GetComponent<BreakingDownRoot> ().FROMATION_TYPE;
		switch(type) {
		case DUST_FROMATION_TYPE.SIMPLE_RANDOM:
			relativePosWithRootOriginal = GetSimpleRandomPosition ();
			relativePosWithRootCurrent = relativePosWithRootOriginal;
			break;
		case DUST_FROMATION_TYPE.MESH_REGARDING:
			relativePosWithRootOriginal = GetMeshRandomPosition ();
			relativePosWithRootCurrent = relativePosWithRootOriginal;
			break;
		default:
			relativePosWithRootOriginal = Vector3.zero;
			relativePosWithRootCurrent = relativePosWithRootOriginal;
			break;
		}
		if (isMoveImmediatery) {
			transform.position = mGrandParent.transform.position + relativePosWithRootCurrent;
		}
	}

	public void SetDustRotation() {
		Vector3 euler_angle 
		= new Vector3 (Random.Range (0f, 180f), Random.Range (0f, 180f), Random.Range (0f, 180f));
		Quaternion rotation = Quaternion.identity;
		rotation.eulerAngles = euler_angle;
		transform.rotation = rotation;
	}

	private Vector3 GetSimpleRandomPosition() {
		Vector3 region = mGrandParent.transform.lossyScale;
		return new Vector3 (
			Random.Range (-region.x/2, region.x/2),
			Random.Range (-region.y/2, region.y/2),
			Random.Range (-region.z/2, region.z/2));
	}

	private Vector3 GetMeshRandomPosition() {
		MeshFilter meshFilter = mGrandParent.GetComponent<MeshFilter> ();
		Mesh mesh = meshFilter.mesh;
		List<Vector3> positions = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		Vector3 scale = mGrandParent.transform.lossyScale;
		mesh.GetVertices (positions);
		mesh.GetNormals (normals);
		int index = Random.Range (0, positions.Count );
		Vector3 position = positions [index];
		position.x *= scale.x;
		position.y *= scale.y;
		position.z *= scale.z;
		position =  position + normals[index] * 0.0f;
		//position = ModifyingRelativePosition (position);
		return position;
	}

	// Use this for initialization
	void Start () {
		originalColor = this.GetComponent<Renderer> ().material.color;
	}
	
	// Update is called once per frame
	void Update () {
		MovingProc ();
		IllusionProc ();
	}

	void FixedUpdate() {
		//MovingProc ();
	}

	private void MovingProc() {
		if (isMoving) {
			SetDestination ();
			current_moving_time += Time.deltaTime;
			transform.position = Vector3.Lerp (original_pos, destinationPos, current_moving_time / requred_moving_time);
			if (current_moving_time >= requred_moving_time) {
				SetDestination ();
				this.transform.parent = mParent.transform;
				isMoving = false;
			}
		}
	}

	private void IllusionProc() {
		if (isIllusion) {
			current_illusion_time += Time.deltaTime;
			Color tempColor;
			if (current_illusion_time < ILLUSION_TIME / 2f) {
				tempColor = Color.Lerp (originalColor, targetColor, current_illusion_time / (ILLUSION_TIME / 2f));
				if (current_illusion_time >= ILLUSION_TIME / 2f) {
					tempColor = targetColor;
				}
			} else {
				tempColor = Color.Lerp (targetColor, originalColor, (current_illusion_time-ILLUSION_TIME/2f) / (ILLUSION_TIME / 2f));
				if (current_illusion_time >= ILLUSION_TIME) {
					tempColor = originalColor;
					isIllusion = false;
				}
			}

			this.GetComponent<Renderer> ().material.color = tempColor;
		}
	}

	private void SetDestination() {
		relativePosWithRootCurrent = ModifyingRelativePosition (relativePosWithRootOriginal);
		destinationPos = mParent.transform.position + relativePosWithRootCurrent;
		//Debug.Log ("SetDestination : current = " + relativePosWithRootCurrent + " origin = " + relativePosWithRootOriginal);
	}

	private Vector3 ModifyingRelativePosition(Vector3 dest) {
		if (mGrandParent.GetComponent<BaseMoving> ().isRotation) {
			dest = mGrandParent.transform.rotation * dest;
		}
		return dest;
	}

	public void setParent(GameObject parent) {
		mParent = parent;
		this.transform.parent = mParent.transform;
	}

	public void setGrandParent(GameObject granpa) {
		mGrandParent = granpa;
	}

	public void Departure(GameObject target) {
		mParent = target.GetComponent<BreakingDownRoot> ().air_guard;
		mGrandParent = target;
		SetDustPosition (false);
		this.transform.parent = null;
		StartCoroutine(StartMove (0.0f));
	}

	IEnumerator StartMove(float delayTime) {
		yield return new WaitForSeconds (delayTime);
		this.GetComponent<Rigidbody> ().useGravity = false;
		this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		this.GetComponent<SphereCollider> ().enabled = false;
		original_pos = transform.position;
		SetDestination ();
		float distance = Vector3.Distance (destinationPos, original_pos);
		float speed = Random.Range (MIGRATION_SPEED / 2, MIGRATION_SPEED);
		requred_moving_time = distance / speed;
		current_moving_time = 0f;
		isMoving = true;
		yield return null;
	}

	IEnumerator StartIllusion(float delayTime) {
		yield return new WaitForSeconds (delayTime/2f);
		delayTime = Mathf.Clamp (delayTime, 0.1f, 1f);
		float rgb =  0.1f / delayTime;
		targetColor = new Color(rgb, rgb, rgb, 1f);
		//Debug.Log ("StartIllusion : " + targetColor);
		current_illusion_time = 0f;
		isIllusion = true;
		yield return null;
	}

	public void SetQueueReversePosition(float delay_time) {
		StartCoroutine (StartMove(delay_time));
	}

	public void SetQueueIllusion(float delay_time) {
		StartCoroutine (StartIllusion (delay_time));
	}
}
