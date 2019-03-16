using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionMaterialRenderer))]
public class Ball : MonoBehaviour {

	private CollisionMaterialRenderer material;

	private EnvironmentForceController environmentForces;

	public Vector3 Velocity = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 AngularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
	public float Mass = 0.1f;
	public float Drag = 0.02f;
	public float AngularDrag = 0.02f;
	public float ShortestDistanceFromPlane;
	public float MinimumSpeedModifier = 10f;

	private float[,] InverseInertiaTensor;
    
	void Start () {
		ShortestDistanceFromPlane = transform.position.y;
		material = GetComponent<CollisionMaterialRenderer>();
		environmentForces = FindObjectOfType<EnvironmentForceController>();
		InverseInertiaTensor = CalculateInverseInertiaTensor();
	}


    void FixedUpdate() {
		Vector3 pos = transform.position;
		Vector3 rot = transform.rotation.eulerAngles;
		Vector3 intersectOffset = Vector3.zero;
		float shortestDistance = Mathf.Infinity;
        
        Plane[] planes = FindObjectsOfType<Plane>();
		
		pos += Velocity * Time.fixedDeltaTime * (1f - Drag);
		rot += AngularVelocity * Time.fixedDeltaTime * (1f - AngularDrag);

		foreach (Plane plane in planes) {
			float distance = distanceFromPlane(plane, pos);
			shortestDistance = Mathf.Abs(distance) < shortestDistance ? Mathf.Abs(distance) : shortestDistance;

            if (HitPlane(plane, distance) && InPlaneBounds(plane)) {
				PlaneCollisionResponse(plane);
				intersectOffset += plane.GetNormal() * (GetRadius() - distance);
			}
        }

		ShortestDistanceFromPlane = shortestDistance;

		Ball[] balls = FindObjectsOfType<Ball>();

		foreach (Ball ball in balls) {
			if (ball != this) {
				if (HitBall(ball)) {
					BallCollisionResponse(ball);
				}
			}
		}

        transform.position = pos + intersectOffset;
		transform.rotation = Quaternion.Euler(rot);
    }

	public float GetRadius() {
		return transform.localScale.x / 2;
	}

	bool HitPlane(Plane plane, float distance) {
		return distance < GetRadius();
	}

	float distanceFromPlane(Plane plane, Vector3 position) {
		float A = plane.transform.up.x;
		float Ax = A * position.x;
		float B = plane.transform.up.y;
		float By = B * position.y;
		float C = plane.transform.up.z;
		float Cz = C * position.z;
		float D = -Vector3.Dot(plane.transform.up, plane.transform.position);
		return (Ax + By + Cz + D) / (float)System.Math.Sqrt(A * A + B * B + C * C);
	}

	bool InPlaneBounds(Plane plane) {
		Vector3[] bounds = plane.Bounds;

		Vector3 planeParallelVectorZ = bounds[2] - bounds[0];
		Vector3 planeParallelVectorX = bounds[3] - bounds[1];

		float projectedDistanceToBack = Vector3.Project(transform.position - bounds[0], planeParallelVectorZ).magnitude;
		float projectedDistanceToLeft = Vector3.Project(transform.position - bounds[1], planeParallelVectorX).magnitude;
		float projectedDistanceToForward = Vector3.Project(transform.position - bounds[2], - planeParallelVectorZ).magnitude;
		float projectedDistanceToRight = Vector3.Project(transform.position - bounds[3], - planeParallelVectorX).magnitude;

		bool inZBounds1 = projectedDistanceToBack <= plane.transform.lossyScale.z * Plane.SCALE;
		bool inXBounds1 = projectedDistanceToLeft <= plane.transform.lossyScale.x * Plane.SCALE;
		bool inZBounds2 = projectedDistanceToForward <= plane.transform.lossyScale.z * Plane.SCALE;
		bool inXBounds2 = projectedDistanceToRight <= plane.transform.lossyScale.x * Plane.SCALE;

		return inZBounds1 && inXBounds1 && inZBounds2 && inXBounds2;
	}

