namespace Fluent.Internal
{
    using System;

    /// <summary>
    /// Scope guard to prevent reentrancy.
    /// </summary>
    public class ScopeGuard : IDisposable
    {
        /// <summary>
        /// Gets whether this instance is still active (not disposed) or not.
        /// </summary>
        public bool IsActive { get; private set; } = true;

        /// <inheritdoc />
        public void Dispose()
        {
            this.IsActive = false;
        }
    }
}