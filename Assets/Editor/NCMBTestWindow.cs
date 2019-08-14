using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ateam
{
	/// <summary>
	/// NCMBテスト ウィンドウ
	/// </summary>
	public class NCMBTestWindow : EditorWindow
	{
		[SerializeField]
		private string defaultScenePath = string.Empty;

		/// <summary>
		/// 疎通テスト
		/// </summary>
		[MenuItem("NCMB/Tester")]
		private static void CommunicateTest()
		{
			NCMBTestWindow window = GetWindow<NCMBTestWindow>();
			window.titleContent = new GUIContent("NCMBテスター");
		}

		/// <summary>
		/// OnGUI
		/// </summary>
		private void OnGUI()
		{
			EditorGUILayout.LabelField("疎通テスト");

			EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
			if (GUILayout.Button("実行"))
			{
				Scene defaultScene = EditorSceneManager.GetActiveScene();
				EditorSceneManager.OpenScene(string.Format("Assets/Ateam/Scenes/{0}.unity", Define.Scenes.NCMBTEST), OpenSceneMode.Additive);
				EditorSceneManager.CloseScene(defaultScene, false);
				this.defaultScenePath = defaultScene.path;

				EditorApplication.ExecuteMenuItem("Edit/Play");
			}
			EditorGUI.EndDisabledGroup();
		}

		/// <summary>
		/// Update
		/// </summary>
		private void Update()
		{
			if (EditorApplication.isPlaying == false)
			{
				return;
			}

			NCMBTester tester = EditorSceneManager.GetActiveScene().GetRootGameObjects().Select(x => x.GetComponent<NCMBTester>()).First();
			if (tester != null && tester.IsRunning == false)
			{
				tester.StartCommunicateTest(() =>
				{
					EditorApplication.ExecuteMenuItem("Edit/Play");
				});
			}
		}

		/// <summary>
		/// OnDestroy
		/// </summary>
		private void OnDestroy()
		{
			if (string.IsNullOrEmpty(this.defaultScenePath) || EditorApplication.isPlaying == true)
			{
				return;
			}

			Scene currentScene = EditorSceneManager.GetActiveScene();
			EditorSceneManager.OpenScene(this.defaultScenePath);
			EditorSceneManager.CloseScene(currentScene, true);
			this.defaultScenePath = string.Empty;
		}
	}
}