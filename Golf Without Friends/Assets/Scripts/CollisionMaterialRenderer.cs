using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CollisionMaterialRenderer : MonoBehaviour {

	[SerializeField]
	private MaterialType _type = MaterialType.Test;
	public MaterialType Type {
		get { return _type; }
		set { _type = value; }
	}
	
	void Start () {
		GetComponent<Renderer>().material = CollisionMaterial.Material[Type];
	}
}
