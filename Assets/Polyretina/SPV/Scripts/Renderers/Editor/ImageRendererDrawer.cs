using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace LNE.ProstheticVision.UI
{
	using ObjectExts;
	using Reflection;
	using ReflectionExts;
	using LNE.UI;
	using LNE.UI.Attributes;

	[CustomPropertyDrawer(typeof(ImageRenderer))]
	public class ImageRendererDrawer : ExtendedPropertyDrawer
	{
		/*
		 * Private fields/properties
		 */

		private static UnityEngine.Object objectReference;
		private static Type objectType => objectReference.GetType();
		private static bool objectIsNull => objectReference == null;
		private static bool objectIsNotNull => objectReference != null;

		private static float space => 9;

		/*
		 * Callback methods
		 */

		[InitializeOnLoadMethod]
		static void AddDrawer()
		{
			AddDrawer(typeof(ImageRenderer), OnGUI, GetPropertyHeight);
		}

		public static void OnGUI(ExtendedPropertyDrawer drawer)
		{
			objectReference = drawer.Property.objectReferenceValue;

			drawer.position.y += space;

			DrawBox(drawer);
			DrawTitle(drawer);

			if (objectIsNull)
			{
				return;
			}

			DrawOnToggle(drawer);

			var foldout = DrawFoldout(drawer);
			if (foldout == false)
			{
				return;
			}

			DrawParameters(drawer);

			drawer.position.y += 2.25f;

			DrawSelectButton(drawer);
			DrawSaveButton(drawer);
		}

		public static float GetPropertyHeight(ExtendedPropertyDrawer drawer)
		{
			objectReference = drawer.Property.objectReferenceValue;

			var height = space + drawer.FieldHeight;

			if (objectIsNull || Mirror.GetValue<bool>(objectReference, "foldout") == false)
			{
				return height;
			}

			foreach (var field in objectType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
			{
				if (field.HasCustomAttribute<HeaderAttribute>())
				{
					height += drawer.FieldHeight * 1.5f;
				}

				var space = field.GetCustomAttribute<SpaceAttribute>();
				if (space != null)
				{
					height += space.height;
				}

				height += drawer.FieldHeight;
			}

			height += drawer.FieldHeight * 1.125f;

			return height;
		}

		/*
		 * Private methods
		 */

		private static void DrawBox(ExtendedPropertyDrawer drawer)
		{
			var backgroundRect = drawer.Area;
			backgroundRect.y -= 2;
			backgroundRect.width += 2;
			backgroundRect.height += 2;

			backgroundRect.y += space;
			backgroundRect.height -= space;

			var shadowRect = backgroundRect;
			shadowRect.x -= 1;
			shadowRect.y -= 1;
			shadowRect.width += 2;
			shadowRect.height += 2;

			GUI.DrawTexture(shadowRect, ColourTexture.Get(0, 0, 0, 255));
			GUI.DrawTexture(backgroundRect, ColourTexture.Get(180, 180, 180, 255));
		}

		private static void DrawTitle(ExtendedPropertyDrawer drawer)
		{
			var style = new GUIStyle(EditorStyles.boldLabel) { fontSize = 11 };

			drawer.position.x += 18;
			drawer.position.width -= 18;
			drawer.PropertyField(style);

			drawer.EndChangeCheckEarly();

			// reset reference in case it was changed
			objectReference = drawer.Property.objectReferenceValue;
		}

		private static void DrawOnToggle(ExtendedPropertyDrawer drawer)
		{
			drawer.position.x -= 15;
			drawer.position.width = 16;

			var onField = objectType.GetField("on");
			var onValue = onField.GetValue<bool>(objectReference);
			onField.SetValue(
				objectReference,
				EditorGUI.ToggleLeft(drawer.position, "", onValue)
			);
		}

		private static bool DrawFoldout(ExtendedPropertyDrawer drawer)
		{
			drawer.position.x -= 3;
			drawer.position.width = 0;

			var foldoutField = objectType.GetField("foldout");
			var foldoutValue = foldoutField.GetValue<bool>(objectReference);
			foldoutField.SetValue(
				objectReference,
				EditorGUI.Foldout(drawer.position, foldoutValue, "")
			);

			return foldoutValue;
		}

		private static void DrawParameters(ExtendedPropertyDrawer drawer)
		{
			drawer.position.width = drawer.Area.width;

			EditorGUI.BeginChangeCheck();
			EditorGUI.indentLevel++;

			foreach (var field in objectType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
			{
				DrawDecorators(drawer, field);
				var label = field.GetCustomAttribute<CustomLabelAttribute>()?.label;

				drawer.position.y += drawer.FieldHeight;

				if (field.FieldType == typeof(int))
				{
					DrawIntField(drawer, field, label);
				}
				else if (field.FieldType == typeof(float))
				{
					DrawFloatField(drawer, field, label);
				}
				else if (field.FieldType == typeof(bool))
				{
					DrawBoolField(drawer, field);
				}
				else if (field.FieldType.IsEnum)
				{
					DrawEnumField(drawer, field);
				}
				else if (field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
				{
					DrawObjectField(drawer, field);
				}
				else
				{
					EditorGUI.LabelField(drawer.position, UnityApp.ToDisplayFormat(field.Name), "[Select asset to edit]");
				}
			}

			EditorGUI.indentLevel--;

			var changed = EditorGUI.EndChangeCheck();
			if (changed && objectIsNotNull)
			{
				if (Application.isPlaying)
				{
					Mirror.InvokeMethod(objectReference, "Update");

					if (Settings.SaveRuntimeChangesAutomatically)
					{
						var target = objectReference as ImageRenderer;

						target.CopyTo(target.original);
						EditorUtility.SetDirty(target.original);
					}
				}
				else
				{
					EditorUtility.SetDirty(objectReference);
				}
			}
		}

		private static void DrawDecorators(ExtendedPropertyDrawer drawer, FieldInfo field)
		{
			// header decorator
			var header = field.GetCustomAttribute<HeaderAttribute>();
			if (header != null)
			{
				drawer.position.y += 27;
				drawer.LabelField(header.header, EditorStyles.boldLabel);
			}

			// space decorator
			var space = field.GetCustomAttribute<SpaceAttribute>();
			if (space != null)
			{
				drawer.position.y += space.height;
			}
		}

		private static void DrawIntField(ExtendedPropertyDrawer drawer, FieldInfo field, string label = null)
		{
			var range = field.GetCustomAttribute<RangeAttribute>();
			if (range != null)
			{
				field.SetValue(
					objectReference,
					EditorGUI.IntSlider(
						drawer.position,
						label ?? UnityApp.ToDisplayFormat(field.Name),
						field.GetValue<int>(objectReference),
						Mathf.RoundToInt(range.min),
						Mathf.RoundToInt(range.max))
				);
			}
			else
			{
				field.SetValue(
					objectReference,
					EditorGUI.IntField(
						drawer.position,
						label ?? UnityApp.ToDisplayFormat(field.Name),
						field.GetValue<int>(objectReference))
				);
			}
		}

		private static void DrawFloatField(ExtendedPropertyDrawer drawer, FieldInfo field, string label = null)
		{
			var range = field.GetCustomAttribute<RangeAttribute>();
			if (range != null)
			{
				field.SetValue(
					objectReference,
					EditorGUI.Slider(
						drawer.position,
						label ?? UnityApp.ToDisplayFormat(field.Name),
						field.GetValue<float>(objectReference),
						range.min,
						range.max)
				);
			}
			else
			{
				field.SetValue(
					objectReference,
					EditorGUI.FloatField(
						drawer.position,
						label ?? UnityApp.ToDisplayFormat(field.Name),
						field.GetValue<float>(objectReference))
				);
			}
		}

		private static void DrawBoolField(ExtendedPropertyDrawer drawer, FieldInfo field)
		{
			field.SetValue(
				objectReference,
				EditorGUI.Toggle(
					drawer.position,
					UnityApp.ToDisplayFormat(field.Name),
					field.GetValue<bool>(objectReference))
			);
		}

		private static void DrawEnumField(ExtendedPropertyDrawer drawer, FieldInfo field)
		{
			field.SetValue(
				objectReference,
				EditorGUI.EnumPopup(
					drawer.position,
					UnityApp.ToDisplayFormat(field.Name),
					field.GetValue<Enum>(objectReference))
			);
		}

		private static void DrawObjectField(ExtendedPropertyDrawer drawer, FieldInfo field)
		{
			field.SetValue(
				objectReference,
				EditorGUI.ObjectField(
					drawer.position,
					UnityApp.ToDisplayFormat(field.Name),
					field.GetValue<UnityEngine.Object>(objectReference),
					field.FieldType,
					false)
			);
		}

		private static void DrawSelectButton(ExtendedPropertyDrawer drawer)
		{
			drawer.position.x += 2;
			drawer.position.y += drawer.FieldHeight;

			if (Settings.SaveRuntimeChangesAutomatically || Application.isPlaying == false)
			{
				drawer.position.width -= 2;
			}
			else
			{
				drawer.position.width -= 3;
				drawer.position.width /= 2;
			}

			if (GUI.Button(drawer.position, "Select"))
			{
				Selection.activeObject = objectReference;
			}
		}

		private static void DrawSaveButton(ExtendedPropertyDrawer drawer)
		{
			if (Settings.SaveRuntimeChangesAutomatically || Application.isPlaying == false)
				return;

			drawer.position.x += drawer.position.width + 2;

			if (GUI.Button(drawer.position, "Save"))
			{
				var target = objectReference as ImageRenderer;

				target.CopyTo(target.original);
				EditorUtility.SetDirty(target.original);
			}
		}
	}
}
