
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SimpleJSON;


/*
Uses reflection to serialise data of a generic class instance into JSON data. Once serialized it can be deserialised back into an instance of that class. 
It reads each property and recursively builds up JSON representations. Similarly for deserialization.
*NOTE* the SimpleJSON library is external and I did not write this.
*/
public static class Serializer
{
	
	public static string SerializeToCompressedJSON<T> (T toSerialize)
	{
		return SerializeObjectToJSON(toSerialize, typeof(T)).SaveToCompressedBase64();
	}

	public static string SerializeToRawJSON<T> (T toSerialize)
	{
		return SerializeObjectToJSON(toSerialize, typeof(T)).ToString();
	}
	
	public static JSONNode SerializeToJSON<T> (T toSerialize)
	{
		return SerializeObjectToJSON(toSerialize, typeof(T));
	}

	
	static JSONNode SerializeObjectToJSON (System.Object toSerialize, System.Type type)
	{
//		Debug.Log("Serialising "+toSerialize+" -> "+type.ToString());
		if (type.IsEnum)
		{
			return toSerialize.ToString();
		}
		else if (type.IsPrimitive)
		{
			if (type == typeof(int)   ||
				type == typeof(float) ||
				type == typeof(bool))
			{
				return toSerialize.ToString();
			}
			else
			{
				Debug.LogWarning("Could not parse "+toSerialize+" ("+type+")");
			}
		}
		else if (type.IsClass || type.IsValueType)
		{
			if (toSerialize == null)
			{
				return "null";
			}
			if (type == typeof(string))
			{
				return (string)toSerialize;
			}
			else // it's just a class of some sort
			{
				System.Type iListType = GetIListType(type);
				System.Type keyType;
				System.Type valueType;
				GetIDictionaryType(type, out keyType, out valueType);
				if(iListType != null)
				{
//					Debug.Log ("Found generic IList "+iListType);
					
					JSONArray jsonList = new JSONArray();
					
					IList list = (IList)toSerialize;
					foreach(System.Object subObj in list)
					{
						jsonList[-1] = SerializeObjectToJSON(subObj, iListType);
					}
					return jsonList;
				}
				if(keyType != null)
				{
//					Debug.Log ("Found generic IDictionary "+keyType);
					
					JSONArray jsonList = new JSONArray();
					
					IDictionary dictionary = (IDictionary)toSerialize;
					foreach(DictionaryEntry keyValue in dictionary)
					{
						System.Object keyObj = keyValue.Key;
						System.Object valueObj = keyValue.Value;
						JSONArray keyValNode = new JSONArray();
						keyValNode[0] = SerializeObjectToJSON(keyObj, keyType);
						keyValNode[1] = SerializeObjectToJSON(valueObj, valueType);
						jsonList[-1] = keyValNode;
					}
					return jsonList;
				}
				else if(type.IsArray)
				{
//					Debug.Log ("Found array");
					
					JSONArray jsonArray = new JSONArray();
					
					System.Type elementType = type.GetElementType();
					System.Array array = (System.Array)toSerialize;
					
					foreach(System.Object subObj in array)
					{
						jsonArray[-1] = SerializeObjectToJSON(subObj, elementType);
					}
					return jsonArray;
				}
				else
				{
					
					FieldInfo [] fieldInfo = toSerialize.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
					
					
					JSONClass json = new JSONClass();
					
					foreach(FieldInfo info in fieldInfo)
					{
						System.Object fieldVal = info.GetValue(toSerialize);
						System.Type fieldType = info.FieldType;
						string fieldName = info.Name;
						json[fieldName] = SerializeObjectToJSON(fieldVal, fieldType);
					}
					return json;
					
					
				}
			}
		}
		
		return null;
		
	}
	

	
	public static T DeserializeFromJSON <T>(string compressedOrRawJSON)
	{
		try
		{
//			Debug.Log("compressedOrRawJSON: "+compressedOrRawJSON);
			if (string.IsNullOrEmpty(compressedOrRawJSON))
			{
				Debug.LogError("Deserializing an empty or null string!");
			}
			try
			{
				return DeserializeFromCompressedJSON<T>(compressedOrRawJSON);
			}
			catch(System.FormatException)
			{
				return DeserializeFromRawJSON<T>(compressedOrRawJSON);
			}
		}
		catch(System.Exception e )
		{
			Debug.LogError("Exception Caught while deserialising: "+compressedOrRawJSON);
			Debug.LogError("Exception Message: "+e.Message);
			Debug.LogError("StackTrace\n"+e.StackTrace);
			return default(T);
		}
	}


	public static T DeserializeFromRawJSON <T>(string rawJSON)
	{
		return DeserializeFromJSONNode<T>(JSONNode.Parse(rawJSON));
	}


	public static T DeserializeFromCompressedJSON <T>(string compressedJSON)
	{
		return DeserializeFromJSONNode<T>(JSONLazyCreator.LoadFromCompressedBase64(compressedJSON));
	}

	
	static T DeserializeFromJSONNode <T>(JSONNode json)
	{
		System.Object deserialised = DeserializeObjectFromRawJSON(json, typeof(T));;
		
		if (deserialised == null)
		{
			Debug.LogWarning("Deserialised object of type "+typeof(T)+" is null");
		}
		return (T) deserialised;
	}
	
