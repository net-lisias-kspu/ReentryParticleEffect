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
using UnityEngine;

namespace ReentryParticleEffect
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	internal class Startup : MonoBehaviour
	{
		private void Start()
		{
			Log.force("Version {0}", Version.Text);

			try
			{
				KSPe.Util.Installation.Check<Startup>(typeof(Version));
			}
			catch (KSPe.Util.InstallmentException e)
			{
				Log.error(e, this);
				KSPe.Common.Dialogs.ShowStopperAlertBox.Show(e);
			}
		}
	}
}
