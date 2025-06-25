using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace Libs
{
	public class Data
	{
		public Data root;

		public Data ()
		{
		}

		protected virtual void InitData ()
		{

		}

		public void SetData (Object data)
		{
			Data.CopyValue (data, this);
		}

		public override string ToString ()
		{
			return Data.ToString (this);
		}

		public static string ToString (object data)
		{
			string result = "";
			if (data is IList || data is Array) {
				result += "[";
				foreach (object v in data as IList) {
					result += Data.ToString (v) + "\n";
				}
				result += "]";
			} else if (data is Data) {
				Type type = data.GetType ();
				FieldInfo[] fields = type.GetFields ();

				foreach (FieldInfo field in fields) {
					object v = field.GetValue (data);
					result += field.Name + ":" + Data.ToString (v) + "\n";
				}
			} else {
				result = data != null ? data.ToString () : null;
			}
			return result;
		}

		public static void CopyValue (object source, object target)
		{
			foreach (FieldInfo fi in target.GetType().GetFields()) {
				fi.SetValue (target, fi.GetValue (source));
			}

			if (target is Data)
				(target as Data).InitData ();
		}

		public static T CreateObject<T> (object source)
		{
			T t = Activator.CreateInstance<T> ();
			CopyValue (source, t);
			return t;
		}

		public static List<T> CreateList<T> (IEnumerable sourceList)
		{
			List<T> result = new List<T> ();

			Type type = null;

			foreach (object node in sourceList) {
				if (type == null) {
					type = node.GetType ();
				}
				result.Add (CreateObject<T> (node));
			}
			return result;
		}

		public static Dictionary<object, T> ListToDic<T> (IEnumerable sourceList, string fieldId = "id")
		{
			Dictionary<object, T> result = new Dictionary<object, T> ();

			Type type = null;
			FieldInfo idField = null;
			foreach (object node in sourceList) {
				if (type == null) {
					type = node.GetType ();
					idField = type.GetField (fieldId);
				}
				T t = CreateObject<T> (node);
				object id = idField.GetValue (node);
				result.Add (id, t);
			}
			return result;
		}
		/// <summary>
		/// dic to dicList
		/// </summary>
		/// <returns>The to dic.</returns>
		/// <param name="sourceDictionary">Source dictionary.</param>
		/// <param name="fieldId">Field identifier.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static Dictionary<object, T> DictionaryToDic<T> (IEnumerable sourceDictionaryList, string keyId = "id")
		{
			Dictionary<object, T> result = new Dictionary<object, T> ();
			
			Type type = null;

			foreach (Dictionary<string,object> node in sourceDictionaryList) {
				T t = Activator.CreateInstance<T> ();
				Type fieldsType = typeof(T);
				foreach (KeyValuePair<string, object> item in node)
				{
					string s= item.Key;
					FieldInfo field = fieldsType.GetField (item.Key);
					field.SetValue (t,Convert.ChangeType( item.Value,field.FieldType));
				}
			
				object id = node [keyId];
				result.Add (id, t);
			}
			return result;
		}

		/// <summary>
		/// Jsons to object.
		/// 参照http://www.cnblogs.com/chusiping/p/3492552.html
		/// </summary>
		/// <returns>The to object.</returns>
		/// <param name="jsonstr">Jsonstr.</param>
		/// <param name="objectType">Object type.</param>
		public static object jsonToObject(string jsonstr, Type objectType)//传递两个参数，一个是json字符串，一个是要创建的对象的类型
		{
			string[] jsons = jsonstr.Split(new char[] { ',' });//将json字符串分解成 “属性：值”数组
			for (int i = 0; i < jsons.Length; i++)
			{
				jsons[i] = jsons[i].Replace("\"", "");
			}//去掉json字符串的双引号
			object obj = System.Activator.CreateInstance(objectType); //使用反射动态创建对象
			PropertyInfo[] pis = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);//获得对象的所有public属性

			if (pis != null)//如果获得了属性
				foreach (PropertyInfo pi in pis)//针对每一个属性进行循环
				{
					for (int i = 0; i < jsons.Length; i++)//检查json字符串中的所有“属性：值”类表
					{
						if (jsons[i].Split(new char[] { ':' })[0] == pi.Name)//如果对象的属性名称恰好和json中的属性名相同
						{
							Type proertyType = pi.PropertyType; //获得对象属性的类型
							pi.SetValue(obj, Convert.ChangeType(jsons[i].Split(new char[] { ':' })[1], proertyType), null);
							//将json字符串中的字符串类型的“值”转换为对象属性的类型，并赋值给对象属性
						}
					}
				}
			return obj;
		}

		public static List<T> ListDicToListObj<T>(List<Dictionary<string,object>> sources)
		{
			List<T> result = new List<T> ();
			foreach(Dictionary<string,object> dic in sources)
			{
				result.Add (DictionaryToObject<T>(dic));
			}
			return result;
		}

		public static T DictionaryToObject<T>(Dictionary<string,object> sourceDictionary)//传递两个参数，一个是json字符串，一个是要创建的对象的类型
		{			
			T obj = System.Activator.CreateInstance<T> ();; //使用反射动态创建对象
			PropertyInfo[] pis = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);//获得对象的所有public属性

			if (pis != null)//如果获得了属性
				foreach (PropertyInfo pi in pis)//针对每一个属性进行循环
				{
					foreach (KeyValuePair<string, object>entry in sourceDictionary) {
						if (pi.Name == entry.Key) {
							Type propertyType = pi.PropertyType; //获得对象属性的类型
							try{
								pi.SetValue(obj, Convert.ChangeType(entry.Value,propertyType),null);
							}catch(Exception e){

							}
						}
					}
				}
			return obj;
		}
	}
}
