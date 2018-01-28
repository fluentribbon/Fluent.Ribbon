// ReSharper disable once CheckNamespace
namespace Fluent
{
#if NET40
    using System;
#endif

    /// <summary>
    /// Represents the result of <see cref="IKeyTipedControl.OnKeyTipPressed"/>.
    /// </summary>
    public class KeyTipPressedResult
#if NET40
        : EventArgs
#endif
    {
        /// <summary>
        /// An empty default instance.
        /// </summary>
#if NET40
        public static new readonly KeyTipPressedResult Empty = new KeyTipPressedResult();
#else
        public static readonly KeyTipPressedResult Empty = new KeyTipPressedResult();
#endif

        private KeyTipPressedResult()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="pressedElementAquiredFocus">Defines if the pressed element aquired focus or not.</param>
        /// <param name="pressedElementOpenedPopup">Defines if the pressed element opened a popup or not.</param>
        public KeyTipPressedResult(bool pressedElementAquiredFocus, bool pressedElementOpenedPopup)
        {
            this.PressedElementAquiredFocus = pressedElementAquiredFocus;
            this.PressedElementOpenedPopup = pressedElementOpenedPopup;
        }

        /// <summary>
        /// Defines if the pressed element aquired focus or not.
        /// </summary>
        public bool PressedElementAquiredFocus { get; }

        /// <summary>
        /// Defines if the pressed element opened a popup or not.
        /// </summary>
        public bool PressedElementOpenedPopup { get; }
    }
}