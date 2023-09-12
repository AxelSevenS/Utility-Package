#if USE_URP_CORE

using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Rendering;

using Object = UnityEngine.Object;

namespace SevenGame.Utility {

    public abstract class TypeInstanceList<T, TEditor> where T : class, IDisposable where TEditor : Editor {

        public abstract GUIContent RenderFeaturesGUI {get;}
        public abstract GUIContent MissingFeatureGUI {get;}
        public abstract GUIContent PassNameFieldGUI {get;}
        public abstract GUIContent NoInstanceGUI {get;}
        public abstract GUIContent AddInstanceGUI {get;}
        public abstract FilterWindow.IProvider InstanceProvider {get;}
        public virtual bool CanFix => false;

        public readonly TEditor editor;
        protected readonly SerializedProperty soInstances;
        protected readonly SerializedProperty soInstanceGUIDs;

        protected readonly List<Editor> _editors = new();

        public TypeInstanceList(TEditor editor, SerializedProperty soInstances, SerializedProperty soInstanceGUIDs) {
            this.editor = editor;
            this.soInstances = soInstances;
            this.soInstanceGUIDs = soInstanceGUIDs;
        }


        protected virtual void AttemptFix() {}

        /// <summary>
        /// Checks if the type is already in the list
        /// </summary>
        /// <param name="type"></param>
        /// <returns>
        /// Whether the type is already in the list and should not be added.
        /// </returns>
        protected virtual bool DuplicateCheck(Type type) {
            // Always allow duplicates by default.
            return false;
        }


        private bool TryGetTooltip(Type type, out string tooltip) {
            TooltipAttribute attribute = type.GetCustomAttribute<TooltipAttribute>();
            if (attribute != null) {
                tooltip = attribute.tooltip;
                return true;
            }
            tooltip = string.Empty;
            return false;
        }

        public void UpdateEditorList() {
            ClearEditorsList();
            for (int i = 0; i < soInstances.arraySize; i++) {
                _editors.Add( Editor.CreateEditor(soInstances.GetArrayElementAtIndex(i).objectReferenceValue) );
            }
        }

        //To avoid leaking memory we destroy editors when we clear editors list
        public void ClearEditorsList() {
            for (int i = _editors.Count - 1; i >= 0; --i) {
                Object.DestroyImmediate(_editors[i]);
            }
            _editors.Clear();
        }



        public void AddInstance(Type type) {

            if ( !typeof(T).IsAssignableFrom(type) || DuplicateCheck(type) ) {
                return;
            }

            editor.serializedObject.Update();

            ScriptableObject component = ScriptableObject.CreateInstance(type);
            component.name = $"{type.Name}";
            Undo.RegisterCreatedObjectUndo(component, "Add Renderer Feature");

            // Store this new effect as a sub-asset so we can reference it safely afterwards
            // Only when we're not dealing with an instantiated asset
            if (EditorUtility.IsPersistent(editor.target)) {
                AssetDatabase.AddObjectToAsset(component, editor.target);
            }
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(component, out var guid, out long localId);

            // Grow the list first, then add - that's how serialized lists work in Unity
            soInstances.arraySize++;
            SerializedProperty componentProp = soInstances.GetArrayElementAtIndex(soInstances.arraySize - 1);
            componentProp.objectReferenceValue = component;

            // Update GUID Map
            soInstanceGUIDs.arraySize++;
            SerializedProperty guidProp = soInstanceGUIDs.GetArrayElementAtIndex(soInstanceGUIDs.arraySize - 1);
            guidProp.longValue = localId;
            UpdateEditorList();
            editor.serializedObject.ApplyModifiedProperties();

            // Force save / refresh
            if (EditorUtility.IsPersistent(editor.target)) {
                EditorUtility.SetDirty(editor.target);
            }
            editor.serializedObject.ApplyModifiedProperties();
        }

        private void RemoveInstance(int index) {
            SerializedProperty property = soInstances.GetArrayElementAtIndex(index);
            Object component = property.objectReferenceValue;
            property.objectReferenceValue = null;

            Undo.SetCurrentGroupName(component == null ? "Remove Renderer Feature" : $"Remove {component.name}");

            // remove the array index itself from the list
            soInstances.DeleteArrayElementAtIndex(index);
            soInstanceGUIDs.DeleteArrayElementAtIndex(index);
            UpdateEditorList();
            editor.serializedObject.ApplyModifiedProperties();

            // Destroy the setting object after ApplyModifiedProperties(). If we do it before, redo
            // actions will be in the wrong order and the reference to the setting object in the
            // list will be lost.
            if (component != null) {
                Undo.DestroyObjectImmediate(component);

                T feature = component as T;
                feature?.Dispose();
            }

            // Force save / refresh
            EditorUtility.SetDirty(editor.target);
        }

        private void MoveInstance(int id, int offset) {
            Undo.SetCurrentGroupName("Move Render Feature");
            editor.serializedObject.Update();
            soInstances.MoveArrayElement(id, id + offset);
            soInstanceGUIDs.MoveArrayElement(id, id + offset);
            UpdateEditorList();
            editor.serializedObject.ApplyModifiedProperties();

            // Force save / refresh
            EditorUtility.SetDirty(editor.target);
        }



