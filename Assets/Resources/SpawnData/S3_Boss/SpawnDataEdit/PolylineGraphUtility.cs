using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PolylineGraphUtility
{
	private const int padding = 20;

	private static GUIStyle labelStyle;
	private static GUIStyle GetLabelStyle()
	{
		if (labelStyle == null)
		{
			labelStyle = new GUIStyle(EditorStyles.miniLabel);
			labelStyle.fontSize = 12;
			labelStyle.normal.textColor = Color.white;
		}
		return labelStyle;
	}

	public static void DrawPolylineGraph(List<Vector3> points, System.Func<Vector3, Vector2> selector, string title, Vector2 min, Vector2 max)
	{
		if (points == null || points.Count < 2) return;

		float totalWidth = EditorGUIUtility.currentViewWidth - 40;
		float graphWidth = totalWidth;
		float graphHeight = graphWidth / 2f;

		GUILayout.BeginVertical();
		DrawGraph(title, points, selector, min, max, graphWidth, graphHeight, true);
		GUILayout.EndVertical();
	}

	private static void DrawGraph(string title, List<Vector3> data, System.Func<Vector3, Vector2> selector, Vector2 min, Vector2 max, float graphWidth, float graphHeight, bool showZeroOnYAxisOnly)
	{
		GUIStyle style = GetLabelStyle();
		GUILayout.Label(title, EditorStyles.boldLabel);

		Rect rect = GUILayoutUtility.GetRect(graphWidth, graphHeight);
		EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f));

		Vector2 size = max - min;

		Handles.BeginGUI();

		float xMin = rect.x + padding;
		float xMax = rect.x + rect.width - padding;
		float yTop = rect.y + padding;
		float yBottom = rect.y + rect.height - padding;
		Handles.color = new Color(1, 1, 1, 0.05f);
		Handles.DrawLine(new Vector2(xMin, yTop), new Vector2(xMin, yBottom));
		Handles.DrawLine(new Vector2(xMax, yTop), new Vector2(xMax, yBottom));
		Handles.DrawLine(new Vector2(xMin, yTop), new Vector2(xMax, yTop));
		Handles.DrawLine(new Vector2(xMin, yBottom), new Vector2(xMax, yBottom));

		// X/Y 標籤
		GUI.color = Color.white;
		GUI.Label(new Rect(rect.x + padding - 10, rect.y + rect.height - padding + 4, 40, 16), min.x.ToString("F0"), style);
		GUI.Label(new Rect(rect.x + rect.width - padding - 15, rect.y + rect.height - padding + 4, 40, 16), max.x.ToString("F0"), style);
		if (showZeroOnYAxisOnly)
			GUI.Label(new Rect(rect.x, rect.y + rect.height - padding - 8, padding - 5, 16), "0", style);
		GUI.Label(new Rect(rect.x, rect.y + padding - 8, padding - 5, 16), max.y.ToString("F0"), style);

		// 折線與節點（先畫線）
		for (int i = 0; i < data.Count - 1; i++)
		{
			Vector2 p1 = selector(data[i]);
			Vector2 p2 = selector(data[i + 1]);

			Vector2 norm1 = new Vector2((p1.x - min.x) / size.x, (p1.y - min.y) / size.y);
			Vector2 norm2 = new Vector2((p2.x - min.x) / size.x, (p2.y - min.y) / size.y);

			Vector2 guiP1 = new Vector2(rect.x + padding + norm1.x * (rect.width - 2 * padding),
										rect.y + padding + (1 - norm1.y) * (rect.height - 2 * padding));  // Y 軸反轉
			Vector2 guiP2 = new Vector2(rect.x + padding + norm2.x * (rect.width - 2 * padding),
										rect.y + padding + (1 - norm2.y) * (rect.height - 2 * padding));  // Y 軸反轉

			Handles.color = Color.cyan;
			Handles.DrawLine(guiP1, guiP2);
		}

		// 畫點和標籤（最後畫，確保在最上層）
		for (int i = 0; i < data.Count; i++)
		{
			Vector2 p = selector(data[i]);
			Vector2 norm = new Vector2((p.x - min.x) / size.x, (p.y - min.y) / size.y);
			Vector2 guiP = new Vector2(rect.x + padding + norm.x * (rect.width - 2 * padding),
									   rect.y + padding + (1 - norm.y) * (rect.height - 2 * padding)); // Y 軸反轉

			// 起點和終點顏色與大小不同
			bool isStart = (i == 0);
			bool isEnd = (i == data.Count - 1);
			Handles.color = (isStart || isEnd) ? Color.red : Color.yellow;
			float radius = (isStart || isEnd) ? 4f : 2f;
			Handles.DrawSolidDisc(guiP, Vector3.forward, radius);

			// 標籤內容
			string labelText = isStart ? $"Start ({p.x:F0},{p.y:F0})" :
							  isEnd ? $"End ({p.x:F0},{p.y:F0})" :
							  $"({p.x:F0},{p.y:F0})";

			Vector2 labelOffset = new Vector2(5, -15); // 偏移讓標籤不蓋點
			Handles.Label(guiP + labelOffset, labelText, style);
		}

		Handles.EndGUI();
	}

}