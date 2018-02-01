using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jesse.Character
{
	public delegate void SpottedTargetHandler(object sender, SpottedTargetEventArgs d);
	public class SpottedTargetEventArgs : EventArgs
	{
		public SpottedTargetEventArgs()
		{

		}
	}
}
