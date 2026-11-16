using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace HoraDaFesta.Tests
{
    public class StateTests
    {
        [Test]
        public void SnapshotDecodesPanelArraysAndEffectDeadline()
        {
            var e = JsonUtility.FromJson<Envelope>("{\"type\":\"game-state\",\"revision\":7,\"state\":{\"mode\":2,\"people\":[{\"name\":\"alice\",\"role\":\"banda\",\"roses\":4}],\"band\":[{\"id\":\"alice\",\"instrument\":\"Sanfona\"}],\"couple\":{\"noiva\":\"bia\"},\"camera\":\"wide\",\"effect\":{\"kind\":\"fogos\",\"until\":1893456000000}}}");
            Assert.AreEqual(7, e.revision);
            Assert.AreEqual("alice", e.state.people[0].name);
            Assert.AreEqual("Sanfona", e.state.band[0].instrument);
            Assert.AreEqual("bia", e.state.couple.noiva);
            Assert.AreEqual(1893456000000L, e.state.effect.until);
        }

        [Test]
        public void GeneratorCreatesUrpAndBuildScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Editor.ArraialBuilder.Generate();
            Assert.IsNotNull(Object.FindAnyObjectByType<Arraial>());
            Assert.IsNotNull(UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline);
            Assert.IsTrue(EditorBuildSettings.scenes[0].enabled);
            Assert.IsTrue(EditorBuildSettings.scenes[0].path.EndsWith("Arraial.unity"));
        }
    }
}
