using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.ADAOPS
{
	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: ConfigParameters
	/// </summary>
	//////////////////////////////////////////////////////////////////////////
	public class ConfigParameters
	{
		// public
		public bool showBasicActions = false;
		public bool showAdvancedActions = false;
		public bool showMeshDecalsConfigOptions = false;
		public bool showPrefabConfigOptions = false;
		public bool showProjectedDecalsConfigOptions = false;
		public bool showBasicGeneralOptions = false;
		public bool showBasicActionsAlways = true;
		public bool showAdvancedActionsAlways = true;
		public bool hideBasicHelp = false;
		public bool showExtraOptions = false;


		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// ConfigParameters -- Constructor
		/// </summary>
		//////////////////////////////////////////////////////////////////////////
		public ConfigParameters ()
		{
		}
	}
}