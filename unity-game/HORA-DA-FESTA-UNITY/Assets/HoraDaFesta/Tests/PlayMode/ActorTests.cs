using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoraDaFesta.Tests
{
    public class ActorTests
    {
        [UnityTest]
        public IEnumerator SnapshotReusesActorsKeepsInstrumentSlotsAndResets()
        {
            var root = new GameObject("Test Arraial");
            var game = root.AddComponent<Arraial>();
            game.connectBridge = false;
            yield return null;
            var state = new FestaState
            {
                mode = 1,
                people = new[]
                {
                    new Person
                    {
                        name = "alice",
                        role = "banda"
                    }
                },
                band = new[]
                {
                    new Musician
                    {
                        id = "alice",
                        instrument = "Zabumba"
                    }
                },
                couple = new Couple(),
                camera = "automatic"
            };
            game.Apply(state);
            var actor = root.transform.Find("Convidados/alice").GetComponent<Inseto>();
            Assert.AreEqual(new Vector3(-4.5f, .85f, 11), actor.Target);
            Assert.IsTrue(root.transform.Find("Convidados/Musico 0").gameObject.activeSelf);
            Assert.IsFalse(root.transform.Find("Convidados/Musico 1").gameObject.activeSelf);
            game.Apply(state);
            Assert.AreSame(actor, root.transform.Find("Convidados/alice").GetComponent<Inseto>());
            state.people[0].role = "noiva";
            state.band = new Musician[0];
            state.couple.noiva = "alice";
            game.Apply(state);
            Assert.AreEqual("noiva", actor.Role);
            Assert.IsTrue(root.transform.Find("Convidados/Musico 1").gameObject.activeSelf);
            state.people = new Person[0];
            state.couple.noiva = null;
            game.Apply(state);
            yield return null;
            Assert.IsNull(root.transform.Find("Convidados/alice"));
            Object.Destroy(root);
            yield return null;
        }
    }
}
