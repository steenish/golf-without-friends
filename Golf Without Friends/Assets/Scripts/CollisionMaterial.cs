using System.Collections.Generic;
using UnityEngine;

public class CollisionMaterial {
	public static Dictionary<MaterialType, float> EnergyLoss = new Dictionary<MaterialType, float> {
		{ MaterialType.Test, .8f },
		{ MaterialType.Ball, .9f },
		{ MaterialType.Rough, .4f },
		{ MaterialType.Fairway, .45f },
		{ MaterialType.Green, .7f },
		{ MaterialType.Sand, .01f }
	};

	// 0.2 is extremely high, 0.1 too, 0.05 is also very high.
	// TODO: Set appropriate friction values.
	public static Dictionary<MaterialType, float> StaticFriction = new Dictionary<MaterialType, float> {
		{ MaterialType.Test, .01f },
		{ MaterialType.Rough, .07f },
		{ MaterialType.Fairway, .02f },
		{ MaterialType.Green, .01f },
		{ MaterialType.Sand, .2f }
	};

	public static Dictionary<MaterialType, float> DynamicFriction = new Dictionary<MaterialType, float> {
		{ MaterialType.Test, .75f * StaticFriction[MaterialType.Test] },
		{ MaterialType.Rough, .75f * StaticFriction[MaterialType.Rough] },
		{ MaterialType.Fairway, .75f * StaticFriction[MaterialType.Fairway] },
		{ MaterialType.Green, .75f * StaticFriction[MaterialType.Green] },
		{ MaterialType.Sand, .75f * StaticFriction[MaterialType.Sand] }
	};

	public static Dictionary<MaterialType, float> AngularFriction = new Dictionary<MaterialType, float> {
		{ MaterialType.Test, 0.5f },
		{ MaterialType.Rough, 0.3f },
		{ MaterialType.Fairway, 0.1f },
		{ MaterialType.Green, 0.01f },
		{ MaterialType.Sand, 1f }
	};

	public static Dictionary<MaterialType, Material> Material = new Dictionary<MaterialType, Material> {
		{ MaterialType.Test, Resources.Load<Material>("Materials/Test") },
		{ MaterialType.Ball, Resources.Load<Material>("Materials/Ball") },
		{ MaterialType.Rough, Resources.Load<Material>("Materials/Rough") },
		{ MaterialType.Fairway, Resources.Load<Material>("Materials/Fairway") },
		{ MaterialType.Green, Resources.Load<Material>("Materials/Green") },
		{ MaterialType.Sand, Resources.Load<Material>("Materials/Sand") }
	};
}