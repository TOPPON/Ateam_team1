using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using NCMB;

namespace Ateam
{
	/// <summary>
	/// NCMBのテスター
	/// </summary>
	public class NCMBTester : MonoBehaviour
	{
		private const string PATH_NCMBSETTING_PREFAB = "Prefabs/Settings/NCMBSetting";

		public bool IsRunning { private set; get; }

		/// <summary>
		/// 疎通テスト開始
		/// </summary>
		/// <param name="i_callback">完了コールバック</param>
		public void StartCommunicateTest(Action i_callback)
		{
			StartCoroutine(this.CommunicateTestProcess(i_callback));
		}
		
		/// <summary>
		/// 疎通テストのプロセス
		/// </summary>
		/// <param name="i_callback">完了コールバック</param>
		/// <returns></returns>
		private IEnumerator CommunicateTestProcess(Action i_callback)
		{
			this.IsRunning = true;

			UnityEngine.Object settingprefab = Resources.Load(PATH_NCMBSETTING_PREFAB);
			GameObject settingObject = GameObject.Instantiate<GameObject>(settingprefab as GameObject);
			NCMBSettings settings = settingObject != null ? settingObject.GetComponent<NCMBSettings>() : null;
			if (settings == null)
			{
				yield break;
			}

			// マルチシーンで意図しないシーンにインスタンス化しないように親子付け
			settingObject.transform.SetParent(this.transform);

			// アプリケーションキー、クライアントキー設定確認(空かどうかだけ)
			if (string.IsNullOrEmpty(settings.applicationKey) || string.IsNullOrEmpty(settings.clientKey))
			{
				EditorUtility.DisplayDialog("Error", "NCMBSettingsの設定不備があります。\n講師へ申し出をして下さい。", "OK");
				yield break;
			}

			// 疎通用送信データとしてローカルIPアドレスを取得
			string ipAddress = GetIpAddress();
			if (ipAddress == string.Empty)
			{
				EditorUtility.DisplayDialog("Error", "ローカルIPアドレスの取得に失敗しました。\nネットワークに未接続の可能性があります。", "OK");
				yield break;
			}

			bool isFinshed = false;
			NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("CommunicateTest");
			query.WhereEqualTo("IP", ipAddress);
			query.FindAsync((List<NCMBObject> i_objectList, NCMBException i_exception) =>
			{
				if (i_exception != null)
				{
					EditorUtility.DisplayDialog("Error", string.Format("NCMB接続時にエラーが発生しました。\n講師へ申し出をして下さい。\n(エラーメッセージ：{0})", i_exception.Message), "OK");
					return;
				}

				if (i_objectList.Count > 0)
				{
					i_objectList[0]["IP"] = ipAddress;
					i_objectList[0].Save();
				}
				else
				{
					NCMBObject newObject = new NCMBObject("CommunicateTest");
					newObject["IP"] = ipAddress;
					newObject.Save();
				}

				EditorUtility.DisplayDialog("完了", "疎通テストが正常に完了しました。", "OK");

				isFinshed = true;
			});

			while (isFinshed == false)
			{
				yield return null;
			}

			if (i_callback != null)
			{
				i_callback();
			}

			this.IsRunning = false;
		}

		/// <summary>
		/// IPアドレスを取得
		/// </summary>
		/// <returns>IPアドレス</returns>
		private string GetIpAddress()
		{
			try
			{
				string hostName = Dns.GetHostName();
				IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);

				IPAddress ipAddress = ipAddresses.ToList().Find(x => x.AddressFamily == AddressFamily.InterNetwork);
				if (ipAddress != null)
				{
					return ipAddress.ToString();
				}
			}
			catch (Exception)
			{
				return string.Empty;
			}

			return string.Empty;
		}
	}
}