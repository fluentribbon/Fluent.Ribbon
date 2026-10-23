// ReSharper disable once CheckNamespace
namespace Fluent;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Fluent.Internal.KnownBoxes;

/// <summary>
/// Handles loading and saving the state of a <see cref="Ribbon"/> from/to a <see cref="MemoryStream"/>, for temporary storage, and from/to <see cref="IsolatedStorage"/>, for persistent storage.
/// </summary>
public class RibbonStateStorage : IRibbonStateStorage
{
    private readonly Ribbon ribbon;

    // Name of the isolated storage file
    private string? isolatedStorageFileName;

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
            if (this.isolatedStorageFileName is not null)
            {
                return this.isolatedStorageFileName;
            }

            var stringForHash = string.Empty;
            var window = Window.GetWindow(this.ribbon);

            if (window is not null)
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

            this.isolatedStorageFileName = "Fluent.Ribbon.State." + GetHashForFileName(stringForHash).ToString("X");
            return this.isolatedStorageFileName;
        }
    }

    /// <summary>
    /// Gets the hash used in the name of the isolated storage file.
    /// It's the same as in earlier versions (the first 4 bytes of the MD5 hash), so saved states are still found.
    /// </summary>
    /// <remarks>
    /// On .NET Framework MD5 can't be created when Windows only allows FIPS validated algorithms, so a simple, stable hash is used then.
    /// A new hasher is created for each call, because a shared one isn't thread-safe and ribbons on different UI threads can get their names at the same time.
    /// </remarks>
    internal static int GetHashForFileName(string value)
    {
        var bytes = Encoding.Default.GetBytes(value);

        if (CryptoConfig.AllowOnlyFipsAlgorithms)
        {
            return GetStableHash(bytes);
        }

        using (var md5Hasher = MD5.Create())
        {
            return BitConverter.ToInt32(md5Hasher.ComputeHash(bytes), 0);
        }
    }

    // FNV-1a, which gives the same result on every run and every .NET version (unlike string.GetHashCode).
    internal static int GetStableHash(byte[] bytes)
    {
        unchecked
        {
            var hash = (int)2166136261;

            foreach (var b in bytes)
            {
                hash = (hash ^ b) * 16777619;
            }

            return hash;
        }
    }

    /// <inheritdoc />
    public virtual void SaveTemporary()
    {
        // The stream is reused for every save. Cut it to zero, not just rewind it:
        // a shorter state written over a longer one would otherwise keep the old tail
        // ("True,True,True" over "False,False,False" gives "True,True,Truelse").
        this.memoryStream.SetLength(0);
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
        builder.Append(',');
        builder.Append(this.ribbon.IsSimplified.ToString(CultureInfo.InvariantCulture));

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

            // Don't mark the state as loaded here.
            // Otherwise enabling AutomaticStateManagement later (Ribbon.LoadInitialState checks IsLoaded) would never load the persisted state,
            // and the next Save would overwrite it with the current, unloaded state.
            // Save is still safe while disabled, because it checks AutomaticStateManagement before IsLoaded.
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
                    // Empty the temporary stream first, so nothing a longer earlier state left behind remains.
                    stream.Position = 0;
                    this.memoryStream.SetLength(0);
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
        // The values are written with SetCurrentValue, so an app's OneWay bindings on these properties survive loading
        // (SetValue would replace them with local values and the app could no longer change the ribbon).
        var ribbonProperties = data.Split(',');

        if (ribbonProperties.Length <= 0)
        {
            return;
        }

        if (this.ribbon.CanMinimize
            && bool.TryParse(ribbonProperties[0], out var isMinimized))
        {
            this.ribbon.SetCurrentValue(Ribbon.IsMinimizedProperty, BooleanBoxes.Box(isMinimized));
        }

        if (ribbonProperties.Length <= 1)
        {
            return;
        }

        if (bool.TryParse(ribbonProperties[1], out var showQuickAccessToolBarAboveRibbon))
        {
            this.ribbon.SetCurrentValue(Ribbon.ShowQuickAccessToolBarAboveRibbonProperty, BooleanBoxes.Box(showQuickAccessToolBarAboveRibbon));
        }

        if (ribbonProperties.Length <= 2)
        {
            return;
        }

        if (this.ribbon.CanUseSimplified
            && bool.TryParse(ribbonProperties[2], out var isSimplified))
        {
            this.ribbon.SetCurrentValue(Ribbon.IsSimplifiedProperty, BooleanBoxes.Box(isSimplified));
        }
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
    /// Deletes all saved ribbon states in the isolated storage (for every ribbon, not only this one).
    /// The current state is not changed.
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