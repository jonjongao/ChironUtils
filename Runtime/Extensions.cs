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

		public static string ReadFirstLine(string path)
		{
			return File.ReadLines(path).First();
		}
	}
}