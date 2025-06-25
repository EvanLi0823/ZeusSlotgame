using System.Collections.Generic;

namespace Utils
{
	public class JSONUtil
	{

		public static Dictionary<string,object> DictionaryValue (Dictionary<string,object> dictValues, string key, Dictionary<string,object> defaultValue = null)
		{
			return  PrimitiveValue<Dictionary<string,object>> (dictValues, key, defaultValue);
		}
	
		public static string StringValue (Dictionary<string,object> dictValues, string key, string defaultValue = null)
		{
			return PrimitiveValue<string> (dictValues, key, defaultValue);
		}
	
		public static int IntValue (Dictionary<string,object>dictValues, string key, int defaultValue = 0)
		{
			return PrimitiveValue<int> (dictValues, key, defaultValue);
		}

		public static System.Int64 Int64Value(Dictionary<string,object>dictValues, string key, System.Int64 defaultValue = 0)
		{
			return PrimitiveValue<System.Int64> (dictValues, key, defaultValue);
		}

	
		public static long LongValue (Dictionary<string,object>dictValues, string key, long defaultValue = 0)
		{
			return PrimitiveValue<long> (dictValues, key, defaultValue);
		}
	
		public static bool BoolValue (Dictionary<string, object> dictValues, string key, bool defaultValue)
		{
			return PrimitiveValue<bool> (dictValues, key, defaultValue);
		}

		public static Dictionary<string,string> DictionaryWithStringKeyAndValues(Dictionary<string,object> dictValues, Dictionary<string,string> defaultDict = null)
		{
			Dictionary<string,string> result = new Dictionary<string, string> ();
			if (dictValues != null) {
				foreach (string key in dictValues.Keys) {
					result.Add (key, (string)dictValues [key]);
				}
			}
			return result;
		}
	
		public static List<T> ListValue <T>(Dictionary<string,object> dictValues, string key, List<T> defaultValue = null)
		{
			List<T> result = new List<T>();
			List<object> values =(List<object>) dictValues[key];
			if (values != null) {
				foreach (object v in values) {
					result.Add ((T)v);
				}
			}
			return result;
		}

		private static T PrimitiveValue<T> (Dictionary<string,object>dictValues, string key, T defaultValue)
		{
			if (dictValues != null && dictValues.ContainsKey (key)) {
				return (T)dictValues [key];
			} else {
				return defaultValue;
			}
		}
	
	}
}
