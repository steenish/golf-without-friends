using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentForceController : MonoBehaviour {

	public static float Gravity = -9.8f;
	public Vector3 Wind;
	public float WindUpdateInterval = 10f;
	public float MaxWindAmplitude = 10f;
	public float MaxWindUpdateAngle = 10f;
	public float MaxWindUpdateAmplitude = 1f;
	public float WindHeightFactor = 5f;

	private Ball[] _balls;
	private float windAmplitude;
	private float windAngle = 0f;

	private float conversionFactor = Mathf.PI / 180f;

	void Start () {
		_balls = FindObjectsOfType<Ball>();

		InvokeRepeating("UpdateWind", WindUpdateInterval, WindUpdateInterval);
	}
	
	void FixedUpdate () {
		foreach (Ball ball in _balls) {
			ApplyGravity(ball);
			ApplyWind(ball);
		}
	}

	void ApplyGravity(Ball ball) {
		ball.Velocity += Vector3.up * Gravity * Time.fixedDeltaTime;
	}

	void ApplyWind(Ball ball) {
		float height = ball.ShortestDistanceFromPlane - ball.GetRadius() < 0 ? 0 : ball.ShortestDistanceFromPlane - ball.GetRadius();
		ball.Velocity += Wind * Time.fixedDeltaTime * Mathf.Pow(height, 1f / WindHeightFactor);
	}

	void UpdateWind() {
		windAmplitude = Mathf.Clamp(UnityEngine.Random.Range(windAmplitude - MaxWindUpdateAmplitude, windAmplitude + MaxWindUpdateAmplitude), 0f, MaxWindAmplitude);

		windAngle = UnityEngine.Random.Range(windAngle - MaxWindUpdateAngle, windAngle + MaxWindUpdateAngle);

		// Convert angle to 0 <= angle <= 360 degrees.
		windAngle = windAngle > 360 ? windAngle - 360 : windAngle;
		windAngle = windAngle < 0 ? windAngle + 360 : windAngle;

		Wind = new Vector3(Mathf.Cos(windAngle * conversionFactor), 0, Mathf.Sin(windAngle * conversionFactor)) * windAmplitude;
	}

	public static Vector3 GetGravitationalForce(float mass) {
		return Gravity * Vector3.up * mass;
	}

	public Vector3 GetWindForce(float mass) {
		return Wind * mass;
	}

	public Vector3 GetForceSum(float mass) {
		return GetWindForce(mass) + GetGravitationalForce(mass);
	}
}
