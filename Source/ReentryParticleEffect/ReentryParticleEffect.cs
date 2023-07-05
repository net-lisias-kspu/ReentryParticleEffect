/*
	This file is part of Reentry Particle Effect /L Unleashed
		© 2021 Lisias T : http://lisias.net <support@lisias.net>
		© 2016-19 pizzaoverhead

	Reentry Particle Effect /L is licensed as follows:

		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

	Reentry Particle Effect /L Unleashed is distributed in the hope that
	it will be useful, but WITHOUT ANY WARRANTY; without even the implied
	warranty of	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Reentry Particle Effect /L Unleashed.
	If not, see <https://www.gnu.org/licenses/>.

*/
using System;
using System.Collections.Generic;

using KSPe.Annotations;

using UnityEngine;

namespace ReentryParticleEffect
{
    /*
     * BUGS
     * 
     * Trail too dense
     * Trail doesn't ease in at top of atmosphere
     * Trail isn't deleted on craft destroyed
     * Trail too short?
     */
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ReentryParticleEffect : MonoBehaviour
    {
		internal static bool DrawGui = true;
		private readonly Settings.Parameters settings = Settings.Parameters.Instance;

#if DEBUG
		private readonly Settings.UI ui;
		public ReentryParticleEffect() : base()
		{
			this.ui = Settings.UI.CreateFor(this);
		}
#endif

		[UsedImplicitly]
		private void Start()
		{
			settings.Load();
			GameEvents.onVesselDestroy.Add(OnVesselDestroy);
		}

		[UsedImplicitly]
		private void OnDestroy()
		{
			GameEvents.onVesselDestroy.Remove(OnVesselDestroy);
		}

        public class ReentryEffect
        {
            public ReentryEffect(GameObject effect)
            {
                ParticleSystem[] particleSystems = effect.GetComponentsInChildren<ParticleSystem>();
                Trail = particleSystems[0];
                Sparks = particleSystems[1];
                FXPrefab[] prefabs = effect.GetComponentsInChildren<FXPrefab>();
                trailPrefab = prefabs[0];
            }

            public FXPrefab trailPrefab;
            public ParticleSystem Trail;
            public ParticleSystem Sparks;

            public void Die()
            {
                Destroy(trailPrefab);
                Destroy(Trail);
                Destroy(Sparks);
            }
        }

        public ReentryEffect CreateEffect()
        {
            GameObject effect = (GameObject)GameObject.Instantiate(Resources.Load("Effects/fx_reentryTrail"));
            ReentryEffect reentryFx = new ReentryEffect(effect);
            // Set the effect speed high to animate as fast as is visible.
            ParticleSystem.MainModule trailMain = reentryFx.Trail.main;
            reentryFx.Trail.transform.localScale = new Vector3(this.settings.TrailScale, this.settings.TrailScale, this.settings.TrailScale);
            trailMain.scalingMode = ParticleSystemScalingMode.Local;
            trailMain.simulationSpeed = 5;

            ParticleSystem.MainModule sparksMain = reentryFx.Sparks.main;
            sparksMain.simulationSpeed = 5;

            return reentryFx;
        }

        public static readonly Dictionary<Guid, ReentryEffect> VesselDict = new Dictionary<Guid, ReentryEffect>();

#if AERODYNAMICSFX
		internal AerodynamicsFX afx1 = null;
#endif
		[UsedImplicitly]
        private void FixedUpdate()
        {
            float effectStrength = (AeroFX.FxScalar * AeroFX.state - this.settings.EffectThreshold) * (1 / this.settings.EffectThreshold);
            List<Vessel> vessels = FlightGlobals.Vessels;
            for (int i = vessels.Count - 1; i >= 0; --i)
            {
                Vessel vessel = vessels[i];
                ReentryEffect effects = null;
                if (VesselDict.ContainsKey(vessel.id))
                    effects = VesselDict[vessel.id];
                else
                {
                    if (vessel.loaded)
                    {
                        effects = CreateEffect();
                        VesselDict.Add(vessel.id, effects);
                    }
                    else
                        continue;
                }

                if (!vessel.loaded)
                {
                    if (effects != null)
                    {
                        effects.Die();
                    }
                    effects = null;
                    continue;
                }

                if (effects == null || effects.Trail == null || effects.Sparks == null)
                    continue;

                ParticleSystem.EmissionModule trailEmission = effects.Trail.emission;
                ParticleSystem.EmissionModule sparksEmission = effects.Sparks.emission;
                if (AeroFX != null)
                {
                    //effects.Trail.transform.localScale = new Vector3(TrailScale, TrailScale, TrailScale);
#if AERODYNAMICSFX
                    this.afx1 = AeroFX;
                    effects.Trail.transform.localScale = new Vector3(this.settings.scaleX, this.settings.scaleY, this.settings.scaleZ);
                    ParticleSystem.MainModule main = effects.Trail.main;
                    main.scalingMode = this.settings.scalingMode;
#endif

                    // FxScalar: Strength of the effects.
                    // state: 0 = condensation, 1 = reentry.
                    if (effectStrength > 0)
                    {
                        // Ensure the particles don't lag a frame behind.
                        effects.Trail.transform.position = vessel.CoM + vessel.rb_velocity * Time.fixedDeltaTime;
                        trailEmission.enabled = true;
                        effects.Sparks.transform.position = vessel.CoM + vessel.rb_velocity * Time.fixedDeltaTime;
                        sparksEmission.enabled = true;

                        this.settings.Velocity = AeroFX.velocity * (float)AeroFX.airSpeed;

                        ParticleSystem.MainModule trailMain = effects.Trail.main;
                        trailMain.startSpeed = this.settings.Velocity.magnitude;
                        effects.Trail.transform.forward = -this.settings.Velocity.normalized;
                        trailMain.maxParticles = (int)(this.settings.MaxParticles * effectStrength);
                        trailEmission.rateOverTime = (int)(this.settings.MaxEmissionRate * effectStrength);

                        // startSpeed controls the emission cone angle. Greater than ~1 is too wide.
                        //reentryTrailSparks.startSpeed = velocity.magnitude;
                        ParticleSystem.MainModule sparksMain = effects.Sparks.main;
                        effects.Sparks.transform.forward = -this.settings.Velocity.normalized;
                        sparksMain.maxParticles = (int)(this.settings.MaxParticles * effectStrength);
                        sparksEmission.rateOverTime = (int)(this.settings.MaxEmissionRate * effectStrength);
                    }
                    else
                    {
                        trailEmission.enabled = false;
                        sparksEmission.enabled = false;
                    }
                }
                else
                {
                    trailEmission.enabled = false;
                    sparksEmission.enabled = false;
                }
            }
        }

        public void OnVesselDestroy(Vessel vessel)
        {
            if (VesselDict.ContainsKey(vessel.id))
            {
                ReentryEffect effects = VesselDict[vessel.id];
                if (effects != null)
                {
                    effects.Die();
                }
                VesselDict.Remove(vessel.id);
            }
        }

        private AerodynamicsFX _aeroFX;
        AerodynamicsFX AeroFX
        {
            get
            {
                if (_aeroFX == null)
                {
                    GameObject fxLogicObject = GameObject.Find("FXLogic");
                    if (fxLogicObject != null)
                        _aeroFX = fxLogicObject.GetComponent<AerodynamicsFX>();
                }
                return _aeroFX;
            }
        }


        public static Color BlackBodyToRgb(float tempKelvin)
        {
            // C# implementation of Tanner Helland's approximation from here:
            // http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/
            // For use with temperatures between 1000 and 40,000 K.
            float temp = tempKelvin / 100;
            // Colour values (0 to 255).
            float red = 0;
            float green = 0;
            float blue = 0;

            // Calculate red
            if (temp <= 66)
            {
                red = 255;
            }
            else
            {
                red = temp - 60f;
                red = 329.698727446f * (float)(Math.Pow(red, -0.1332047592f));
                red = Math.Max(Math.Min(red, 255), 0);
            }

            // Calculate green
            if (temp <= 66)
            {
                green = temp;
                green = 99.4708025861f * (float)Math.Log(green) - 161.1195681661f;
            }
            else
            {
                green = temp - 60;
                green = 288.1221695283f * (float)Math.Pow(green, -0.0755148492f);
            }
            green = Math.Max(Math.Min(green, 255), 0);

            // Calculate Blue
            if (temp <= 66)
            {
                blue = 255;
            }
            else
            {
                blue = temp - 10;
                blue = 138.5177312231f * (float)Math.Log(blue) - 305.0447927307f;
                blue = Math.Max(Math.Min(blue, 255), 0);
            }

            return new Color(red/255, green/255, blue/255, 1);
        }

	#if DEBUG
		[UsedImplicitly]
		private void OnGUI()
		{
			if (!DrawGui) return;
			this.ui.OnGUI();
		}
	#endif
	}

#if DEBUG && AUTOCHEAT
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    class AutoStartup : UnityEngine.MonoBehaviour
    {
        public static bool first = true;
        public void Start()
        {
            //only do it on the first entry to the menu
            if (first)
            {
                first = false;
                HighLogic.SaveFolder = "test";
                Game game = GamePersistence.LoadGame("persistent", HighLogic.SaveFolder, true, false);
                if (game != null && game.flightState != null && game.compatible)
                    FlightDriver.StartAndFocusVessel(game, game.flightState.activeVesselIdx);
                CheatOptions.InfinitePropellant = true;
                CheatOptions.InfiniteElectricity = true;
                CheatOptions.IgnoreMaxTemperature = true;
                Log.force("Cheat activated!!!");
            }
        }
    }
#endif
}
