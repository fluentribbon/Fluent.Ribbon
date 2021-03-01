// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Fluent.Helpers;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents a ribbon slider
    /// </summary>
    //[TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class RibbonSlider : Slider, IQuickAccessItemProvider, IRibbonControl
    {
        private static readonly Type controlType = typeof(RibbonSlider);

        static RibbonSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(controlType, new FrameworkPropertyMetadata(controlType));
        }

        /// <inheritdoc />
        public FrameworkElement CreateQuickAccessItem()
        {
            var control = new RibbonSlider();
            this.BindQuickAccessItem(control);
            return control;
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected virtual void BindQuickAccessItem(FrameworkElement element)
        {
            RibbonControl.BindQuickAccessItem(this, element);
            RibbonControl.Bind(this, element, MinimumProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, MaximumProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, ValueProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, DelayProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, OrientationProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, IsDirectionReversedProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, IntervalProperty, BindingMode.TwoWay);

            RibbonControl.Bind(this, element, TickFrequencyProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, TicksProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, IsSelectionRangeEnabledProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, SelectionStartProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, SelectionEndProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, IsMoveToPointEnabledProperty, BindingMode.TwoWay);
        }

        /// <inheritdoc />
        public bool CanAddToQuickAccessToolBar
        {
            get => (bool)this.GetValue(CanAddToQuickAccessToolBarProperty);
            set => this.SetValue(CanAddToQuickAccessToolBarProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="CanAddToQuickAccessToolBar"/> dependency property.</summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(controlType, new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolBarChanged));

        /// <inheritdoc />
        public object? Header
        {
            get => this.GetValue(HeaderProperty);
            set => this.SetValue(HeaderProperty, value);
        }

        /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(controlType, new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        /// <inheritdoc />
        public string? KeyTip
        {
            get => (string?)this.GetValue(KeyTipProperty);
            set => this.SetValue(KeyTipProperty, value);
        }

        /// <summary>Identifies the <see cref="KeyTip"/> dependency property.</summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(controlType);

        /// <inheritdoc />
        public KeyTipPressedResult OnKeyTipPressed()
        {
            this.Focus();
            return new KeyTipPressedResult(true, false);
        }

        /// <inheritdoc />
        public void OnKeyTipBack()
        {
        }

        /// <inheritdoc />
        void ILogicalChildSupport.AddLogicalChild(object child)
        {
            this.AddLogicalChild(child);
        }

        /// <inheritdoc />
        void ILogicalChildSupport.RemoveLogicalChild(object child)
        {
            this.RemoveLogicalChild(child);
        }

        /// <inheritdoc />
        public RibbonControlSize Size
        {
            get => (RibbonControlSize)this.GetValue(SizeProperty);
            set => this.SetValue(SizeProperty, value);
        }

        /// <summary>Identifies the <see cref="Size"/> dependency property.</summary>
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(controlType);

        /// <inheritdoc />
        public RibbonControlSizeDefinition SizeDefinition
        {
            get => (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty);
            set => this.SetValue(SizeDefinitionProperty, value);
        }

        /// <summary>Identifies the <see cref="SizeDefinition"/> dependency property.</summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(controlType);

        /// <inheritdoc />
        public object? Icon
        {
            get => this.GetValue(IconProperty);
            set => this.SetValue(IconProperty, value);
        }

        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(controlType, new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));
    }
}