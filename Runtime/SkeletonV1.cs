using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chiron.Skeleton
{
	[System.Serializable]
	public struct SkeletonV1
	{
		[System.Serializable]
		public struct Joint
		{
			/*
             * -1 = Root, <-1 = Null
             * Other will follow up UnityEngine.HumanBodyBones
             */
			public int type;
			public float conf;
			public Vector3 pos;
			public Quaternion rot;

			public Joint(int type)
			{
				this.type = type;
				this.conf = 0f;
				this.pos = Vector3.zero;
				this.rot = Quaternion.identity;
			}
		}

		public const string type = "skeletonv1";
		public bool hasSkeleton;
		public float overallConf;
		public float upperConf;
		public float lowerConf;
		public Joint[] joints;
		public Vector3 distanceShoulder;

		public Joint GetJoint(HumanBodyBones type)
		{
			var t = (int)type;
			for (int i = 0; i < joints.Length; i++)
			{
				if (joints[i].type == t)
					return joints[i];
			}
			return new Joint(-2);
		}
	}
}