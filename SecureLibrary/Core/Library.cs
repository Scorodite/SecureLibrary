using SecureLibrary.Core.Storage;
using SecureLibrary.Core.Storage.Methods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace SecureLibrary.Core
{
    public class Library : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private BinaryStorage? _Storage;
        private StorageMethod? _StorageMethod;

        public Library()
        {
            Root = new() { IsModified = false };
            Resources = new();
        }

        public Library(LibraryItem root, AttachedResourceCollection resources)
        {
            Root = root;
            Resources = resources;
            ResetModified();
        }

        /// <summary>
        /// Root library item
        /// </summary>
        public LibraryItem Root { get; }

        /// <summary>
        /// Dynamic resources that can be saved and loaded
        /// </summary>
        public AttachedResourceCollection Resources { get; }

        public BinaryStorage? Storage
        {
            get => _Storage;
            set
            {
                _Storage = value;
                PropertyChanged?.Invoke(this, new(nameof(Storage)));
            }
        }

        public StorageMethod? StorageMethod
        {
            get => _StorageMethod;
            set
            {
                _StorageMethod = value;
                PropertyChanged?.Invoke(this, new(nameof(StorageMethod)));
            }
        }

        public bool IsModified
        {
            get
            {
                bool mod = false;
                Root.InvokeForTree(i => mod |= i.IsModified);
                return mod;
            }
        }

        public void ResetModified()
        {
            Root.InvokeForTree(i => i.IsModified = false);
        }

        public void Save(ErrorAllower? errorAllower, out ErrorCollection errors)
        {
            if (Storage is null)
            {
                throw new NullReferenceException(nameof(Storage));
            }
            if (StorageMethod is null)
            {
                throw new NullReferenceException(nameof(StorageMethod));
            }
            using MemoryStream memory = new();
            using (BinaryWriter writer = new(memory, Encoding.UTF8, true))
            {
                Serialize(writer, out errors);
            }
            memory.Position = 0;
            if (errors.Count == 0 || errorAllower is not null && errorAllower(errors))
            {
                using Stream stream = StorageMethod.OpenWrite(Storage);
                memory.CopyTo(stream);
            }
            else
            {
                throw new IOException("Saving failed with errors");
            }
        }

        public static Library Load(BinaryStorage storage, StorageMethod method, out ErrorCollection errors)
        {
            using Stream stream = method.OpenRead(storage);
            using BinaryReader reader = new(stream, Encoding.UTF8);
            Library lib = Deserialize(reader, out errors);
            lib.Storage = storage;
            lib.StorageMethod = method;
            return lib;
        }

        public void Serialize(BinaryWriter writer, out ErrorCollection errors)
        {
            errors = new();

            writer.Write(Resources.Count);
            foreach ((string name, ISerializable resource) in Resources)
            {
                writer.Write(name);
                if (resource is FailedToLoadAttachedResource failed)
                {
                    resource.Serialize(writer, failed.Type, in errors);
                }
                else
                {
                    resource.Serialize(writer, in errors);
                }
            }

            SerializeLibraryItem(writer, Root, errors);
        }

        public static Library Deserialize(BinaryReader reader, out ErrorCollection errors)
        {
            errors = new();
            AttachedResourceCollection resources = new();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = reader.ReadString();
                AttachedResource resource = ISerializable.Deserialize<AttachedResource>(
                    reader, (n, s) => new FailedToLoadAttachedResource(n, s),
                    App.Current.Plugins.Assemblies, in errors
                );
                resources.Add(name, resource);
            }

            return new(DeserializeLibraryItem(reader, errors), resources);
        }

        private static void SerializeLibraryItem(BinaryWriter writer, LibraryItem item, ErrorCollection errors)
        {
            if (item is FailedToLoadLibraryItem failed)
            {
                (item as ISerializable).Serialize(writer, failed.Type, in errors);
            }
            else
            {
                (item as ISerializable).Serialize(writer, in errors);
            }
            writer.Write(item.Count);
            foreach (LibraryItem sub in item)
            {
                SerializeLibraryItem(writer, sub, errors);
            }
        }

        private static LibraryItem DeserializeLibraryItem(BinaryReader reader, ErrorCollection errors)
        {
            LibraryItem item = ISerializable.Deserialize<LibraryItem>(
                reader, (n, s) => new FailedToLoadLibraryItem(n, s), App.Current.Plugins.Assemblies, in errors
            );

            int subitemsCount = reader.ReadInt32();
            for (int i = 0; i < subitemsCount; i++)
            {
                item.Add(DeserializeLibraryItem(reader, errors));
            }
            return item;
        }

        /// <summary>
        /// Delegate that allows or disallows saving and loading library on errors
        /// </summary>
        public delegate bool ErrorAllower(ErrorCollection errors);
    }
}
