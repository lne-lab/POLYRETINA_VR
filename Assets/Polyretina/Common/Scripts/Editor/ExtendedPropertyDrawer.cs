using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace LNE.UI
{
	using Attributes;
	using ReflectionExts;

	public class ExtendedPropertyDrawer : PropertyDrawer
	{
		/*
		 * Static
		 */

		private static Dictionary<Type, (Action<ExtendedPropertyDrawer>, Func<ExtendedPropertyDrawer, float>)> drawers
			= new Dictionary<Type, (Action<ExtendedPropertyDrawer>, Func<ExtendedPropertyDrawer, float>)>();

		public static void AddDrawer(Type type, Action<ExtendedPropertyDrawer> onGUI, Func<ExtendedPropertyDrawer, float> getPropertyHeight)
		{
			if (drawers == null)
				drawers = new Dictionary<Type, (Action<ExtendedPropertyDrawer>, Func<ExtendedPropertyDrawer, float>)>();

			drawers[type] = (onGUI, getPropertyHeight);
		}

		/*
		 * Instance
		 */

		/*
		 * Public fields
		 */

		public Rect position;

		/*
		 * Private fields
		 */

		private bool checkChanges;

		/*
		 * Public properties
		 */

		public Rect Area { get; private set; }

		public SerializedProperty Property { get; private set; }

		public GUIContent DefaultLabel { get; private set; }

		public GUIContent Label => new GUIContent(GetAttribute<CustomLabelAttribute>()?.GUIContent ?? DefaultLabel);

		public MonoBehaviour Component => Property.serializedObject.targetObject as MonoBehaviour;

		public object Target => fieldInfo.GetValue(Component);

		public float LineHeight => EditorGUIUtility.singleLineHeight;

		public float FieldHeight => LineHeight + 2;

		public bool ExecuteInEditMode
		{
			get
			{
				return HasAttribute<ExecuteInEditMode>() ||
						HasAttribute<ExecuteAlways>() ||
						Component.GetType().GetCustomAttribute<ExecuteInEditMode>() != null ||
						Component.GetType().GetCustomAttribute<ExecuteAlways>() != null;
			}
		}

		/*
		 * Public methods
		 */

		public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			BeginGUI(rect, property, label);

			var found = drawers.TryGetValue(fieldInfo.FieldType, out var drawer);
			if (found && drawer.Item1 != null)
			{
				drawer.Item1(this);
			}
			else
			{
				EditorGUI.PropertyField(Area, Property, Label);
			}

			EndGUI();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			Property = property;
			DefaultLabel = label;

			var found = drawers.TryGetValue(fieldInfo.FieldType, out var drawer);
			if (found && drawer.Item2 != null)
			{
				return drawer.Item2(this);
			}
			else
			{
				return LineHeight;
			}
		}

		public void BeginGUI(Rect rect, SerializedProperty property, GUIContent defaultLabel)
		{
			position = rect;
			position.height = LineHeight;

			Area = rect;
			Property = property;
			DefaultLabel = defaultLabel;

			checkChanges = true;
			EditorGUI.BeginChangeCheck();
		}
		
		public void EndChangeCheckEarly()
		{
			EndChangeCheck();
		}

		public void EndGUI()
		{
			EndChangeCheck();
		}

		public bool Button(GUIStyle style = null)
		{
			if (style != null)	return GUI.Button(position, Label, style);
			else				return GUI.Button(position, Label);
		}

		public void LabelField(string label, GUIStyle style = null)
		{
			if (style != null)	EditorGUI.LabelField(position, label, style);
			else				EditorGUI.LabelField(position, label);
		}

		public void PropertyField(GUIStyle style = null)
		{
			PrefixLabel(style);
			EditorGUI.PropertyField(position, Property, new GUIContent(" "));
		}

		public void Slider(float min, float max, GUIStyle style = null)
		{
			PrefixLabel(style);

			if (fieldInfo.FieldType == typeof(int))
			{
				EditorGUI.IntSlider(position, Property, (int)min, (int)max, new GUIContent(" "));
			}
			else if (fieldInfo.FieldType == typeof(float))
			{
				EditorGUI.Slider(position, Property, min, max, new GUIContent(" "));
			}
		}

		public void PrefixLabel(GUIStyle style = null)
		{
			PrefixLabel(Label, style);
		}

		public void PrefixLabel(GUIContent label, GUIStyle style = null)
		{
			if (style != null)
				EditorGUI.PrefixLabel(position, label, style);
			else
				EditorGUI.PrefixLabel(position, label);
		}

		public Color SetColour(Color colour)
		{
			var prevCol = GUI.color;
			GUI.color = colour;
			return prevCol;
		}

		public void SplitFromRight(float pixels, out Rect left, out Rect right)
		{
			left = new Rect(position);
			right = new Rect(position);

			left.width = left.width - (pixels + 4);

			right.x = right.xMax - pixels;
			right.width = pixels;
		}

		public T GetTarget<T>()
		{
			return (T)Target;
		}

		public T GetAttribute<T>() where T : Attribute
		{
			return fieldInfo.GetCustomAttribute<T>();
		}

		public bool HasAttribute<T>() where T : Attribute
		{
			return fieldInfo.HasCustomAttribute<T>();
		}

		/*
		 * Private methods
		 */

		private void EndChangeCheck()
		{
			var changed = checkChanges && EditorGUI.EndChangeCheck();
			var ovcAttribute = GetAttribute<OnValueChangedAttribute>();
			if (changed && ovcAttribute != null && (Application.isPlaying || ExecuteInEditMode || ovcAttribute.executeInEditMode))
			{
				Property.serializedObject.ApplyModifiedProperties();
				ovcAttribute.Invoke(Component, fieldInfo);
			}

			checkChanges = false;
		}
	}
}
