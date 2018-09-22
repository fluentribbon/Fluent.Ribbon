namespace Fluent
{
    using System;

    /// <summary>
    /// Interface for handling loading and saving the state of a <see cref="Ribbon"/>.
    /// </summary>
    public interface IRibbonStateStorage : IDisposable
    {
        /// <summary>
        /// Gets whether state is currently loading.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Gets or sets whether state is loaded.
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Save current state to a temporary storage.
        /// </summary>
        void SaveTemporary();

        /// <summary>
        /// Save current state to a persistent storage.
        /// </summary>
        void Save();

        /// <summary>
        /// Load state from a temporary storage.
        /// </summary>
        void LoadTemporary();

        /// <summary>
        /// Loads the state from a persistent storage.
        /// </summary>
        /// <remarks>
        /// Sets <see cref="RibbonStateStorage.IsLoaded" /> after it's finished to prevent a race condition with saving the state to the temporary storage.
        /// </remarks>
        void Load();

        /// <summary>
        /// Resets saved state.
        /// </summary>
        void Reset();
    }
}