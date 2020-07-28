using UnityEngine;
using UnityEditor;

namespace LNE.ProstheticVision
{
	using ArrayExts;
	using ImageExts;
	using Threading;
	using Threading.UI;
	using VectorExts;

	public static class PreprocessedDataFactory
	{
		public enum DataType { Phosphene, Axon }

		/*
		 * Public methods
		 */

		/// <summary>
		/// Creates a data texture. A threadGroup is returned to keep track of creation progress; Null if GPU accelerated.
		/// </summary>
		public static ThreadGroup Create(DataType type, HeadsetModel headset, ElectrodePattern pattern, ElectrodeLayout layout, bool gpuAccel, string path)
		{
			if (gpuAccel)
			{
				StartGPU(type, headset, pattern, layout, path);
				return null;
			}
			else
			{
				return StartCPU(type, headset, pattern, layout, path);
			}
		}

		/*
		 * Private methods
		 */

		private static void StartGPU(DataType type, HeadsetModel headset, ElectrodePattern pattern, ElectrodeLayout layout, string path)
		{
			var shader = LoadAsset<ComputeShader>($"{type}");
			var kernel = shader.FindKernel("CSMain");

			var electrodeBuffer = default(ComputeBuffer);
			if (type == DataType.Phosphene)
			{
				var electrodes = pattern.GetElectrodePositions(layout);
				electrodeBuffer = new ComputeBuffer(electrodes.Length, sizeof(float) * 2);
				electrodeBuffer.SetData(electrodes);
				shader.SetBuffer(kernel, "_electrodes", electrodeBuffer);
			}

			var texture = headset.CreateRenderTexture();
			shader.SetTexture(kernel, "_result", texture);
			shader.SetVector("_headset_diameter", headset.GetRetinalDiameter());
			shader.SetVector("_headset_resolution", headset.GetResolution());

			shader.Dispatch(kernel, headset.GetWidth() / 8, headset.GetHeight() / 8, 1);
			electrodeBuffer?.Dispose();

			Texture2D asset = texture.ToTexture2D(TextureFormat.RGBAFloat, true);
			texture.Release();

			asset.anisoLevel = 0;
			asset.filterMode = FilterMode.Point;

			if (type == DataType.Phosphene)
			{
				AddRandomSeeds(asset);
			}

			SaveAsset(asset, path);
		}

		private static ThreadGroup StartCPU(DataType type, HeadsetModel headset, ElectrodePattern pattern, ElectrodeLayout layout, string path)
		{
			// declare colour matrix and electrodes (if needed)
			var matrix = new Color[headset.GetWidth(), headset.GetHeight()];
			var electrodes = type == DataType.Phosphene ? pattern.GetElectrodePositions(layout) 
														: default;

			// setup threadGroup
			var threadGroup = new ThreadGroup();
			threadGroup.OnAllThreadsFinished += () =>
			{
				EditorCallback.InvokeOnMainThread(() =>
				{
					var asset = headset.CreateTexture();
					asset.SetPixels(matrix.Flatten(false, true, true));
					asset.Apply();
					SaveAsset(asset, path);
				});
			};

			// generate the texture asynchronously
			threadGroup.ProcessArray(headset.GetWidth(), (i) =>
			{
				var n = headset.GetHeight();
				for (int j = 0; j < n; j++)
				{
					switch (type)
					{
						case DataType.Phosphene:
							matrix[i, j] = PhospheneMatrix.CalculatePoint(i, j, headset, electrodes).ToColour();
							matrix[i, j] = AddRandomSeeds(matrix[i, j]);
							break;
						case DataType.Axon:
							matrix[i, j] = AxonMatrix.CalculatePoint(i, j, headset).Colour;
							break;
					}
				}
			});

			// provide caller with threadGroup for completion statistics
			return threadGroup;
		}

		private static void AddRandomSeeds(Texture2D phospheneTexture)
		{
			phospheneTexture.Apply_Parallelised(AddRandomSeeds);
		}

		private static Color AddRandomSeeds(Color pixel)
		{
			var random = new System.Random((int)AuxMath.CantorPair(pixel.r, pixel.g));

			pixel.b = (float)random.NextDouble();
			pixel.a = (float)random.NextDouble();

			return pixel;
		}

		private static T LoadAsset<T>(string filter) where T : Object
		{
			filter = (filter ?? "") + $" t:{typeof(T).Name}";

			var assets = AssetDatabase.FindAssets(filter);
			
			Debug.Assert(assets.Length == 1, $"{assets.Length} assets found.");

			var path = AssetDatabase.GUIDToAssetPath(assets[0]);
			var asset = AssetDatabase.LoadAssetAtPath<T>(path);

			return asset;
		}

		private static void SaveAsset(Object asset, string path)
		{
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();

			EditorApplication.Beep();
		}
	}
}
