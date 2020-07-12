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

            // Save Ribbon State
            builder.Append(this.ribbon.IsMinimized.ToString(CultureInfo.InvariantCulture));
            builder.Append(',');
            builder.Append(this.ribbon.ShowQuickAccessToolBarAboveRibbon.ToString(CultureInfo.InvariantCulture));

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
            // Load Ribbon State
            var ribbonProperties = data.Split(',');

            this.ribbon.IsMinimized = bool.Parse(ribbonProperties[0]);

            this.ribbon.ShowQuickAccessToolBarAboveRibbon = bool.Parse(ribbonProperties[1]);
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