	void PlaneCollisionResponse(Plane plane) {
		// Compute post-collision velocities:
		// Prelim: Calculate v_r
		Vector3 normal = plane.GetNormal();
		Vector3 impactVector = -normal * GetRadius();
		Vector3 relativeVelocity = Velocity + Vector3.Cross(AngularVelocity, impactVector);

		// 1. Compute the reaction impulse magnitude j_r in terms of v_r, m_1, m_2, I_1, I_2, r_1, r_2, n and e.
		float impulseMagnitude = -(1 + CollisionMaterial.EnergyLoss[plane.Material.Type]) * Vector3.Dot(relativeVelocity, normal) / (1 / Mass);

		// 2. Compute the reaction impulse vector j_r in terms of its magnitude j_r and contact normal n.
		Vector3 impulseVector = impulseMagnitude * normal;

		// Friction:
		Vector3 tangent = Vector3.zero;
		Vector3 forceSum = environmentForces.GetForceSum(Mass);
		if (Vector3.Dot(relativeVelocity, normal) != 0) {
			tangent = relativeVelocity - Vector3.Dot(relativeVelocity, normal) * normal;
		} else if (Vector3.Dot(forceSum, normal) != 0) {
			tangent = forceSum - Vector3.Dot(forceSum, normal) * normal;
		}

		float statImpulseMagnitude = CollisionMaterial.StaticFriction[plane.Material.Type] * impulseMagnitude;
		float dynImpulseMagnitude = CollisionMaterial.DynamicFriction[plane.Material.Type] * impulseMagnitude;

		Vector3 frictionImpulseVector = -dynImpulseMagnitude * tangent;
		if (Vector3.Dot(relativeVelocity, tangent) == 0 && Vector3.Dot(Mass * relativeVelocity, tangent) <= statImpulseMagnitude) {
			frictionImpulseVector = - Vector3.Dot(Mass * relativeVelocity, tangent) * tangent;
		}

		impulseVector += frictionImpulseVector;
		impulseMagnitude = impulseVector.magnitude;

		// 3. Compute new linear velocities v'_i in terms of old velocities v_i, masses m_i and reaction impulse vector j_r.
		Velocity = Velocity + impulseVector / Mass;

		// Stop the ball if the velocity is smaller than the static friction.
		Velocity = Velocity.magnitude < CollisionMaterial.StaticFriction[plane.Material.Type] * MinimumSpeedModifier ? Vector3.zero : Velocity;

		// 4. Compute new angular velocities w'_i in terms of old angular velocities w_i, intertia tensors I_i and reaction impulse j_r.
		AngularVelocity = AngularVelocity + impulseMagnitude * MatrixVectorMult(InverseInertiaTensor, Vector3.Cross(impactVector, normal));

		AngularVelocity = AngularVelocity * (1 - CollisionMaterial.AngularFriction[plane.Material.Type]);
	}

	bool HitBall(Ball ball) {
		return (transform.position - ball.transform.position).magnitude < GetRadius() + ball.GetRadius();
	}

	void BallCollisionResponse(Ball ball) {
		// Compute post-collision velocities:
		// Prelim: Calculate v_r
		Vector3 normal = (transform.position - ball.transform.position).normalized;
		Vector3 impactVector = -normal * GetRadius();
		Vector3 relativeVelocity = Velocity + Vector3.Cross(AngularVelocity, impactVector);

		// 1. Compute the reaction impulse magnitude j_r in terms of v_r, m_1, m_2, I_1, I_2, r_1, r_2, n and e.
		float impulseMagnitude = -(1 + CollisionMaterial.EnergyLoss[material.Type]) * Vector3.Dot(relativeVelocity, normal) / ((1 / ball.Mass) + (1 / Mass));

		// 2. Compute the reaction impulse vector j_r in terms of its magnitude j_r and contact normal n.
		Vector3 impulseVector = impulseMagnitude * normal;

		// 3. Compute new linear velocities v'_i in terms of old velocities v_i, masses m_i and reaction impulse vector j_r.
		Velocity = Velocity + impulseVector / Mass;

		// 4. Compute new angular velocities w'_i in terms of old angular velocities w_i, intertia tensors I_i and reaction impulse j_r.
		AngularVelocity = AngularVelocity + impulseMagnitude * MatrixVectorMult(InverseInertiaTensor, Vector3.Cross(impactVector, normal));
	}

	float[,] CalculateInverseInertiaTensor() {
		// Formula from https://en.wikipedia.org/wiki/List_of_moments_of_inertia
		float radius = GetRadius();
		float element = 5 / 2 * (1 / (Mass * radius * radius));
		float[,] res = { { element, 0, 0 },
						 { 0, element, 0 },
						 { 0, 0, element } };
		return res;
	}

	Vector3 MatrixVectorMult(float[,] matrix, Vector3 vector) {
		return new Vector3(matrix[0, 0] * vector.x, matrix[1, 1] * vector.y, matrix[2, 2] * vector.z);
	}
}
