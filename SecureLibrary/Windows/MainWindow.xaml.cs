using Material.Icons;
using SecureLibrary.Core;
using SecureLibrary.Core.Features.LibraryItems;
using SecureLibrary.Core.Features.Storage;
using SecureLibrary.Core.Features.Storage.Modifiers;
using SecureLibrary.Core.Storage;
using SecureLibrary.Core.Storage.Methods;
using SecureLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SecureLibrary.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty LibraryProperty = DependencyProperty.Register(
            nameof(Library),
            typeof(Library),
            typeof(MainWindow),
            new FrameworkPropertyMetadata(null, OnLibraryPropertyChanged)
        );

        public static readonly DependencyProperty CurrentItemProperty = DependencyProperty.Register(
            nameof(CurrentItem),
            typeof(LibraryItem),
            typeof(MainWindow),
            new FrameworkPropertyMetadata(null)
        );

        public MainWindow()
        {
            MenuButtons = new();

            InitializeComponent();

            BuildMenuButtons();

            // This action can not be performed in MainWindow.xaml
            ((HierarchicalDataTemplate)Resources["LibraryItemTemplate"]).ItemTemplate =
             (HierarchicalDataTemplate)Resources["LibraryItemTemplate"];
        }

        public MainWindow(BinaryStorage storage, StorageMethod method) : this()
        {
            LoadLibrary(storage, method);
        }

        public MenuButtonCollection MenuButtons { get; }

        public Library? Library
        {
            get => (Library)GetValue(LibraryProperty);
            set => SetValue(LibraryProperty, value);
        }

        /// <summary>
        /// Item selected as tree root
        /// </summary>
        public LibraryItem? CurrentItem
        {
            get => (LibraryItem)GetValue(CurrentItemProperty);
            set => SetValue(CurrentItemProperty, value);
        }

        #region Builders

        private void BuildMenuButtons()
        {
            MenuButtons.Add(new("New library", MaterialIconKind.FilePlus, NewLibrary_Click));
            MenuButtons.Add(new("Open library", MaterialIconKind.FolderMove, OpenLibrary_Click));
            MenuButtons.Add(new("Save library", MaterialIconKind.FloppyDisk,
                                SaveLibrary_Click, SaveLibrary_RightClick));
        }

        #endregion

        #region Events

        #region Properties change handlers

        private static void OnLibraryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MainWindow window && e.OldValue != e.NewValue)
            {
                window.OnLibraryChanged();
            }
        }

        private void OnLibraryChanged()
        {
            LibraryTabs.Items.Clear();
            CurrentItem = Library?.Root;
        }

        #endregion

        #region Menu

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element &&
                element.DataContext is MenuButton button)
            {
                button.Click(this);
            }
        }

        private void MenuButton_RightClick(object? parameter)
        {
            if (parameter is MenuButton button)
            {
                button.RightClick(this);
            }
        }

        private void NewLibrary_Click(MenuButton sender, MainWindow? window)
        {
            OpenPickBinaryStorageContextMenu(StorageFactoryContext.New);
        }

        private void OpenLibrary_Click(MenuButton sender, MainWindow? window)
        {
            OpenPickBinaryStorageContextMenu(StorageFactoryContext.Load);
        }

        private void SaveLibrary_Click(MenuButton sender, MainWindow? window)
        {
            SaveLibrary();
        }

        private void SaveLibrary_RightClick(MenuButton sender, MainWindow? window)
        {
            if (Library is not null)
            {
                OpenPickBinaryStorageContextMenu(StorageFactoryContext.Save);
            }
        }

        #endregion

        #region Library tree items

        private void AddLibraryItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is FrameworkElement src &&
                e.OriginalSource is FrameworkElement originalSrc &&
                src.DataContext is LibraryItem item &&
                originalSrc.DataContext is LibraryItemFactory factory &&
                factory.Create() is LibraryItem newItem)
            {
                item.Add(newItem);

                if (TryFindAssociatedTree(item, out var associatedTree))
                {
                    associatedTree.IsExpanded = true;

                    if (associatedTree.ItemContainerGenerator.ContainerFromItem(newItem) is
                        TreeViewItem newAssociatedTree)
                    {
                        newAssociatedTree.IsSelected = true;
                    }
                }
                else if (LibraryTree.ItemContainerGenerator.ContainerFromItem(newItem) is
                         TreeViewItem newAssociatedTree)
                {
                    newAssociatedTree.IsSelected = true;
                }
            }
        }

        private void RenameLibraryItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is FrameworkElement src &&
                src.DataContext is LibraryItem item &&
                App.InputBox("Rename", item.Name) is string name)
            {
                item.Name = name;
            }
        }

        private void ChangeLibraryItemIcon_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is FrameworkElement src &&
                src.DataContext is LibraryItem item)
            {
                IconDialog dialog = new()
                {
                    SelectedIcon = item.Icon,
                    IconColor = item.IconColor ?? (Color)App.Current.Resources["TextColor"],
                };
                if (dialog.ShowDialog() == true)
                {
                    item.Icon = dialog.SelectedIcon;
                    item.IconColor = dialog.IconColor;
                }
            }
        }

        private void RemoveLibraryItemIcon_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is FrameworkElement src &&
                src.DataContext is LibraryItem item)
            {
                item.IconColor = null;
            }
        }

        private void ChangeLibraryItemSortWeight_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is FrameworkElement src &&
                src.DataContext is LibraryItem item &&
                App.InputBox("Sort weight", item.SortWeight.ToString()) is string weightStr)
            {
                if (float.TryParse(weightStr, out var weight))
                {
                    item.SortWeight = weight;
                }
                else
                {
                    App.MessageBox("Error", "Incorrect digit format", MessageBoxButton.OK,
                                   MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }
        }

        private void SetLibraryItemAsRoot_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is FrameworkElement src &&
                src.DataContext is LibraryItem item)
            {
                CurrentItem = item;
            }
        }

        private void LibraryItemAction_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement src &&
                src.DataContext is MenuButton button)
            {
                button.Click(this);
            }
        }

        private void RemoveLibraryItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is FrameworkElement src &&
                src.DataContext is LibraryItem item &&
                App.MessageBox("Remove", $"Delete item '{item.Name}'? This action can not be undone.",
                               MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)
                == MessageBoxResult.Yes)
            {
                item.RemoveFromOwner();
                item.InvokeForTree(LibraryTabs.Items.Remove);
            }
        }

        private void LibraryItem_DoubleClick(object? parameter)
        {
            if (LibraryTree.SelectedItem is LibraryItem item)
            {
                OpenTab(item);
            }
        }

        /// <summary>
        /// Required to fix drag and drop bug on exit from dialog via double click
        /// </summary>
        private int MouseMoveTicks = 0;

        private void LibraryItemTree_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                e.Source is FrameworkElement src &&
                src.DataContext is LibraryItem item)
            {
                if (MouseMoveTicks++ > 2)
                {
                    DragDrop.DoDragDrop(this, new DataObject("item", item), DragDropEffects.Copy);
                }
            }
            else
            {
                MouseMoveTicks = 0;
            }
        }

        private void LibraryItemTree_Drop(object sender, DragEventArgs e)
        {
            if (e.Source is FrameworkElement src &&
                src.DataContext is LibraryItem item &&
                e.Data.GetData("item") is LibraryItem droppedItem)
            {
                if (item != droppedItem &&
                    !droppedItem.TreeContains(item))
                {
                    droppedItem.RemoveFromOwner();
                    item.Add(droppedItem);

                    if (TryFindAssociatedTree(item, out var tree))
                    {
                        tree.IsExpanded = true;
                        tree.UpdateLayout();

                        if (tree.ItemContainerGenerator.ContainerFromItem(droppedItem) is
                            TreeViewItem droppedTree)
                        {
                            droppedTree.IsSelected = true;
                        }
                    }
                }

                e.Handled = true;
            }
        }

        private void LibraryTree_Drop(object sender, DragEventArgs e)
        {
            if (CurrentItem is not null &&
                e.Data.GetData("item") is LibraryItem droppedItem &&
                !CurrentItem.Contains(droppedItem))
            {
                droppedItem.RemoveFromOwner();
                CurrentItem.Add(droppedItem);

                if (LibraryTree.ItemContainerGenerator.ContainerFromItem(droppedItem) is
                    TreeViewItem droppedTree)
                {
                    droppedTree.IsSelected = true;
                }

                e.Handled = true;
            }
        }

        #endregion

        #region Window keybinds

        private void NewKeybind_Click(object? property)
        {
            OpenPickBinaryStorageContextMenu(StorageFactoryContext.New);
        }

        private void OpenKeybind_Click(object? property)
        {
            OpenPickBinaryStorageContextMenu(StorageFactoryContext.Load);
        }

        private void SaveKeybind_Click(object? property)
        {
            SaveLibrary();
        }

        #endregion

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem?.Owner is not null)
            {
                CurrentItem = CurrentItem.Owner;
            }
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is FrameworkElement source &&
                source.DataContext is LibraryItem item)
            {
                CloseTab(item);
            }
        }

        private void CurrentItemPathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box)
            {
                box.ScrollToHorizontalOffset(double.PositiveInfinity);
            }
        }

        private void PickBinaryStorageMenuItem_Click(object? parameter)
        {
            StorageFactoryContext context = GetPickBinaryStorageContextMenuContext();

            if (parameter is ItemsControl control &&
                control.Tag is FrameworkElement parentControl &&
                control.DataContext is StorageMethodFactory methodFactory &&
                parentControl.DataContext is BinaryStorageFactory storageFactory &&
                (context == StorageFactoryContext.Save || ValidateSaved()) &&
                storageFactory.Create(context) is BinaryStorage storage &&
                methodFactory.Create(context) is StorageMethod method)
            {
                switch (context)
                {
                    case StorageFactoryContext.New:
                        NewLibrary(storage, method);
                        break;
                    case StorageFactoryContext.Load:
                        LoadLibrary(storage, method);
                        break;
                    case StorageFactoryContext.Save:
                        SaveLibrary(storage, method);
                        break;
                }
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !ValidateSaved();
        }

        #endregion

        #region Utilities

        public bool OpenTab(LibraryItem item)
        {
            if (item.UI is not null)
            {
                if (!LibraryTabs.Items.Contains(item))
                {
                    LibraryTabs.Items.Add(item);
                }
                LibraryTabs.SelectedItem = item;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CloseTab(LibraryItem item)
        {
            if (LibraryTabs.Items.Contains(item))
            {
                LibraryTabs.Items.Remove(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SelectItem(LibraryItem item)
        {
            if (TryFindAssociatedTree(item, out var tree))
            {
                tree.IsSelected = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Finds TreeViewItem that is associated with LibraryItem
        /// </summary>
        private bool TryFindAssociatedTree(LibraryItem item, [MaybeNullWhen(false)] out TreeViewItem value)
        {
            value = FindAssociatedTree(item, LibraryTree.Items, LibraryTree.ItemContainerGenerator);
            return value is not null;
        }

        private TreeViewItem? FindAssociatedTree(LibraryItem item, ItemCollection items,
                                                 ItemContainerGenerator generator)
        {
            if (items.Contains(item))
            {
                return generator.ContainerFromItem(item) as TreeViewItem;
            }
            return (from object i in items
                    let c = generator.ContainerFromItem(i)
                    where c is TreeViewItem
                    let t = (TreeViewItem)c
                    let a = FindAssociatedTree(item, t.Items, t.ItemContainerGenerator)
                    where a is not null
                    select a).FirstOrDefault();
        }

        private void OpenPickBinaryStorageContextMenu(StorageFactoryContext context)
        {
            if (Resources["PickBinaryStorageContextMenu"] is ContextMenu menu)
            {
                menu.Tag = context;
                menu.IsOpen = true;
            }
        }

        private StorageFactoryContext GetPickBinaryStorageContextMenuContext()
        {
            return 
                Resources["PickBinaryStorageContextMenu"] is ContextMenu menu &&
                menu.Tag is StorageFactoryContext context ?
                    context :
                    StorageFactoryContext.None;
        }

        private bool ValidateSaved()
        {
            return Library is null || !Library.IsModified ||

                App.MessageBox("Save", "Save library changes?",
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes) switch
                {
                    MessageBoxResult.Yes => SaveLibrary(),
                    MessageBoxResult.No => true,
                    _ => false,
                };
        }

        private void NewLibrary(BinaryStorage storage, StorageMethod method)
        {
            Library = new()
            {
                Storage = storage,
                StorageMethod = method,
            };
        }

        public void LoadLibrary(BinaryStorage storage, StorageMethod method)
        {
            Library lib;
            ErrorCollection errors;
#if DEBUG
            lib = Library.Load(storage, method, out errors);
#else
            try
            {
                lib = Library.Load(storage, method, out errors);
            }
            catch (Exception ex)
            {
                App.MessageBox("Fatal error", "Could not load library: " + ex.Message,
                               MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
#endif
            if (errors.Count == 0 ||
                App.MessageBox("Error", $"Errors occured during loading:\n{string.Join('\n', errors)}\n" +
                                         "Corrupted data may be lost. Proceed loading?",
                               MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No)
                == MessageBoxResult.Yes)
            {
                Library = lib;
            }
        }

        private void SaveLibrary(BinaryStorage storage, StorageMethod method)
        {
            if (Library is not null)
            {
                Library.Storage = storage;
                Library.StorageMethod = method;
                SaveLibrary();
            }
        }

        private bool SaveLibrary()
        {
            if (Library is not null)
            {
                if (Library.Storage is null || Library.StorageMethod is null)
                {
                    OpenPickBinaryStorageContextMenu(StorageFactoryContext.Save);
                }
                else
                {
#if !DEBUG
                    try
                    {
#endif
                        Library.Save(
                            e => App.MessageBox("Error", $"Errors occured during saving:\n{string.Join('\n', e)}\n" +
                                                         $"Corrupted data will be lost! Proceed saving?",
                                                MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No
                            ) == MessageBoxResult.Yes, out _
                        );
                        Library.ResetModified();
                        return true;
#if !DEBUG
                    }
                    catch (Exception ex)
                    {
                        App.MessageBox("Fatal error", "Could not save library: " + ex.Message,
                                       MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    }
#endif
                }
            }
            return false;
        }

        #endregion
    }
}
