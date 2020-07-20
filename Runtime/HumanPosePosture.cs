using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chiron.Skeleton
{
	[System.Serializable]
	public struct HumanPosePosture
	{
		public const string type = "humanposeposture";
		public string name;
		[HideInInspector]
		public SerializedHumanPose pose;

		public HumanPosePosture(string name, ref HumanPose pose)
		{
			this.name = name;
			this.pose = new SerializedHumanPose(ref pose);
		}

		public HumanPosePosture(string name)
		{
			this.name = name;
			this.pose = new SerializedHumanPose();
		}
	}
}