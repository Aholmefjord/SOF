using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Flocking
{
	public interface iRule
	{
		float minDist { get; set; }
		float maxDist { get; set; }
		float scalar { get; set; }
		Vector3 getResult(List<Boid> boids, int current);
	}
}