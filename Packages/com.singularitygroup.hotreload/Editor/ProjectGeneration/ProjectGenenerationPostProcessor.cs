using System.IO;
using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor.ProjectGeneration {
	class ProjectGenenerationPostProcessor : AssetPostprocessor {
		// Called once before any generation of sln/csproj files happens, can return true to disable generation altogether
		static bool OnPreGeneratingCSProjectFiles() {
			ProjectGeneration.GenerateSlnAndCsprojFiles(Application.dataPath).Forget();
			return false;
		}
	}
}

