using System;
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
		public static string GetAbsoluteFilePath(string assetPath)
		{
			var dataPath = Application.dataPath;
			dataPath = dataPath.Remove(dataPath.Length - 6, 6);
			return dataPath + assetPath;
		}

		//public static string ReadFileFirstLine(string path)
		//{
		//	return File.ReadLines(path).First();
		//}

		public static string ReadFirstLine(this string str)
		{
			return new StringReader(str).ReadLine();
		}

		public static string RemoveFirstLine(this string str)
		{
			var h = new StringReader(str).ReadLine();
			return str.Remove(0, h.Length);
		}

		public static string AddFirstLine(this string str, string value)
		{
			return value + Environment.NewLine + str;
		}

		public static float Truncate(this float value, int digits)
		{
			double mult = Math.Pow(10.0, digits);
			double result = Math.Truncate(mult * value) / mult;
			return (float)result;
		}
	}

	public static class Utility
	{
		public static SkeletonV1 CreateOverrideSkeleton(Animator animator, string name)
		{
			var total = (int)HumanBodyBones.LastBone;
			var sk = new SkeletonV1();
			sk.variation = SkeletonV1.Variation.skeletonv1posture;
			sk.name = name;
			sk.hasSkeleton = true;

			var list = new List<SkeletonV1.Joint>();

			Transform lShoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
			Transform rShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
			var distShoulder = lShoulder.position - rShoulder.position;

			for (int i = 0; i < total; i++)
			{
				var joint = animator.GetBoneTransform((HumanBodyBones)i);
				if (joint)
				{
					var jt = new SkeletonV1.Joint();
					jt.type = i;
					jt.pos = joint.localPosition;
					jt.rot = joint.rotation;
					//package.joints[i] = jt;
					list.Add(jt);
				}
				else
				{
					Debug.LogWarningFormat("This avatar doesn't have {0}", (HumanBodyBones)i);
				}
			}

			var root = new SkeletonV1.Joint();
			root.type = -1;
			root.pos = animator.transform.localPosition;
			root.rot = animator.transform.rotation;
			list.Add(root);

			sk.upperConf = 1f;
			sk.lowerConf = 1f;
			sk.overallConf = 1f;
			sk.distanceShoulder = distShoulder;
			sk.joints = list.ToArray();

			return sk;
		}

		public static SkeletonV1 CreateRelativedSkeleton(Animator animator, SkeletonV1 tpose, string name)
		{
			var sk = new SkeletonV1();
			var upperConf = 0f; var upperNum = 0;
			var lowerConf = 0f; var lowerNum = 0;
			var overallConf = 0f;

			sk.variation = Chiron.Skeleton.SkeletonV1.Variation.skeletonv1default;
			sk.name = name;
			sk.hasSkeleton = true;
			sk.joints = new SkeletonV1.Joint[tpose.joints.Length];

			Transform lShoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
			Transform rShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
			var distShoulder = lShoulder.position - rShoulder.position;

			Transform hip = animator.GetBoneTransform(HumanBodyBones.Hips);

			var index = 0;
			/*
			 * Length of VNect joint
			 * Because VNect bone not always existing in Avatar
			 * Meaning you can't get correct length on VNectModel, length of 28 is incorrect
			 * So we need to count it base on Avatar, then cross reference with VNectModel
			 */
			var vcount = 0;
			foreach (var i in tpose.joints)
			{
				if (i.type == -1)
					continue;

				var joint = animator.GetBoneTransform((HumanBodyBones)i.type);
				if (joint)
				{
					var jt = new SkeletonV1.Joint();
					jt.type = i.type;
					jt.pos = joint.localPosition - tpose.GetJoint(i.type).pos;
					jt.rot = joint.rotation * Quaternion.Inverse(tpose.GetJoint(i.type).rot);

					sk.joints[index] = jt;

					if (IsUpperBody(i.type))
					{
						upperConf += 1f;
						upperNum++;
					}
					else if (IsLowerBody(i.type))
					{
						lowerConf += 1f;
						lowerNum++;
					}
					overallConf += 1f;

					vcount++;
				}
				index++;
			}

			var root = new SkeletonV1.Joint();
			root.type = -1;
			root.pos = animator.transform.localPosition;
			root.rot = animator.transform.rotation;
			sk.joints[sk.joints.Length - 1] = root;

			sk.upperConf = upperConf / (float)upperNum;
			sk.lowerConf = lowerConf / (float)lowerNum;
			sk.overallConf = overallConf / (float)vcount;
			sk.distanceShoulder = distShoulder;
			return sk;
		}
		
		public static void ApplyRelativedSkeleton(ref SkeletonV1 targetPose, Animator animator, Transform container, SkeletonV1 tpose,
			JointMask mask, float deltaTime)
		{
			for (int i = 0; i < targetPose.joints.Length - 1; i++)
			{
				var j = targetPose.joints[i];
				var bone = animator.GetBoneTransform((HumanBodyBones)j.type);
				if (bone == null)
				{
					//Debug.LogFormat("Model {0} doesn't have bone {1}", animator.gameObject.name, (HumanBodyBones)j.type);
					continue;
				}

				/*
				 * SKIP If TPoseRot doesn't contain the joint
				 * Means we don't have startup data for that bone
				 * We'll get incorrect rotation anyway
				 */
				if (tpose.HasJoint(j.type) == false)
					continue;

				var def = tpose.GetJoint(j.type);

				/*
				 * If mask out UpperBody
				 */
				if (mask.HasFlag(JointMask.UpperBody) == false)
					if (Chiron.Skeleton.Utility.IsUpperBody((HumanBodyBones)j.type))
						continue;

				/*
				 * If mask out LowerBody
				 */
				if (mask.HasFlag(JointMask.LowerBody) == false)
					if (Chiron.Skeleton.Utility.IsLowerBody((HumanBodyBones)j.type))
						continue;

				/* 
				 * If mask out Hip
				 */
				if (mask.HasFlag(JointMask.Hip) == false)
					if (Chiron.Skeleton.Utility.IsHip((HumanBodyBones)j.type))
						continue;

				var rlt = container ? Quaternion.Inverse(container.rotation) * j.rot :
					j.rot;
				var rot = rlt * def.rot;
				//rot.x *= flip.x;
				//rot.y *= flip.y;
				//rot.z *= flip.z;
				//rot.w *= flip.w;

				var pos = j.pos + def.pos;
				//bone.rotation = Quaternion.Lerp(bone.rotation, rot, normalize);
				bone.rotation = Quaternion.Lerp(bone.rotation, rot, deltaTime);
				bone.localPosition = Vector3.Lerp(bone.localPosition, pos, deltaTime);
			}
		}

		public static void ApplyOverrideSkeleton(Animator animator, Transform container, SkeletonV1 skeleton)
		{
			for (int i = 0; i < skeleton.joints.Length; i++)
			{
				var j = skeleton.joints[i];

				if (j.type < 0)
					continue;

				var b = animator.GetBoneTransform((HumanBodyBones)j.type);

				if (b)
				{
					var rot = j.rot;
					b.rotation = rot;
				}
				else
				{
					Debug.LogWarningFormat("Miss bone {0}", (HumanBodyBones)j.type);
				}
			}
		}

		public static bool IsUpperBody(HumanBodyBones bone) => IsUpperBody((int)bone);
		public static bool IsUpperBody(int bone)
		{
			/*
			 * Between Spine to UpperChest, LastBone
			 */
			if (bone >= 7 &&
				 bone <= 55) return true;

			return false;
		}

		public static bool IsLowerBody(int bone)
		{
			/*
			 * Between Hips(Not included) to RightFoot
			 */
			if ((int)bone >= 1 &&
				(int)bone <= 6) return true;

			return false;
		}
		public static bool IsLowerBody(HumanBodyBones bone) => IsLowerBody((int)bone);

		public static bool IsHip(HumanBodyBones bone)
		{
			if ((int)bone == 0) return true;
			return false;
		}

		public static bool IsRightArm(HumanBodyBones bone)
		{
			switch ((int)bone)
			{
				case 12:
				case 14:
				case 16:
				case 18:
				case int n when (n >= 39 && n <= 53):
					return true;
					break;
			}

			return false;
		}

		public static bool IsLeftArm(HumanBodyBones bone)
		{
			switch ((int)bone)
			{
				case 11:
				case 13:
				case 15:
				case 17:
				case int n when (n >= 24 && n <= 38):
					return true;
					break;
			}

			return false;
		}

		public static bool IsRightLeg(HumanBodyBones bone)
		{
			switch ((int)bone)
			{
				case 2:
				case 4:
				case 6:
				case 20:
					return true;
					break;
			}

			return false;
		}

		public static bool IsLeftLeg(HumanBodyBones bone)
		{
			switch ((int)bone)
			{
				case 1:
				case 3:
				case 5:
				case 19:
					return true;
					break;
			}

			return false;
		}
	}
}