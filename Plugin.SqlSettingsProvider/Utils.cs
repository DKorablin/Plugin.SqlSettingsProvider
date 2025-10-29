using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace Plugin.SqlSettingsProvider
{
	/// <summary>Various utilities</summary>
	internal static class Utils
	{
		private sealed class UniversalDeserializationBinder : SerializationBinder
		{
			public override Type BindToType(String assemblyName, String typeName)
				=> Type.GetType(typeName);
		}

		private static BinaryFormatter BinSerializer => new BinaryFormatter() { AssemblyFormat = FormatterAssemblyStyle.Simple, TypeFormat = FormatterTypeStyle.TypesWhenNeeded, Binder = new UniversalDeserializationBinder(), };

		/// <summary>Serialize an object to a string</summary>
		/// <param name="obj">Object to serialize</param>
		/// <returns>Serialized object represented in Base64</returns>
		public static String SerializeToString(Object obj)
		{
			_ = obj ?? throw new ArgumentNullException(nameof(obj));

			Byte[] result = Utils.SerializeObject(obj);
			return Convert.ToBase64String(result);
		}

		/// <summary>Convert a Base64 string to an object</summary>
		/// <param name="str">The Base64 string to convert</param>
		/// <returns>The deserialized object from the Base64 string</returns>
		public static Object DeserializeFromString(String str)
		{
			_ = str ?? throw new ArgumentNullException(nameof(str));

			Byte[] bytes = Convert.FromBase64String(str);
			return Utils.DeserializeObject(bytes);
		}

		/// <summary>Serialize an object into a byte array</summary>
		/// <remarks>Copy-pasted from <c>AlphaOmega.Windows.Forms.Flatbed.Utils</c></remarks>
		/// <param name="obj">Object to serialize</param>
		/// <returns>The serialized object represented in a byte array</returns>
		public static Byte[] SerializeObject(Object obj)
		{
			_ = obj ?? throw new ArgumentNullException(nameof(obj));

			BinaryFormatter formatter = Utils.BinSerializer;
			using(MemoryStream stream = new MemoryStream())
			{
				formatter.Serialize(stream, obj);
				return stream.ToArray();
			}
		}

		/// <summary>Convert an object from a byte array to a string</summary>
		/// <remarks>Copy-pasted from <c>AlphaOmega.Windows.Forms.Flatbed.Utils</c></remarks>
		/// <param name="bytes">Byte array to convert</param>
		/// <returns>Deserialized object from a byte array</returns>
		public static Object DeserializeObject(Byte[] bytes)
		{
			_ = bytes ?? throw new ArgumentNullException(nameof(bytes));

			BinaryFormatter formatter = Utils.BinSerializer;
			using(MemoryStream stream = new MemoryStream(bytes))
				return formatter.Deserialize(stream);
		}
	}
}