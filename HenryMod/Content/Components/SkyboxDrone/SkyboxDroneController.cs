using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoffeinPilot.Content.Components.SkyboxDrone
{
    public class SkyboxDroneController : MonoBehaviour
    {
        public float forwardSpeed;
        public float turnAngleSpeed;

        public static GameObject instance;
        public static Dictionary<string, OrbitInfo> stageOrbitInfos = new Dictionary<string, OrbitInfo>
        {
            {
                "defaultorbitinfo",
                new OrbitInfo
                {
                    startPosition = new Vector3(0f, 220f, 0f),
                    disableOnStage = false
                }
            },

            {
                "village",
                new OrbitInfo
                {
                    startPosition = new Vector3(0f, 240f, 0f),
                    disableOnStage = false
                }
            },

            {
                "villagenight",
                new OrbitInfo
                {
                    startPosition = new Vector3(0f, 240f, 0f),
                    disableOnStage = false
                }
            },

            {
                "snowyforest",
                new OrbitInfo
                {
                    startPosition = new Vector3(-100f, 200f, -80f),
                    disableOnStage = false
                }
            },

            {
                "blackbeach",
                new OrbitInfo
                {
                    startPosition = new Vector3(-420f, 0f, 0f),
                    disableOnStage = false
                }
            },

            {
                "blackbeach2",
                new OrbitInfo
                {
                    startPosition = new Vector3(-230f, 260f, 450f),
                    turnAngleSpeed = 8f,
                    disableOnStage = false
                }
            },

            {
                "golemplains",
                new OrbitInfo
                {
                    startPosition = new Vector3(-280f, 50f, -150f),
                    disableOnStage = false
                }
            },

            {
                "golemplains2",
                new OrbitInfo
                {
                    startPosition = new Vector3(-240f, 240f, 140f),
                    disableOnStage = false
                }
            },

            {
                "lakes",
                new OrbitInfo
                {
                    startPosition = new Vector3(-180f, 240f, -270f),
                    disableOnStage = false
                }
            },

            {
                "lakesnight",
                new OrbitInfo
                {
                    startPosition = new Vector3(-180f, 240f, -270f),
                    disableOnStage = false
                }
            },

            {
                "goolake",
                new OrbitInfo
                {
                    startPosition = new Vector3(0f, 90f, 0f),
                    disableOnStage = false
                }
            },

            {
                "foggyswamp",
                new OrbitInfo
                {
                    startPosition = new Vector3(-100f, 20f, -420f),
                    turnAngleSpeed = 6f,
                    disableOnStage = false
                }
            },

            {
                "ancientloft",
                new OrbitInfo
                {
                    startPosition = new Vector3(-420f, 155f, -130f),
                    turnAngleSpeed = 5f,
                    disableOnStage = false
                }
            },

            {
                "lemuriantemple",
                new OrbitInfo
                {
                    disableOnStage = true
                }
            },

            {
                "frozenwall",
                new OrbitInfo
                {
                    startPosition = new Vector3(-470f, 180f, -80f),
                    turnAngleSpeed = 5f,
                    disableOnStage = false
                }
            },

            {
                "wispgraveyard",
                new OrbitInfo
                {
                    startPosition = new Vector3(-230f, 165f, 40f),
                    turnAngleSpeed = 8f,
                    disableOnStage = false
                }
            },

            {
                "sulfurpools",
                new OrbitInfo
                {
                    startPosition = new Vector3(-170f, 100f, 55f),
                    turnAngleSpeed = 5f,
                    disableOnStage = false
                }
            },

            {
                "habitat",
                new OrbitInfo
                {
                    startPosition = new Vector3(-370f, 85f, 255f),
                    turnAngleSpeed = 12f,
                    disableOnStage = false
                }
            },

            {
                "habitatfall",
                new OrbitInfo
                {
                    startPosition = new Vector3(-370f, 85f, 255f),
                    turnAngleSpeed = 12f,
                    disableOnStage = false
                }
            },

            {
                "dampcavesimple",
                new OrbitInfo
                {
                    disableOnStage = true
                }
            },

            {
                "shipgraveyard",
                new OrbitInfo
                {
                    startPosition = new Vector3(-370f, 160f, -60f),
                    turnAngleSpeed = 6f,
                    disableOnStage = false
                }
            },

            {
                "rootjungle",
                new OrbitInfo
                {
                    startPosition = new Vector3(-120f, 140f, 80f),
                    turnAngleSpeed = 20f/3f,
                    disableOnStage = false
                }
            },

            {
                "skymeadow",
                new OrbitInfo
                {
                    startPosition = new Vector3(-145f, 190f, 15f),
                    disableOnStage = false
                }
            },

            {
                "helminthroost",
                new OrbitInfo
                {
                    startPosition = new Vector3(-840f,240f,345f),
                    turnAngleSpeed = 6f,
                    disableOnStage = false
                }
            },

            {
                "meridian",
                new OrbitInfo
                {
                    startPosition = new Vector3(0f, 300f, 0f),
                    disableOnStage = false
                }
            },

            {
                "moon2",
                new OrbitInfo
                {
                    startPosition = new Vector3(-460f, 740f, -80f),
                    disableOnStage = false
                }
            },

            {
                "moon",
                new OrbitInfo
                {
                    startPosition = new Vector3(-500f, 750f, 0f),
                    disableOnStage = false
                }
            },
        };

        public class OrbitInfo
        {
            public Vector3 startPosition;
            public float turnAngleSpeed = 4f;
            public float forwardSpeed = 20f;
            public float startRotation = 0f;
            public bool disableOnStage = false;
        }

        private void Update()
        {
            base.transform.position += base.transform.forward * Time.deltaTime * forwardSpeed;
            base.transform.RotateAround(base.transform.position, Vector3.up, Time.deltaTime * turnAngleSpeed);
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(instance);
                instance = null;
            }
            instance = base.gameObject;
        }

        private void OnDestroy()
        {
            if (instance == base.gameObject)
            {
                instance = null;
            }
        }

        internal static void CharacterBody_onBodyStartGlobal(RoR2.CharacterBody body)
        {
            if (!instance && body.bodyIndex == BodyCatalog.FindBodyIndex("MoffeinPilotBody"))
            {
                string currentSceneName = "defaultorbitinfo";

                var scene = SceneManager.GetActiveScene();


                if (scene != null)
                {
                    currentSceneName = scene.name;

                    if (!stageOrbitInfos.ContainsKey(currentSceneName.ToLower()))
                    {
                        Debug.Log("Pilot: No drone orbit info defineed for " + currentSceneName + ". Using default.");
                        currentSceneName = "defaultorbitinfo";
                    }

                    //Check hidden realm
                    var sceneDef = SceneCatalog.GetSceneDefForCurrentScene();
                    bool isHiddenRealm = sceneDef && sceneDef.blockOrbitalSkills;

                    stageOrbitInfos.TryGetValue(currentSceneName, out OrbitInfo orbitInfo);
                    if (!isHiddenRealm && orbitInfo != null && !orbitInfo.disableOnStage)
                    {
                        instance = Instantiate(MoffeinPilot.Modules.Asset.SkyboxDronePrefab);
                        instance.transform.position = orbitInfo.startPosition;
                        instance.transform.RotateAround(instance.transform.position, Vector3.up, orbitInfo.startRotation);

                        SkyboxDroneController controller = instance.GetComponent<SkyboxDroneController>();
                        controller.forwardSpeed = orbitInfo.forwardSpeed;
                        controller.turnAngleSpeed = orbitInfo.turnAngleSpeed;
                    }
                }
            }
        }
    }
}
