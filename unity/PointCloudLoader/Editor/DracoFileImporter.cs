using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DracoFileImporter : AssetPostprocessor
{

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
		foreach (string str in importedAssets) {
			Debug.Log (str + " -- type is -- " + AssetDatabase.GetMainAssetTypeAtPath (str));
			// Debug
			if (str.IndexOf (".obj") > 0) {
				Debug.Log (AssetDatabase.GetMainAssetTypeAtPath (str));
				string[] subs = AssetDatabase.GetSubFolders(str);
				for (int i = 0; i < subs.Length; ++i) {
					Debug.Log ("Sub: " + subs [i] + " is " + AssetDatabase.GetMainAssetTypeAtPath (subs [i]));
				}
			}

			if (str.IndexOf(".drc.bytes") == -1) {
				return;
			}
				
			List<Vector3> points = new List<Vector3>();
			List<Color32> colors = new List<Color32>();
			DracoPointCloudLoader draco_loader = new DracoPointCloudLoader ();
			// TODO: Fix name
			str.LastIndexOf('/');
			int length = str.Length - ".drc.bytes".Length - str.LastIndexOf('/') - 1;
			string file_name = str.Substring(str.LastIndexOf('/') + 1, length);
			Debug.Log ("File name: " + file_name);
			int num_faces = draco_loader.LoadPointsFromAsset (file_name+".drc", ref points, ref colors);

			// TODO: points.
			if (num_faces > 0) {

				GameObject new_asset = new GameObject ();
				new_asset.hideFlags = HideFlags.HideInHierarchy;
				for (int i = 0; i < points.Count; ++i) {
					//UnityEngine.Object.Instantiate (points [i], new_asset.transform);


					GameObject sub_object = new GameObject ();

					/*
					sub_object.hideFlags = HideFlags.HideInHierarchy;
					sub_object.AddComponent<MeshFilter> ();
					sub_object.AddComponent<MeshRenderer> ();
					sub_object.GetComponent<MeshFilter> ().mesh = UnityEngine.Object.Instantiate(points [i]);
					sub_object.transform.parent = new_asset.transform;
					*/
				}
				PrefabUtility.CreatePrefab ("Assets/Resources/" + file_name + ".prefab", new_asset);
				//AssetDatabase.CreateAsset (new_asset, "Assets/Resources/" + file_name + ".asset");

				/*
				AssetDatabase.CreateAsset (points [0], "Assets/Resources/" + file_name + ".asset");
				AssetDatabase.SaveAssets ();
				for (int i = 1; i < points.Count; ++i) {
					AssetDatabase.AddObjectToAsset (points [i], points [0]);
					AssetDatabase.ImportAsset (AssetDatabase.GetAssetPath (points [i]));
				}
				*/

				// AssetDatabase.Refresh ();

				//var go = (GameObject)AssetDatabase.LoadAssetAtPath ("Assets/decompressed_mesh", typeof(GameObject));
				//PrefabUtility.CreatePrefab("Assets/sphere_new.prefab", go);
				/*
				var go = new GameObject ();
				go.AddComponent <MeshFilter>();
				go.AddComponent <MeshRenderer>();
				go.GetComponent<MeshFilter> ().mesh = mesh;
				*/
				//PrefabUtility.CreatePrefab("Assets/sphere.prefab", go);
			} else {
				// TODO: Report error.
			}

			//var new_go = (GameObject)AssetDatabase.LoadAssetAtPath ("Assets/Resources/bunny_norm.obj", typeof(GameObject));
			//PrefabUtility.CreatePrefab("Assets/sphere_new.prefab", new_go);
			//AssetDatabase.AddObjectToAsset(

			//EditorUtility.DisplayDialog ("Post-Process", "It's post processing all assets", "Yes");
		}
	}

	void OnPreprocessModel (GameObject g) {
		//EditorUtility.DisplayDialog ("Pre-Process", "It's pre processing", "Yes");
	}
} 

