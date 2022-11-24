using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Tes123 {
	[System.Serializable]
	public class MeshSerializable {
		public Vector3[] vertices;
		public int[] triangles;
		public Vector3[] normals;
	}

	[System.Serializable]
	public class SimpleSerializable {
		public string Name;
		public string ShaderName;
		public int ID;
		public int ParentID;
		public Vector3 Position;
		public Quaternion Rotation;
		public Vector3 Scale;
		public Color32 Color;
		public MeshSerializable MeshData;
		public bool isMesh = false;
		public bool isLight = false;
		public bool isCamera = false;

		public SimpleSerializable(Transform go, int rootID) {
			Name = go.name;
			MeshData = new MeshSerializable();
			ParentID = go.parent.GetHashCode() == rootID ? 0 : go.parent.GetHashCode();
			ID = go.GetHashCode();
			Position = go.position;
			Rotation = go.rotation;
			Scale = go.transform.localScale;
			MeshFilter meshFilter = go.GetComponent<MeshFilter>();
			if (meshFilter) {
				isMesh = true;
				Renderer renderer = go.GetComponent<Renderer>();
				ShaderName = renderer.material.shader.name;
				Color = renderer.material.color;
				MeshData.normals = meshFilter.mesh.normals;
				MeshData.triangles = meshFilter.mesh.triangles;
				MeshData.vertices = meshFilter.mesh.vertices;
			}
			Camera cam = go.GetComponent<Camera>();
			if (cam) {
				isCamera = true;
			}
			Light light = go.GetComponent<Light>();
			if (light) {
				isLight = true;
			}
		}

	}
	public static class LoadSaveControl {
		static string path = Application.persistentDataPath + "/test123.data";
		public static void Save(Transform root) {
			Transform[] transforms = root.GetComponentsInChildren<Transform>();
			StreamWriter fileOut = File.CreateText(path);
			foreach (var item in transforms) {
				if (item == root)
					continue;
				fileOut.WriteLine(JsonUtility.ToJson(new SimpleSerializable(item, root.GetHashCode())));
			}
			fileOut.Close();
		}
		public static void Load(Transform root) {
			Dictionary<int, Transform> Hash2name = new Dictionary<int, Transform>();
			StreamReader fileIn = File.OpenText(path);
			while (!fileIn.EndOfStream) {
				SimpleSerializable goData = JsonUtility.FromJson<SimpleSerializable>(fileIn.ReadLine());
				GameObject go = new GameObject(goData.Name);
				SetParent();
				if (goData.isMesh) {
					MeshFilter meshFilter = go.AddComponent<MeshFilter>();
					meshFilter.mesh = SetMesh();
					SetRenderer();
				}
				if (goData.isLight) {
					go.AddComponent<Light>();
				}
				if (goData.isCamera) {
					go.AddComponent<Camera>();
				}

				Mesh SetMesh() {
					Mesh mesh = new Mesh();
					mesh.vertices = goData.MeshData.vertices;
					mesh.triangles = goData.MeshData.triangles;
					mesh.normals = goData.MeshData.normals;
					return mesh;
				}
				void SetParent() {
					Hash2name.Add(goData.ID, go.transform);

					go.transform.SetParent(goData.ParentID == 0 ? root : Hash2name[goData.ParentID]);
					go.transform.position = goData.Position;
					go.transform.rotation = goData.Rotation;
					go.transform.localScale = goData.Scale;
				}
				void SetRenderer() {
					Renderer renderer = go.AddComponent<MeshRenderer>();
					renderer.material = new Material(Shader.Find(goData.ShaderName));
					renderer.material.color = goData.Color;
				}
			}
			Hash2name = null;
			fileIn.Close();
		}

	}
}