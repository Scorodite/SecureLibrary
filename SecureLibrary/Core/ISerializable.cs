using SecureLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SecureLibrary.Core
{
    /// <summary>
    /// Provides serialization methods
    /// </summary>
    public interface ISerializable
    {
        public void ReadData(BinaryReader reader);
        public void WriteData(BinaryWriter writer);

        public static T Deserialize<T>(BinaryReader reader, in ErrorCollection errors)
                                       where T : ISerializable, new()
        {
            return Deserialize(
                reader,
                (n, s) =>
                {
                    s.Dispose();
                    return new T();
                },
                null,
                in errors
            );
        }

        public static T Deserialize<T>(BinaryReader reader, FallbackValueProvider<T> fallback,
                                       IEnumerable<Assembly>? assemblies,
                                       in ErrorCollection errors) where T : ISerializable
        {
            string name = reader.ReadString();
            T? value;
            bool failed = false;
            Type? foundType = Type.GetType(name) ??
                              assemblies?.Select(a => a.GetType(name))
                                         .Where(t => t is not null)
                                         .FirstOrDefault();

#if DEBUG
            value = (T)Activator.CreateInstance(foundType!)!;
#else
            if (foundType is Type type)
            {
                try
                {
                    if (Activator.CreateInstance(type) is T activated)
                    {
                        value = activated;
                    }
                    else
                    {
                        value = default;
                        failed = true;
                        errors.Add($"Type '{name}' is not subclass of '{typeof(T)}'");
                    }
                }
                catch (Exception ex)
                {
                    value = default;
                    failed = true;
                    errors.Add($"Failed to create '{name}' instance: {ex.Message}");
                }
            }
            else
            {
                value = default;
                failed = true;
                errors.Add($"Type '{name}' not found");
            }
#endif
            int dataLength = reader.ReadInt32();
            MemoryStream dataStream = new(dataLength);
            BinaryReader dataReader = new(dataStream);
            reader.BaseStream.CopyToLimited(dataStream, dataLength);
            dataStream.Position = 0;

#if DEBUG

            value.ReadData(dataReader);
#else
            if (value is null || failed)
            {
                failed = true;
                value = fallback(name, dataStream);
            }
            else
            {
                try
                {
                    value.ReadData(dataReader);
                }
                catch (Exception ex)
                {
                    dataStream.Position = 0;
                    value = fallback(name, dataStream);
                    failed = true;
                    errors.Add($"Object of type '{name}' failed to deserialize: {ex.Message}");
                }
            }
#endif
            if (!failed)
            {
                dataStream.Dispose();
            }

            return value;
        }

        public void Serialize(BinaryWriter writer, in ErrorCollection errors)
        {
            Serialize(writer, GetType().FullName ?? GetType().Name, errors);
        }

        public void Serialize(BinaryWriter writer, string typeName, in ErrorCollection errors)
        {
            writer.Write(typeName);

            using MemoryStream dataStream = new();
            using BinaryWriter dataWriter = new(dataStream);
#if DEBUG
            WriteData(dataWriter);
#else
            try
            {
                WriteData(dataWriter);
            }
            catch (Exception ex)
            {
                errors.Add($"Object of type '{typeName}' failed to serialize: {ex.Message}");
            }
#endif
            writer.Write((int)dataStream.Length);
            dataStream.Position = 0;
            dataStream.CopyTo(writer.BaseStream);
        }

        public delegate T FallbackValueProvider<T>(string typeName, Stream dataStream);
    }
}