        // private void AddShowAdditionalPropertiesMenuItem(ref GenericMenu menu, int id) {
        //     if (_editors[id].GetType() == typeof(FullScreenPassRendererFeatureEditor)) {
        //         var featureReference = _editors[id] as FullScreenPassRendererFeatureEditor;
        //         bool additionalPropertiesAreCurrentlyOn = featureReference.showAdditionalProperties;
        //         menu.AddItem(EditorGUIUtility.TrTextContent("Show Additional Properties"), additionalPropertiesAreCurrentlyOn, () => featureReference.showAdditionalProperties = !additionalPropertiesAreCurrentlyOn);
        //     }
        // }
        private void OnContextClick(Vector2 position, int id)  {
            var menu = new GenericMenu();

            if (id == 0) {
                menu.AddDisabledItem(EditorGUIUtility.TrTextContent("Move Up"));
            } else {
                menu.AddItem(EditorGUIUtility.TrTextContent("Move Up"), false, () => MoveInstance(id, -1));
            }

            if (id == soInstances.arraySize - 1) {
                menu.AddDisabledItem(EditorGUIUtility.TrTextContent("Move Down"));
            } else {
                menu.AddItem(EditorGUIUtility.TrTextContent("Move Down"), false, () => MoveInstance(id, 1));
            }

            // AddShowAdditionalPropertiesMenuItem(ref menu, id);

            menu.AddSeparator(string.Empty);
            menu.AddItem(EditorGUIUtility.TrTextContent("Remove"), false, () => RemoveInstance(id));

            menu.DropDown(new Rect(position, Vector2.zero));
        }



        private void DrawInstance(int index, ref SerializedProperty renderFeatureProperty) {
            Object rendererFeatureObjRef = renderFeatureProperty.objectReferenceValue;
            if (rendererFeatureObjRef == null) {
                EditorGUILayout.HelpBox(MissingFeatureGUI.tooltip, MessageType.Error);
                if (CanFix && GUILayout.Button("Attempt Fix", EditorStyles.miniButton)) {
                    AttemptFix();
                }
                return;
            }

            bool hasChangedProperties = false;

            string title = ObjectNames.GetInspectorTitle(rendererFeatureObjRef);
            TryGetTooltip(rendererFeatureObjRef.GetType(), out string tooltip);
            DocumentationUtils.TryGetHelpURL(rendererFeatureObjRef.GetType(), out string helpURL);

            // Get the serialized object for the editor script & update it
            Editor rendererFeatureEditor = _editors[index];
            SerializedObject serializedRendererFeaturesEditor = rendererFeatureEditor.serializedObject;
            serializedRendererFeaturesEditor.Update();

            // Foldout header
            EditorGUI.BeginChangeCheck();
            SerializedProperty activeProperty = serializedRendererFeaturesEditor.FindProperty("m_Active");
            bool displayContent = CoreEditorUtils.DrawHeaderToggle(EditorGUIUtility.TrTextContent(title, tooltip), renderFeatureProperty, activeProperty, pos => OnContextClick(pos, index), null, null, helpURL);
            hasChangedProperties |= EditorGUI.EndChangeCheck();


            if (displayContent) {
                EditorGUI.BeginChangeCheck();
                SerializedProperty nameProperty = serializedRendererFeaturesEditor.FindProperty("m_Name");
                nameProperty.stringValue = EditorGUILayout.DelayedTextField(PassNameFieldGUI, nameProperty.stringValue);
                if (EditorGUI.EndChangeCheck()) {
                    hasChangedProperties = true;

                    // We need to update sub-asset name
                    rendererFeatureObjRef.name = nameProperty.stringValue;
                    AssetDatabase.SaveAssets();

                    // Triggers update for sub-asset name change
                    ProjectWindowUtil.ShowCreatedAsset(editor.target);
                }

                EditorGUI.BeginChangeCheck();
                rendererFeatureEditor.OnInspectorGUI();
                hasChangedProperties |= EditorGUI.EndChangeCheck();

                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            }

            // Apply changes and save if the user has modified any settings
            if (hasChangedProperties) {
                serializedRendererFeaturesEditor.ApplyModifiedProperties();
                editor.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(editor.target);
            }
        }

        public void DrawList() {
            EditorGUILayout.LabelField(RenderFeaturesGUI, EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (soInstances.arraySize == 0) {
                EditorGUILayout.HelpBox(NoInstanceGUI);
            } else {
                //Draw List
                CoreEditorUtils.DrawSplitter();
                for (int i = 0; i < soInstances.arraySize; i++) {
                    SerializedProperty renderFeaturesProperty = soInstances.GetArrayElementAtIndex(i);
                    DrawInstance(i, ref renderFeaturesProperty);
                    CoreEditorUtils.DrawSplitter();
                }
            }
            EditorGUILayout.Space();

            // "Add instance" button
            using (var hscope = new EditorGUILayout.HorizontalScope()) {
                if (GUILayout.Button(AddInstanceGUI, EditorStyles.miniButton)) {
                    var r = hscope.rect;
                    var pos = new Vector2(r.x + r.width / 2f, r.yMax + 18f);
                    FilterWindow.Show(pos, InstanceProvider);
                }
            }
        }
    }
}

#endif