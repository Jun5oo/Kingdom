using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Refactor.Editors
{
    // [SerializeReference, SubclassSelector] 필드에 구현 타입 선택 드롭다운을 그린다.
    // List<T> 필드에 붙이면 각 원소마다 적용된다.
    [CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
    public class SubclassSelectorDrawer : PropertyDrawer
    {
        static readonly Dictionary<Type, Type[]> Cache = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
                return EditorGUI.GetPropertyHeight(property, label, true);

            float height = EditorGUIUtility.singleLineHeight;

            if (HasValue(property) && property.isExpanded)
            {
                SerializedProperty end = property.GetEndProperty();
                SerializedProperty it = property.Copy();
                bool enter = true;
                while (it.NextVisible(enter) && !SerializedProperty.EqualContents(it, end))
                {
                    enter = false;
                    height += EditorGUI.GetPropertyHeight(it, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            label = EditorGUI.BeginProperty(position, label, property);

            bool hasValue = HasValue(property);
            var line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // 폴드아웃(값이 있을 때만) + 라벨
            if (hasValue)
                property.isExpanded = EditorGUI.Foldout(
                    new Rect(line.x, line.y, EditorGUIUtility.labelWidth, line.height),
                    property.isExpanded, label, true);
            else
                EditorGUI.LabelField(new Rect(line.x, line.y, EditorGUIUtility.labelWidth, line.height), label);

            // 타입 선택 버튼
            var dropdownRect = new Rect(
                line.x + EditorGUIUtility.labelWidth + 2f, line.y,
                Mathf.Max(60f, line.width - EditorGUIUtility.labelWidth - 2f), line.height);

            if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(CurrentName(property)), FocusType.Keyboard))
                ShowMenu(property, dropdownRect);

            // 하위 필드 직접 그리기
            if (hasValue && property.isExpanded)
            {
                EditorGUI.indentLevel++;
                float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                SerializedProperty end = property.GetEndProperty();
                SerializedProperty it = property.Copy();
                bool enter = true;
                while (it.NextVisible(enter) && !SerializedProperty.EqualContents(it, end))
                {
                    enter = false;
                    float h = EditorGUI.GetPropertyHeight(it, true);
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), it, true);
                    y += h + EditorGUIUtility.standardVerticalSpacing;
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        void ShowMenu(SerializedProperty property, Rect rect)
        {
            Type baseType = ResolveType(property.managedReferenceFieldTypename);

            // 콜백은 나중에 실행되므로 만료 가능한 SerializedProperty/SerializedObject를 캡처하지 않는다.
            UnityEngine.Object[] targets = property.serializedObject.targetObjects;
            string path = property.propertyPath;
            string current = CurrentName(property);

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("None"), current == "None", () => Assign(targets, path, null));

            foreach (Type type in GetCandidates(baseType))
            {
                Type captured = type;
                menu.AddItem(new GUIContent(captured.Name), current == captured.Name,
                    () => Assign(targets, path, captured));
            }

            menu.DropDown(rect);
        }

        static void Assign(UnityEngine.Object[] targets, string path, Type type)
        {
            foreach (var target in targets)
            {
                var so = new SerializedObject(target);
                var p = so.FindProperty(path);
                if (p == null) continue;

                p.managedReferenceValue = type == null ? null : Activator.CreateInstance(type);
                p.isExpanded = true;
                so.ApplyModifiedProperties();
            }
        }

        static bool HasValue(SerializedProperty property)
            => !string.IsNullOrEmpty(property.managedReferenceFullTypename);

        static Type[] GetCandidates(Type baseType)
        {
            if (baseType == null) return Array.Empty<Type>();
            if (Cache.TryGetValue(baseType, out var cached)) return cached;

            var list = TypeCache.GetTypesDerivedFrom(baseType)
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition
                            && !typeof(UnityEngine.Object).IsAssignableFrom(t)
                            && Attribute.IsDefined(t, typeof(SerializableAttribute))
                            && t.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(t => t.Name)
                .ToArray();

            Cache[baseType] = list;
            return list;
        }

        static Type ResolveType(string managedReferenceTypename)
        {
            // "Assembly-CSharp Refactor.Condition" 형태
            if (string.IsNullOrEmpty(managedReferenceTypename)) return null;
            var parts = managedReferenceTypename.Split(' ');
            if (parts.Length != 2) return null;

            return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == parts[0])
                ?.GetType(parts[1]);
        }

        static string CurrentName(SerializedProperty property)
        {
            string full = property.managedReferenceFullTypename;
            if (string.IsNullOrEmpty(full)) return "None";
            var parts = full.Split(' ');
            string name = parts.Length == 2 ? parts[1] : full;
            int dot = name.LastIndexOf('.');
            return dot >= 0 ? name.Substring(dot + 1) : name;
        }
    }
}
