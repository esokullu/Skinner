using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Skinner
{
    [CustomEditor(typeof(SkinnerParticleTemplate))]
    public class SkinnerParticleTemplateEditor : Editor
    {
        #region Editor functions

        SerializedProperty _shapes;
        SerializedProperty _maxInstanceCount;

        void OnEnable()
        {
            _shapes = serializedObject.FindProperty("_shapes");
            _maxInstanceCount = serializedObject.FindProperty("_maxInstanceCount");
        }

        public override void OnInspectorGUI()
        {
            var template = (SkinnerParticleTemplate)target;

            // Editable properties
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_shapes, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_maxInstanceCount);
            var rebuild = EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            // Readonly members
            EditorGUILayout.LabelField("Instance Count", template.instanceCount.ToString());

            EditorGUILayout.Space();

            // Rebuild button
            rebuild |= GUILayout.Button("Rebuild");

            // Rebuild the template mesh when the properties are changed.
            if (rebuild) template.RebuildMesh();
        }

        #endregion

        #region Create menu item functions

        [MenuItem("Assets/Create/Skinner/Particle Template")]
        public static void CreateTemplateAsset()
        {
            // Make a proper path from the current selection.
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(path), "");
            var assetPathName = AssetDatabase.GenerateUniqueAssetPath(path + "/Skinner Particle.asset");

            // Create a template asset.
            var asset = ScriptableObject.CreateInstance<SkinnerParticleTemplate>();
            AssetDatabase.CreateAsset(asset, assetPathName);
            AssetDatabase.AddObjectToAsset(asset.mesh, asset);

            // Build an initial mesh for the asset.
            asset.RebuildMesh();

            // Save the generated mesh asset.
            AssetDatabase.SaveAssets();

            // Tweak the selection.
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        #endregion
    }
}
