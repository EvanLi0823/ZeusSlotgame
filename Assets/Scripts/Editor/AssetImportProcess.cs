using UnityEngine;
using System.Collections;
using UnityEditor;
using TextureCompressionQuality = UnityEditor.TextureCompressionQuality;

public class AssetImportProcess : AssetPostprocessor {
	private const string CHANGE_FOLDOR = "ManualPicture";
	void OnPreprocessTexture() {
		TextureImporter textureImporter = (TextureImporter)assetImporter;

		if (textureImporter.textureType.Equals (TextureImporterType.Sprite)) {
			string path = textureImporter.assetPath;
			if (path.Contains ("Libs")) {
//				OnPostprocessTexture444 (texture);
				return;
			}
                textureImporter.mipmapEnabled = false;
                TextureImporterPlatformSettings psAndrdoid = textureImporter.GetPlatformTextureSettings("Android");
                TextureImporterPlatformSettings psIphone = textureImporter.GetPlatformTextureSettings("iPhone");
                psAndrdoid.overridden = true;
                psIphone.overridden = true;
                Debug.Log(this.assetPath);
                //按照jpg的无alpha，png的有alpha处理,后暂不处理
//                if(this.assetPath.EndsWith("jpg"))
//                {
//	                psAndrdoid.format = TextureImporterFormat.ETC2_RGB4;
//	                psIphone.format = TextureImporterFormat.ASTC_RGB_6x6;
//                }
//                else
//                {
	                psAndrdoid.format = TextureImporterFormat.ETC2_RGBA8;
	                psIphone.format = TextureImporterFormat.ASTC_6x6;
//                }
			if (path.Contains("SpriteAtlas"))
			{
				psAndrdoid.compressionQuality = (int)TextureCompressionQuality.Best;
				psIphone.compressionQuality = (int)TextureCompressionQuality.Best;
			}
			else
			{
				psAndrdoid.compressionQuality = (int)TextureCompressionQuality.Fast;
				psIphone.compressionQuality = (int)TextureCompressionQuality.Best;
			}
			
                
                textureImporter.SetPlatformTextureSettings(psAndrdoid);
                textureImporter.SetPlatformTextureSettings(psIphone);
		}
//		textureImporter.SetPlatformTextureSettings("Android", textureImporter.maxTextureSize,
//			TextureImporterFormat.ETC2_RGBA8, (int) UnityEditor.TextureCompressionQuality.Fast, false);
//
//		textureImporter.SetPlatformTextureSettings("iPhone", textureImporter.maxTextureSize,
//			TextureImporterFormat.ASTC_RGBA_6x6, (int) UnityEditor.TextureCompressionQuality.Fast, false);
//		EditorUtility.SetDirty (textureImporter);
//		textureImporter.SaveAndReimport ();
//		AssetDatabase.Refresh ();
//		textureImporter.filterMode = FilterMode.Bilinear;
//		EditorUtility.DisplayDialog ("warnning", "请记得打图集", "ok");
	}
	
	void OnPreprocessAudio()
	{
			AudioImporter audioImporter = (AudioImporter)assetImporter;
			AudioImporterSampleSettings settings = audioImporter.defaultSampleSettings;
//			AudioImporterSampleSettings settings = new AudioImporterSampleSettings();
			settings.loadType = AudioClipLoadType.Streaming;
			settings.quality = 0.5f;
			settings.compressionFormat = AudioCompressionFormat.Vorbis;
//			audioImporter.defaultSampleSettings = settings;
			
//			audioImporter.SetOverrideSampleSettings("Android",settings);
//			audioImporter.SetOverrideSampleSettings("iOS",settings);
//			audioImporter.SetOverrideSampleSettings("Default",settings);
	}

