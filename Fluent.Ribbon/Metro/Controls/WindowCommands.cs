﻿// ReSharper disable once CheckNamespace
namespace Fluent;

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Fluent.Helpers;
using Fluent.Internal.KnownBoxes;
using Windows.Win32;

/// <summary>
/// Contains commands for <see cref="RibbonWindow"/>
/// </summary>
[TemplatePart(Name = "PART_Min", Type = typeof(Button))]
[TemplatePart(Name = "PART_Max", Type = typeof(Button))]
[TemplatePart(Name = "PART_Restore", Type = typeof(Button))]
[TemplatePart(Name = "PART_Close", Type = typeof(Button))]
[TemplatePart(Name = "PART_Items", Type = typeof(ItemsControl))]
public class WindowCommands : ItemsControl, IDisposable
{
    private static string? minimize;
    private static string? maximize;
    private static string? closeText;
    private static string? restore;

    private System.Windows.Controls.Button? minimizeButton;
    private System.Windows.Controls.Button? maximizeButton;
    private System.Windows.Controls.Button? restoreButton;
    private System.Windows.Controls.Button? closeButton;

    private SafeHandle? user32;

    private bool disposed;

    static WindowCommands()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommands), new FrameworkPropertyMetadata(typeof(WindowCommands)));
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="WindowCommands"/> class.
    /// </summary>
    ~WindowCommands()
    {
        this.Dispose(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios.
    /// If disposing equals true, the method has been called directly
    /// or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed.
    /// If disposing equals false, the method has been called by the
    /// runtime from inside the finalizer and you should not reference
    /// other objects. Only unmanaged resources can be disposed.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (this.disposed)
        {
            return;
        }

        // If disposing equals true, dispose all managed
        // and unmanaged resources.
        if (disposing)
        {
            // Dispose managed resources.
        }

        // Call the appropriate methods to clean up
        // unmanaged resources here.
        // If disposing is false,
        // only the following code is executed.
        if (this.user32 is not null)
        {
            this.user32.Close();
            this.user32 = null;
        }

        // Note disposing has been done.
        this.disposed = true;
    }

    /// <summary>
    /// Retrieves the translated string for Minimize
    /// </summary>
    public string? Minimize
    {
        get
        {
            if (string.IsNullOrEmpty(minimize))
            {
                minimize = this.GetCaption(900);
            }

            return minimize;
        }
    }

    /// <summary>
    /// Retrieves the translated string for Maximize
    /// </summary>
    public string? Maximize
    {
        get
        {
            if (string.IsNullOrEmpty(maximize))
            {
                maximize = this.GetCaption(901);
            }

            return maximize;
        }
    }

    /// <summary>
    /// Retrieves the translated string for Restore
    /// </summary>
    public string? Restore
    {
        get
        {
            if (string.IsNullOrEmpty(restore))
            {
                restore = this.GetCaption(903);
            }

            return restore;
        }
    }

    /// <summary>
    /// Retrieves the translated string for Close
    /// </summary>
    public string? Close
    {
        get
        {
            if (string.IsNullOrEmpty(closeText))
            {
                closeText = this.GetCaption(905);
            }

            return closeText;
        }
    }

    /// <summary>
    /// Gets the <see cref="System.Windows.Controls.ItemsControl"/> responsible for showing <see cref="System.Windows.Controls.ItemsControl.Items"/>.
    /// </summary>
    public ItemsControl? ItemsControl { get; private set; }

    /// <summary>Identifies the <see cref="ItemsPanelVisibility"/> dependency property.</summary>
    public static readonly DependencyProperty ItemsPanelVisibilityProperty = DependencyProperty.Register(nameof(ItemsPanelVisibility), typeof(Visibility), typeof(WindowCommands), new PropertyMetadata(VisibilityBoxes.Visible));

    /// <summary>
    /// Gets or sets the <see cref="Visibility"/> of the panel which contains <see cref="System.Windows.Controls.ItemsControl.Items"/>.
    /// </summary>
    public Visibility ItemsPanelVisibility
    {
        get => (Visibility)this.GetValue(ItemsPanelVisibilityProperty);
        set => this.SetValue(ItemsPanelVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets the button brush
    /// </summary>
    public Brush ButtonBrush
    {
        get => (Brush)this.GetValue(ButtonBrushProperty);
        set => this.SetValue(ButtonBrushProperty, value);
    }

    /// <summary>Identifies the <see cref="ButtonBrush"/> dependency property.</summary>
    public static readonly DependencyProperty ButtonBrushProperty = DependencyProperty.Register(nameof(ButtonBrush), typeof(Brush), typeof(WindowCommands), new PropertyMetadata(Brushes.Black));

    private unsafe string GetCaption(uint id)
    {
        if (this.user32 is null)
        {
            this.user32 = PInvoke.LoadLibrary(Path.Combine(Environment.SystemDirectory, "User32.dll"));
        }

        var chars = new char[256];
        if (PInvoke.LoadString(this.user32, id, chars.AsSpan(), 256) == 0)
        {
            return $"String with id '{id}' could not be found.";
        }
#pragma warning disable CA1307 // Specify StringComparison for clarity
        return new string(chars).Replace("&", string.Empty);
#pragma warning restore CA1307 // Specify StringComparison for clarity
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this.minimizeButton = this.Template.FindName("PART_Min", this) as System.Windows.Controls.Button;
        if (this.minimizeButton is not null)
        {
            this.minimizeButton.Click += this.MinimizeClick;
        }

        this.maximizeButton = this.Template.FindName("PART_Max", this) as System.Windows.Controls.Button;
        if (this.maximizeButton is not null)
        {
            this.maximizeButton.Click += this.MaximiseClick;
        }

        this.restoreButton = this.Template.FindName("PART_Restore", this) as System.Windows.Controls.Button;
        if (this.restoreButton is not null)
        {
            this.restoreButton.Click += this.RestoreClick;
        }

        this.closeButton = this.GetTemplateChild("PART_Close") as System.Windows.Controls.Button;
        if (this.closeButton is not null)
        {
            this.closeButton.Click += this.CloseClick;
        }

        this.ItemsControl = this.Template.FindName("PART_Items", this) as ItemsControl;
    }

    /// <inheritdoc />
    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseRightButtonDown(e);

        WindowSteeringHelper.ShowSystemMenu(this, e);
    }

    private void MinimizeClick(object sender, RoutedEventArgs e)
    {
        var parentWindow = this.GetParentWindow();
        if (parentWindow is not null)
        {
#pragma warning disable 618
            ControlzEx.SystemCommands.MinimizeWindow(parentWindow);
#pragma warning restore 618
        }
    }

    private void MaximiseClick(object sender, RoutedEventArgs e)
    {
        var parentWindow = this.GetParentWindow();
        if (parentWindow is not null)
        {
#pragma warning disable 618
            ControlzEx.SystemCommands.MaximizeWindow(parentWindow);
#pragma warning restore 618
        }
    }

    private void RestoreClick(object sender, RoutedEventArgs e)
    {
        var parentWindow = this.GetParentWindow();
        if (parentWindow is not null)
        {
#pragma warning disable 618
            ControlzEx.SystemCommands.RestoreWindow(parentWindow);
#pragma warning restore 618
        }
    }

    private void CloseClick(object sender, RoutedEventArgs e)
    {
        var parentWindow = this.GetParentWindow();

        if (parentWindow is not null)
        {
#pragma warning disable 618
            ControlzEx.SystemCommands.CloseWindow(parentWindow);
#pragma warning restore 618
        }
    }

    private Window? GetParentWindow()
    {
        var window = Window.GetWindow(this);

        if (window is not null)
        {
            return window;
        }

        var parent = VisualTreeHelper.GetParent(this);
        Window? parentWindow = null;

        while (parent is not null
               && (parentWindow = parent as Window) is null)
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return parentWindow;
    }
}