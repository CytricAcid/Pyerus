// Toony Colors Pro+Mobile 2
// (c) 2014-2017 Jean Moreno

//#define DEBUG_MODE

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

// Utility to generate custom Toony Colors Pro 2 shaders with specific features

public class TCP2_ShaderGenerator : EditorWindow
{
	//--------------------------------------------------------------------------------------------------

	[MenuItem(TCP2_Menu.MENU_PATH + "Shader Generator", false, 500)]
	static void OpenTool()
	{
		GetWindowTCP2();
	}

	static public void OpenWithShader(Shader shader)
	{
		TCP2_ShaderGenerator shaderGenerator = GetWindowTCP2();
		shaderGenerator.LoadCurrentConfigFromShader(shader);
	}

	static private TCP2_ShaderGenerator GetWindowTCP2()
	{
		TCP2_ShaderGenerator window = EditorWindow.GetWindow<TCP2_ShaderGenerator>(true, "TCP2 : Shader Generator", true);
		window.minSize = new Vector2(375f, 400f);
		window.maxSize = new Vector2(500f, 900f);
		return window;
	}

	//--------------------------------------------------------------------------------------------------
	// UI
	
	//Represents a template
	public class ShaderGeneratorTemplate
	{
		public TextAsset textAsset { get; private set; }
		public string templateInfo;
		public string templateWarning;
		public string templateType;
		public bool newSystem;			//if false, use the hard-coded GUI and dependencies/conditions
		public UIFeature[] uiFeatures;

		public ShaderGeneratorTemplate()
		{
			TryLoadTextAsset();
		}

		public void SetTextAsset( TextAsset templateAsset )
		{
			if (this.textAsset != templateAsset)
			{
				this.textAsset = templateAsset;
				UpdateTemplateMeta();
			}
		}

		public void FeaturesGUI(TCP2_Config config)
		{
			if (this.uiFeatures == null)
			{
				EditorGUILayout.HelpBox("Couldn't parse the features from the Template.", MessageType.Error);
				return;
			}

			int length = this.uiFeatures.Length;
			for (int i = 0; i < length; i++)
			{
				this.uiFeatures[i].DrawGUI(config);
			}
		}

		public string GetMaskDisplayName(string maskFeature)
		{
			foreach(var uiFeature in this.uiFeatures)
			{
				if(uiFeature is UIFeature_Mask && (uiFeature as UIFeature_Mask).MaskKeyword == maskFeature)
				{
					return (uiFeature as UIFeature_Mask).DisplayName;
				}
			}

			return "Unknown Mask";
		}

		public bool GetMaskDependency(string maskFeature, TCP2_Config config)
		{
			foreach (var uiFeature in this.uiFeatures)
			{
				if (uiFeature is UIFeature_Mask && (uiFeature as UIFeature_Mask).Keyword == maskFeature)
				{
					return uiFeature.Enabled(config);
				}
			}

			return true;
		}

		//Try to load a Template according to a config type and/or file
		public void TryLoadTextAsset( TCP2_Config config = null )
		{
			string configFile = config != null ? config.templateFile : null;

			//Append file extension if necessary
			if (!string.IsNullOrEmpty(configFile) && !configFile.EndsWith(".txt"))
			{
				configFile = configFile + ".txt";
			}

			TextAsset loadedTextAsset = null;

			if (!string.IsNullOrEmpty(configFile))
			{
				TextAsset conf = LoadTextAsset(configFile);
				if (conf != null)
				{
					loadedTextAsset = conf;
					if (loadedTextAsset != null)
					{
						SetTextAsset(loadedTextAsset);
						return;
					}
				}
			}

			//New name as of 2.3
			loadedTextAsset = LoadTextAsset("TCP2_ShaderTemplate_Default.txt");
			if (loadedTextAsset != null)
			{
				SetTextAsset(loadedTextAsset);
				return;
			}

			//Old legacy name
			loadedTextAsset = LoadTextAsset("TCP2_User_Unity5.txt");
			if (loadedTextAsset != null)
			{
				SetTextAsset(loadedTextAsset);
				return;
			}
		}

		//--------

		private TextAsset LoadTextAsset( string filename )
		{
			TextAsset asset = AssetDatabase.LoadAssetAtPath(string.Format("Assets/JMO Assets/Toony Colors Pro/Editor/Shader Templates/{0}", filename), typeof(TextAsset)) as TextAsset;
			if (asset == null)
			{
				string filenameNoExtension = Path.GetFileNameWithoutExtension(filename);
				string[] guids = AssetDatabase.FindAssets(string.Format("{0} t:TextAsset", filenameNoExtension));
				if (guids.Length >= 1)
				{
					string path = AssetDatabase.GUIDToAssetPath(guids[0]);
					asset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
				}
			}

			return asset;
		}

