using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoraDaFesta
{
    public class Arraial : MonoBehaviour
    {
        [NonSerialized]
        public bool connectBridge = true;
        public FestaSocket socket;
        public FestaState state;
        public Camera festaCamera;
        readonly Dictionary<string, Inseto> actors = new Dictionary<string, Inseto>();
        readonly Dictionary<Color, Material> materials = new Dictionary<Color, Material>();
        Transform guests;
        ParticleSystem fireworks;
        string focusUser;
        float focusUntil;
        long revision;
        public static readonly string[] Instruments =
        {
            "Sanfona",
            "Zabumba",
            "Triângulo",
            "Saxofone",
            "Teclado",
            "Violão"
        };
        public static Color Gold = new Color(1, .65f, .13f);
        void Start()
        {
            Application.runInBackground = true;
            Build();
            if (connectBridge)
                socket = gameObject.AddComponent<FestaSocket>();
        }

        public GameObject Shape(string label, PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, Color color)
        {
            var o = GameObject.CreatePrimitive(type);
            o.name = label;
            o.transform.SetParent(parent, false);
            o.transform.localPosition = position;
            o.transform.localScale = scale;
            var collider = o.GetComponent<Collider>();
            if (collider)
                Destroy(collider);
            if (!materials.TryGetValue(color, out var mat))
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null)
                    shader = Shader.Find("Standard");
                var source = Resources.Load<Material>("ArraialLit");
                mat = source ? new Material(source) : new Material(shader);
                mat.color = color;
                materials[color] = mat;
            }

            o.GetComponent<Renderer>().sharedMaterial = mat;
            return o;
        }

        void Build()
        {
            guests = new GameObject("Convidados").transform;
            guests.SetParent(transform);
            Shape("Terreiro", PrimitiveType.Cube, transform, new Vector3(0, -.3f, 0), new Vector3(36, .5f, 30), new Color(.24f, .12f, .06f));
            Shape("Pista", PrimitiveType.Cube, transform, new Vector3(0, 0, 0), new Vector3(19, .12f, 16), new Color(.65f, .36f, .16f));
            Shape("Palco", PrimitiveType.Cube, transform, new Vector3(0, .4f, 11), new Vector3(18, .8f, 5), new Color(.3f, .13f, .07f));
            for (int i = 0; i < 6; i++)
            {
                var musician = CreateActor("Musico " + i);
                musician.SetRole("banda", i);
                musician.Target = new Vector3(-7.5f + i * 3, .85f, 11);
                musician.transform.position = musician.Target;
            }

            for (int row = 0; row < 4; row++)
                for (int i = 0; i < 28; i++)
                {
                    float x = -17 + i * 1.25f, z = -12 + row * 8;
                    var flag = new GameObject("Bandeirinha");
                    flag.transform.SetParent(transform);
                    flag.transform.localPosition = new Vector3(x, 6 - Mathf.Sin(i * Mathf.PI / 27) * .8f, z);
                    var mesh = new Mesh();
                    mesh.vertices = new[]
                    {
                        Vector3.zero,
                        new Vector3(.8f, 0, 0),
                        new Vector3(.4f, -.9f, 0),
                        Vector3.zero,
                        new Vector3(.8f, 0, 0),
                        new Vector3(.4f, -.9f, 0)
                    };
                    mesh.triangles = new[]
                    {
                        0,
                        1,
                        2,
                        5,
                        4,
                        3
                    };
                    mesh.RecalculateNormals();
                    flag.AddComponent<MeshFilter>().sharedMesh = mesh;
                    var template = Shape("Suporte", PrimitiveType.Sphere, transform, new Vector3(x, 6, z), Vector3.one * .08f, Color.HSVToRGB((i % 5) / 5f, .8f, 1));
                    flag.AddComponent<MeshRenderer>().sharedMaterial = template.GetComponent<Renderer>().sharedMaterial;
                }

            for (int side = -1; side <= 1; side += 2)
                for (int i = 0; i < 3; i++)
                {
                    var p = new Vector3(side * 14, 1, -8 + i * 8);
                    Shape("Barraca", PrimitiveType.Cube, transform, p, new Vector3(4, 2, 3), new Color(.55f, .2f, .08f));
                    Shape("Toldo", PrimitiveType.Cube, transform, p + Vector3.up * 1.5f, new Vector3(4.5f, .3f, 3.5f), Color.HSVToRGB(i / 3f, .8f, 1));
                }

            var lightObject = new GameObject("Luar");
            lightObject.transform.SetParent(transform);
            var sun = lightObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.2f;
            sun.transform.rotation = Quaternion.Euler(45, -35, 0);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.4f, .4f, .55f);
            for (int i = 0; i < 6; i++)
            {
                var lamp = new GameObject("Luz quente");
                lamp.transform.SetParent(transform);
                lamp.transform.position = new Vector3(-12 + i * 5, 5, 2);
                var l = lamp.AddComponent<Light>();
                l.type = LightType.Point;
                l.range = 12;
                l.intensity = 5;
                l.color = Gold;
            }

            foreach (var c in FindObjectsByType<Camera>())
                c.enabled = false;
            festaCamera = new GameObject("Camera Arraial").AddComponent<Camera>();
            festaCamera.transform.SetParent(transform);
            festaCamera.tag = "MainCamera";
            festaCamera.backgroundColor = new Color(.025f, .035f, .1f);
            festaCamera.clearFlags = CameraClearFlags.SolidColor;
            festaCamera.fieldOfView = 52;
            festaCamera.transform.position = new Vector3(0, 18, -24);
            festaCamera.transform.LookAt(new Vector3(0, 1, 3));
            var fx = new GameObject("Fogos");
            fx.transform.SetParent(transform);
            fx.transform.position = new Vector3(0, 8, 7);
            fireworks = fx.AddComponent<ParticleSystem>();
            var main = fireworks.main;
            main.startLifetime = 2;
            main.startSpeed = 6;
            main.startSize = .18f;
            main.startColor = new ParticleSystem.MinMaxGradient(Gold, Color.magenta);
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = fireworks.emission;
            emission.rateOverTime = 70;
            var shape = fireworks.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 4;
            var renderer = fx.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = Resources.Load<Material>("ArraialParticles");
            if (renderer.sharedMaterial == null)
                renderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
            fireworks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        Inseto CreateActor(string id)
        {
            var obj = new GameObject(id);
            obj.transform.SetParent(guests);
            var actor = obj.AddComponent<Inseto>();
            actor.Build(this, id);
            return actor;
        }

        public void Apply(FestaState next)
        {
            if (next == null || next.people == null)
                return;
            state = next;
            if (next.focus != null && next.focus.until > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            {
                focusUser = next.focus.user;
                focusUntil = Time.time + (next.focus.until - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / 1000f;
            }

            var live = new HashSet<string>();
            int dancer = 0;
            foreach (var p in state.people)
            {
                if (string.IsNullOrEmpty(p.name))
                    continue;
                live.Add(p.name);
                if (!actors.TryGetValue(p.name, out var a))
                {
                    a = CreateActor(p.name);
                    actors[p.name] = a;
                }

                int member = Array.FindIndex(state.band ?? Array.Empty<Musician>(), m => m.id == p.name);
                int slot = member < 0 ? -1 : Array.IndexOf(Instruments, state.band[member].instrument);
                a.SetRole(p.role, slot);
                if (p.role == "banda")
                    a.Target = new Vector3(-7.5f + Mathf.Max(0, slot) * 3, .85f, 11);
                else if (p.role == "noivo" || p.role == "noiva")
                    a.Target = new Vector3(p.role == "noivo" ? -1.4f : 1.4f, 0, 6);
                else
                {
                    a.DanceIndex = dancer++;
                    a.Target = Vector3.zero;
                }
            }

            var removed = new List<string>();
            foreach (var pair in actors)
                if (!live.Contains(pair.Key))
                {
                    Destroy(pair.Value.gameObject);
                    removed.Add(pair.Key);
                }

            foreach (var id in removed)
                actors.Remove(id);
            // Six stage placeholders are visible only when the corresponding instrument is vacant.
            foreach (Transform child in guests)
                if (child.name.StartsWith("Musico "))
                {
                    int slot = int.Parse(child.name.Substring(7));
                    bool occupied = state.band != null && Array.Exists(state.band, m => m.instrument == Instruments[slot]);
                    child.gameObject.SetActive(!occupied);
                }
        }

        void Update()
        {
            if (socket != null)
            {
                int budget = 32;
                while (budget-- > 0 && socket.Messages.TryDequeue(out var raw))
                {
                    try
                    {
                        var e = JsonUtility.FromJson<Envelope>(raw);
                        if (e.type == "game-state")
                        {
                            revision = e.revision;
                            Apply(e.state);
                        }
                        else if (e.type == "tiktok-event")
                        {
                            focusUser = e.user;
                            focusUntil = Time.time + 4;
                        }
                        else if (e.type == "game-command")
                            Command(e);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("Mensagem ignorada: " + ex.Message);
                    }
                }
            }

            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            bool active = state?.effect != null && state.effect.until > now;
            bool fogos = active && state.effect.kind == "fogos";
            bool crazy = active && state.effect.kind == "quadrilha";
            if (fogos && !fireworks.isPlaying)
                fireworks.Play();
            if (!fogos && fireworks.isPlaying)
                fireworks.Stop();
            foreach (var a in actors.Values)
                a.Crazy = crazy;
            string mode = state?.camera ?? "automatic";
            Vector3 target = new Vector3(0, 1, 3), pos = new Vector3(0, 18, -24);
            if (fogos || mode == "wide")
            {
            }
            else if (mode == "band")
            {
                target = new Vector3(0, 2, 11);
                pos = new Vector3(0, 6, -2);
            }
            else if (mode == "couple")
            {
                target = new Vector3(0, 1.5f, 6);
                pos = new Vector3(0, 5, -4);
            }
            else if (mode == "automatic")
            {
                if (Time.time < focusUntil && focusUser != null && actors.TryGetValue(focusUser, out var focused))
                {
                    target = focused.transform.position + Vector3.up;
                    pos = target + new Vector3(0, 4, -8);
                }
                else
                {
                    float angle = Time.time * .04f;
                    pos = new Vector3(Mathf.Sin(angle) * 24, 15, -Mathf.Cos(angle) * 24);
                }
            }

            festaCamera.transform.position = Vector3.Lerp(festaCamera.transform.position, pos, Time.deltaTime * 2);
            festaCamera.transform.rotation = Quaternion.Slerp(festaCamera.transform.rotation, Quaternion.LookRotation(target - festaCamera.transform.position), Time.deltaTime * 3);
        }

        void Command(Envelope e)
        {
            if (state == null || e.payload == null)
                return;
            if (e.command == "camera" && Array.IndexOf(new[] { "automatic", "wide", "band", "couple" }, e.payload.mode) >= 0)
                state.camera = e.payload.mode;
            if (e.command == "fireworks" || e.command == "dance")
                state.effect = new Effect
                {
                    kind = e.command == "fireworks" ? "fogos" : "quadrilha",
                    until = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (long)(Mathf.Clamp(e.payload.seconds, 1, 120) * 1000)
                };
        }

        void OnGUI()
        {
            GUI.Box(new Rect(12, 12, 470, 70), "HORA DA FESTA — Arraia dos Insetos\n" + (socket ? socket.Status : "") + " | Participantes: " + actors.Count + " | Revisao: " + revision + "\nAbra o painel para adicionar espectadores e presentes.");
        }

        void OnDestroy()
        {
            foreach (var mat in materials.Values)
                Destroy(mat);
        }
    }
}
