#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Game.Inspector
{
    [CustomEditor(typeof(MeshGenerator))]
    [CanEditMultipleObjects]
    public class MeshGeneratorEditor : Editor
    {
        private MeshGenerator refScript;

        private SerializedProperty position;
        private SerializedProperty size;

        // cylinder properties
        SerializedProperty section;
        SerializedProperty radius;
        SerializedProperty length;

        // Start is called before the first frame update
        void OnEnable()
        {
            refScript = target as MeshGenerator;
            position = serializedObject.FindProperty("position");
            size = serializedObject.FindProperty("size");

            section = serializedObject.FindProperty("section");
            radius = serializedObject.FindProperty("radius");
            length = serializedObject.FindProperty("length");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(position);
            EditorGUILayout.PropertyField(size);

            EditorGUILayout.PropertyField(section);
            EditorGUILayout.PropertyField(radius);
            EditorGUILayout.PropertyField(length);

            if (GUILayout.Button("Make Tetrahedron"))
            {
                if (refScript.GenerateTetrahedron())
                   SavePrefab();
            }

            if (GUILayout.Button("Clear Model"))
            {
                if (refScript.ClearModel())
                    SavePrefab();
            }

            if (GUILayout.Button("Save Model"))
            {
                SaveMeshInPlace();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SavePrefab()
        {
            var go = Selection.activeGameObject;
            EditorUtility.SetDirty(go);
        }

        public void SaveMeshInPlace()
        {
            Mesh m = refScript.GeneratedMesh;
            SaveMesh(m, m.name, false);
        }

        public void SaveMesh(Mesh mesh, string name, bool makeNewInstance)
        {
            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);

            Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

            AssetDatabase.CreateAsset(meshToSave, path);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif //UNITY_EDITOR
