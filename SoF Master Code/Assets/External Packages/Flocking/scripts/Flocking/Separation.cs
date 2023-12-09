using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Flocking
{
	public class Separation : iRule
	{
		public float minDist { get; set; }
		public float maxDist { get; set; }
		public float scalar { get; set; }

		public Separation()
		{

		}

		public Vector3 getResult(List<Boid> boids, int current)
		{
			Vector3 result = Vector3.zero;
			int count = 0;
			for (int i = 0; i < boids.Count; ++i)
			{
				//don't do anything for self.
				if (i != current)
				{
					//get the boid from the loop.
					Boid b = boids[current];
					Boid other = boids[i];
					Vector3 bPos = b.transform.position;
					Vector3 dif = bPos - other.transform.position;

					float dist = Vector3.Magnitude(dif);
					if (dist <= maxDist && dist >= minDist)
					{
						dif.Normalize();
						result += dif / dist;
						count++;
					}
				}
			}

			if (count > 0)
			{
				result /= count;
				result.Normalize();
				return result * scalar;
			}
			return result;
		}
	}
}