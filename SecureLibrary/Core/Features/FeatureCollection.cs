using SecureLibrary.Core.Features.LibraryItems;
using SecureLibrary.Core.Features.Storage;
using SecureLibrary.Core.Features.Storage.Methods;
using SecureLibrary.Core.Storage;
using SecureLibrary.Core.Storage.Methods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace SecureLibrary.Core.Features
{
    public class FeatureCollection : ICollection<Feature>, INotifyCollectionChanged
    {
        public IEnumerable<LibraryItemFactory> ItemFactories { get; }
        public IEnumerable<BinaryStorageFactory> StorageFactories { get; }
        public IEnumerable<StorageMethodFactory> StorageMethodFactories { get; }

        public FeatureCollection()
        {
            ItemFactories = new NotifyEnumerable<LibraryItemFactory>(this.OfType<LibraryItemFactory>(), this);
            StorageFactories = new NotifyEnumerable<BinaryStorageFactory>(this.OfType<BinaryStorageFactory>(), this);
            StorageMethodFactories = new NotifyEnumerable<StorageMethodFactory>(this.OfType<StorageMethodFactory>(), this);
        }

        public Feature[] AddRegistered(Assembly assembly)
        {
            Feature[] features = (from t in assembly.DefinedTypes
                                  where t.IsSubclassOf(typeof(Feature))
                                     && t.GetCustomAttribute<RegisterFeatureAttribute>() is not null
                                  let f = Activator.CreateInstance(t)
                                  where f is Feature
                                  select (Feature)f).ToArray();
            foreach (Feature feature in features)
            {
                Add(feature);
            }
            return features;
        }

        public void InitAll(App app)
        {
            foreach (Feature item in this.ToArray())
            {
                item.Init(app);
            }
            Sort();
        }

        public BinaryStorage? CreateStorageFromArgs(string[] args)
        {
            return StorageFactories.Select(f => f.CreateFromArgs(args))
                   .Where(i => i is not null)
                   .FirstOrDefault();
        }

        public StorageMethod? CreateMethodFromArgs(string[] args)
        {
            return StorageMethodFactories
                   .Select(f => f.CreateFromArgs(args))
                   .Where(i => i is not null)
                   .FirstOrDefault();
        }

        public void Sort()
        {
            Items.Sort((a, b) => a.Priority == b.Priority ?
                                     a.Name.CompareTo(b.Name) :
                                     -a.Priority.CompareTo(b.Priority));
        }


        #region ICollection<LibraryItem> implementation

        private readonly List<Feature> Items = new();

        public int Count => Items.Count;
        public Feature this[int index] => Items[index];
        bool ICollection<Feature>.IsReadOnly => false;

        public void Add(Feature item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Items.Add(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, Items.Count - 1);
        }

        public void Clear()
        {
            Items.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool Contains(Feature item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(Feature[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Feature> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(Feature item)
        {
            int index = Items.IndexOf(item);
            if (index > -1)
            {
                Items.Remove(item);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
                return true;
            }
            return false;
        }

        #endregion

        #region INotifyCollectionChanged implementation

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            CollectionChanged?.Invoke(this, new(action));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object? item, int index)
        {
            CollectionChanged?.Invoke(this, new(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object? item, int index,
                                         int oldIndex)
        {
            CollectionChanged?.Invoke(this, new(action, item, index, oldIndex));
        }

        #endregion

        private class NotifyEnumerable<T> : IEnumerable<T>, INotifyCollectionChanged
        {
            private readonly IEnumerable<T> Enumerable;
            private readonly INotifyCollectionChanged NotifySource;
            private readonly object ThreadLocker;

            private event NotifyCollectionChangedEventHandler? Handlers;

            public NotifyEnumerable(IEnumerable<T> enumerable, INotifyCollectionChanged notifySource)
            {
                Enumerable = enumerable;
                NotifySource = notifySource;
                ThreadLocker = new();
            }

            public event NotifyCollectionChangedEventHandler? CollectionChanged
            {
                add
                {
                    lock (ThreadLocker)
                    {
                        if (Handlers is null || Handlers.GetInvocationList().Length == 0)
                        {
                            NotifySource.CollectionChanged += NotifySource_CollectionChanged;
                        }
                        Handlers += value;
                    }
                }

                remove
                {
                    lock (ThreadLocker)
                    {
                        Handlers -= value;
                        if (Handlers is null || Handlers.GetInvocationList().Length == 0)
                        {
                            NotifySource.CollectionChanged -= NotifySource_CollectionChanged;
                        }
                    }
                }
            }

            public IEnumerator<T> GetEnumerator()
            {
                return Enumerable.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return Enumerable.GetEnumerator();
            }

            private void NotifySource_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                Handlers?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
            }
        }
    }
}
