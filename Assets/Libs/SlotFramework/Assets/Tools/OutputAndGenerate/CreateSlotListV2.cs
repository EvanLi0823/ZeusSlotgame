using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CreateSlotListV2 : MonoBehaviour {

	public TextAsset inputFile;

	private const string INDENT_2 = "\t\t";
	private const string INDENT_3 = "\t\t\t";

	void Start()
	{
		string[] inputLines = inputFile.text.Split('\n');
		string[] propertyNames = inputLines[0].Split('\t');

		FileStream outputStream = new FileStream(Application.dataPath + "/CreateReelStrips/SlotListXml_V2.plist", FileMode.Create);
		StreamWriter writer = new StreamWriter(outputStream);

		writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF - 8\"?>");
		writer.WriteLine("<!DOCTYPE plist PUBLIC \" -//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
		writer.WriteLine("<plist version=\"1.0\">");
		writer.WriteLine("<dict>");
		writer.WriteLine("\t<key>SlotList</key>");
		writer.WriteLine("\t<array>");

		for (int i = 1; i < inputLines.Length; i++) {
			writer.WriteLine(INDENT_2 + "<dict>");

			string[] currentLine = inputLines[i].Split('\t');
			for (int j = 0; j < currentLine.Length; j++) {
				if (currentLine[j] != string.Empty) {
					writer.WriteLine(INDENT_3 + "<key>" + propertyNames[j] + "</key>");

					PropertyType propertyType;
					object result = ParseProperty(currentLine[j], out propertyType);
					switch (propertyType) {
						case PropertyType.Bool:
							if ((bool)result) {
								writer.WriteLine(INDENT_3 + "<true/>");
							} else {
								writer.WriteLine(INDENT_3 + "<false/>");
							}
							break;

						case PropertyType.Integer:
							writer.WriteLine(INDENT_3 + "<integer>" + currentLine[j] + "</integer>");
							break;

						case PropertyType.Real:
							writer.WriteLine(INDENT_3 + "<real>" + currentLine[j] + "</real>");
							break;

						case PropertyType.String:
							writer.WriteLine(INDENT_3 + "<string>" + currentLine[j] + "</string>");
							break;
					}
				}
			}

			writer.WriteLine(INDENT_2 + "</dict>");
		}

		writer.WriteLine("\t</array>");
		writer.WriteLine("</dict>");
		writer.WriteLine("</plist>");

		writer.Close();
		outputStream.Close();

		Debug.Log("Parse Complete");

		System.Diagnostics.Process.Start(Application.dataPath + "/CreateReelStrips", null);
	}

	private enum PropertyType {
		String = 0,
		Integer = 1,
		Real = 2,
		Bool = 3
	}

	private object ParseProperty(string input, out PropertyType propertyType) {
		int intResult;
		if (int.TryParse(input, out intResult)) {
			propertyType = PropertyType.Integer;
			return intResult;
		}

		float floatResult;
		if (float.TryParse(input, out floatResult)) {
			propertyType = PropertyType.Real;
			return floatResult;
		}

		if (input == "YES" || input == "NO") {
			propertyType = PropertyType.Bool;
			return (input == "YES");
		}

		propertyType = PropertyType.String;
		return input;
	}

}
