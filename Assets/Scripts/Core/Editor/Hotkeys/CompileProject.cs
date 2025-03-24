using UnityEditor;
using UnityEditor.Compilation;

namespace Core.Editor.Hotkeys
{
	public static class CompileProject
	{
		[MenuItem("File/Compile _F5")]
		private static void Compile()
		{
			CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
		}
	}
}