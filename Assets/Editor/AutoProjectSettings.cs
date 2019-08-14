using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Rendering;

namespace Ateam
{
	/// <summary>
	/// プロジェクトの自動設定クラス
	/// </summary>
	[InitializeOnLoad]
	public class AutoProjectSettings
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		static AutoProjectSettings()
		{
			EditorApplication.update += ApplySettings;
		}

		/// <summary>
		/// 設定の適用
		/// </summary>
		private static void ApplySettings()
		{
			EditorApplication.update -= ApplySettings;

			GraphicsSetting();
		}

		/// <summary>
		/// グラフィック設定
		/// </summary>
		private static void GraphicsSetting()
		{
			TierSettings tier1Settings = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier1);
			TierSettings tier2Settings = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier2);
			TierSettings tier3Settings = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier3);

			tier1Settings.standardShaderQuality = ShaderQuality.Low;
			tier1Settings.renderingPath = RenderingPath.VertexLit;

			tier2Settings.standardShaderQuality = ShaderQuality.Medium;
			tier2Settings.renderingPath = RenderingPath.VertexLit;

			tier3Settings.standardShaderQuality = ShaderQuality.High;
			tier3Settings.renderingPath = RenderingPath.VertexLit;

			EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier1, tier1Settings);
			EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier2, tier2Settings);
			EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier3, tier3Settings);
		}
	}
}