	static System.Object DeserializeObjectFromRawJSON (JSONNode node, System.Type type)
	{
		if (type.IsEnum)
		{
            System.Object obj = node.Value == "" ? System.Enum.GetValues(type).GetValue(0) : System.Enum.Parse(type, node.Value);
            return obj;
		}
		else if (type.IsPrimitive)
		{
			if (type == typeof(int))
			{
				return node.AsInt;
			}
			else if (type == typeof(float))
			{
				return node.AsFloat;
			}
			else if (type == typeof(bool))
			{
				return node.AsBool;
			}
		}
		else if (type.IsClass || type.IsValueType) // is class 
		{
			if (node.Value == "null")
			{
				return null;
			}
			if (type == typeof(string))
			{
				return node.Value;
			}
			else // it's just a thing
			{
				
				System.Type iListType = GetIListType(type);
				System.Type keyType;
				System.Type valueType;
				GetIDictionaryType(type, out keyType, out valueType);
//				Debug.Log ("genericTypes " + string.Join(", ", genericTypes.ConvertAll((input) => input.ToString()).ToArray()));
				if(iListType != null)
				{
//					Debug.Log ("Found generic IList");
				
					IList list = (IList)System.Activator.CreateInstance(type);
					foreach(JSONNode subNode in node.AsArray)
					{
						list.Add(DeserializeObjectFromRawJSON(subNode, iListType));
					}
					return list;
				}
				if(keyType != null)
				{
//					Debug.Log ("Found generic IDictionary");
				
					IDictionary dictionary = (IDictionary)System.Activator.CreateInstance(type);
					foreach(JSONNode subNode in node.AsArray)
					{
						dictionary.Add(
							DeserializeObjectFromRawJSON(subNode[0], keyType),
							DeserializeObjectFromRawJSON(subNode[1], valueType));
					}
					return dictionary;
				}
				else if(type.IsArray)
				{
//					Debug.Log ("Found Array");
					System.Type elementType = type.GetElementType();
					System.Array array = System.Array.CreateInstance(elementType, node.AsArray.Count);
					for(int i = 0 ; i < node.AsArray.Count ; i++)
					{
						array.SetValue(DeserializeObjectFromRawJSON(node.AsArray[i], elementType), i);
					}
					return array;
				}
				else
				{
					System.Object targetObject;
					
					if (type.IsClass)
					{
						targetObject = System.Activator.CreateInstance(type);
					}
					else if (type.IsValueType)
					{
//						Debug.Log(type);
						targetObject = System.Activator.CreateInstance(type);
//						targetObject = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
					}
					else
					{
						return null;
					}
					
					FieldInfo [] fieldInfo = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
					
					foreach(FieldInfo info in fieldInfo)
					{
						System.Type fieldType = info.FieldType;
						string fieldName = info.Name;
//						Debug.Log("Field "+fieldName);
						
						info.SetValue(targetObject, DeserializeObjectFromRawJSON(node.AsObject[fieldName], fieldType));
					}
					
					return targetObject;
				}
			}
		}
		
		return null;
		
	}
	
	static System.Type GetIListType(System.Type type)
	{
		List<System.Type> interfaces = new List<System.Type>(type.GetInterfaces());
//		Debug.Log ("interfaces " + string.Join("\n", interfaces.ConvertAll((input) => input.ToString()).ToArray()));
				
		if (interfaces.Contains(typeof(IList)))
		{
//			Debug.Log("IsGenericType "+type.IsGenericType);
//			Debug.Log("IsArray "+type.IsArray);
			
			if (type.IsGenericType)
			{
				List<System.Type> genericTypes = new List<System.Type>(type.GetGenericArguments());
//				Debug.Log ("genericTypes " + string.Join("\n", genericTypes.ConvertAll((input) => input.ToString()).ToArray()));
				
				if (interfaces.Contains(typeof(IList<>).MakeGenericType(genericTypes[0])))
				{
					return genericTypes[0];
				}
			}
			
		}
		return null;
	}
	
	static void GetIDictionaryType(System.Type type, out System.Type keyType, out System.Type valueType)
	{
		List<System.Type> interfaces = new List<System.Type>(type.GetInterfaces());
//		Debug.Log ("interfaces " + string.Join("\n", interfaces.ConvertAll((input) => input.ToString()).ToArray()));
		
		if (interfaces.Contains(typeof(IDictionary)))
		{
//			Debug.Log("IsGenericType "+type.IsGenericType);
//			Debug.Log("IsArray "+type.IsArray);
			
			if (type.IsGenericType)
			{
				List<System.Type> genericTypes = new List<System.Type>(type.GetGenericArguments());
//				Debug.Log ("genericTypes " + string.Join("\n", genericTypes.ConvertAll((input) => input.ToString()).ToArray()));
				
				if (interfaces.Contains(typeof(IDictionary<,>).MakeGenericType(genericTypes[0], genericTypes[1])))
				{
					keyType = genericTypes[0];
					valueType = genericTypes[1];
					return;
				}
			}
			
		}
		keyType = null;
		valueType = null;
		return;
	}
	
}
