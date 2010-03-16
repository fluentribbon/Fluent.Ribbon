#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;

namespace Fluent
{
    /// <summary>
    /// Represents button control that allows 
    /// you to add menu and handle clicks
    /// </summary>
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    public class SplitButton:DropDownButton
    {
        #region Fields

        // Inner button
        Button button;
        // QAT clone
        SplitButton quickAccessButton;

        #endregion

        #region Properties

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                if (Items != null) list.AddRange(Items);
                if (button != null) list.Add(button);
                return list.GetEnumerator();
            }
        }
        
        #endregion

        #region Events

        // TODO: use base Click, RaiseClick and so on

        /// <summary>
        /// Occurs when user clicks
        /// </summary>
        public new event RoutedEventHandler Click;

        #endregion

        #region Constructors

        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
            FocusVisualStyleProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(null));            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SplitButton()
        {

        }

        #endregion

        #region Overrides

        void OnClick(object sender, RoutedEventArgs e)
        {            
            e.Handled = true;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked 
        /// whenever application code or internal processes call ApplyTemplate
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (button != null) button.Click -= OnButtonClick;
            button = GetTemplateChild("PART_Button") as Button;
            if(button!=null)
            {
                Binding binding = new Binding("Command");
                binding.Source = this;
                binding.Mode = BindingMode.OneTime;
                button.SetBinding(CommandProperty, binding);

                binding = new Binding("CommandTarget");
                binding.Source = this;
                binding.Mode = BindingMode.OneTime;
                button.SetBinding(CommandTargetProperty, binding);
                
                binding = new Binding("CommandParameter");
                binding.Source = this;
                binding.Mode = BindingMode.OneTime;
                button.SetBinding(CommandParameterProperty, binding);
                button.Click += OnButtonClick;
            }
        }

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (Click != null) Click(this, e);
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown routed event 
        /// reaches an element in its route that is derived from this class. Implement this method to add 
        /// class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {            
            if(button==null) base.OnMouseLeftButtonDown(e);
            else
            {
                Point position = e.GetPosition(button);
                if (((position.X >= 0.0) && (position.X <= button.ActualWidth)) &&
                    ((position.Y >= 0.0) && (position.Y <= button.ActualHeight))) e.Handled = true;
                else base.OnMouseLeftButtonDown(e);
            }
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override FrameworkElement CreateQuickAccessItem()
        {
            SplitButton splitButton = new SplitButton();
            splitButton.Loaded += OnQuickAccessButtonLoaded;
            
            BindQuickAccessItem(splitButton);
            splitButton.Opened += OnQuickAccessClick;
            return splitButton;
        }

        void OnQuickAccessButtonLoaded(object sender, RoutedEventArgs e)
        {
            SplitButton splitButton = (SplitButton)sender;
            if (splitButton.button != null)
            {
                splitButton.Loaded -= OnQuickAccessButtonLoaded;
                splitButton.button.CanAddToQuickAccessToolBar = false;
            }
        }

        void OnQuickAccessClick(object sender, EventArgs e)
        {
            SplitButton splitButton = (SplitButton)sender;
            for (int i = 0; i < Items.Count; i++)
            {
                UIElement item = Items[0];
                Items.Remove(item);
                splitButton.Items.Add(item);
                i--;
            }
            splitButton.Closed += OnQuickAccessMenuClosed;
            quickAccessButton = splitButton;
        }

        void OnQuickAccessMenuClosed(object sender, EventArgs e)
        {
            quickAccessButton.Closed -= OnQuickAccessMenuClosed;
            for (int i = 0; i < quickAccessButton.Items.Count; i++)
            {
                UIElement item = quickAccessButton.Items[0];
                quickAccessButton.Items.Remove(item);
                Items.Add(item);
                i--;
            }
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            SplitButton splitButton = (SplitButton)element;
            Bind(this, splitButton, "ResizeMode", ResizeModeProperty, BindingMode.Default);
            splitButton.Click += delegate(object sender, RoutedEventArgs e) { e.Handled = true; if(Click!=null)Click(this,e); };
            base.BindQuickAccessItem(element);
        }

        #endregion
    }
}
