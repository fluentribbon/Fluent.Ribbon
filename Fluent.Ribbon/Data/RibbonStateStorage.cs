// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows;

    /// <summary>
    /// Handles loading and saving the state of a <see cref="Ribbon"/> from/to a <see cref="MemoryStream"/>, for temporary storage, and from/to <see cref="IsolatedStorage"/>, for persistent storage.
    /// </summary>
    public class RibbonStateStorage : IRibbonStateStorage
    {
        private static readonly MD5 md5Hasher = MD5.Create();

        private readonly Ribbon ribbon;

        // Name of the isolated storage file
        private string isolatedStorageFileName;
        private readonly Stream memoryStream;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="ribbon">The <see cref="Ribbon"/> of which the state should be stored.</param>
        public RibbonStateStorage(Ribbon ribbon)
        {
            this.ribbon = ribbon;
            this.memoryStream = new MemoryStream();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RibbonStateStorage"/> class.
        /// </summary>
        ~RibbonStateStorage()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets whether this object already got disposed.
        /// </summary>
        protected bool Disposed { get; private set; }

        /// <inheritdoc />
        public bool IsLoading { get; private set; }

        /// <inheritdoc />
        public bool IsLoaded { get; private set; }

        /// <summary>
        ///     Gets name of the isolated storage file
        /// </summary>
        protected string IsolatedStorageFileName
        {
            get
            {
                if (this.isolatedStorageFileName != null)
                {
                    return this.isolatedStorageFileName;
                }

                var stringForHash = string.Empty;
                var window = Window.GetWindow(this.ribbon);

                if (window != null)
                {
                    stringForHash += "." + window.GetType().FullName;

                    if (string.IsNullOrEmpty(window.Name) == false
                        && window.Name.Trim().Length > 0)
                    {
                        stringForHash += "." + window.Name;
                    }
                }

                if (string.IsNullOrEmpty(this.ribbon.Name) == false
                    && this.ribbon.Name.Trim().Length > 0)
                {
                    stringForHash += "." + this.ribbon.Name;
                }

                this.isolatedStorageFileName = "Fluent.Ribbon.State." + BitConverter.ToInt32(md5Hasher.ComputeHash(Encoding.Default.GetBytes(stringForHash)), 0).ToString("X");
                return this.isolatedStorageFileName;
            }
        }

        /// <inheritdoc />
        public virtual void SaveTemporary()
        {
            this.memoryStream.Position = 0;
            this.Save(this.memoryStream);
        }

        /// <inheritdoc />
        public virtual void Save()
        {
            // Check whether automatic save is valid now
            if (this.ribbon.AutomaticStateManagement == false)
            {
                Debug.WriteLine("State not saved to isolated storage. Because automatic state management is disabled.");
                return;
            }

            if (this.IsLoaded == false)
            {
                Debug.WriteLine("State not saved to isolated storage. Because state was not loaded before.");
                return;
            }

            try
            {
                var storage = GetIsolatedStorageFile();

                using (var stream = new IsolatedStorageFileStream(this.IsolatedStorageFileName, FileMode.Create, FileAccess.Write, storage))
                {
                    this.Save(stream);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error while trying to save Ribbon state. Error: {ex}");
            }
        }

        /// <summary>
        /// Saves state to <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Stream</param>
        protected virtual void Save(Stream stream)
        {
            // Don't save or load state in design mode
            if (DesignerProperties.GetIsInDesignMode(this.ribbon))
            {
                return;
            }

            var builder = this.CreateStateData();

            var writer = new StreamWriter(stream);
            writer.Write(builder.ToString());

            writer.Flush();
        }

        /// <summary>
        /// Create the serialized state data which should be saved later.
        /// </summary>
        /// <returns><see cref="StringBuilder"/> which contains the serialized state data.</returns>
        protected virtual StringBuilder CreateStateData()
        {
            var builder = new StringBuilder();

            var isMinimizedSaveState = this.ribbon.IsMinimized;

            // Save Ribbon State
            builder.Append(isMinimizedSaveState.ToString(CultureInfo.InvariantCulture));
            builder.Append(',');
            builder.Append(this.ribbon.ShowQuickAccessToolBarAboveRibbon.ToString(CultureInfo.InvariantCulture));
            builder.Append('|');

            // Save QAT items
            var paths = new Dictionary<FrameworkElement, string>();
            this.ribbon.TraverseLogicalTree(this.ribbon, string.Empty, paths);

            // Foreach items and see whether path is found for the item
            foreach (var element in this.ribbon.GetQuickAccessElements())
            {
                var control = element.Key as FrameworkElement;

                if (control != null
                    && paths.TryGetValue(control, out var path))
                {
                    builder.Append(path);
                    builder.Append(';');
                }
                else
                {
                    // Item is not found in logical tree, output to debug console
#if DEBUG
                    var controlName = control != null && string.IsNullOrEmpty(control.Name) == false
                                          ? string.Format(CultureInfo.InvariantCulture, " (name of the control is {0})", control.Name)
                                          : string.Empty;
                    Debug.WriteLine($"Control \"{controlName}\" of type \"{element.Key.GetType().Name}\" is not found in logical tree during QAT saving");
#endif
                }
            }

            return builder;
        }

        /// <inheritdoc />
        public virtual void LoadTemporary()
        {
            this.memoryStream.Position = 0;
            this.Load(this.memoryStream);
        }

        /// <inheritdoc />
        public virtual void Load()
        {
            // Don't save or load state in design mode
            if (DesignerProperties.GetIsInDesignMode(this.ribbon))
            {
                Debug.WriteLine("State not loaded from isolated storage. Because we are in design mode.");
                this.IsLoaded = true;
                return;
            }

            if (this.ribbon.AutomaticStateManagement == false)
            {
                Debug.WriteLine("State not loaded from isolated storage. Because automatic state management is disabled.");
                this.IsLoaded = true;
                return;
            }

            try
            {
                var storage = GetIsolatedStorageFile();
                if (IsolatedStorageFileExists(storage, this.IsolatedStorageFileName))
                {
                    using (var stream = new IsolatedStorageFileStream(this.IsolatedStorageFileName, FileMode.Open, FileAccess.Read, storage))
                    {
                        this.Load(stream);

                        // Copy loaded state to MemoryStream for temporary storage.
                        // Temporary storage is used for style changes etc. so we can apply the current state again.
                        stream.Position = 0;
                        this.memoryStream.Position = 0;
                        stream.CopyTo(this.memoryStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error while trying to load Ribbon state. Error: {ex}");
            }

            this.IsLoaded = true;
        }

        /// <summary>
        /// Loads state from <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the state from.</param>
        protected virtual void Load(Stream stream)
        {
            this.IsLoading = true;

            try
            {
                this.LoadStateCore(stream);
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        /// <summary>
        /// Loads state from <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the state from.</param>
        protected virtual void LoadStateCore(Stream stream)
        {
            var reader = new StreamReader(stream);
            var data = reader.ReadToEnd();
            this.LoadState(data);
        }

        /// <summary>
        /// Loads state from <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="string"/> to load the state from.</param>
        protected virtual void LoadState(string data)
        {
            var splitted = data.Split('|');

            if (splitted.Length != 2)
            {
                return;
            }

            // Load Ribbon State
            var ribbonProperties = splitted[0].Split(',');

            var isMinimized = bool.Parse(ribbonProperties[0]);

            this.ribbon.IsMinimized = isMinimized;

            this.ribbon.ShowQuickAccessToolBarAboveRibbon = bool.Parse(ribbonProperties[1]);

            this.LoadQuickAccessItems(splitted[1]);
        }

        /// <summary>
        /// Loads quick access items from <paramref name="quickAccessItemsData"/>.
        /// </summary>
        /// <param name="quickAccessItemsData">Serialized data for generating quick access items.</param>
        protected virtual void LoadQuickAccessItems(string quickAccessItemsData)
        {
            // Load items
            var items = quickAccessItemsData.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            this.ribbon.ClearQuickAccessToolBar();

            foreach (var item in items)
            {
                var quickAccessItem = this.CreateQuickAccessItem(item);

                if (quickAccessItem != null)
                {
                    this.ribbon.AddToQuickAccessToolBar(quickAccessItem);
                }
            }

            // Sync QAT menu items
            foreach (var menuItem in this.ribbon.QuickAccessItems)
            {
                menuItem.IsChecked = this.ribbon.IsInQuickAccessToolBar(menuItem.Target);
            }
        }

        /// <summary>
        /// Creates a quick access item (<see cref="UIElement"/>) from the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Serialized data for one quick access item.</param>
        /// <returns>The created quick access item or <c>null</c> of the creation failed.</returns>
        protected virtual UIElement CreateQuickAccessItem(string data)
        {
            var indices = data.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(x => int.Parse(x, CultureInfo.InvariantCulture)).ToList();

            DependencyObject current = this.ribbon;

            foreach (var index in indices)
            {
                var children = LogicalTreeHelper.GetChildren(current).OfType<object>().ToList();
                var indexIsInvalid = children.Count <= index;
                var item = indexIsInvalid
                               ? null
                               : children[index] as DependencyObject;

                if (item == null)
                {
                    // Path is incorrect
                    Debug.WriteLine("Error while QAT items loading: one of the paths is invalid");
                    return null;
                }

                current = item;
            }

            var result = current as UIElement;
            if (result == null
                || QuickAccessItemsProvider.IsSupported(result) == false)
            {
                // Item is invalid
                Debug.WriteLine($"Error while QAT items loading. Could not add \"{current}\" to QAT.");
                return null;
            }

            return result;
        }

        /// <summary>
        /// Determines whether the given file exists in the given storage
        /// </summary>
        protected static bool IsolatedStorageFileExists(IsolatedStorageFile storage, string fileName)
        {
            var files = storage.GetFileNames(fileName);
            return files.Length != 0;
        }

        /// <summary>
        /// Get this <see cref="IsolatedStorageFile"/> which should be used to store the current state.
        /// </summary>
        /// <returns><see cref="IsolatedStorageFile.GetUserStoreForDomain"/> or <see cref="IsolatedStorageFile.GetUserStoreForAssembly"/> if <see cref="IsolatedStorageFile.GetUserStoreForDomain"/> threw an exception.</returns>
        protected static IsolatedStorageFile GetIsolatedStorageFile()
        {
            try
            {
                return IsolatedStorageFile.GetUserStoreForDomain();
            }
            catch
            {
                return IsolatedStorageFile.GetUserStoreForAssembly();
            }
        }

        /// <summary>
        /// Resets saved state.
        /// </summary>
        public virtual void Reset()
        {
            var storage = GetIsolatedStorageFile();

            foreach (var filename in storage.GetFileNames("*Fluent.Ribbon.State*"))
            {
                storage.DeleteFile(filename);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Defines whether managed resources should also be freed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.Disposed)
            {
                return;
            }

            if (disposing)
            {
                this.memoryStream.Dispose();
            }

            this.Disposed = true;
        }
    }
}