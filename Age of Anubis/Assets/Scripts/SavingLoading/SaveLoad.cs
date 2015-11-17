using UnityEngine;    // For Debug.Log, etc.

using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

// === This is the info container class ===
[Serializable()]
public class SaveData : ISerializable
{

	// === Values ===
	// Edit these during gameplay
	public int currentLevel = 1;
	public int exp = 0;
	public int gold = 0;
	//public string weapon1 = "";
	//public string weapon2 = "";
	// === /Values ===

	// The default constructor. Included for when we call it during Save() and Load()
	public SaveData() { }

	// This constructor is called automatically by the parent class, ISerializable
	// We get to custom-implement the serialization process here
	public SaveData(SerializationInfo info, StreamingContext ctxt)
	{
		// Get the values from info and assign them to the appropriate properties. Make sure to cast each variable.
		// Do this for each var defined in the Values section above
		currentLevel = (int)info.GetValue("currentLevel", typeof(int));
		exp = (int)info.GetValue("exp", typeof(int));
		gold = (int)info.GetValue("gold", typeof(int));
		//weapon1 = (string)info.GetValue("weapon1", typeof(string));
		//weapon2 = (string)info.GetValue("weapon2", typeof(string));

		//levelReached = (int)info.GetValue("levelReached", typeof(int));
	}

	// Required by the ISerializable class to be properly serialized. This is called automatically
	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		// Repeat this for each var defined in the Values section
		info.AddValue("currentLevel", (currentLevel));
		info.AddValue("exp", exp);
		info.AddValue("gold", gold);
		//info.AddValue("weapon1", weapon1);
		//info.AddValue("weapon2", weapon2);
	}
}

// === This is the class that will be accessed from scripts ===
public class SaveLoad
{

	public static string currentFilePath = Application.persistentDataPath + "Savegame.fiin";    // Edit this for different save files

	// Call this to write data
	public static void Save()  // Overloaded
	{
		Save(currentFilePath);
	}

	public static void Save(string filePath)
	{
		Save(filePath, GameManager.inst.m_saveManager);
	}

	public static void Save(string filePath, SaveManager sm)
	{
		SaveData data = new SaveData();

		data.currentLevel = sm.m_currentLevel;
		data.exp = sm.m_exp;
		data.gold = sm.m_gold;
		//data.weapon1 = sm.m_weapon1;
		//data.weapon2 = sm.m_weapon2;

		Stream stream = File.Open(filePath, FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder();
		bformatter.Serialize(stream, data);
		stream.Close();

		Debug.Log("Saving");
	}

	// Call this to load from a file into "data"
	public static void Load() { Load(currentFilePath); }   // Overloaded
	public static void Load(string filePath)
	{
		SaveData data = new SaveData();
		Stream stream = File.Open(filePath, FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder();
		data = (SaveData)bformatter.Deserialize(stream);
		stream.Close();

		GameManager.inst.m_saveManager.m_currentLevel = data.currentLevel;
		GameManager.inst.m_saveManager.m_exp = data.exp;
		GameManager.inst.m_saveManager.m_gold = data.gold;
		//GameManager.inst.m_saveManager.m_weapon1 = data.weapon1;
		//GameManager.inst.m_saveManager.m_weapon2 = data.weapon2;
	}

}

// === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
// Do not change this
public sealed class VersionDeserializationBinder : SerializationBinder
{
	public override Type BindToType(string assemblyName, string typeName)
	{
		if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
		{
			Type typeToDeserialize = null;

			assemblyName = Assembly.GetExecutingAssembly().FullName;

			// The following line of code returns the type. 
			typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

			return typeToDeserialize;
		}

		return null;
	}
}