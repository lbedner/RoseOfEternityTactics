using System.IO;

namespace EternalEngine {
	public class Utility {

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
	}
}