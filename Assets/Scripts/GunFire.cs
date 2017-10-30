//#define OCULUS_MODE
#define DAYDREAM_MODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFire : MonoBehaviour {

	[SerializeField]
	private GameObject projecTile = null;
	[SerializeField]
	private float speed = 10f;
	static public GameObject preceding_detected_root;
	public bool IsRight = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		LineRenderer lineRenderer = GetComponent<LineRenderer> ();
		lineRenderer.SetPositions (new Vector3[2] {
			this.transform.position,
			this.transform.position + this.transform.forward * 10f
		});
			
		#if OCULUS_MODE
		if ((OVRInput.GetDown (OVRInput.Button.PrimaryIndexTrigger) && !IsRight)
			|| (OVRInput.GetDown (OVRInput.Button.SecondaryIndexTrigger) && IsRight)) {
			Fire ();
		}

		if (OVRInput.GetDown (OVRInput.Button.One)) {
			CheckRootDetection ();
		}
		#endif
		#if DAYDREAM_MODE
		if (GvrControllerInput.ClickButtonDown) {
			Fire ();
		}

		if (GvrControllerInput.AppButtonDown) {
			CheckRootDetection ();
		}


		transform.rotation = GvrControllerInput.Orientation;

		#endif
	}

	private void CheckRootDetection() {
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, Mathf.Infinity)) {
			if (hit.transform.tag == "BreakingRoot") {
				if (preceding_detected_root == null) {
					preceding_detected_root = hit.transform.gameObject;
					preceding_detected_root.GetComponent<BreakingDownRoot> ().SetActiveColor ();
				} else if(preceding_detected_root != hit.transform.gameObject) {
					preceding_detected_root.GetComponent<BreakingDownRoot> ().migrationDusts (hit.transform.gameObject);
					preceding_detected_root = null;
				}
			}
		}
	}

	private void Fire() {
		//Debug.Log ("Fire");
		GameObject prj = GameObject.Instantiate (projecTile, transform.position + transform.forward * 0.5f, Quaternion.identity);
		//prj.transform.Translate (this.transform.forward * 0.1f);
		prj.GetComponent<Rigidbody> ().AddForce (this.transform.forward * speed, ForceMode.Impulse);
		Destroy (prj, 10f);
	}
}
