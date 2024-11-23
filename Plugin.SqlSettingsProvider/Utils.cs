using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace Plugin.SqlSettingsProvider
{
	/// <summary>Разные утилиты</summary>
	internal static class Utils
	{
		private sealed class UniversalDeserializationBinder : SerializationBinder
		{
			public override Type BindToType(String assemblyName, String typeName)
				=> Type.GetType(typeName);
		}

		private static BinaryFormatter BinSerializer => new BinaryFormatter() { AssemblyFormat = FormatterAssemblyStyle.Simple, TypeFormat = FormatterTypeStyle.TypesWhenNeeded, Binder = new UniversalDeserializationBinder(), };

		/// <summary>Сериализовать объект в строку</summary>
		/// <param name="obj">Объект для сериализации</param>
		/// <returns>Сериализованный объект представленный в Base64</returns>
		public static String SerializeToString(Object obj)
		{
			_ = obj ?? throw new ArgumentNullException(nameof(obj));

			Byte[] result = Utils.SerializeObject(obj);
			return Convert.ToBase64String(result);
		}

		/// <summary>Преобразовать объект Base64 строки в объект</summary>
		/// <param name="str">Строка в формате Base64 для преобразования</param>
		/// <returns>Десериализованный объект из Base64 строки</returns>
		public static Object DeserializeFromString(String str)
		{
			_ = str ?? throw new ArgumentNullException(nameof(str));

			Byte[] bytes = Convert.FromBase64String(str);
			return Utils.DeserializeObject(bytes);
		}

		/// <summary>Сериализовать объект в массив байт</summary>
		/// <remarks>Копипаст в <c>AlphaOmega.Windows.Forms.Flatbed.Utils</c></remarks>
		/// <param name="obj">Объект для сериализации</param>
		/// <returns>Сериализованный объект представленный в массиве байт</returns>
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

		/// <summary>Преобразовать объект из массива байт в строку</summary>
		/// <remarks>Копипаст в <c>AlphaOmega.Windows.Forms.Flatbed.Utils</c></remarks>
		/// <param name="bytes">Массив байт для преобразования</param>
		/// <returns>Десериализованный объект из массива байт</returns>
		public static Object DeserializeObject(Byte[] bytes)
		{
			_ = bytes ?? throw new ArgumentNullException(nameof(bytes));

			BinaryFormatter formatter = Utils.BinSerializer;
			using(MemoryStream stream = new MemoryStream(bytes))
				return formatter.Deserialize(stream);
		}
	}
}