using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionMaterialRenderer))]
public class Plane : MonoBehaviour {

	public CollisionMaterialRenderer Material;
	public Vector3[] Bounds;

	// The amount of meters per "scale" unit.
	public static float SCALE = 10f; 

	void Start() {
		Material = GetComponent<CollisionMaterialRenderer>();
		Bounds = GetBounds();
	}

	public Vector3 GetNormal() {
        return transform.rotation * Vector3.up;
    }

	// Returns in the order of back, left, forward, right.
	private Vector3[] GetBounds() {
		return new Vector3[] { transform.position - transform.forward * (SCALE / 2) * transform.lossyScale.z,
							   transform.position - transform.right * (SCALE / 2) * transform.lossyScale.x,
							   transform.position + transform.forward * (SCALE / 2) * transform.lossyScale.z,
							   transform.position + transform.right * (SCALE / 2) * transform.lossyScale.x };
	}

}
