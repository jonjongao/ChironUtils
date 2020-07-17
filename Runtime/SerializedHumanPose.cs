using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chiron.Skeleton
{
	[System.Serializable]
	public struct SerializedHumanPose
	{
		public Vector3 bodyPosition;

		public Quaternion bodyRotation;

		public float[] muscles;

		public HumanPose GetPose()
		{
			return new HumanPose
			{
				bodyPosition = bodyPosition,
				bodyRotation = bodyRotation,
				muscles = muscles
			};
		}

		public SerializedHumanPose(ref HumanPose pose)
		{
			bodyPosition = pose.bodyPosition;
			bodyRotation = pose.bodyRotation;
			muscles = (float[])pose.muscles.Clone();
		}
	}
}