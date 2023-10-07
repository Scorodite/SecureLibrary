using Material.Icons;
using SecureLibrary.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SecureLibrary.Core
{
    /// <summary>
    /// Main library node class
    /// </summary>
    public class LibraryItem : ISerializable, IComparable<LibraryItem>, ICollection<LibraryItem>,
                               INotifyPropertyChanged, INotifyCollectionChanged
    {
        private string _Name;
        private float _SortWeight;
        private MaterialIconKind _Icon;
        private Color? _IconColor;
        private LibraryItem? _Owner;

        public LibraryItem()
        {
            _Name = string.Empty;
            _Icon = MaterialIconKind.Folder;
            Items = new();
            Actions = new();
            IsModified = true;
        }

        public LibraryItem(string name) : this()
        {
            _Name = name;
        }

        public string Name { get => _Name; set => SetName(value); }

        /// <summary>
        /// Value that will be used in sorting this item in Owner.Items
        /// </summary>
        public float SortWeight { get => _SortWeight; set => SetSortWeight(value); }
        public MaterialIconKind Icon { get => _Icon; set => SetIcon(value); }
        public Color? IconColor { get => _IconColor; set => SetIconColor(value); }

        /// <summary>
        /// Item that contains this item
        /// </summary>
        public LibraryItem? Owner { get => _Owner; private set => SetOwner(value); }

        /// <summary>
        /// Path of this item inside tree
        /// </summary>
        public string Path => CalculatePath();

        public bool IsModified { get; set; }

        /// <summary>
        /// Special item actions that will be displayed in context menu
        /// </summary>
        public MenuButtonCollection Actions { get; }

        /// <summary>
        /// UI element that will be displayed in tab
        /// If returned null, tab will not be opened
        /// </summary>
        public virtual FrameworkElement? UI => null;

        public bool RemoveFromOwner()
        {
            return Owner is null || Owner.Remove(this);
        }

        public bool TreeContains(LibraryItem other)
        {
            return Items.Contains(other) || Items.Any(i => i.TreeContains(other));
        }

        /// <summary>
        /// Recursively invokes action for the entire tree including this item
        /// </summary>
        /// <param name="action"></param>
        public void InvokeForTree(Action<LibraryItem> action)
        {
            action(this);
            foreach (LibraryItem item in Items)
            {
                item.InvokeForTree(action);
            }
        }

        #region Setters/Getters

        private int SetName(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _Name = value;
            int index = Owner?.Resort(this) ?? 0;
            OnPropertyChanged(nameof(Name));
            return index;
        }

        private int SetSortWeight(float value)
        {
            _SortWeight = value;
            int index = Owner?.Resort(this) ?? 0;
            OnPropertyChanged(nameof(SortWeight));
            return index;
        }

        private void SetIcon(MaterialIconKind value)
        {
            _Icon = value;
            OnPropertyChanged(nameof(Icon));
        }

        private void SetIconColor(Color? value)
        {
            _IconColor = value;
            OnPropertyChanged(nameof(IconColor));
        }

        private void SetOwner(LibraryItem? value)
        {
            _Owner = value;
            OnPropertyChanged(nameof(Owner));
            OnPropertyChanged(nameof(Path));
        }

        private string CalculatePath()
        {
            if (Owner is null) return string.Empty;
            string ownerPath = Owner.Path;
            return string.IsNullOrEmpty(ownerPath) ? Name : ownerPath + "/" + Name;
        }

        #endregion

        #region ISerializable implementation

        public virtual void WriteData(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(SortWeight);
            writer.Write((int)Icon);
            if (IconColor is Color color)
            {
                writer.Write(true);
                writer.Write(color.R);
                writer.Write(color.G);
                writer.Write(color.B);
            }
            else
            {
                writer.Write(false);
            }
        }

        public virtual void ReadData(BinaryReader reader)
        {
            Name = reader.ReadString();
            SortWeight = reader.ReadSingle();
            Icon = (MaterialIconKind)reader.ReadInt32();
            IconColor =
                reader.ReadBoolean() ?
                    new()
                    {
                        R = reader.ReadByte(),
                        G = reader.ReadByte(),
                        B = reader.ReadByte(),
                        A = byte.MaxValue,
                    } :
                    null;
        }

        #endregion

        #region IComparable<LibraryItem> implementation

        public int CompareTo(LibraryItem? other)
        {
            return
                other is null ?
                    1 :
                    SortWeight == other.SortWeight ?
                        Name.CompareTo(other.Name) :
                        SortWeight.CompareTo(other.SortWeight);
        }

        #endregion

        #region ICollection<LibraryItem> implementation

        private readonly List<LibraryItem> Items = new();

        public int Count => Items.Count;
        public LibraryItem this[int index] => Items[index];
        bool ICollection<LibraryItem>.IsReadOnly => false;

        void ICollection<LibraryItem>.Add(LibraryItem item)
        {
            Add(item);
        }

        public int Add(LibraryItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (item.Owner is not null)
            {
                throw new InvalidOperationException("This item is already owned");
            }

            item.Owner = this;
            int index = InsertSortedItem(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            return index;
        }

        public void Clear()
        {
            foreach (LibraryItem item in Items)
            {
                item.Owner = null;
            }
            Items.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool Contains(LibraryItem item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(LibraryItem[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<LibraryItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(LibraryItem item)
        {
            int index = Items.IndexOf(item);
            if (index > -1)
            {
                Items.Remove(item);
                item.Owner = null;
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
                return true;
            }
            return false;
        }

        private int Resort(LibraryItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (item.Owner != this)
            {
                throw new ArgumentException("Item is not owned by this collection", nameof(item));
            }

            int oldIndex = Items.IndexOf(item);
            Items.Remove(item);
            int index = InsertSortedItem(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Move, item, index, oldIndex);
            return index;
        }

        private int InsertSortedItem(LibraryItem item)
        {
            int index = 0;
            while (index < Items.Count && Items[index].CompareTo(item) < 0) index++;
            Items.Insert(index, item);
            return index;
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
            IsModified = true;
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
    }
}
