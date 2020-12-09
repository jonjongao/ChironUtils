using System.Collections;
using System.Collections.Generic;
using System;
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

		[System.NonSerialized]
		public const string type = "skeletonv1";

		public enum Variation
		{
			skeletonv1default = 0,
			skeletonv1posture = 1
		}
		public Variation variation;
		public string name;
		[HideInInspector]
		public bool hasSkeleton;
		[HideInInspector]
		public Vector3 centerOffset;
		[HideInInspector]
		public float overallConf;
		[HideInInspector]
		public float upperConf;
		[HideInInspector]
		public float lowerConf;
		public Joint[] joints;
		[HideInInspector]
		public Vector3 distanceShoulder;

		public int FindIndexOfType(int type)
		{
			for (int i = 0; i < joints.Length; i++)
			{
				if (joints[i].type == type)
					return i;
			}
			return -2;
		}

		public Joint GetJoint(int type)
		{
			for (int i = 0; i < joints.Length; i++)
			{
				if (joints[i].type == type)
					return joints[i];
			}
			return new Joint(-2);
		}

		public bool GetJoint(HumanBodyBones type, ref Joint joint)
		{
			joint = GetJoint((int)type);
			if (joint.type == -2)
				return false;
			else
				return true;
		}

		public bool HasJoint(int type)
		{
			for (int i = 0; i < joints.Length; i++)
			{
				if (joints[i].type == type)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Save from Dictionary<int, TransformLite>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="keys"></param>
		/// <param name="localPositions"></param>
		/// <param name="worldRotations"></param>
		public SkeletonV1(string name, int[] keys, Vector3[] localPositions, Quaternion[] worldRotations)
		{
			this.variation = Variation.skeletonv1posture;
			this.name = name;
			this.hasSkeleton = true;
			this.centerOffset = Vector3.zero;
			this.overallConf = 1f;
			this.upperConf = 1f;
			this.lowerConf = 1f;
			this.distanceShoulder = Vector3.zero;
			var arr = new List<Joint>();

			/*
			 * Map bone -1 ~ 54
			 */
			for (int i = -1; i < (int)HumanBodyBones.LastBone; i++)
			{
				var contain = false;
				foreach(var k in keys)
				{
					if (k == i)
					{
						contain = true;
						break;
					}
				}

				if (contain)
				{
					var index = Array.FindIndex<int>(keys, k => k == i);

					var jt = new Joint();
					jt.type = i;
					jt.conf = 1f;
					/*
					 * Local position
					 * World rotation
					 */
					jt.pos = localPositions[index];
					jt.rot = worldRotations[index];
					arr.Add(jt);
				}
			}
			this.joints = arr.ToArray();
		}

		/// <summary>
		/// Save from Gesture struct
		/// </summary>
		/// <param name="name"></param>
		/// <param name="boneOrder"></param>
		/// <param name="worldPose"></param>
		/// <param name="relativePose"></param>
		public SkeletonV1(string name, HumanBodyBones[] boneOrder, Pose[] worldPose, Pose[] relativePose)
		{
			this.variation = Variation.skeletonv1posture;
			this.name = name;
			this.hasSkeleton = true;
			this.centerOffset = Vector3.zero;
			this.overallConf = 1f;
			this.upperConf = 1f;
			this.lowerConf = 1f;
			this.distanceShoulder = Vector3.zero;
			var arr = new List<Joint>();

			int relativePoseTypeShift = 50;

			for (int i = 0; i < boneOrder.Length; i++)
			{
				var jt = new Joint();
				jt.type = (int)boneOrder[i];
				jt.conf = 1f;
				jt.pos = worldPose[i].position;
				jt.rot = worldPose[i].rotation;
				arr.Add(jt);

				var jt2 = new Joint();
				jt2.type = (int)boneOrder[i] + relativePoseTypeShift;
				jt2.conf = 1f;
				jt2.pos = relativePose[i].position;
				jt2.rot = relativePose[i].rotation;
				arr.Add(jt2);
			}
			this.joints = arr.ToArray();
		}

		public SkeletonV1 ShallowCopy()
		{
			return (SkeletonV1)this.MemberwiseClone();
		}

		public SkeletonV1 DeepCopy()
		{
			SkeletonV1 n = (SkeletonV1)this.MemberwiseClone();
			n.joints = new Joint[this.joints.Length];
			this.joints.CopyTo(n.joints, 0);
			return n;
		}
	}

	[System.Serializable]
	public struct SkeletonV1Video
	{
		public const string type = "skeletonv1video";
		public double frameRate;
		public ulong frameCount;
		public double length;
		public int samples;
		public int sampleRate;
		public List<long> keys;
		public List<SkeletonV1> poses;

		public void Add(long videoFrame, ref SkeletonV1 pose)
		{
			if (keys == null) keys = new List<long>();
			keys.Add(videoFrame);
			if (poses == null) poses = new List<SkeletonV1>();
			poses.Add(pose);
			samples++;
		}

		public void Clear()
		{
			if (keys != null) keys.Clear();
			if (poses != null) poses.Clear();
			samples = 0;
		}

		public SkeletonV1Video ShallowCopy()
		{
			return (SkeletonV1Video)this.MemberwiseClone();
		}

		public SkeletonV1Video DeepCopy()
		{
			SkeletonV1Video n = (SkeletonV1Video)this.MemberwiseClone();
			n.keys = new List<long>();
			n.poses = new List<SkeletonV1>();
			for (int i = 0; i < samples; i++)
			{
				n.keys.Add(keys[i]);
				n.poses.Add(poses[i].DeepCopy());
			}
			return n;
		}
	}
}