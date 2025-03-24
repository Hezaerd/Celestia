#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Game.World
{
	[CustomEditor(typeof(IsometricWorldGenerator))]
	public class IsometricWorldGeneratorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			IsometricWorldGenerator worldGen = (IsometricWorldGenerator)target;

			DrawDefaultInspector();

			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("Generation Controls", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Generate World", GUILayout.Height(30)))
			{
				worldGen.GenerateWorld();
			}

			if (GUILayout.Button("Clear World", GUILayout.Height(30)))
			{
				worldGen.ClearWorld();
			}

			EditorGUILayout.EndHorizontal();

			if (worldGen.worldSettings)
			{
				EditorGUILayout.Space(5);
				EditorGUILayout.LabelField("Current Settings", EditorStyles.boldLabel);
				EditorGUILayout.LabelField($"World Size: {worldGen.worldSettings.width} x {worldGen.worldSettings.height}");
				EditorGUILayout.LabelField($"Chunk Size: {worldGen.worldSettings.chunkSize}");
				EditorGUILayout.LabelField($"View Distance: {worldGen.worldSettings.viewDistance} chunks");
				EditorGUILayout.LabelField($"Seed: {worldGen.worldSettings.seed}");

				if (worldGen.worldSettings.terrainTypes != null)
					EditorGUILayout.LabelField($"Terrain Types: {worldGen.worldSettings.terrainTypes.Length}");
			}
		}
	}
}
#endif