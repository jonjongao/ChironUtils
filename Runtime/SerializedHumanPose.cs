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

		public static SerializedHumanPose New()
		{
			var n = new SerializedHumanPose();
			n.bodyPosition = Vector3.zero;
			n.bodyRotation = Quaternion.identity;
			n.muscles = new float[HumanTrait.MuscleCount];
			return n;
		}

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

		public SerializedHumanPose ShallowCopy()
		{
			return (SerializedHumanPose)this.MemberwiseClone();
		}

		public SerializedHumanPose DeepCopy()
		{
			SerializedHumanPose n = (SerializedHumanPose)this.MemberwiseClone();
			n.muscles = new float[this.muscles.Length];
			this.muscles.CopyTo(n.muscles, 0);
			return n;
		}
	}
}