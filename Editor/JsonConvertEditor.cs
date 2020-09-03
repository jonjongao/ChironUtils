using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace Chiron.Skeleton
{
	public class JsonConvertEditor : EditorWindow
	{
		private static JsonConvertEditor window;
		TextAsset fromFile;
		System.Type type;
		System.Object raw;
		SkeletonV1 result;
		string log;

		[MenuItem("Chiron/Convert to SkeletonV1")]
		public static void ShowWindow()
		{
			window = GetWindow<JsonConvertEditor>();
			window.titleContent = new GUIContent("Convert to SkeletonV1");
			window.minSize = new Vector2(550, window.minSize.y);
			window.Show();
		}

		void OnInspectorUpdate()
		{
			Repaint();
		}

		void OnDestroy()
		{
			fromFile = null;
			type = null;
			raw = null;
		}

		void OnGUI()
		{
			var o = EditorGUILayout.ObjectField("File", fromFile, typeof(TextAsset), false);
			if (o != null)
			{
				if (fromFile != null &&
					fromFile != o)
				{
					log = string.Empty;
					raw = null;
					type = null;
				}
				fromFile = o as TextAsset;
			}

			if (fromFile && raw == null)
			{
				var header = fromFile.text.ReadFirstLine();
				switch (header)
				{
					case var _ when header.Equals(SkeletonV1.type):
						log = "This file is already SkeletonV1 type";
						break;
					case var _ when header.Equals(HumanPoseVideo.type):
						log = "HumanPoseVideo can't direct convert to SkeletonV1";
						break;
					default:
						try
						{
							var b = JsonUtility.FromJson<Gesture>(fromFile.text);
							var c = JsonUtility.FromJson<Serialization<int, TransformLite>>(fromFile.text).ToDictionary();
							if (string.IsNullOrEmpty(b.gestureName) == false)
							{
								log = "Type Gesture need convert via PoseRecognizer, " +
									"Please use original PoseRecognizer component which hold those gesture data to convert from";
							}
							else if (c.Count > 0)
							{
								type = typeof(TransformLite);
								raw = c;
							}
							break;
						}
						catch (System.ArgumentException)
						{
							log = "Not support type";
							fromFile = null;
							break;
						}
				}
			}

			if(string.IsNullOrEmpty(log)==false)
			{
				EditorGUILayout.HelpBox(log, MessageType.Error);
			}

			if (fromFile != null && type != null)
			{
				if (string.IsNullOrEmpty(result.name))
				{
					if (type == typeof(TransformLite))
					{
						var cast = (Dictionary<int, TransformLite>)raw;
						var keys = cast.Keys.ToArray();
						var values = cast.Values.ToArray();
						List<Vector3> pos = new List<Vector3>();
						List<Quaternion> rot = new List<Quaternion>();
						foreach (var v in values)
						{
							pos.Add(v.a);
							rot.Add(v.b);
						}
						result = new SkeletonV1("tpose", keys, pos.ToArray(), rot.ToArray());
					}
				}

				if (GUILayout.Button(string.Format("Convert {0} to SkeletonV1", type.Name)))
				{
					var path = EditorUtility.SaveFilePanel(
							  "Save gesture as JSON",
							  "",
							  fromFile.name + "_SKV1.json",
							  "json");

					if (string.IsNullOrEmpty(path) == false)
					{
						var j = JsonUtility.ToJson(result, false);
						File.WriteAllText(path, j.AddFirstLine(SkeletonV1.type));
						AssetDatabase.Refresh();
					}
				}
			}
		}
	}
}