	/// <summary>
	/// 不支持alpha通道
	/// Raises the postprocess texture dither565 event.
	/// </summary>
	/// <param name="texture">Texture.</param>
	void OnPostprocessTextureDither565 (Texture2D texture)
	{
		if(assetPath.Contains ("dither565"))
		{
			Log.Trace ("_dither565");
			var texw = texture.width;
			var texh = texture.height;

			var pixels = texture.GetPixels ();
			var offs = 0;

			var k1Per31 = 1.0f / 31.0f;

			var k1Per32 = 1.0f / 32.0f;
			var k5Per32 = 5.0f / 32.0f;
			var k11Per32 = 11.0f / 32.0f;
			var k15Per32 = 15.0f / 32.0f;

			var k1Per63 = 1.0f / 63.0f;

			var k3Per64 = 3.0f / 64.0f;
			var k11Per64 = 11.0f / 64.0f;
			var k21Per64 = 21.0f / 64.0f;
			var k29Per64 = 29.0f / 64.0f;

			var k_r = 32; //R&B压缩到5位，所以取2的5次方
			var k_g = 64; //G压缩到6位，所以取2的6次方

			for(var y = 0; y < texh; y++){
				for(var x = 0; x < texw; x++){
					float r = pixels [offs].r;
					float g = pixels [offs].g;
					float b = pixels [offs].b;

					var r2 = Mathf.Clamp01 (Mathf.Floor (r * k_r) * k1Per31);
					var g2 = Mathf.Clamp01 (Mathf.Floor (g * k_g) * k1Per63);
					var b2 = Mathf.Clamp01 (Mathf.Floor (b * k_r) * k1Per31);

					var re = r - r2;
					var ge = g - g2;
					var be = b - b2;

					var n1 = offs + 1;
					var n2 = offs + texw - 1;
					var n3 = offs + texw;
					var n4 = offs + texw + 1;

					if(x < texw - 1){
						pixels [n1].r += re * k15Per32;
						pixels [n1].g += ge * k29Per64;
						pixels [n1].b += be * k15Per32;
					}

					if(y < texh - 1){
						pixels [n3].r += re * k11Per32;
						pixels [n3].g += ge * k21Per64;
						pixels [n3].b += be * k11Per32;

						if(x > 0){
							pixels [n2].r += re * k5Per32;
							pixels [n2].g += ge * k11Per64;
							pixels [n2].b += be * k5Per32;
						}

						if(x < texw - 1){
							pixels [n4].r += re * k1Per32;
							pixels [n4].g += ge * k3Per64;
							pixels [n4].b += be * k1Per32;
						}
					}

					pixels [offs].r = r2;
					pixels [offs].g = g2;
					pixels [offs].b = b2;

					offs++;
				}
			}

			texture.SetPixels (pixels);
			EditorUtility.CompressTexture (texture, TextureFormat.RGB565, UnityEditor.TextureCompressionQuality.Best);
		}


	}
	/// <summary>
	/// 支持alpha通道
	/// Raises the postprocess texture444 event.
	/// </summary>
	/// <param name="texture">Texture.</param>
	void OnPostprocessTexture444 (Texture2D texture)
	{
		Log.Trace ("_dither444");

		var texw = texture.width;
		var texh = texture.height;

		var pixels = texture.GetPixels ();
		var offs = 0;

		var k1Per15 = 1.0f / 15.0f;
		var k1Per16 = 1.0f / 16.0f;
		var k3Per16 = 3.0f / 16.0f;
		var k5Per16 = 5.0f / 16.0f;
		var k7Per16 = 7.0f / 16.0f;

		for (var y = 0; y < texh; y++) {
			for (var x = 0; x < texw; x++) {
				float a = pixels [offs].a;
				float r = pixels [offs].r;
				float g = pixels [offs].g;
				float b = pixels [offs].b;

				var a2 = Mathf.Clamp01 (Mathf.Floor (a * 16) * k1Per15);
				var r2 = Mathf.Clamp01 (Mathf.Floor (r * 16) * k1Per15);
				var g2 = Mathf.Clamp01 (Mathf.Floor (g * 16) * k1Per15);
				var b2 = Mathf.Clamp01 (Mathf.Floor (b * 16) * k1Per15);

				var ae = a - a2;
				var re = r - r2;
				var ge = g - g2;
				var be = b - b2;

				pixels [offs].a = a2;
				pixels [offs].r = r2;
				pixels [offs].g = g2;
				pixels [offs].b = b2;

				var n1 = offs + 1;   // (x+1,y)
				var n2 = offs + texw - 1; // (x-1 , y+1)
				var n3 = offs + texw;  // (x, y+1)
				var n4 = offs + texw + 1; // (x+1 , y+1)

				if (x < texw - 1) {
					pixels [n1].a += ae * k7Per16;
					pixels [n1].r += re * k7Per16;
					pixels [n1].g += ge * k7Per16;
					pixels [n1].b += be * k7Per16;
				}

				if (y < texh - 1) {
					pixels [n3].a += ae * k5Per16;
					pixels [n3].r += re * k5Per16;
					pixels [n3].g += ge * k5Per16;
					pixels [n3].b += be * k5Per16;

					if (x > 0) {
						pixels [n2].a += ae * k3Per16;
						pixels [n2].r += re * k3Per16;
						pixels [n2].g += ge * k3Per16;
						pixels [n2].b += be * k3Per16;
					}

					if (x < texw - 1) {
						pixels [n4].a += ae * k1Per16;
						pixels [n4].r += re * k1Per16;
						pixels [n4].g += ge * k1Per16;
						pixels [n4].b += be * k1Per16;
					}
				}

				offs++;
			}
		}

		texture.SetPixels (pixels);
		EditorUtility.CompressTexture (texture, TextureFormat.RGBA4444, UnityEditor.TextureCompressionQuality.Best);
	}

}
