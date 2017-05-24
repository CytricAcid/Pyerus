using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Serialization;

namespace STB.ADAOPS
{
	public class ConfigSaver
	{
		public ConfigParameters parameters = null;


		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// ConfigSaver -- Constructor
		/// </summary>
		//////////////////////////////////////////////////////////////////////////
		public ConfigSaver ()
		{
			// create config folder
			Directory.CreateDirectory (Application.dataPath + "/STB/ADAOPS/Config/");

			// load parameters
			parameters = new ConfigParameters ();

			// read actual config
			ReadConfig ();
		}
		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetConfigFilePath
		/// </summary>
		//////////////////////////////////////////////////////////////////////////
		string GetConfigFilePath ()
		{
			string configFileName = "ADAOPS_Config_0004.txt";
			
			string configFilePath = Application.dataPath + "/STB/ADAOPS/Config/" + configFileName;	
			
#if UNITY_ANDROID && !UNITY_EDITOR
			configFilePath = Application.persistentDataPath + configFileName;
#endif
			
			//print ("configFilePath: " + configFilePath);
			
			return configFilePath;
		}
		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// CreateFileIfNotExists
		/// </summary>
		//////////////////////////////////////////////////////////////////////////
		void CreateFileIfNotExists (string filePath)
		{
			try
			{
				StreamReader reader = new StreamReader (filePath);
				
				reader.Read ();
				
				reader.Close ();
			}
			catch (Exception)
			{
				Debug.Log ("Create file in " + filePath);
				StreamWriter writer = new StreamWriter (filePath);
				writer.Close ();
			}
		}
		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// ReadBool
		/// </summary>
		//////////////////////////////////////////////////////////////////////////
		public bool ReadBool (StreamReader reader)
		{
			string actualParameter = reader.ReadLine ();

			return ((actualParameter == "True") || (actualParameter == "true") || (actualParameter == "TRUE"));
		}
		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// ReadConfig
		/// </summary>
		//////////////////////////////////////////////////////////////////////////
		public void ReadConfig ()
		{
			try
			{
				StreamReader reader = new StreamReader (GetConfigFilePath ());

				parameters.showBasicActions = ReadBool (reader);
				parameters.showAdvancedActions = ReadBool (reader);
				parameters.showMeshDecalsConfigOptions = ReadBool (reader);
				parameters.showPrefabConfigOptions = ReadBool (reader);
				parameters.showProjectedDecalsConfigOptions = ReadBool (reader);
				parameters.showBasicGeneralOptions = ReadBool (reader);
				parameters.showBasicActionsAlways = ReadBool (reader);
				parameters.showAdvancedActionsAlways = ReadBool (reader);
				parameters.hideBasicHelp = ReadBool (reader);
				parameters.showExtraOptions = ReadBool (reader);
				
				reader.Close ();
			}
			catch (Exception)
			{
				//Debug.Log ("NOTE: something is wrong when ReadConfig -> " + e);
			}
			
			
			SaveConfig ();
		}
		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// SaveConfig
		/// </summary>
		//////////////////////////////////////////////////////////////////////////
		public void SaveConfig ()
		{
			try
			{	
				
				StreamWriter writer = new StreamWriter (GetConfigFilePath ());

				writer.WriteLine (parameters.showBasicActions);
				writer.WriteLine (parameters.showAdvancedActions);
				writer.WriteLine (parameters.showMeshDecalsConfigOptions);
				writer.WriteLine (parameters.showPrefabConfigOptions);
				writer.WriteLine (parameters.showProjectedDecalsConfigOptions);
				writer.WriteLine (parameters.showBasicGeneralOptions);
				writer.WriteLine (parameters.showBasicActionsAlways);
				writer.WriteLine (parameters.showAdvancedActionsAlways);
				writer.WriteLine (parameters.hideBasicHelp);
				writer.WriteLine (parameters.showExtraOptions);
				
				writer.Close ();
			}
			catch (Exception e)
			{
				Debug.Log ("NOTE: something is wrong when ReadConfig -> " + e);
			}
		}
	}
}
