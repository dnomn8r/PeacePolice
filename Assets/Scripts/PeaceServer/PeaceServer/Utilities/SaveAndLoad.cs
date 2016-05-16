using System.Collections;
using System.Text;
using System.IO;
using System;

public class SaveAndLoad{

	public static void Save(string directory, string filename, string data, bool compress = false){
	
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		string pathToFile = Application.persistentDataPath + "/" + directory + "/";
		
#else
		string pathToFile = directory + "/";
#endif
		
		if(!System.IO.Directory.Exists(pathToFile)){
			
			System.IO.Directory.CreateDirectory(pathToFile);	
		}
		
		byte[] bytesToSave = Encoding.UTF8.GetBytes(data);
		
		//Debug.Log("save uncompressed length: " + serializedBytes.Length);
		
		if(compress){
		
			bytesToSave = CLZF2.Compress(bytesToSave);
		}
		
		File.WriteAllBytes(pathToFile + filename, bytesToSave);	
		
	}
	
	public static string Load(string directory, string filename, bool decompress = false){
		
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		
		string pathToFile = Application.persistentDataPath + "/" + directory + "/";
#else
		string pathToFile = directory + "/";
#endif
	
		// if we don't have a file saved, try to load the default..
		if(File.Exists(pathToFile + filename)){
		
			byte[] readBytes = File.ReadAllBytes(pathToFile + filename);
			
			//Debug.Log("laoded compessed size: " + compressedBytes.Length);
			if(decompress){
			
				readBytes = CLZF2.Decompress(readBytes);
			}
			
			return Encoding.UTF8.GetString(readBytes);
			
		}else{
			
			Console.WriteLine("cannot load file: " + pathToFile + filename + " it does not exist");
			
			return "";
		}
		/*
		else{
		
			string newPath = Application.dataPath + "/Raw/Maps/" + filename;
			
			if(File.Exists(newPath)){
				
				byte[] compressedBytes = File.ReadAllBytes(newPath);
				
				
				byte[] decompressedBytes = CLZF2.Decompress(compressedBytes);
				
				string deserializedString = Encoding.UTF8.GetString(decompressedBytes);
				
				return deserializedString;
				
				
			}else{
			
				Debug.LogWarning("cannot load file: " + pathToFile + filename + " it does not exist");
			
				return "";
			}
		}
		*/
		
		
	}
	
	
	public static string CompressString(string data){
		
		byte[] serializedBytes = Encoding.UTF8.GetBytes(data);
		
		//Debug.Log("save uncompressed length: " + serializedBytes.Length);
		
		byte[] compressedBytes = CLZF2.Compress(serializedBytes);	
		
		return System.Convert.ToBase64String(compressedBytes);
	}
	
	public static string DecompressString(byte[] compressedBytes){
		
		byte[] decompressedBytes = CLZF2.Decompress(compressedBytes);
				
		return Encoding.UTF8.GetString(decompressedBytes);
	}

}

