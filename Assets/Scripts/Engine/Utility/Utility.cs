using UnityEngine;
using System.Collections;
using System.IO;

namespace EternalEngine {
	public static class Utility {

		/// <summary>
		/// Combines the paths.
		/// </summary>
		/// <returns>The paths.</returns>
		/// <param name="args">Arguments.</param>
		public static string CombinePaths(params string[] paths) {
			string combinedPath = "";
			foreach (var path in paths)
				combinedPath = Path.Combine (combinedPath, path);
			return combinedPath;
		}

		/// <summary>
		/// Scales the button.
		/// </summary>
		/// <returns>The button.</returns>
		/// <param name="timeToMove">Time to move.</param>
		/// <param name="transform">Transform of component to scale.</param>
		/// <param name="startingPosition">Starting position.</param>
		/// <param name="endingPosition">Ending position.</param>
		/// <param name="startingScale">Starting scale.</param>
		/// <param name="endingScale">Ending scale.</param>
		public static IEnumerator ScaleComponent(float timeToMove, Transform transform, Vector3 startingPosition, Vector3 endingPosition, Vector3 startingScale, Vector3 endingScale) {
			float elapsedTime = 0.0f;
			while (elapsedTime < timeToMove) {

				// Don't change position if start and end are the same
				transform.localPosition = Vector3.Lerp (startingPosition, endingPosition, (elapsedTime / timeToMove));
				transform.localScale = Vector3.Lerp (startingScale, endingScale, (elapsedTime / timeToMove));
				elapsedTime += Time.deltaTime;
				yield return null;
			}		
		}
	}
}