using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject Ball;
	
	[SerializeField]
	private float turnSpeed = 100f;

	// The radius of the circle around the ball.
	[SerializeField]
	private float radius = 0.5f;

	[SerializeField]
	private float verticalOffset = 0.5f;

	// The angle (in degrees) the camera position has on the circle around the ball.
	[SerializeField]
	private float angle = 270f;

	private float conversionFactor = Mathf.PI / 180f;

	void Update () {
		// Get input.
		float input = Input.GetAxis("Horizontal");

		// Update angle.
		angle += input * Time.deltaTime * turnSpeed;

		// Set position to ball position with vertical offset.
		transform.position = Ball.transform.position + Vector3.up * verticalOffset;

		// Place camera on cirle around ball depending on angle.
		transform.position += new Vector3(radius * Mathf.Cos(angle * conversionFactor), 0, radius * Mathf.Sin(angle * conversionFactor));

		// Turn camera towards ball.
		transform.LookAt(Ball.transform);

		// Convert angle to 0 <= angle <= 360 degrees.
		angle = angle > 360 ? angle - 360 : angle;
		angle = angle < 0 ? angle + 360 : angle;
	}
}
