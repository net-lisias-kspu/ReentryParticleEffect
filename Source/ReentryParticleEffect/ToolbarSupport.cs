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
using System.Collections.Generic;

using UnityEngine;
using KSP.UI.Screens;

using KSPe.Annotations;
using Toolbar = KSPe.UI.Toolbar;
using GUI = KSPe.UI.GUI;
using GUILayout = KSPe.UI.GUILayout;

namespace ReentryParticleEffect
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class ToolbarController : MonoBehaviour
	{
		internal static KSPe.UI.Toolbar.Toolbar Instance => KSPe.UI.Toolbar.Controller.Instance.Get<ToolbarController>();

		[UsedImplicitly]
		private void Start()
		{
			KSPe.UI.Toolbar.Controller.Instance.Register<ToolbarController>(Version.FriendlyName);
		}
	}

	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class ToolbarButton : MonoBehaviour
	{
		internal static KSPe.UI.Toolbar.Toolbar Instance => KSPe.UI.Toolbar.Controller.Instance.Get<ToolbarController>();

		[UsedImplicitly]
		private void Start()
		{
		#if DEBUG
			Toolbar.Button b = Toolbar.Button.Create(this
					, ApplicationLauncher.AppScenes.FLIGHT
					, Version.FriendlyName
				);
			b.Toolbar.Add(Toolbar.Button.ToolbarEvents.Kind.Active
					, new Toolbar.Button.Event(this.OnActivated, this.OnDeactivated)
				);
		#endif
		}

		private void OnDeactivated()
		{
			ReentryParticleEffect.DrawGui = false;
		}

		private void OnActivated()
		{
			ReentryParticleEffect.DrawGui = true;
		}

		[UsedImplicitly]
		private void OnDestroy()
		{
			Instance.Destroy();
		}
	}

}
