using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Flocking
{
	public class Alignment : iRule
	{
		public float minDist { get; set; }
		public float maxDist { get; set; }
		public float scalar { get; set; }

		public Alignment()
		{

		}

		public Vector3 getResult(List<Boid> boids, int current)
		{
			//create an empty vector to store the result of the flocking rule.
			Vector3 result = Vector3.zero;
			int count = 0;
			for (int i = 0; i < boids.Count; ++i)
			{
				//don't do anything for self.
				if (i != current)
				{
					Boid b = boids[current];
					Boid other = boids[i];
					Vector3 bPos = b.transform.position;
					//get the vector between the current boid and the neighbor boid.
					Vector3 dif = bPos - other.transform.position;

					//get the squared magnitude. Only update the velocity if the magnitude is bigger than the minimum distance and smaller than the maximum distance.
					float dist = Vector3.SqrMagnitude(dif);
					if (dist <= maxDist * maxDist && dist >= minDist * minDist)
					{
						result += other.velocity;
						count++;
					}
				}
			}

			if (count > 0)
			{
				result /= count;
				result.Normalize();
				//multiply the result by the scale set in the UI.
				return result * scalar;
			}
			return result;
		}
	}
}