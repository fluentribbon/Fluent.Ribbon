namespace Fluent.Internal
{
    using System;

    /// <summary>
    /// Scope guard to prevent reentrancy.
    /// </summary>
    public class ScopeGuard : IDisposable
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Action onEntry;
        private readonly Action onDispose;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ScopeGuard()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="onEntry">Action being called on entry.</param>
        /// <param name="onDispose">Action being called on dispose.</param>
        public ScopeGuard(Action onEntry, Action onDispose)
        {
            this.onEntry = onEntry ?? throw new ArgumentNullException(nameof(onEntry));
            this.onDispose = onDispose ?? throw new ArgumentNullException(nameof(onDispose));
        }

        /// <summary>
        /// Gets whether this instance is still active (not disposed) or not.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Starts the scope guard.
        /// </summary>
        /// <returns>The current instance for fluent usage.</returns>
        public ScopeGuard Start()
        {
            if (this.IsActive)
            {
                return this;
            }

            this.IsActive = true;
            this.onEntry?.Invoke();

            return this;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var wasActive = this.IsActive;
            this.IsActive = false;

            if (wasActive)
            {
                this.onDispose?.Invoke();
            }
        }
    }
}