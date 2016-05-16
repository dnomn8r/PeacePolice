using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LumenWorks.Framework.IO.Csv;

public class TranslationRow
{
	public string key;
	public string english;
	public string french;
	public string german;
	public string italian;
	public string spanish;
	
	public string GetText(string lang)
	{
		if(lang == "Spanish")
		{
			return(spanish);
		}
		else if(lang == "Italian")
		{
			return(italian);
		}
		else if(lang == "German")
		{
			return(german);
		}
		else if(lang == "French")
		{
			return(french);
		}
		else
		{
			return(english);
		}
	}
}

public enum LanguageCode
{
	en,
	fr,
	de,
	it,
	es
}

public enum Language
{
	English,
	French,
	German,
	Italian,
	Spanish,
}

public class LanguageManager : MonoBehaviour 
{
	// Localization file used before definitions have loaded.
	private static readonly string LOCALIZATION_FILE = "DTQALocalization";
	private const Language DEFAULT_LANGUAGE = Language.English;
	
	private static LanguageManager instance;
	
	public Language editorLanguage = Language.English;

    // TODO: Use enums everywhere to avoid creating strings
	private Dictionary<string, string> languageCodeMap;
	
	private bool setupComplete = false;
	
	private Dictionary<string, Dictionary<string, string>> localizationDictionaries;
	
	public static LanguageManager Instance
	{
        get 
		{ 
			if(instance != null && !instance.setupComplete)
			{
				instance.Setup();
			}
			
			return instance; 
		}
    }
	
	private string myLanguageCode = "";
	public string MyLanguageCode
	{
		get
		{
			if(myLanguageCode == "")
			{

#if UNITY_IPHONE && !UNITY_EDITOR

				myLanguageCode = EtceteraBinding.getCurrentLanguage();

#elif UNITY_ANDROID && !UNITY_EDITOR

				myLanguageCode = Application.systemLanguage.ToString();
#else
				myLanguageCode = languageCodeMap.First (pair => pair.Value == EditorLanguage.ToString ()).Key;
#endif

			}
			
			return(myLanguageCode);
		}
	}
	
	public Language EditorLanguage
	{
		get{ return editorLanguage; }
		set{ editorLanguage = value; }
	}
	
	void Awake()
	{
		instance = this;
	}
	
	public void Setup()
	{
		if(!setupComplete)
		{	
			this.languageCodeMap = new Dictionary<string, string>
			{
				{ LanguageCode.en.ToString(), Language.English.ToString() },
				{ LanguageCode.fr.ToString(), Language.French.ToString() },
				{ LanguageCode.de.ToString(), Language.German.ToString() },
				{ LanguageCode.it.ToString(), Language.Italian.ToString() },
				{ LanguageCode.es.ToString(), Language.Spanish.ToString() },
			};

			object file = Resources.Load(LOCALIZATION_FILE);
			string[][] localizationTable = null;
			if(file != null)
			{
				localizationTable = ParseCSVData( new StringReader(file.ToString()) );
			}
			
			if( localizationTable != null )
			{
				localizationDictionaries = GenLocalizationDictionary( localizationTable );
			}
			
			setupComplete = true;
		}
	}

	public void SetTranslations(List<TranslationRow> rows)
	{
		rows = rows.FindAll(r => r.key != null);
		
		var dict = new Dictionary<string, Dictionary<string, string>>();
		
		string curLang = GetCurrentLanguage();
		
		if(!System.Enum.IsDefined(typeof(Language), curLang))
		{
			curLang = DEFAULT_LANGUAGE.ToString();
		}
		
		dict.Add(curLang, new Dictionary<string,string>(rows.Count));

		foreach (TranslationRow r in rows)
		{
			dict[curLang].Add(r.key, r.GetText(curLang));
		}
		
		this.localizationDictionaries = dict;
	}
	
	public string GetCurrentLanguageCode()
	{
		return(MyLanguageCode);
	}
	
