using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chiron.Skeleton
{
	[System.Serializable]
	public class HumanPoseVideo
	{
		public const string type = "humanposevideo";
		public double frameRate;
		public ulong frameCount;
		public double length;
		public int samples;
		public int sampleRate;
		[HideInInspector]
		public List<long> keys;
		[HideInInspector]
		public List<SerializedHumanPose> poses;

		public void Add(long videoFrame, ref SerializedHumanPose pose)
		{
			if (keys == null) keys = new List<long>();
			keys.Add(videoFrame);
			if (poses == null) poses = new List<SerializedHumanPose>();
			poses.Add(pose);
			samples++;
		}

		public void Clear()
		{
			if (keys != null) keys.Clear();
			if (poses != null) poses.Clear();
			samples = 0;
		}
	}
}