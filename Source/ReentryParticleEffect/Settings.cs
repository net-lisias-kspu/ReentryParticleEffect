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
using UnityEngine;

using static ReentryParticleEffect.ReentryParticleEffect;

using GUI = KSPe.UI.GUI;
using GUILayout = KSPe.UI.GUILayout;

namespace ReentryParticleEffect
{
	internal static class Settings
	{
		internal class Parameters
		{
			private static Parameters instance = null;
			internal static Parameters Instance = instance ?? (instance = new Parameters());

			internal Vector3 Velocity;
			internal int MaxParticles = 3000;
			internal int MaxEmissionRate = 400;
			internal float TrailScale = 0.15f;

			// Minimum reentry strength that the effects will activate at.
			// 0 = Activate at the first sign of the flame effects.
			// 1 = Never activate, even at the strongest reentry strength.
			internal float EffectThreshold = 0.4f;

			internal ParticleSystemScalingMode scalingMode;
			internal float scaleX;
			internal float scaleY;
			internal float scaleZ;

			private Parameters()
			{
			}

			internal void Load()
			{
				// Nothing to do yet
			}
		}

		internal class UI
		{
			private float effectStrength = 0;
			private Rect windowPos = new Rect(Screen.width / 4, Screen.height / 4, 10f, 10f);

			/// <summary>
			/// GUI draw event. Called (at least once) each frame.
			/// </summary>
			public void OnGUI()
			{
				if (DrawGui)
					windowPos = GUILayout.Window(this.owner.GetInstanceID(), windowPos, Gui, "Test GUI", GUILayout.Width(600), GUILayout.Height(50));
			}

			private string _trailPlaybackText = "5";
			private string _sparksPlaybackText = "5";
			private string scaleXText = "1";
			private string scaleYText = "1";
			private string scaleZText = "1";

			private readonly ReentryParticleEffect owner;
			private UI(ReentryParticleEffect owner)
			{
				this.owner = owner;
			}
			internal static UI CreateFor(ReentryParticleEffect owner)
			{
				return new UI(owner);
			}

