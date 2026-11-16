using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
namespace HoraDaFesta.Editor
{
    public static class FestaBuild
    {
        [MenuItem("HORA DA FESTA/Criar jogo Windows")]
        public static void Windows()
        {
            string output = Environment.GetEnvironmentVariable("HDF_BUILD_OUTPUT");
            if (string.IsNullOrEmpty(output)) output = Path.GetFullPath("Builds/Windows/HoraDaFesta.exe");
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            if (!File.Exists("Assets/HoraDaFesta/Generated/Arraial.unity")) ArraialBuilder.Generate();
            PlayerSettings.productName = "HORA DA FESTA";
            PlayerSettings.companyName = "HORA DA FESTA";
            PlayerSettings.runInBackground = true;
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.defaultIsNativeResolution = false;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new[] { GraphicsDeviceType.Direct3D11 });
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[] { "Assets/HoraDaFesta/Generated/Arraial.unity" },
                locationPathName = output,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.CompressWithLz4HC
            });
            if (report.summary.result != BuildResult.Succeeded) throw new Exception("Build Windows falhou: " + report.summary.result);
            Debug.Log("HDF_BUILD_READY " + output);
        }
    }
}
