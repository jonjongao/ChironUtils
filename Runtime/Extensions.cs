using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using System.Linq;
#endif

namespace Chiron.Skeleton
{
	public static class Extensions
	{
		public static SerializedHumanPose Serialize(this HumanPose pose)
		{
			return new SerializedHumanPose(ref pose);
		}
	}
}