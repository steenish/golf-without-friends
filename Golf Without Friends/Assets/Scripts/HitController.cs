using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ball))]
public class HitController : MonoBehaviour {

	public Transform Camera;
	public Vector3 Putter;
	public Vector3 Wedge;
	public Vector3 WedgeAngular;
	public Vector3 Drive;
	public Vector3 DriveAngular;

	private Ball ball;

	void Start() {
		ball = GetComponent<Ball>();
	}

	void Update() {
		Quaternion cameraY = Camera.rotation;
		cameraY.x = 0;

		if (Input.GetMouseButtonUp(0)) {
			PutterHit(cameraY);
		} else if (Input.GetMouseButtonUp(1)) {
			WedgeHit(cameraY);
		} else if (Input.GetMouseButtonUp(2)) {
			DriveHit(cameraY);
		}
	}

	void PutterHit(Quaternion cameraRotation) {
		ball.Velocity += cameraRotation * Putter;
	}

	void WedgeHit(Quaternion cameraRotation) {
		ball.Velocity += cameraRotation * Wedge;
		ball.AngularVelocity += cameraRotation * WedgeAngular;
	}

	void DriveHit(Quaternion cameraRotation) {
		ball.Velocity += cameraRotation * Drive;
		ball.AngularVelocity += cameraRotation * DriveAngular;
	}
}