			public void Gui(int windowID)
			{
				ReentryEffect effects = null;
				if (VesselDict.ContainsKey(FlightGlobals.ActiveVessel.id))
					effects = VesselDict[FlightGlobals.ActiveVessel.id];

				if (effects == null)
				{
					GUILayout.Label("ReentryFX is null");
					return;
				}
				if (effects.Trail == null)
				{
					GUILayout.Label("Trail is null");
				}
				if (effects.Sparks == null)
				{
					GUILayout.Label("Sparks is null");
				}
				#if AERODYNAMICSFX
					GuiUtils.label("Effect Strength", effectStrength);
					GuiUtils.label("Stock effect Strength", this.owner.afx1.FxScalar * this.owner.afx1.state);
				#endif
				float highestTemp = GetVesselMaxSkinTemp();
				Color blackBodyColor = BlackBodyToRgb(highestTemp);
				if (FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.rootPart != null)
				{
					GuiUtils.label("Highest temp", highestTemp);
					GuiUtils.label("Blackbody colour", blackBodyColor);

					GuiUtils.label("temperature", FlightGlobals.ActiveVessel.rootPart.temperature);
					GuiUtils.label("skinTemperature", FlightGlobals.ActiveVessel.rootPart.skinTemperature);
					GuiUtils.label("skinUnexposedExternalTemp", FlightGlobals.ActiveVessel.rootPart.skinUnexposedExternalTemp);
					GuiUtils.label("tempExplodeChance", FlightGlobals.ActiveVessel.rootPart.tempExplodeChance);

					GuiUtils.label("MaxTemp", FlightGlobals.ActiveVessel.rootPart.maxTemp);
					GuiUtils.label("SkinMaxTemp", FlightGlobals.ActiveVessel.rootPart.skinMaxTemp);
				}

				GUILayout.Label("Max Particles");
				Parameters.Instance.MaxParticles = (int)GUILayout.HorizontalSlider(Parameters.Instance.MaxParticles, 0, 10000);
				GUILayout.Label("Max Emission Rate");
				Parameters.Instance.MaxEmissionRate = (int)GUILayout.HorizontalSlider(Parameters.Instance.MaxEmissionRate, 0, 1000);

				GUILayout.Label("Trail");
				if (effects.Trail == null)
					GUILayout.Label("Trail is null");
				else
				{
					ParticleSystem.MainModule trailMain = effects.Trail.main;
					float trailPlaybackSpeed = trailMain.simulationSpeed;
					_trailPlaybackText = GuiUtils.editFloat("Playback speed", _trailPlaybackText, out trailPlaybackSpeed, 5);
					trailMain.simulationSpeed = trailPlaybackSpeed;

					//Color key0 = new Color(1f, 0.545f, 0.192f, 1f);
					//Color key1 = new Color(0.725f, 0.169f, 0f, 1f);
					Color key0 = effects.Trail.colorOverLifetime.color.gradient.colorKeys[0].color;
					Color key1 = effects.Trail.colorOverLifetime.color.gradient.colorKeys[1].color;

					/*Gradient grad = new Gradient();
                    grad.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.red, 1.0f) }, 
                        new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
                        );
                    */

					key0 = GuiUtils.rgbaSlider("Colour Gradient 0", ref key0.r, ref key0.g, ref key0.b, ref key0.a, 0f, 1f);
					key1 = GuiUtils.rgbaSlider("Colour Gradient 1", ref key1.r, ref key1.g, ref key1.b, ref key1.a, 0f, 1f);

					effects.Trail.colorOverLifetime.color.gradient.colorKeys[0].color = key0;
					effects.Trail.colorOverLifetime.color.gradient.colorKeys[1].color = key1;

					//Color cMax = trailMain.startColor.colorMax;
					//Color cMin = trailMain.startColor.colorMin;
					//cMax = GuiUtils.rgbaSlider("Max Colour", ref cMax.r, ref cMax.g, ref cMax.b, ref cMax.a, 0f, 1f);
					//cMin = GuiUtils.rgbaSlider("Min Colour", ref cMin.r, ref cMin.g, ref cMin.b, ref cMin.a, 0f, 1f);
					//trailMain.startColor = new ParticleSystem.MinMaxGradient(cMin, cMax);
					//trailMain.startColor = new ParticleSystem.MinMaxGradient(blackBodyColor, BlackBodyToRgb(highestTemp * 2));
				}

				GUILayout.Label("Sparks");
				if (null == effects.Sparks)
					GUILayout.Label("Sparks is null");
				else
				{
					ParticleSystem.MainModule sparksMain = effects.Sparks.main;
					float sparksPlaybackSpeed = sparksMain.simulationSpeed;
					_sparksPlaybackText = GuiUtils.editFloat("Playback speed", _sparksPlaybackText, out sparksPlaybackSpeed, 5);
					sparksMain.simulationSpeed = sparksPlaybackSpeed;
				}

				scaleXText = GuiUtils.editFloat("X Scale", scaleXText, out Parameters.Instance.scaleX, 1);
				scaleYText = GuiUtils.editFloat("Y Scale", scaleYText, out Parameters.Instance.scaleY, 1);
				scaleZText = GuiUtils.editFloat("Z Scale", scaleZText, out Parameters.Instance.scaleZ, 1);
				GUI.DragWindow();
			}

			public static float GetVesselMaxSkinTemp()
			{
				float maxTemp = 0;

				int partCount = FlightGlobals.ActiveVessel.parts.Count;
				for (int i = 0;i < partCount;i++)
				{
					maxTemp = (float)Math.Max(FlightGlobals.ActiveVessel.parts[i].skinTemperature, maxTemp);
				}

				return maxTemp;
			}
		}
	}
}
