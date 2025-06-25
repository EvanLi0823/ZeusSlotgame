using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
namespace Libs
{
    public class ReplaceGuid : EditorWindow
    {
        [MenuItem("Libs/guid替换工具")]
        static void ShowPlistWindow()
        {
            ReplaceGuid editor = (ReplaceGuid)EditorWindow.GetWindowWithRect(typeof(ReplaceGuid), new Rect(100, 100,360, 150), false, "guid替换工具");
            editor.Show();
        }

        Dictionary<string,string> testDict = new Dictionary<string, string>();

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            string strTip = string.Empty;
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("替换", GUILayout.Width(100)))
            {
                testDict.Clear();
                testDict["08f89453a8d9e594c90d61f7c37daf50"] = "34e2c9b9d9e44953933afe37461f44e6";
                testDict["02e0e69a1d7086f4492ba6ebadff6392"] = "900f1a451c764dc3bdcc0de815a15935";
                testDict["1cd4a25f3285e1e4cbfdde3d8b82a97e"] = "78b9ad527fe44d7cb05bbb77fbf351c0";
                testDict["1a1578b9753d2604f98d608cb4239e2f"] = "9541d86e2fd84c1d9990edf0852d74ab";
                testDict["183c6bc9fa7cc324dbf7781542c24975"] = "30bed781e402439ab8ce4e3357708115";
                testDict["13259b4ce497b194eb52a33d8eda0bdc"] = "ab2114bdc8544297b417dfefe9f1e410";
                testDict["2be0e7663436d7e489736e101798314d"] = "aa76955fe5bb44f7915d91db8c7043c4";
                testDict["329fb9c37d74a564ab25f1381c697e01"] = "3069a00b8c364df395994d7d379e0a99";
                testDict["3d015705b1c6c50408a08e3094b37951"] = "e87e16ece4884c3bb85cc0e02f133a9f";
                testDict["3b34fc186f40e8043b977d4fa70db8c5"] = "32d40088a6124c578ad6b428df586e2e";
                testDict["496f2e385b0c62542b5c739ccfafd8da"] = "f4688fdb7df04437aeb418b961361dc5";
                testDict["4ff212e36e2c2ed4fb6db42dde06d660"] = "691db8cb70c4426a8ae718465c21345f";
                testDict["4c28b8e37a3c4724a9f15c6a08e48721"] = "0386b6eb838c47138cd51d1c1b879a35";
                testDict["74dfce233ddb29b4294c3e23c1d3650d"] = "71c1514a6bd24e1e882cebbe1904ce04";
                testDict["746823f9e1ba32440a514416c0999d8b"] = "4ae64f3f72004807a9f919f9c27af0db";
                testDict["7c302acd71c26654d809bfa6bbb7f299"] = "20a9b557a46149dfbfa04a3a7080f5aa";
                testDict["d09b930f44e6c5b4db230120295009b2"] = "96e9072453a441618754c478755b3028";
                testDict["a7a3d2105d504af41bc244c1183faf43"] = "383966e89d344865a36addd5d378ffd3";
                testDict["4edbb63168f708c4ab4ac69d7605c50a"] = "9edc9283e7d6409fab242fe8fb6a822c";
                testDict["6e5767b0fdf1d4749b7c1390d66e97b4"] = "f695b5f9415c40b39ae877eaff41c96e";
                testDict["7bc5908043d637b4f9f754791fb07e5d"] = "effb76e1937b45ff8adf45e51a4c08cf";
                testDict["107d333ade494294b84623420397e168"] = "8f8b248abe6b4dcebd6cdd0d754717f4";
                testDict["2481ed331b06c764ba89816f298b7fb8"] = "88ed537c17c34f339121fe9a7d6d7a0e";
                testDict["fc13bb6a5a6c0944c8a7e1d67fb12516"] = "fea49a0730244a98bf1087f7ca9410a8";
                testDict["293ca108339990043ad3550e978a7418"] = "02893ffb522b490a9fa28eedd2584309";
                testDict["a947892a3b8f7d747a446c2ea8c790c5"] = "f4935fb862d54980b1bcbca942962642";
                testDict["81aef1919125e68418e68bfb8adb2777"] = "871f8edd56e84b8fb295b10cc3c78f36";
                testDict["90940d439ca0ef746af0b48419b92d2e"] = "84a92b25f83d49b9bc132d206b370281";
                testDict["9cbdbf02abacd684b9299b513f77ca69"] = "87ab1bebe13f41f89d5427e7d2c34d58";
                testDict["9af14103d0739514da98f1a5aafd79a4"] = "44e1d646473a40178712cb2150f54cec";
                testDict["aafc3c7b9e915d64e8ec3d2c88b3a231"] = "2705215ac5b84b70bacc50632be6e391";
                testDict["a58a71284970117429add3889cc994a9"] = "30a939dce2fd4073955f2f20e659d506";
                testDict["c9ca60a7f24cd95499feea7fe34907d0"] = "7065397ff8184621aa3ca4f854491259";
                testDict["d4af2b8d75840424cb07cfa9abf76811"] = "4f0ca6874aa74540bb3d4fe5a0f86bcc";
                testDict["e2c4405608b405a4680436e183e53c45"] = "3bda1886f58f4e0ab1139400b160c3ee";
                testDict["e818f6ab0a78efe46bf0a444fb55fa47"] = "968a09f153574430a6e15ae975145768";
                testDict["f48830ebdef0c1a43ade7d12ca590746"] = "62a7573e463b4f68b578fcba3a94110c";

                testDict["dca26082f9cb439469295791d9f76fe5"] = "68e6db2ebdc24f95958faec2be5558d6";
                testDict["d1cf17907700cb647aa3ea423ba38f2e"] = "1e3b057af24249748ff873be7fafee47";
                testDict["edfcf888cd11d9245b91d2883049a57e"] = "128e987d567d4e2c824d754223b3f3b0";
                testDict["4a7755d6b5b67874f89c85f56f95fe97"] = "dd89cf5b9246416f84610a006f916af7";
                testDict["9ecb3fe313cb5f7478141eba4a2d54ed"] = "a02a7d8c237544f1962732b55a9aebf1";
                testDict["cafd18099dfc0114896e0a8b277b81b6"] = "fe393ace9b354375a9cb14cdbbc28be4";
                testDict["3c2ea7753c1425145a74d106ec1cd852"] = "85187c2149c549c5b33f0cdb02836b17";
                testDict["8e6b9842dbb1a5a4887378afab854e63"] = "f7ada0af4f174f0694ca6a487b8f543d";

                testDict["dca26082f9cb439469295791d9f76fe5"] = "68e6db2ebdc24f95958faec2be5558d6";
                testDict["dca26082f9cb439469295791d9f76fe5"] = "68e6db2ebdc24f95958faec2be5558d6";

                List<string> errorFileList = new List<string>();
                string[] arrSourcePath = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);
                //循环遍历每一个路径，单独加载
                foreach (string filePath in arrSourcePath)
                {
                    if (filePath.EndsWith(".meta", System.StringComparison.CurrentCultureIgnoreCase)
                        ||filePath.EndsWith(".controller", System.StringComparison.CurrentCultureIgnoreCase)
                        ||filePath.EndsWith(".anim", System.StringComparison.CurrentCultureIgnoreCase)
                        ||filePath.EndsWith(".mat", System.StringComparison.CurrentCultureIgnoreCase)
                        ||filePath.EndsWith(".prefab", System.StringComparison.CurrentCultureIgnoreCase)
                        ||filePath.EndsWith(".unity", System.StringComparison.CurrentCultureIgnoreCase)
                        ||filePath.EndsWith(".asset", System.StringComparison.CurrentCultureIgnoreCase)
                        ||filePath.EndsWith(".guiskin", System.StringComparison.CurrentCultureIgnoreCase)
                        ||filePath.EndsWith(".fontsettings", System.StringComparison.CurrentCultureIgnoreCase))
                    {

                        try{
                            string strTxt = File.ReadAllText(filePath);
                            if (string.IsNullOrEmpty(strTxt)) continue;
                            if (strTxt.Length < 10) continue;

                            bool hasReplace = false;
                            foreach (string key in testDict.Keys)
                            {
                                if(!strTxt.Contains(key)) continue;
                                strTxt = strTxt.Replace(key, testDict[key]);
                                hasReplace = true;
                            }

                            if(!hasReplace) continue;

                            File.WriteAllText(filePath, strTxt);
                        }catch(System.Exception e){
                            errorFileList.Add(filePath);
                        }
                    }
                }

                for (int i = 0; i < errorFileList.Count; i++)
                {
                    Debug.Log(errorFileList[i]);
                }

                AssetDatabase.Refresh();
                strTip = "替换成功";
            }


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(strTip);

            EditorGUILayout.EndVertical();
        }
    }
}
