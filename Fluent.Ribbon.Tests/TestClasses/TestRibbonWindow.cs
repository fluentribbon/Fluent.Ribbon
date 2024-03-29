﻿namespace Fluent.Tests.TestClasses;

using System;
using System.Diagnostics;

public sealed class TestRibbonWindow : RibbonWindow, IDisposable
{
    public TestRibbonWindow()
        : this(null)
    {
    }

    public TestRibbonWindow(object content)
    {
        this.Width = 800;
        this.Height = 600;

        this.ShowActivated = false;
        this.ShowInTaskbar = false;

        if (Debugger.IsAttached == false)
        {
            this.Left = int.MinValue;
            this.Top = int.MinValue;
        }

        // As Ribbon uses layout rounding we should use it here too
        FrameworkHelper.SetUseLayoutRounding(this, true);

        this.Content = content;

        this.Show();
    }

    public void Dispose()
    {
        this.Close();
    }
}