		private void UpdateTemplateMeta()
		{
			uiFeatures = null;
			this.newSystem = false;
			this.templateInfo = null;
			this.templateWarning = null;
			this.templateType = null;

			if (this.textAsset != null && !string.IsNullOrEmpty(this.textAsset.text))
			{
				using (System.IO.StringReader reader = new StringReader(this.textAsset.text))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						if (line.StartsWith("#INFO="))
						{
							this.templateInfo = line.Substring(6).TrimEnd().Replace("  ", "\n");
						}

						else if (line.StartsWith("#WARNING="))
						{
							this.templateWarning = line.Substring(9).TrimEnd().Replace("  ", "\n");
						}

						else if (line.StartsWith("#CONFIG="))
						{
							this.templateType = line.Substring(8).TrimEnd().ToLower();
						}

						else if (line.StartsWith("#FEATURES"))
						{
							this.newSystem = true;
							this.uiFeatures = UIFeature.GetUIFeatures(reader);
						}

						//Config meta should appear before the Shader name line
						else if (line.StartsWith("Shader"))
						{
							return;
						}
					}
				}
			}
		}
	}

	private ShaderGeneratorTemplate _Template;
	private ShaderGeneratorTemplate Template
	{
		get
		{
			if(_Template == null)
				_Template = new ShaderGeneratorTemplate();
			return _Template;
		}
	}

	private TextAsset[] LoadAllTemplates()
	{
		var list = new List<TextAsset>();

		string systemPath = Application.dataPath + @"/JMO Assets/Toony Colors Pro/Editor/Shader Templates/";
		if(!Directory.Exists(systemPath))
		{
			string rootDir = TCP2_Utils.FindReadmePath();
			systemPath = rootDir.Replace(@"\", "/") + "/Editor/Shader Templates/";
		}

		if(Directory.Exists(systemPath))
		{

			string[] txtFiles = Directory.GetFiles(systemPath, "*.txt", SearchOption.AllDirectories);

			foreach (var sysPath in txtFiles)
			{
				string unityPath = sysPath;
				if (TCP2_Utils.SystemToUnityPath(ref unityPath))
				{
					var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(unityPath);
					if (textAsset != null && !list.Contains(textAsset))
					{
						list.Add(textAsset);
					}
				}
			}

			list.Sort(( x, y ) => x.name.CompareTo(y.name));
			return list.ToArray();
		}

		return null;
	}

	//--------------------------------------------------------------------------------------------------
	// UI from Template System

	#region UIFeatures

	public class UIFeature
	{
		protected const float LABEL_WIDTH = 210f;

		static GUIContent tempContent = new GUIContent();
		static protected GUIContent TempContent(string label, string tooltip = null)
		{
			tempContent.text = label;
			tempContent.tooltip = tooltip;
			return tempContent;
		}

		public string label;
		public string tooltip;
		public string[] requires;	//features required for this feature to be enabled (AND)
		public string[] requiresOr;	//features required for this feature to be enabled (OR)
		public string[] excludes;   //features required to be OFF for this feature to be enabled
		public string[] excludesAll;   //features required to be OFF for this feature to be enabled
		public bool showHelp = true;
		public bool increaseIndent;
		public string helpTopic;

		public bool customGUI;	//complete custom GUI that overrides the default behaviors (e.g. separator)

		//Initialize a UIFeature given a list of arbitrary properties
		public UIFeature( List<KeyValuePair<string, string>> list )
		{
			if(list != null)
			{
				foreach(var kvp in list)
				{
					ProcessProperty(kvp.Key, kvp.Value);
				}
			}
		}

		//Process a property from the Template in the form key=value
		virtual protected void ProcessProperty( string key, string value )
		{
			//Common properties to all UIFeature classes
			switch (key)
			{
				case "lbl": this.label = value.Replace("  ", "\n"); break;
				case "tt": this.tooltip = value.Replace(@"\n", "\n"); break;
				case "help": this.showHelp = bool.Parse(value); break;
				case "indent": this.increaseIndent = bool.Parse(value); break;
				case "needs": this.requires = value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries); break;
				case "needsOr": this.requiresOr = value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries); break;
				case "excl": this.excludes = value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries); break;
				case "exclAll": this.excludesAll = value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries); break;
				case "hlptop": this.helpTopic = value; break;
			}
		}

		static Rect HeaderRect(ref Rect lineRect, float width)
		{
			Rect rect = lineRect;
			rect.width = width;

			lineRect.x += rect.width;
			lineRect.width -= rect.width;

			return rect;
		}

		public void DrawGUI(TCP2_Config config)
		{
			if(customGUI)
			{
				DrawGUI(new Rect(), config);
				return;
			}

			bool enabled = this.Enabled(config);
			GUI.enabled = enabled;
			bool visible = (sHideDisabled && this.increaseIndent) ? enabled : true;

			if (visible)
			{
				//Total line rect
				Rect position = EditorGUILayout.GetControlRect();

				//Help
				if (this.showHelp)
				{
					Rect helpRect = HeaderRect(ref position, 20f);
					TCP2_GUI.HelpButton(helpRect, label, string.IsNullOrEmpty(helpTopic) ? label : helpTopic);
				}
				else
				{
					HeaderRect(ref position, 20f);
				}

				//Indent
				if (this.increaseIndent)
				{
					HeaderRect(ref position, 6f);
				}

				//Label
				Rect labelPosition = HeaderRect(ref position, LABEL_WIDTH - position.x);
				TCP2_GUI.SubHeader(labelPosition, TempContent((increaseIndent ? "▪ " : "") + this.label, this.tooltip), this.Highlighted(config));

				//Actual property
				DrawGUI(position, config);
			}

			GUI.enabled = sGUIEnabled;
		}

		//Internal DrawGUI: actually draws the feature
		virtual protected void DrawGUI(Rect position, TCP2_Config config)
		{
			GUI.Label(position, "Unknown feature type for: " + this.label);
		}

		//Defines if the feature is selected/toggle/etc. or not
		virtual protected bool Highlighted(TCP2_Config config)
		{
			return false;
		}

		public bool Enabled(TCP2_Config config)
		{
			bool enabled = true;
			if (this.requiresOr != null)
			{
				enabled = false;
				enabled |= config.HasFeaturesAny(this.requiresOr);
			}
			if (this.excludesAll != null)
				enabled &= !config.HasFeaturesAll(this.excludesAll);
			if (this.requires != null)
				enabled &= config.HasFeaturesAll(this.requires);
			if (this.excludes != null)
				enabled &= !config.HasFeaturesAny(this.excludes);
			return enabled;
		}

		//Parses a #FEATURES text block
		static public UIFeature[] GetUIFeatures( StringReader reader )
		{
			List<UIFeature> uiFeaturesList = new List<UIFeature>();
			string subline;
			int overflow = 0;
			while ((subline = reader.ReadLine()) != "#END")
			{
				//Just in case template file is badly written
				overflow++;
				if (overflow > 99999)
					break;

				//Empty line
				if (string.IsNullOrEmpty(subline))
					continue;

				string[] data = subline.Split(new char[] { '\t' }, System.StringSplitOptions.RemoveEmptyEntries);

				//Skip empty or comment # lines
				if (data == null || data.Length == 0 || (data.Length > 0 && data[0].StartsWith("#")))
					continue;

				List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();
				for (int i = 1; i < data.Length; i++)
				{
					var sdata = data[i].Split('=');
					kvpList.Add(new KeyValuePair<string, string>(sdata[0], sdata[1]));
				}

				UIFeature feature = null;
				switch (data[0])
				{
					case "---":				feature = new UIFeature_Separator(); break;
					case "space":			feature = new UIFeature_Space(kvpList); break;
					case "flag":			feature = new UIFeature_Flag(kvpList);	break;
					case "float":			feature = new UIFeature_Float(kvpList); break;
					case "subh":			feature = new UIFeature_SubHeader(kvpList); break;
					case "header":			feature = new UIFeature_Header(kvpList); break;
					case "warning":			feature = new UIFeature_Warning(kvpList); break;
					case "sngl":			feature = new UIFeature_Single(kvpList); break;
					case "mult":			feature = new UIFeature_Multiple(kvpList); break;
					case "keyword":			feature = new UIFeature_Keyword(kvpList); break;
					case "mask":			feature = new UIFeature_Mask(kvpList); break;
					case "shader_target":	feature = new UIFeature_ShaderTarget(); break;
					
					default: feature = new UIFeature(kvpList); break;
				}

				uiFeaturesList.Add(feature);
			}
			return uiFeaturesList.ToArray();
		}
	}

	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	// SINGLE FEATURE TOGGLE

	public class UIFeature_Single : UIFeature
	{
		string keyword;
		string[] toggles;    //features forced to be toggled when this feature is enabled

		public UIFeature_Single( List<KeyValuePair<string, string>> list ) : base(list) { }

		protected override void ProcessProperty( string key, string value )
		{
			if(key == "kw")
				this.keyword = value;
			else if(key == "toggles")
				this.toggles = value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
			else
				base.ProcessProperty(key, value);
		}

		protected override void DrawGUI(Rect position, TCP2_Config config)
		{
			bool feature = Highlighted(config);
			EditorGUI.BeginChangeCheck();
			feature = EditorGUI.Toggle(position, feature);

			if (EditorGUI.EndChangeCheck())
			{
				config.ToggleFeature(this.keyword, feature);

				if (toggles != null)
				{
					foreach (var t in toggles)
						config.ToggleFeature(t, feature);
				}
			}
		}

		protected override bool Highlighted( TCP2_Config config )
		{
			return config.HasFeature(keyword);
		}
	}

	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	// FEATURES COMBOBOX

	public class UIFeature_Multiple : UIFeature
	{
		string[] labels;
		string[] features;

		public UIFeature_Multiple( List<KeyValuePair<string, string>> list ) : base(list) { }

		protected override void ProcessProperty( string key, string value )
		{
			if (key == "kw")
			{
				string[] data = value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
				this.labels = new string[data.Length];
				this.features = new string[data.Length];

				for(int i = 0; i < data.Length; i++)
				{
					string[] lbl_feat = data[i].Split('|');
					if(lbl_feat.Length != 2)
					{
						Debug.LogWarning("[UIFeature_Multiple] Invalid data:" + data[i]);
						continue;
					}

					labels[i] = lbl_feat[0];
					features[i] = lbl_feat[1];
				}
			}
			else
				base.ProcessProperty(key, value);
		}

		protected override void DrawGUI(Rect position, TCP2_Config config)
		{
			int feature = GetSelectedFeature(config);
			if (feature < 0) feature = 0;

			EditorGUI.BeginChangeCheck();
			feature = EditorGUI.Popup(position, feature, labels);
			if (EditorGUI.EndChangeCheck())
			{
				for (int i = 0; i < features.Length; i++)
				{
					bool enable = (i == feature);
					config.ToggleFeature(features[i], enable);
				}
			}
		}

		private int GetSelectedFeature(TCP2_Config config)
		{
			for (int i = 0; i < features.Length; i++)
			{
				if (config.HasFeature(features[i]))
					return i;
			}

			return -1;
		}

		protected override bool Highlighted( TCP2_Config config )
		{
			int feature = GetSelectedFeature(config);
			return feature > 0;
		}
	}

	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	// KEYWORD COMBOBOX

	public class UIFeature_Keyword : UIFeature
	{
		string keyword;
		string[] labels;
		string[] values;
		int defaultValue = 0;
		bool forceValue = false;

		public UIFeature_Keyword( List<KeyValuePair<string, string>> list ) : base(list) { }

		protected override void ProcessProperty( string key, string value )
		{
			if (key == "kw")
				this.keyword = value;
			else if (key == "default")
				this.defaultValue = int.Parse(value);
			else if (key == "forceKeyword")
				this.forceValue = bool.Parse(value);
			else if (key == "values")
			{
				string[] data = value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
				this.labels = new string[data.Length];
				this.values = new string[data.Length];

				for (int i = 0; i < data.Length; i++)
				{
					string[] lbl_feat = data[i].Split('|');
					if (lbl_feat.Length != 2)
					{
						Debug.LogWarning("[UIFeature_Keyword] Invalid data:" + data[i]);
						continue;
					}

					labels[i] = lbl_feat[0];
					values[i] = lbl_feat[1];
				}
			}
			else
				base.ProcessProperty(key, value);
		}

		protected override void DrawGUI( Rect position, TCP2_Config config )
		{
			int selectedValue = GetSelectedValue(config);
			if (selectedValue < 0)
			{
				selectedValue = defaultValue;
				if(forceValue && this.Enabled(config))
				{
					config.SetKeyword(keyword, values[defaultValue]);
				}
			}

			EditorGUI.BeginChangeCheck();
			selectedValue = EditorGUI.Popup(position, selectedValue, labels);
			if (EditorGUI.EndChangeCheck())
			{
				if (string.IsNullOrEmpty(values[selectedValue]))
					config.RemoveKeyword(keyword);
				else
					config.SetKeyword(keyword, values[selectedValue]);
			}
		}

		private int GetSelectedValue( TCP2_Config config )
		{
			string currentValue = config.GetKeyword(keyword);
			for (int i = 0; i < values.Length; i++)
			{
				if (currentValue == values[i])
					return i;
			}

			return -1;
		}

		protected override bool Highlighted( TCP2_Config config )
		{
			int feature = GetSelectedValue(config);
			return feature != defaultValue;
		}
	}

	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	// MASK

	public class UIFeature_Mask : UIFeature
	{
		public string Keyword { get { return keyword; } }
		public string MaskKeyword { get { return maskKeyword; } }
		public string DisplayName { get { return displayName; } }

		string maskKeyword;
		string channelKeyword;
		string keyword;
		string displayName;

		public UIFeature_Mask( List<KeyValuePair<string, string>> list ) : base(list) { }

		protected override void ProcessProperty( string key, string value )
		{
			if (key == "kw")
				this.keyword = value;
			else if (key == "ch")
				this.channelKeyword = value;
			else if (key == "msk")
				this.maskKeyword = value;
			else if (key == "dispName")
				this.displayName = value;
			else
				base.ProcessProperty(key, value);
		}

		string[] labels = new string[] { "Off", "Main Texture", "Mask 1", "Mask 2", "Mask 3", "Vertex Colors" };
		string[] masks = new string[] { "", "mainTex", "mask1", "mask2", "mask3", "vcolors" };
		string[] uvs = new string[] { "Main Tex UV", "Independent UV" };

		protected override void DrawGUI(Rect position, TCP2_Config config)
		{
			//GUIMask(config, this.label, this.tooltip, this.maskKeyword, this.channelKeyword, this.keyword, this.Enabled(config), this.increaseIndent, helpTopic: this.helpTopic, helpIndent: this.helpIndent);

			int curMask = System.Array.IndexOf(masks, config.GetKeyword(this.maskKeyword));
			if (curMask < 0) curMask = 0;
			TCP2_Utils.TextureChannel curChannel = TCP2_Utils.FromShader(config.GetKeyword(this.channelKeyword));
			string uvKey = (curMask > 1 && curMask < 5) ? "UV_" + masks[curMask] : null;
			int curUv = System.Array.IndexOf(uvs, config.GetKeyword(uvKey));
			if (curUv < 0) curUv = 0;

			EditorGUI.BeginChangeCheck();

			//Calculate rects
			Rect helpButton = position;
			helpButton.width = 16f;
			helpButton.x += 2f;
			position.width -= helpButton.width;
			helpButton.x += position.width;

			//Mask type (MainTex, 1, 2, 3)
			Rect sideRect = position;
			sideRect.width = position.width * 0.75f / 2f;
			curMask = EditorGUI.Popup(sideRect, curMask, labels);

			//Mask Channel (RGBA)
			Rect middleRect = position;
			middleRect.width = position.width * 0.25f;
			middleRect.x += sideRect.width;
			GUI.enabled &= curMask > 0;
			curChannel = (TCP2_Utils.TextureChannel)EditorGUI.EnumPopup(middleRect, curChannel);

			//Mask UVs
			sideRect.x += sideRect.width + middleRect.width;
			GUI.enabled &= curMask > 1 && curMask < 5;
			curUv = EditorGUI.Popup(sideRect, curUv, uvs);

			//Mask Help
			TCP2_GUI.HelpButton(helpButton, "Masks");

			if (EditorGUI.EndChangeCheck())
			{
				config.SetKeyword(this.maskKeyword, masks[curMask]);
				if (curMask > 0)
				{
					config.SetKeyword(this.channelKeyword, curChannel.ToShader());
				}
				if (curMask > 1 && !string.IsNullOrEmpty(uvKey))
				{
					config.SetKeyword(uvKey, uvs[curUv]);
				}
				config.ToggleFeature("VCOLORS_MASK", (curMask == 5));
				config.ToggleFeature(this.keyword, (curMask > 0));
			}
		}

		protected override bool Highlighted( TCP2_Config config )
		{
			int curMask = GetCurrentMask(config);
			return curMask > 0;
		}

		int GetCurrentMask(TCP2_Config config)
		{
			int curMask = System.Array.IndexOf(masks, config.GetKeyword(this.maskKeyword));
			return curMask;
		}
	}

	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	// SHADER TARGET

	public class UIFeature_ShaderTarget : UIFeature
	{
		public UIFeature_ShaderTarget() : base(null)
		{
			this.customGUI = true;
		}

		protected override void DrawGUI(Rect position, TCP2_Config config)
		{
			EditorGUILayout.BeginHorizontal();
			TCP2_GUI.HelpButton("Shader Target");
			TCP2_GUI.SubHeader("Shader Target", "Defines the shader target level to compile for", config.shaderTarget != 30, LABEL_WIDTH - 24f);
			int newTarget = EditorGUILayout.IntPopup(config.shaderTarget,
#if UNITY_5_4_OR_NEWER
				new string[] { "2.0", "2.5", "3.0", "3.5", "4.0", "5.0" },
				new int[] { 20, 25, 30, 35, 40, 50 });
#else
				new string[] { "2.0", "3.0", "4.0", "5.0" },
				new int[] { 20, 30, 40, 50 });
#endif
			if (newTarget != config.shaderTarget)
			{
				config.shaderTarget = newTarget;
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	// SURFACE SHADER FLAG

	public class UIFeature_Flag : UIFeature
	{
		string keyword;
		string[] toggles;    //features forced to be toggled when this flag is enabled

		public UIFeature_Flag(List<KeyValuePair<string,string>> list) : base(list)
		{
			this.showHelp = false;
		}

		protected override void ProcessProperty( string key, string value )
		{
			if (key == "kw")
				this.keyword = value;
			else if (key == "toggles")
				this.toggles = value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
			else
				base.ProcessProperty(key, value);
		}

		protected override void DrawGUI( Rect position, TCP2_Config config )
		{
			bool flag = Highlighted(config);
			EditorGUI.BeginChangeCheck();
			flag = EditorGUI.Toggle(position, flag);

			if (EditorGUI.EndChangeCheck())
			{
				config.ToggleFlag(this.keyword, flag);

				if (toggles != null)
				{
					foreach (var t in toggles)
						config.ToggleFeature(t, flag);
				}
			}
		}

		protected override bool Highlighted( TCP2_Config config )
		{
			return config.HasFlag(this.keyword);
		}
	}

	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	// FIXED FLOAT

	public class UIFeature_Float : UIFeature
	{
		string keyword;
		float defaultValue;
		float min = float.MinValue;
		float max = float.MaxValue;

		public UIFeature_Float( List<KeyValuePair<string, string>> list ) : base(list) { }

		protected override void ProcessProperty( string key, string value )
		{
			if (key == "kw")
				this.keyword = value;
			else if (key == "default")
				this.defaultValue = float.Parse(value);
			else if (key == "min")
				this.min = float.Parse(value);
			else if (key == "max")
				this.max = float.Parse(value);
			else
				base.ProcessProperty(key, value);
		}

		protected override void DrawGUI(Rect position, TCP2_Config config)
		{
			string currentValueStr = config.GetKeyword(keyword);
			float currentValue = defaultValue;
			if(!float.TryParse(currentValueStr, out currentValue))
			{
				//Only enforce keyword if feature is enabled
				if(this.Enabled(config))
					config.SetKeyword(keyword, currentValue.ToString("0.0"));
			}

			EditorGUI.BeginChangeCheck();
			float newValue = currentValue;
			newValue = Mathf.Clamp(EditorGUI.FloatField(position, currentValue), min, max);
			if (EditorGUI.EndChangeCheck())
			{
				if (newValue != currentValue)
				{
					config.SetKeyword(keyword, newValue.ToString("0.0"));
				}
			}
		}
	}

	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	// DECORATORS

	public class UIFeature_Separator : UIFeature
	{
		public UIFeature_Separator() : base(null)
		{
			this.customGUI = true;
		}

		protected override void DrawGUI(Rect position, TCP2_Config config)
		{
			Space();
		}
	}

	public class UIFeature_Space : UIFeature
	{
		float space = 8f;

		public UIFeature_Space(List<KeyValuePair<string,string>> list) : base(list)
		{
			this.customGUI = true;
		}

		protected override void ProcessProperty( string key, string value )
		{
			if(key == "space")
				this.space = float.Parse(value);
			else
				base.ProcessProperty(key, value);
		}

		protected override void DrawGUI(Rect position, TCP2_Config config)
		{
			GUILayout.Space(space);
		}
	}

	public class UIFeature_SubHeader : UIFeature
	{
		public UIFeature_SubHeader(List<KeyValuePair<string,string>> list) : base(list)
		{
			this.customGUI = true;
		}

		protected override void DrawGUI(Rect position, TCP2_Config config)
		{
			TCP2_GUI.SubHeaderGray(this.label);
		}
	}

	public class UIFeature_Header : UIFeature
	{
		public UIFeature_Header( List<KeyValuePair<string, string>> list ) : base(list)
		{
			this.customGUI = true;
		}

		protected override void DrawGUI(Rect position, TCP2_Config config)
		{
			TCP2_GUI.Header(this.label);
		}
	}

	public class UIFeature_Warning : UIFeature
	{
		MessageType msgType = MessageType.Warning;

		public UIFeature_Warning(List<KeyValuePair<string, string>> list) : base(list)
		{
			this.customGUI = true;
		}

		protected override void ProcessProperty( string key, string value )
		{
			if(key == "msgType")
				this.msgType = (MessageType)System.Enum.Parse(typeof(MessageType), value, true);
			else
				base.ProcessProperty(key, value);
		}

		protected override void DrawGUI(Rect position, TCP2_Config config)
		{
			if(this.Enabled(config))
				EditorGUILayout.HelpBox(this.label, msgType);
		}
	}

	#endregion

	//--------------------------------------------------------------------------------------------------
	// INTERFACE

	private Shader mCurrentShader;
	private TCP2_Config mCurrentConfig;
	private int mCurrentHash;
	private Shader[] mUserShaders;
	private GenericMenu loadTemplateMenu;
	private List<string> mUserShadersLabels;
	private Vector2 mScrollPosition;
	private int mConfigChoice;
	private bool mIsModified;
	private bool mDirtyConfig;

	//Static
	static private bool sHideDisabled;
	static private bool sAutoNames;
	static private bool sOverwriteConfigs;
	static private bool sLoadAllShaders;
	static private bool sSelectGeneratedShader;
	static private bool sGUIEnabled;

#if DEBUG_MODE
	private string mDebugText;
	private bool mDebugExpandUserData;
	private ShaderImporter mCurrentShaderImporter;
#endif

	void OnEnable()
	{
		LoadUserPrefs();
		ReloadUserShaders();
		if(mUserShaders != null && mUserShaders.Length > 0)
		{
			if((mConfigChoice-1) > 0 && (mConfigChoice-1) < mUserShaders.Length)
			{
				mCurrentShader = mUserShaders[mConfigChoice-1];
				LoadCurrentConfigFromShader(mCurrentShader);
			}
			else
				NewShader();
		}

		var allTemplates = LoadAllTemplates();
		if(allTemplates != null && allTemplates.Length > 0)
		{
			loadTemplateMenu = new GenericMenu();
			foreach (var textAsset in allTemplates)
			{
				//Exceptions
				if (textAsset.name.Contains("TCP2_User_Unity5_Old"))
					continue;

				//Find name
				string name = textAsset.name;
				var sr = new StringReader(textAsset.text);
				string line;
				while((line = sr.ReadLine()) != null)
				{
					if(line.StartsWith("#NAME"))
					{
						name = line.Substring(6);
						break;
					}
					if(line.StartsWith("#FEATURES"))
					{
						break;
					}
				}

				loadTemplateMenu.AddItem(new GUIContent(name), false, OnLoadTemplate, textAsset);
			}
		}
	}

	void OnLoadTemplate( object textAsset)
	{
		Template.SetTextAsset(textAsset as TextAsset);
	}

	void OnDisable()
	{
		SaveUserPrefs();
	}

	void OnGUI()
	{
		sGUIEnabled = GUI.enabled;

		EditorGUILayout.BeginHorizontal();
		TCP2_GUI.HeaderBig("TOONY COLORS PRO 2 - SHADER GENERATOR");
		TCP2_GUI.HelpButton("Shader Generator");
		EditorGUILayout.EndHorizontal();
		TCP2_GUI.Separator();

		float lW = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 105f;

		//Avoid refreshing Template meta at every Repaint
		EditorGUILayout.BeginHorizontal();
		TextAsset _tmpTemplate = EditorGUILayout.ObjectField("Template:", Template.textAsset, typeof(TextAsset), false) as TextAsset;
		if (_tmpTemplate != Template.textAsset)
		{
			Template.SetTextAsset(_tmpTemplate);
		}
		//Load template
		if (loadTemplateMenu != null)
		{
			if (GUILayout.Button("Load ▼", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
			{
				loadTemplateMenu.ShowAsContext();
			}
		}
		EditorGUILayout.EndHorizontal();

		//Template not found
		if (Template == null || Template.textAsset == null)
		{
			EditorGUILayout.HelpBox("Couldn't find template file!\n\nVerify that the file 'TCP2_ShaderTemplate_Default.txt' is in your project.\nPlease reimport the pack if you can't find it!", MessageType.Error);
			return;
		}

		//Infobox for custom templates
		if (!string.IsNullOrEmpty(Template.templateInfo))
		{
			EditorGUILayout.HelpBox(Template.templateInfo, MessageType.Info);
		}
		if (!string.IsNullOrEmpty(Template.templateWarning))
		{
			EditorGUILayout.HelpBox(Template.templateWarning, MessageType.Warning);
		}

		TCP2_GUI.Separator();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginHorizontal();
		mCurrentShader = EditorGUILayout.ObjectField("Current Shader:", mCurrentShader, typeof(Shader), false) as Shader;
		if(EditorGUI.EndChangeCheck())
		{
			if(mCurrentShader != null)
			{
				LoadCurrentConfigFromShader(mCurrentShader);
			}
		}
		if(GUILayout.Button("Copy Shader", EditorStyles.miniButton, GUILayout.Width(78f)))
		{
			CopyShader();
		}
		if(GUILayout.Button("New Shader", EditorStyles.miniButton, GUILayout.Width(76f)))
		{
			NewShader();
		}
		EditorGUILayout.EndHorizontal();

		if(mIsModified)
		{
			EditorGUILayout.HelpBox("It looks like this shader has been modified externally/manually. Updating it will overwrite the changes.", MessageType.Warning);
		}

		if(mUserShaders != null && mUserShaders.Length > 0)
		{
			EditorGUI.BeginChangeCheck();
			int prevChoice = mConfigChoice;
			Color gColor = GUI.color;
			GUI.color = mDirtyConfig ? gColor * Color.yellow : GUI.color;
			GUILayout.BeginHorizontal();
			mConfigChoice = EditorGUILayout.Popup("Load Shader:", mConfigChoice, mUserShadersLabels.ToArray());
			if(GUILayout.Button("◄", EditorStyles.miniButtonLeft, GUILayout.Width(22)))
			{
				mConfigChoice--;
				if(mConfigChoice < 1) mConfigChoice = mUserShaders.Length;
			}
			if(GUILayout.Button("►", EditorStyles.miniButtonRight,GUILayout.Width(22)))
			{
				mConfigChoice++;
				if(mConfigChoice > mUserShaders.Length) mConfigChoice = 1;
			}
			GUILayout.EndHorizontal();
			GUI.color = gColor;
			if(EditorGUI.EndChangeCheck() && prevChoice != mConfigChoice)
			{
				bool load = true;
				if(mDirtyConfig)
				{
					if(mCurrentShader != null)
						load = EditorUtility.DisplayDialog("TCP2 : Shader Generation", "You have unsaved changes for the following shader:\n\n" + mCurrentShader.name + "\n\nDiscard the changes and load a new shader?", "Yes", "No");
					else
						load = EditorUtility.DisplayDialog("TCP2 : Shader Generation", "You have unsaved changes.\n\nDiscard the changes and load a new shader?", "Yes", "No");
				}
				
				if(load)
				{
					//New Shader
					if(mConfigChoice == 0)
					{
						NewShader();
					}
					else
					{
						//Load selected Shader
						Shader selectedShader = mUserShaders[mConfigChoice-1];
						mCurrentShader = selectedShader;
						LoadCurrentConfigFromShader(mCurrentShader);
					}
				}
				else
				{
					//Revert choice
					mConfigChoice = prevChoice;
				}
			}
		}

		EditorGUIUtility.labelWidth = lW;
		
		if(mCurrentConfig == null)
		{
			NewShader();
		}

		//Name & Filename
		TCP2_GUI.Separator();
		GUI.enabled = (mCurrentShader == null);
		EditorGUI.BeginChangeCheck();
		mCurrentConfig.ShaderName = EditorGUILayout.TextField(new GUIContent("Shader Name", "Path will indicate how to find the Shader in Unity's drop-down list"), mCurrentConfig.ShaderName);
		mCurrentConfig.ShaderName = Regex.Replace(mCurrentConfig.ShaderName, @"[^a-zA-Z0-9 _!/]", "");
		if(EditorGUI.EndChangeCheck() && sAutoNames)
		{
			AutoNames();
		}
		GUI.enabled &= !sAutoNames;
		EditorGUILayout.BeginHorizontal();
		mCurrentConfig.Filename = EditorGUILayout.TextField("File Name", mCurrentConfig.Filename);
		mCurrentConfig.Filename = Regex.Replace(mCurrentConfig.Filename, @"[^a-zA-Z0-9 _!/]", "");
		GUILayout.Label(".shader", GUILayout.Width(50f));
		EditorGUILayout.EndHorizontal();
		GUI.enabled = sGUIEnabled;

		Space();

		//########################################################################################################
		// FEATURES

		TCP2_GUI.Header("FEATURES");

		//Scroll view
		mScrollPosition = EditorGUILayout.BeginScrollView(mScrollPosition);
		EditorGUI.BeginChangeCheck();

		if (Template.newSystem)
		{
			//New UI embedded into Template
			Template.FeaturesGUI(mCurrentConfig);
		}
		else
		{
			EditorGUILayout.HelpBox("Old template versions aren't supported anymore.", MessageType.Warning);
		}

#if DEBUG_MODE
		TCP2_GUI.SeparatorBig();

		TCP2_GUI.SubHeaderGray("DEBUG MODE");

		GUILayout.BeginHorizontal();
		mDebugText = EditorGUILayout.TextField("Custom", mDebugText);
		if(GUILayout.Button("Add Feature", EditorStyles.miniButtonLeft, GUILayout.Width(80f)))
			mCurrentConfig.Features.Add(mDebugText);
		if(GUILayout.Button("Add Flag", EditorStyles.miniButtonRight, GUILayout.Width(80f)))
			mCurrentConfig.Flags.Add(mDebugText);

		GUILayout.EndHorizontal();
		GUILayout.Label("Features:");
		GUILayout.BeginHorizontal();
		int count = 0;
		for(int i = 0; i < mCurrentConfig.Features.Count; i++)
		{
			if(count >= 3)
			{
				count = 0;
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
			}
			count++;
			if(GUILayout.Button(mCurrentConfig.Features[i], EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
			{
				mCurrentConfig.Features.RemoveAt(i);
				break;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Flags:");
		GUILayout.BeginHorizontal();
		count = 0;
		for(int i = 0; i < mCurrentConfig.Flags.Count; i++)
		{
			if(count >= 3)
			{
				count = 0;
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
			}
			count++;
			if(GUILayout.Button(mCurrentConfig.Flags[i], EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
			{
				mCurrentConfig.Flags.RemoveAt(i);
				break;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Keywords:");
		GUILayout.BeginHorizontal();
		count = 0;
		foreach(KeyValuePair<string,string> kvp in mCurrentConfig.Keywords)
		{
			if(count >= 3)
			{
				count = 0;
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
			}
			count++;
			if(GUILayout.Button(kvp.Key + ":" + kvp.Value, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
			{
				mCurrentConfig.Keywords.Remove(kvp.Key);
				break;
			}
		}
		GUILayout.EndHorizontal();

		//----------------------------------------------------------------

		Space();
		if(mCurrentShader != null)
		{
			if(mCurrentShaderImporter == null)
			{
				mCurrentShaderImporter = ShaderImporter.GetAtPath(AssetDatabase.GetAssetPath(mCurrentShader)) as ShaderImporter;
			}

			if (mCurrentShaderImporter != null && mCurrentShaderImporter.GetShader() == mCurrentShader)
			{
				mDebugExpandUserData = EditorGUILayout.Foldout(mDebugExpandUserData, "Shader UserData");
				if(mDebugExpandUserData)
				{
					string[] userData = mCurrentShaderImporter.userData.Split(',');
					foreach(var str in userData)
					{
						GUILayout.Label(str);
					}
				}
			}
		}
#endif

		//Update config
		if (EditorGUI.EndChangeCheck())
		{
			int newHash = mCurrentConfig.ToHash();
			if(newHash != mCurrentHash)
			{
				mDirtyConfig = true;
			}
			else
			{
				mDirtyConfig = false;
			}
		}

		//Scroll view
		EditorGUILayout.EndScrollView();

		Space();

		//GENERATE

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
#if DEBUG_MODE
		if(GUILayout.Button("Re-Generate All", GUILayout.Width(120f), GUILayout.Height(30f)))
		{
			float progress = 0;
			float total = mUserShaders.Length;
			foreach(Shader s in mUserShaders)
			{
				progress++;
				EditorUtility.DisplayProgressBar("Hold On", "Generating Shader: " + s.name, progress/total);

				mCurrentShader = null;
				LoadCurrentConfigFromShader(s);
				if(mCurrentShader != null && mCurrentConfig != null)
				{
					TCP2_ShaderGeneratorUtils.Compile(mCurrentConfig, s, Template, false, !sOverwriteConfigs, mIsModified);
				}
			}
			EditorUtility.ClearProgressBar();
		}
#endif
		if(GUILayout.Button(mCurrentShader == null ? "Generate Shader" : "Update Shader", GUILayout.Width(120f), GUILayout.Height(30f)))
		{
			if(Template == null)
			{
				EditorUtility.DisplayDialog("TCP2 : Shader Generation", "Can't generate shader: no Template file defined!\n\nYou most likely want to link the TCP2_User.txt file to the Template field in the Shader Generator.", "Ok");
				return;
			}

			//Set config type
			if (Template.templateType != null)
			{
				mCurrentConfig.configType = Template.templateType;
			}

			//Set config file
			mCurrentConfig.templateFile = Template.textAsset.name;

			Shader generatedShader = TCP2_ShaderGeneratorUtils.Compile(mCurrentConfig, mCurrentShader, Template, true, !sOverwriteConfigs, mIsModified);
			ReloadUserShaders();
			if(generatedShader != null)
			{
				mDirtyConfig = false;
				LoadCurrentConfigFromShader(generatedShader);
				mIsModified = false;
			}
		}
		EditorGUILayout.EndHorizontal();
		TCP2_GUI.Separator();

		// OPTIONS
		TCP2_GUI.Header("OPTIONS");

		GUILayout.BeginHorizontal();
		sSelectGeneratedShader = GUILayout.Toggle(sSelectGeneratedShader, new GUIContent("Select Generated Shader", "Will select the generated file in the Project view"), GUILayout.Width(180f));
		sAutoNames = GUILayout.Toggle(sAutoNames, new GUIContent("Automatic Name", "Will automatically generate the shader filename based on its UI name"), GUILayout.ExpandWidth(false));
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		sOverwriteConfigs = GUILayout.Toggle(sOverwriteConfigs, new GUIContent("Always overwrite shaders", "Overwrite shaders when generating/updating (no prompt)"), GUILayout.Width(180f));
		sHideDisabled = GUILayout.Toggle(sHideDisabled, new GUIContent("Hide disabled fields", "Hide properties settings when they cannot be accessed"), GUILayout.ExpandWidth(false));
		GUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUI.BeginChangeCheck();
		TCP2_ShaderGeneratorUtils.CustomOutputDir = GUILayout.Toggle(TCP2_ShaderGeneratorUtils.CustomOutputDir, new GUIContent("Custom Output Directory:", "Will save the generated shaders in a custom directory within the Project"), GUILayout.Width(165f));
		GUI.enabled &= TCP2_ShaderGeneratorUtils.CustomOutputDir;
		if (TCP2_ShaderGeneratorUtils.CustomOutputDir)
		{
			TCP2_ShaderGeneratorUtils.OutputPath = EditorGUILayout.TextField("", TCP2_ShaderGeneratorUtils.OutputPath);
			if(GUILayout.Button("Select...", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
			{
				string path = EditorUtility.OpenFolderPanel("Choose custom output directory for TCP2 generated shaders", "", "");
				if(!string.IsNullOrEmpty(path))
				{
					bool validPath = TCP2_Utils.SystemToUnityPath(ref path);
					if (validPath)
					{
						TCP2_ShaderGeneratorUtils.OutputPath = path.Substring("Assets/".Length);
					}
					else
					{
						EditorApplication.Beep();
						EditorUtility.DisplayDialog("Invalid Path", "The selected path is invalid.\n\nPlease select a folder inside the \"Assets\" folder of your project!", "Ok");
					}
				}
			}
		}
		else
			EditorGUILayout.TextField("", TCP2_ShaderGeneratorUtils.OUTPUT_PATH);
		if (EditorGUI.EndChangeCheck())
		{
			ReloadUserShaders();
		}

		GUI.enabled = sGUIEnabled;
		EditorGUILayout.EndHorizontal();

		EditorGUI.BeginChangeCheck();
		sLoadAllShaders = GUILayout.Toggle(sLoadAllShaders, new GUIContent("Reload Shaders from all Project", "Load shaders from all your Project folders instead of just Toony Colors Pro 2.\nEnable it if you move your generated shader files outside of the default TCP2 Generated folder."), GUILayout.ExpandWidth(false));
		if(EditorGUI.EndChangeCheck())
		{
			ReloadUserShaders();
		}

		TCP2_ShaderGeneratorUtils.SelectGeneratedShader = sSelectGeneratedShader;
	}

	void OnProjectChange()
	{
		ReloadUserShaders();
		if(mCurrentShader == null && mConfigChoice != 0)
		{
			NewShader();
		}
	}

	//--------------------------------------------------------------------------------------------------
	// MISC

	private void LoadUserPrefs()
	{
		sAutoNames = EditorPrefs.GetBool("TCP2_mAutoNames", true);
		sOverwriteConfigs = EditorPrefs.GetBool("TCP2_mOverwriteConfigs", false);
		sHideDisabled = EditorPrefs.GetBool("TCP2_mHideDisabled", true);
		sSelectGeneratedShader = EditorPrefs.GetBool("TCP2_mSelectGeneratedShader", true);
		sLoadAllShaders = EditorPrefs.GetBool("TCP2_mLoadAllShaders", false);
		mConfigChoice = EditorPrefs.GetInt("TCP2_mConfigChoice", 0);
		TCP2_ShaderGeneratorUtils.CustomOutputDir = EditorPrefs.GetBool("TCP2_TCP2_ShaderGeneratorUtils.CustomOutputDir", false);
	}

	private void SaveUserPrefs()
	{
		EditorPrefs.SetBool("TCP2_mAutoNames", sAutoNames);
		EditorPrefs.SetBool("TCP2_mOverwriteConfigs", sOverwriteConfigs);
		EditorPrefs.SetBool("TCP2_mHideDisabled", sHideDisabled);
		EditorPrefs.SetBool("TCP2_mSelectGeneratedShader", sSelectGeneratedShader);
		EditorPrefs.SetBool("TCP2_mLoadAllShaders", sLoadAllShaders);
		EditorPrefs.SetInt("TCP2_mConfigChoice", mConfigChoice);
		EditorPrefs.SetBool("TCP2_TCP2_ShaderGeneratorUtils.CustomOutputDir", TCP2_ShaderGeneratorUtils.CustomOutputDir);
	}

	private void LoadCurrentConfig(TCP2_Config config)
	{
		mCurrentConfig = config;
		mDirtyConfig = false;
		if(sAutoNames)
		{
			AutoNames();
		}
		mCurrentHash = mCurrentConfig.ToHash();
		Template.TryLoadTextAsset(mCurrentConfig);
	}

	private void NewShader()
	{
		mCurrentShader = null;
		mConfigChoice = 0;
		mIsModified = false;
		LoadCurrentConfig(new TCP2_Config());
	}

	private void CopyShader()
	{
		mCurrentShader = null;
		mConfigChoice = 0;
		mIsModified = false;
		TCP2_Config newConfig = mCurrentConfig.Copy();
		newConfig.ShaderName += " Copy";
		newConfig.Filename += " Copy";
		LoadCurrentConfig(newConfig);
	}

	private void LoadCurrentConfigFromShader(Shader shader)
	{
		ShaderImporter shaderImporter = ShaderImporter.GetAtPath(AssetDatabase.GetAssetPath(shader)) as ShaderImporter;
		string[] features;
		string[] flags;
		string[] customData;
		Dictionary<string,string> keywords;
		TCP2_ShaderGeneratorUtils.ParseUserData(shaderImporter, out features, out flags, out keywords, out customData);
		if(features != null && features.Length > 0 && features[0] == "USER")
		{
			mCurrentConfig = new TCP2_Config();
			mCurrentConfig.ShaderName = shader.name;
			mCurrentConfig.Filename = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(shader));
			mCurrentConfig.Features = new List<string>(features);
			mCurrentConfig.Flags = (flags != null) ? new List<string>(flags) : new List<string>();
			mCurrentConfig.Keywords = (keywords != null) ? new Dictionary<string,string>(keywords) : new Dictionary<string,string>();
			mCurrentShader = shader;
			mConfigChoice = mUserShadersLabels.IndexOf(shader.name);
			mDirtyConfig = false;
			AutoNames();
			mCurrentHash = mCurrentConfig.ToHash();

			mIsModified = false;
			if(customData != null && customData.Length > 0)
			{
				foreach(string data in customData)
				{
					//Hash
					if(data.Length > 0 && data[0] == 'h')
					{
						string dataHash = data;
						string fileHash = TCP2_ShaderGeneratorUtils.GetShaderContentHash(shaderImporter);

						if(!string.IsNullOrEmpty(fileHash) && dataHash != fileHash)
						{
							mIsModified = true;
						}
					}
					//Timestamp
					else
					{
						ulong timestamp;
						if(ulong.TryParse(data, out timestamp))
						{
							if(shaderImporter.assetTimeStamp != timestamp)
							{
								mIsModified = true;
							}
						}
					}

					//Shader Model target
					if (data.StartsWith("SM:"))
					{
						mCurrentConfig.shaderTarget = int.Parse(data.Substring(3));
					}

					//Configuration Type
					if (data.StartsWith("CT:"))
					{
						mCurrentConfig.configType = data.Substring(3);
					}

					//Configuration File
					if (data.StartsWith("CF:"))
					{
						mCurrentConfig.templateFile = data.Substring(3);
					}
				}
			}

			//Load appropriate template
			Template.TryLoadTextAsset(mCurrentConfig);
		}
		else
		{
			EditorApplication.Beep();
			this.ShowNotification(new GUIContent("Invalid shader loaded: it doesn't seem to have been generated by the TCP2 Shader Generator!"));
			mCurrentShader = null;
			NewShader();
		}
	}

	private void ReloadUserShaders()
	{
		mUserShaders = GetUserShaders();
		mUserShadersLabels = new List<string>(GetShaderLabels(mUserShaders));

		if(mCurrentShader != null)
		{
			mConfigChoice = mUserShadersLabels.IndexOf(mCurrentShader.name);
		}
	}

	private Shader[] GetUserShaders()
	{
		string rootPath = Application.dataPath + (sLoadAllShaders ? "" : TCP2_ShaderGeneratorUtils.OutputPath);

		if(System.IO.Directory.Exists(rootPath))
		{
			string[] paths = System.IO.Directory.GetFiles(rootPath, "*.shader", System.IO.SearchOption.AllDirectories);
			List<Shader> shaderList = new List<Shader>();

			foreach(string path in paths)
			{
#if UNITY_EDITOR_WIN
				string assetPath = "Assets" + path.Replace(@"\", @"/").Replace(Application.dataPath, "");
#else
				string assetPath = "Assets" + path.Replace(Application.dataPath, "");
#endif
				ShaderImporter shaderImporter = ShaderImporter.GetAtPath(assetPath) as ShaderImporter;
				if(shaderImporter != null)
				{
					if(shaderImporter.userData.Contains("USER"))
					{
						Shader shader = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Shader)) as Shader;
						if(shader != null && !shaderList.Contains(shader))
							shaderList.Add(shader);
					}
				}
			}

			return shaderList.ToArray();
		}

		return null;
	}

	private string[] GetShaderLabels(Shader[] array, string firstOption = "New Shader")
	{
		if(array == null)
		{
			return new string[0];
		}

		List<string> labelsList = new List<string>();
		if(!string.IsNullOrEmpty(firstOption))
			labelsList.Add(firstOption);
		foreach(Shader shader in array)
		{
			labelsList.Add(shader.name);
		}
		return labelsList.ToArray();
	}

	private void AutoNames()
	{
		string rawName = mCurrentConfig.ShaderName.Replace("Toony Colors Pro 2/", "");
		mCurrentConfig.Filename = rawName;
	}

	static private void Space()
	{
		TCP2_GUI.GUILine(new Color(0.65f,0.65f,0.65f), 1);
		GUILayout.Space(1);
	}
}
