using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMoving : MonoBehaviour {

	public bool isMove = false;
	[SerializeField]
	protected Vector3[] POSITIONS = null;
	[SerializeField]
	protected float MOVING_SPEED = 1f;
	protected float required_moving_time;
	protected float current_moving_time;
	private int moving_index = 0;
	private Vector3 original_pos;
	private Vector3 destination;

	public bool isRotation = false;
	public Vector3 ROTATION_AXIS = Vector3.up;
	[SerializeField]
	protected float ROTATION_SPEED = 0.1f;
	protected float current_rotation;


	// Use this for initialization
	void Start () {
		moving_index = 0;

		setNextPositionParams ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isMove) {
			ProcMoving ();
		}
		if (isRotation) {
			ProcRotation ();
		}
	}

	void FixedUpdate() {

	}

	protected void ProcMoving() {
		current_moving_time += Time.deltaTime;
		transform.position = Vector3.Lerp (original_pos, destination, current_moving_time / required_moving_time);
		if (required_moving_time <= current_moving_time) {
			setNextPositionParams ();
		}
	}

	protected void setNextPositionParams() {
		if (POSITIONS == null || POSITIONS.Length <= 0)
			return;
		moving_index = (moving_index + 1) % POSITIONS.Length;
		destination = POSITIONS [moving_index];
		original_pos = transform.position;
		current_moving_time = 0f;
		required_moving_time = Vector3.Distance (original_pos, destination) / MOVING_SPEED;
	}

	protected void ProcRotation() {
		current_rotation += ROTATION_SPEED * Time.deltaTime;
		//transform.Rotate (ROTATION_AXIS, ROTATION_SPEED * Time.deltaTime);
		Quaternion rotation = transform.rotation;
		rotation.eulerAngles += ROTATION_AXIS * ROTATION_SPEED * Time.deltaTime;
		transform.rotation = rotation;
	}

	public float getCurrentRotationDegree() {
		float current_rotation_ = current_rotation;
		current_rotation = 0f;
		//Debug.Log ("getCurrentRoationDegree : " + current_rotation_);
		return current_rotation_;
	}
}
