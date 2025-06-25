using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace UnityEditor.XCodeEditor
{
	public class XPlist  : System.IDisposable {
	
		private FileInfo projectFileInfo;
		private string filePath;
		List<string> contents = new List<string>();
		public XPlist()
		{
			
		}
		public XPlist( string fPath ) : this()
		{
			filePath = fPath;
			if( !System.IO.Directory.Exists( filePath ) ) {
				Debug.LogWarning( "Path does not exists." );
				return;
			}
			
			projectFileInfo = new FileInfo( Path.Combine( filePath, "info.plist" ) );
			StreamReader sr = projectFileInfo.OpenText();
			while (sr.Peek() >= 0) 
			{
				contents.Add(sr.ReadLine());
			}
			
		//	foreach(string line in contents)
//				Debug.Log(line);
			//while(
			//string contents = projectFileInfo.OpenText().ReadToEnd();
			sr.Close();
			
		}
		public void AddKey(string key)
		{
			if(contents.Count < 2)
				return;
			contents.Insert(contents.Count - 2,key);
			
		}
		
		public void ReplaceKey(string key,string replace){
			for(int i = 0;i < contents.Count;i++){
				if(contents[i].IndexOf(key) != -1){
					contents[i] = contents[i].Replace(key,replace);
				}
			}
		}
		
		public void Save()
		{
			StreamWriter saveFile = File.CreateText( System.IO.Path.Combine( this.filePath, "info.plist" ) );
			foreach(string line in contents)
				saveFile.WriteLine(line);

			saveFile.Close();
		}
		
		public void Dispose()
   		{
   		
	   	}
	}
}
