using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace HoraDaFesta.Editor {
public static class ArraialBuilder {
 [MenuItem("HORA DA FESTA/Gerar cena Arraial")]
 public static void Generate(){
 if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
 System.IO.Directory.CreateDirectory("Assets/HoraDaFesta/Generated");
 const string pipelinePath="Assets/HoraDaFesta/Generated/ArraialURP.asset";
 var pipeline=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(pipelinePath);
 if(pipeline==null){var data=ScriptableObject.CreateInstance<UniversalRendererData>();AssetDatabase.CreateAsset(data,"Assets/HoraDaFesta/Generated/ArraialRenderer.asset");pipeline=UniversalRenderPipelineAsset.Create(data);AssetDatabase.CreateAsset(pipeline,pipelinePath);}
 System.IO.Directory.CreateDirectory("Assets/HoraDaFesta/Generated/Resources");
 foreach(var pair in new[]{new[]{"ArraialLit","Universal Render Pipeline/Lit"},new[]{"ArraialParticles","Universal Render Pipeline/Particles/Unlit"}}){var materialPath="Assets/HoraDaFesta/Generated/Resources/"+pair[0]+".mat";if(AssetDatabase.LoadAssetAtPath<Material>(materialPath)==null)AssetDatabase.CreateAsset(new Material(Shader.Find(pair[1])),materialPath);}
 GraphicsSettings.defaultRenderPipeline=pipeline;QualitySettings.renderPipeline=pipeline;
 var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);new GameObject("HORA DA FESTA").AddComponent<Arraial>();
 EditorSceneManager.SaveScene(scene,"Assets/HoraDaFesta/Generated/Arraial.unity");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/HoraDaFesta/Generated/Arraial.unity",true)};AssetDatabase.SaveAssets();
 }
}
}