	// TODO: Cache lookup
	public string GetCurrentLanguage()
	{

		string lang = GetCurrentLanguageCode();
	
		//Debug.Log("Current Lang: " + lang);

		if( lang == "en" )
			return "English";
		else if( lang == "fr" )
			return "French";
		else if( lang == "de" )
			return "German";
		else if( lang == "it" )
			return "Italian";
		else if( lang == "es" )
			return "Spanish";
		else
			return lang;
	}
	
	public string GetText( string id )
	{
		return GetText( id, GetCurrentLanguage() );
	}

	public string GetLocale(){

#if UNITY_IPHONE && !UNITY_EDITOR

		return EtceteraBinding.getLocale();

#elif UNITY_ANDROID && !UNITY_EDITOR

		using( var pluginClass = new AndroidJavaClass("com.complexgames.ducktales.UnityPlayerCustomActivity")){

			string localeString = pluginClass.CallStatic<string>("_getLocale");

			return localeString;

		}
#endif

		return "CA"; // canada on the editor... cause our office is in canada :P

	}
	
	public bool ShouldPlayVoiceOvers()
	{
		if(GetCurrentLanguage() == Language.English.ToString()) return(true);
		
		if(GetCurrentLanguage() == Language.French.ToString() || GetCurrentLanguage() == Language.German.ToString() || 
			GetCurrentLanguage() == Language.Italian.ToString() || GetCurrentLanguage() == Language.Spanish.ToString())
		{
			return(false);
		}
		
		return(true);
	}
	
	public string GetText( string id, string language )
	{
		string localizedText = id;
		if( localizationDictionaries != null)
		{
			//if we can't find the language then revert to english
			if(!localizationDictionaries.ContainsKey(language))
			{
				language = "English";
			}
			
			if(localizationDictionaries.ContainsKey( language ) && localizationDictionaries[language].ContainsKey( id ) )
			{
				localizedText = localizationDictionaries[language][id];
			}
		}
		
		if( string.IsNullOrEmpty( localizedText ) )
		{
			localizedText = id;
		}
		
		return localizedText;
	}
	
	private Dictionary<string, Dictionary<string, string>> GenLocalizationDictionary( string[][] table )
	{
		Dictionary<string, Dictionary<string, string>> localizationText = new Dictionary<string, Dictionary<string, string>>();

		int defaultLangColPos = 0;
		string[] headers = table[0];
		string[] elements;
		
		//strip colons from header fields and find default language column (most likely remove this later)
		for( int i = 0; i < headers.Length; i++ )
		{
			if(headers[i] == DEFAULT_LANGUAGE.ToString() )
			{
				defaultLangColPos = i;
				break;
			}
		}
		
		int columnPosition = defaultLangColPos;
		string currLanguage = GetCurrentLanguage();
		
		for( int i = defaultLangColPos; i < headers.Length; i++ )
		{
			if(headers[i] == currLanguage)
			{
				columnPosition = i;
				break;
			}
		}
		
		localizationText[headers[columnPosition]] = new Dictionary<string, string>();

		for( int i = 1; i < table.Length; i++ )
		{
			elements = table[i];
			
			localizationText[headers[columnPosition]][elements[0]] = elements[columnPosition];
		}

		return localizationText;
	}
	
	private string[][] ParseCSVFile( string fileName )
	{
		string[][] table = null;
		//string data = null;
		
		if( !File.Exists( fileName ) )
		{
			return table;
		}
		
		using( StreamReader reader = File.OpenText( fileName ) )
		{
			table = ParseCSVData(reader);
		}
		
		return table;
	}
	
	private string[][] ParseCSVData( TextReader reader )
	{
		CsvReader csv = new CsvReader(reader, true, ',', '"','"', '#', ValueTrimmingOptions.All);
		csv.SupportsMultiline = true;
		
		List<string[]> table = new List<string[]>();
		int fieldCount = csv.FieldCount;
		
		table.Add(csv.GetFieldHeaders());
		
		while(csv.ReadNextRecord())
		{
			string[] field = new string[fieldCount];
			
			csv.CopyCurrentRecordTo(field);
			table.Add(field);
		}
		
		return table.ToArray();
	}
	
	void OnDestroy(){
		
		instance = null;	
	}
}
