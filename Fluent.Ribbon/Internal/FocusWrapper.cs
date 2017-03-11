namespace Fluent.Internal
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    internal class FocusWrapper
    {
        private readonly IInputElement inputElement;
        private readonly IntPtr handle;

        private FocusWrapper(IInputElement inputElement)
        {
            this.inputElement = inputElement;
        }

        private FocusWrapper(IntPtr handle)
        {
            this.handle = handle;
        }

        public void Focus()
        {
            if (this.inputElement != null)
            {
                this.inputElement.Focus();
                return;
            }

            if (this.handle != IntPtr.Zero)
            {
                NativeMethods.SetFocus(this.handle);
            }
        }

        public static FocusWrapper GetWrapperForCurrentFocus()
        {
            if (Keyboard.FocusedElement != null)
            {
                return new FocusWrapper(Keyboard.FocusedElement);
            }

            var handle = NativeMethods.GetFocus();

            if (handle != IntPtr.Zero)
            {
                return new FocusWrapper(handle);
            }

            return null;
        }
    }
}