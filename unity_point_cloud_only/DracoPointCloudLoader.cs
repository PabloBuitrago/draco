using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public unsafe class DracoPointCloudLoader
{
	// Must stay the order to be consistent with C++ interface.
	[StructLayout (LayoutKind.Sequential)]
	private struct DracoToUnityPointCloud
	{
		public int num_vertices;
		public IntPtr position;
		public int[] num_color;
		public int num_points;
		public IntPtr vertex_indices;
	}

	private struct DecodedPoints
	{
		public Vector3[] vertices;
		public Color32[] colors;
	}

	[DllImport ("dracodec_unity")]
	private static extern int DecodePointCloudForUnity (byte[] buffer, int length, DracoToUnityPointCloud**tmp_point_cloud);

	static private int max_num_vertices_per_mesh = 60000;
	
	/*
	// Unity only support maximum 65534 vertices per mesh. So large meshes need to be splited.
	private void SplitMesh (List<DecodedPoints> points, ref List<DecodedPoints> split_points)
	{
		List<DecodedPoints> points_left = new List<DecodedPoints> ();
		for (int i = 0; i < points_left.Count; ++i) {
			points_left.Add (points [i]);
		}
		int num_sub_clouds = 0;

		while (points_left.Count > 0) {
			Debug.Log ("Colors left: " + points_left.Count.ToString ());
			num_sub_clouds++;
			List<DecodedPoints> tmp_left_points = new List<DecodedPoints> ();
			List<Color32> colors_extracted = new List<Color32> ();
			List<Vector3> vertices_extracted = new List<Vector3> ();

			//TODO: Get colors for each point

			DecodedPoints new_cloud = new DecodedPoints ();
			sub_cloud.colors = colors_extracted.ToArray ();
			sub_cloud.vertices = vertices_extracted.ToArray ();
			split_points.Add (sub_cloud);

			points_left = tmp_left_points;
		}
	}*/

	private float ReadFloatFromIntPtr (IntPtr data, int offset)
	{
		byte[] byte_array = new byte[4];
		for (int j = 0; j < 4; ++j) {
			byte_array [j] = Marshal.ReadByte (data, offset + j);
		}
		return BitConverter.ToSingle (byte_array, 0);
	}

	public int LoadPointsFromAsset (string asset_name, ref List<Vector3> points)
	{
		TextAsset asset = Resources.Load (asset_name, typeof(TextAsset)) as TextAsset;
		if (asset == null) {
			Debug.Log ("Didn't load file!");
			return -1;
		}
		byte[] bunny_data = asset.bytes;
		Debug.Log (bunny_data.Length.ToString ());
		if (bunny_data.Length == 0) {
			Debug.Log ("Didn't load bunny!");
			return -1;
		}



		return DecodePoints (bunny_data, ref points);
	}

	/*
	public IEnumerator LoadPointsFromURL(string url, ref Vector3 points) {
		WWW www = new WWW (url);
		yield return www;
		if (www.bytes.Length == 0)
			return -1;
		return DecodePoints (www.bytes, ref points);
	}
*/
	public unsafe int DecodePoints (byte[] data, ref List<Vector3> points)
	{

		DracoToUnityPointCloud* tmp_point_cloud;
		if (DecodePointCloudForUnity (data, data.Length, &tmp_point_cloud) <= 0) {
			Debug.Log ("Failed: Decoding error.");
			return -1;
		}

		Debug.Log ("Num points: " + tmp_point_cloud->num_points.ToString ());
		Debug.Log ("Num vertices: " + tmp_point_cloud->num_vertices.ToString ());

		// For floating point numbers, there's no Marshal functions could directly read from the unmanaged data.
		// TODO: Find better way to read float numbers.
		Vector3[] new_vertices = new Vector3[tmp_point_cloud->num_vertices];
		int[] new_colors = new Vector3[tmp_point_cloud->num_color];
		int byte_stride_per_value = 4;
		int num_value_per_vertex = 3;
		int byte_stride_per_vertex = byte_stride_per_value * num_value_per_vertex;
		for (int i = 0; i < tmp_point_cloud->num_vertices; ++i) {
			for (int j = 0; j < 8; ++j) {
				if (j > 3) {
					new_colors [i] [j] = 
						ReadFloatFromIntPtr (tmp_point_cloud->position, i * byte_stride_per_vertex + byte_stride_per_value * j) * 60;
				} else {
					new_vertices [i] [j] = 
						ReadFloatFromIntPtr (tmp_point_cloud->position, i * byte_stride_per_vertex + byte_stride_per_value * j) * 60;
				}
			}
		}

		Marshal.FreeCoTaskMem (tmp_point_cloud->position);
		Marshal.FreeCoTaskMem ((IntPtr)tmp_point_cloud);

		/*
		if (new_vertices.Length > 61000) {
			DecodedPoints decoded_points = new DecodedPoints ();
			decoded_points.vertices = new_vertices;
			List<DecodedPoints> split_points = new List<DecodedPoints> ();

			SplitMesh (decoded_points, ref split_points);
			for (int i = 0; i < split_points.Count; ++i) {
				Vector3 point = new Vector3[split_points.Count] ();
				points.Add (point);
			}
		} else {
			points.Add (new_vertices);
		}*/
		points.Add (new_vertices);
		return points.Count;


	}

}
