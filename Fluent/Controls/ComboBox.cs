﻿#region Copyright and License Information

// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license

#endregion

namespace Fluent
{
	using System;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Threading;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;
	using System.Windows.Data;
	using System.Windows.Input;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;
	using System.Windows.Threading;
	using Fluent.Internal;

    /// <summary>
	///     Represents custom Fluent UI ComboBox
	/// </summary>
	[TemplatePart(Name = "PART_ResizeBothThumb", Type = typeof(Thumb))]
	[TemplatePart(Name = "PART_ResizeVerticalThumb", Type = typeof(Thumb))]
	public class ComboBox : System.Windows.Controls.ComboBox, IQuickAccessItemProvider, IRibbonControl, IDropDownControl
	{
		#region Fields

		// Thumb to resize in both directions
		private Thumb resizeBothThumb;
		// Thumb to resize vertical
		private Thumb resizeVerticalThumb;

		private IInputElement focusedElement;

		private Panel menuPanel;

		private Border dropDownBorder;
		private Border contentBorder;

		private ContentPresenter contentSite;

		// Freezed image (created during snapping)
		private Image snappedImage;

		// Is visual currently snapped
		private bool isSnapped;

		private GalleryPanel galleryPanel;

		private ScrollViewer scrollViewer;

		private bool canSizeY;

		#endregion

		#region Properties

		#region Size

		/// <summary>
		///     Gets or sets Size for the element.
		/// </summary>
		public RibbonControlSize Size
		{
			get { return (RibbonControlSize)this.GetValue(SizeProperty); }
			set { this.SetValue(SizeProperty, value); }
		}

		/// <summary>
		///     Using a DependencyProperty as the backing store for Size.
		///     This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(ComboBox));

		#endregion

		#region SizeDefinition

		/// <summary>
		///     Gets or sets SizeDefinition for element.
		/// </summary>
		public RibbonControlSizeDefinition SizeDefinition
		{
			get { return (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty); }
			set { this.SetValue(SizeDefinitionProperty, value); }
		}

		/// <summary>
		///     Using a DependencyProperty as the backing store for SizeDefinition.
		///     This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(ComboBox));

		#endregion

		#region KeyTip

		/// <summary>
		///     Gets or sets KeyTip for element.
		/// </summary>
		public string KeyTip
		{
			get { return (string)this.GetValue(KeyTipProperty); }
			set { this.SetValue(KeyTipProperty, value); }
		}

		/// <summary>
		///     Using a DependencyProperty as the backing store for Keys.
		///     This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(ComboBox));

		#endregion

		/// <summary>
		///     Gets drop down popup
		/// </summary>
		public Popup DropDownPopup { get; private set; }

		/// <summary>
		///     Gets a value indicating whether context menu is opened
		/// </summary>
		public bool IsContextMenuOpened { get; set; }

		#region Header

		/// <summary>
		///     Gets or sets element Text
		/// </summary>
		public object Header
		{
			get { return (string)this.GetValue(HeaderProperty); }
			set { this.SetValue(HeaderProperty, value); }
		}

		/// <summary>
		///     Using a DependencyProperty as the backing store for Header.
		///     This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(ComboBox));

		#endregion

		#region Icon

		/// <summary>
		///     Gets or sets Icon for the element
		/// </summary>
		public object Icon
		{
			get { return (ImageSource)this.GetValue(IconProperty); }
			set { this.SetValue(IconProperty, value); }
		}

		/// <summary>
		///     Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(ComboBox), new UIPropertyMetadata(null, OnIconChanged));

		private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var element = d as ComboBox;
			var oldElement = e.OldValue as FrameworkElement;
			if (oldElement != null) element.RemoveLogicalChild(oldElement);
			var newElement = e.NewValue as FrameworkElement;
			if (newElement != null) element.AddLogicalChild(newElement);
		}

		#endregion

		#region Menu

		/// <summary>
		///     Gets or sets menu to show in combo box bottom
		/// </summary>
		public RibbonMenu Menu
		{
			get { return (RibbonMenu)this.GetValue(MenuProperty); }
			set { this.SetValue(MenuProperty, value); }
		}

		/// <summary>
		///     Using a DependencyProperty as the backing store for Menu.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty MenuProperty =
			DependencyProperty.Register("Menu", typeof(RibbonMenu), typeof(ComboBox), new UIPropertyMetadata(null));

		#endregion

		#region InputWidth

		/// <summary>
		///     Gets or sets width of the value input part of combobox
		/// </summary>
		public double InputWidth
		{
			get { return (double)this.GetValue(InputWidthProperty); }
			set { this.SetValue(InputWidthProperty, value); }
		}

		/// <summary>
		///     Using a DependencyProperty as the backing store for InputWidth.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty InputWidthProperty =
			DependencyProperty.Register("InputWidth", typeof(double), typeof(ComboBox), new UIPropertyMetadata(double.NaN));

		#endregion

		#region ItemHeight

		/// <summary>
		///     Gets or sets items height
		/// </summary>
		public double ItemHeight
		{
			get { return (double)this.GetValue(ItemHeightProperty); }
			set { this.SetValue(ItemHeightProperty, value); }
		}

		/// <summary>
		///     Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty ItemHeightProperty =
			DependencyProperty.Register("ItemHeight", typeof(double), typeof(ComboBox), new UIPropertyMetadata(double.NaN));

		#endregion

		#region GroupBy

		/// <summary>
		///     Gets or sets name of property which
		///     will use to group items in the ComboBox.
		/// </summary>
		public string GroupBy
		{
			get { return (string)this.GetValue(GroupByProperty); }
			set { this.SetValue(GroupByProperty, value); }
		}

		/// <summary>
		///     Using a DependencyProperty as the backing store for GroupBy.
		///     This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty GroupByProperty =
			DependencyProperty.Register("GroupBy", typeof(string),
				typeof(ComboBox), new UIPropertyMetadata(null));

		#endregion

		#region ResizeMode

		/// <summary>
		///     Gets or sets context menu resize mode
		/// </summary>
		public ContextMenuResizeMode ResizeMode
		{
			get { return (ContextMenuResizeMode)this.GetValue(ResizeModeProperty); }
			set { this.SetValue(ResizeModeProperty, value); }
		}

		/// <summary>
		///     Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty ResizeModeProperty =
			DependencyProperty.Register("ResizeMode", typeof(ContextMenuResizeMode), typeof(ComboBox), new UIPropertyMetadata(ContextMenuResizeMode.None));

		#endregion

		#region Snapping

		/// <summary>
		///     Snaps / Unsnaps the Visual
		///     (remove visuals and substitute with freezed image)
		/// </summary>
		private bool IsSnapped
		{
			get { return this.isSnapped; }
			set
			{
				if (value == this.isSnapped) return;
				if (this.snappedImage == null) return;
				if ((value) && (((int)this.contentSite.ActualWidth > 0) && ((int)this.contentSite.ActualHeight > 0)))
				{
					// Render the freezed image
					RenderOptions.SetBitmapScalingMode(this.snappedImage, BitmapScalingMode.NearestNeighbor);
					var renderTargetBitmap = new RenderTargetBitmap((int)this.contentSite.ActualWidth + (int)this.contentSite.Margin.Left + (int)this.contentSite.Margin.Right,
						(int)this.contentSite.ActualHeight + (int)this.contentSite.Margin.Top + (int)this.contentSite.Margin.Bottom, 96, 96,
						PixelFormats.Pbgra32);
					renderTargetBitmap.Render(this.contentSite);
					this.snappedImage.Source = renderTargetBitmap;
					this.snappedImage.FlowDirection = this.FlowDirection;
					/*snappedImage.Width = contentSite.ActualWidth;
                    snappedImage.Height = contentSite.ActualHeight;*/
					this.snappedImage.Visibility = Visibility.Visible;
					this.contentSite.Visibility = Visibility.Hidden;
					this.isSnapped = value;
				}
				else
				{
					this.snappedImage.Visibility = Visibility.Collapsed;
					this.contentSite.Visibility = Visibility.Visible;
					this.isSnapped = value;
				}

				this.InvalidateVisual();
			}
		}

		#endregion

		#region DropDownHeight

		/// <summary>
		///     Gets or sets initial dropdown height
		/// </summary>
		public double DropDownHeight
		{
			get { return (double)this.GetValue(DropDownHeightProperty); }
			set { this.SetValue(DropDownHeightProperty, value); }
		}

		/// <summary>
		///     /Using a DependencyProperty as the backing store for DropDownHeight.  This enables animation, styling, binding,
		///     etc...
		/// </summary>
		public static readonly DependencyProperty DropDownHeightProperty =
			DependencyProperty.Register("InitialDropDownHeight", typeof(double), typeof(ComboBox), new UIPropertyMetadata(double.NaN));

		#endregion

		#region ShowPopupOnTop

		/// <summary>
		///     Gets a value indicating whether popup is shown on top;
		/// </summary>
		public bool ShowPopupOnTop
		{
			get { return (bool)this.GetValue(ShowPopupOnTopProperty); }
			private set { this.SetValue(ShowPopupOnTopPropertyKey, value); }
		}

		// 
		private static readonly DependencyPropertyKey ShowPopupOnTopPropertyKey = DependencyProperty.RegisterReadOnly("ShowPopupOnTop", typeof(bool), typeof(ComboBox), new UIPropertyMetadata(false));

		/// <summary>
		///     Using a DependencyProperty as the backing store for ShowPopupOnTop.  This enables animation, styling, binding,
		///     etc...
		/// </summary>
		public static readonly DependencyProperty ShowPopupOnTopProperty = ShowPopupOnTopPropertyKey.DependencyProperty;

		#endregion

		#endregion

		#region Constructors

		/// <summary>
		///     Static constructor
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1810")]
		static ComboBox()
		{
			var type = typeof(ComboBox);
			ToolTipService.Attach(type);
			PopupService.Attach(type);
			ContextMenuService.Attach(type);
			DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
			SelectedItemProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(OnSelectionItemChanged, CoerceSelectedItem));
		}

		private static void OnSelectionItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var combo = d as ComboBox;
			if (!combo.isQuickAccessOpened && !combo.isQuickAccessFocused && (combo.quickAccessCombo != null)) combo.UpdateQuickAccessCombo();
		}

		private static object CoerceSelectedItem(DependencyObject d, object basevalue)
		{
			var combo = d as ComboBox;
			if (combo.isQuickAccessOpened || combo.isQuickAccessFocused) return combo.selectedItem;
			return basevalue;
		}

		/// <summary>
		///     Default Constructor
		/// </summary>
		public ComboBox()
		{
			ContextMenuService.Coerce(this);
		}

		#endregion

		#region QuickAccess

		/// <summary>
		///     Gets control which represents shortcut item.
		///     This item MUST be syncronized with the original
		///     and send command to original one control.
		/// </summary>
		/// <returns>Control which represents shortcut item</returns>
		public virtual FrameworkElement CreateQuickAccessItem()
		{
			var combo = new ComboBox();
			RibbonControl.BindQuickAccessItem(this, combo);
			RibbonControl.Bind(this, combo, "GroupBy", GroupByProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "ActualWidth", WidthProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "InputWidth", InputWidthProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "ItemHeight", ItemHeightProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "IsEditable", IsEditableProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "IsReadOnly", IsReadOnlyProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "ResizeMode", ResizeModeProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "Text", TextProperty, BindingMode.TwoWay);

			RibbonControl.Bind(this, combo, "DisplayMemberPath", DisplayMemberPathProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "GroupStyleSelector", GroupStyleSelectorProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "ItemContainerStyle", ItemContainerStyleProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "ItemsPanel", ItemsPanelProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "ItemStringFormat", ItemStringFormatProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "ItemTemplate", ItemTemplateProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "SelectedValuePath", SelectedValuePathProperty, BindingMode.OneWay);
			RibbonControl.Bind(this, combo, "MaxDropDownHeight", MaxDropDownHeightProperty, BindingMode.OneWay);
			combo.DropDownOpened += this.OnQuickAccessOpened;
			if (this.IsEditable) combo.GotFocus += this.OnQuickAccessTextBoxGetFocus;
			this.quickAccessCombo = combo;
			this.UpdateQuickAccessCombo();
			return combo;
		}

		private void OnQuickAccessTextBoxGetFocus(object sender, RoutedEventArgs e)
		{
			this.isQuickAccessFocused = true;
			if (!this.isQuickAccessOpened) this.Freeze();
			this.quickAccessCombo.LostFocus += this.OnQuickAccessTextBoxLostFocus;
		}

		private void OnQuickAccessTextBoxLostFocus(object sender, RoutedEventArgs e)
		{
			this.quickAccessCombo.LostFocus -= this.OnQuickAccessTextBoxLostFocus;
			if (!this.isQuickAccessOpened) this.Unfreeze();
			this.isQuickAccessFocused = false;
		}

		private bool isQuickAccessFocused;
		private bool isQuickAccessOpened;
		private object selectedItem;
		private ComboBox quickAccessCombo;

		private void OnQuickAccessOpened(object sender, EventArgs e)
		{
			this.isQuickAccessOpened = true;
			this.quickAccessCombo.DropDownClosed += this.OnQuickAccessMenuClosed;
			this.quickAccessCombo.UpdateLayout();
			if (!this.isQuickAccessFocused)
				this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, ((ThreadStart)(() =>
				{
					this.Freeze();
					this.Dispatcher.BeginInvoke(DispatcherPriority.Input, ((ThreadStart)(() => { if (this.quickAccessCombo.SelectedItem != null) (this.quickAccessCombo.ItemContainerGenerator.ContainerFromItem(this.quickAccessCombo.SelectedItem) as ComboBoxItem).BringIntoView(); }
						)));
				}
					)));
		}

		private void OnQuickAccessMenuClosed(object sender, EventArgs e)
		{
			this.quickAccessCombo.DropDownClosed -= this.OnQuickAccessMenuClosed;
			if (!this.isQuickAccessFocused) this.Unfreeze();
			this.isQuickAccessOpened = false;
		}

		private void Freeze()
		{
			this.IsSnapped = true;
			this.selectedItem = this.SelectedItem;

            ItemsControlHelper.MoveItemsToDifferentControl(this, this.quickAccessCombo);

            this.SelectedItem = null;
			this.quickAccessCombo.SelectedItem = this.selectedItem;
			this.quickAccessCombo.Menu = this.Menu;
			this.Menu = null;
			this.quickAccessCombo.IsSnapped = false;
		}

		private void Unfreeze()
		{
			var text = this.quickAccessCombo.Text;
			this.selectedItem = this.quickAccessCombo.SelectedItem;
			this.quickAccessCombo.IsSnapped = true;

            ItemsControlHelper.MoveItemsToDifferentControl(this.quickAccessCombo, this);

			this.quickAccessCombo.SelectedItem = null;
			this.SelectedItem = this.selectedItem;
			this.Menu = this.quickAccessCombo.Menu;
			this.quickAccessCombo.Menu = null;
			this.IsSnapped = false;
			this.Text = text;
			this.UpdateLayout();
		}

		private void UpdateQuickAccessCombo()
		{
		    if (this.IsLoaded == false)
		    {
		        this.Loaded += this.OnFirstLoaded;
		    }

		    if (this.IsEditable == false)
		    {
		        this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
		                                                                                      {
		                                                                                          this.quickAccessCombo.IsSnapped = true;
		                                                                                          this.IsSnapped = true;
		                                                                                          if (this.snappedImage != null &&
		                                                                                              this.quickAccessCombo.snappedImage != null)
		                                                                                          {
		                                                                                              this.quickAccessCombo.snappedImage.Source = this.snappedImage.Source;
		                                                                                              this.quickAccessCombo.snappedImage.Visibility = Visibility.Visible;
		                                                                                              if (this.quickAccessCombo.IsSnapped == false)
		                                                                                              {
		                                                                                                  this.quickAccessCombo.isSnapped = true;
		                                                                                              }
		                                                                                          }
		                                                                                          this.IsSnapped = false;
		                                                                                      }));
		    }
		}

		private void OnFirstLoaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= this.OnFirstLoaded;
			this.UpdateQuickAccessCombo();
		}

		/// <summary>
		///     Gets or sets whether control can be added to quick access toolbar
		/// </summary>
		public bool CanAddToQuickAccessToolBar
		{
			get { return (bool)this.GetValue(CanAddToQuickAccessToolBarProperty); }
			set { this.SetValue(CanAddToQuickAccessToolBarProperty, value); }
		}

		/// <summary>
		///     Using a DependencyProperty as the backing store for CanAddToQuickAccessToolBar.  This enables animation, styling,
		///     binding, etc...
		/// </summary>
		public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(ComboBox), new UIPropertyMetadata(true, RibbonControl.OnCanAddToQuickAccessToolbarChanged));

		#endregion

		#region Overrides

		/// <summary>
		///     When overridden in a derived class, is invoked whenever application code or internal processes call
		///     <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
		/// </summary>
		public override void OnApplyTemplate()
		{
			this.DropDownPopup = this.GetTemplateChild("PART_Popup") as Popup;

			if (this.resizeVerticalThumb != null)
			{
				this.resizeVerticalThumb.DragDelta -= this.OnResizeVerticalDelta;
			}
			this.resizeVerticalThumb = this.GetTemplateChild("PART_ResizeVerticalThumb") as Thumb;
			if (this.resizeVerticalThumb != null)
			{
				this.resizeVerticalThumb.DragDelta += this.OnResizeVerticalDelta;
			}

			if (this.resizeBothThumb != null)
			{
				this.resizeBothThumb.DragDelta -= this.OnResizeBothDelta;
			}
			this.resizeBothThumb = this.GetTemplateChild("PART_ResizeBothThumb") as Thumb;
			if (this.resizeBothThumb != null)
			{
				this.resizeBothThumb.DragDelta += this.OnResizeBothDelta;
			}

			this.menuPanel = this.GetTemplateChild("PART_MenuPanel") as Panel;

			this.snappedImage = this.GetTemplateChild("PART_SelectedImage") as Image;
			this.contentSite = this.GetTemplateChild("PART_ContentSite") as ContentPresenter;

			if (this.contentBorder != null) this.contentBorder.PreviewMouseDown -= this.OnContentBorderPreviewMouseDown;
			this.contentBorder = this.GetTemplateChild("PART_ContentBorder") as Border;
			if (this.contentBorder != null) this.contentBorder.PreviewMouseDown += this.OnContentBorderPreviewMouseDown;

			this.galleryPanel = this.GetTemplateChild("PART_GalleryPanel") as GalleryPanel;
			this.scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

			this.dropDownBorder = this.GetTemplateChild("PART_DropDownBorder") as Border;

			base.OnApplyTemplate();
		}

		/// <summary>
		///     Reports when a combo box's popup opens.
		/// </summary>
		/// <param name="e">The event data for the <see cref="E:System.Windows.Controls.ComboBox.DropDownOpened" /> event.</param>
		protected override void OnDropDownOpened(EventArgs e)
		{
			base.OnDropDownOpened(e);

            Mouse.Capture(this, CaptureMode.SubTree);

		    if (this.SelectedItem != null)
		    {
		        Keyboard.Focus(this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as IInputElement);
		    }

		    this.focusedElement = Keyboard.FocusedElement;

		    if (this.focusedElement != null)
		    {
		        this.focusedElement.LostKeyboardFocus += this.OnFocusedElementLostKeyboardFocus;
		    }

		    this.canSizeY = true;

			this.galleryPanel.Width = double.NaN;
			this.scrollViewer.Height = double.NaN;

			var popupChild = this.DropDownPopup.Child as FrameworkElement;
			var heightDelta = popupChild.DesiredSize.Height - this.scrollViewer.DesiredSize.Height;

			var initialHeight = Math.Min(RibbonControl.GetControlWorkArea(this).Height * 2 / 3, this.MaxDropDownHeight);

		    if (double.IsNaN(this.DropDownHeight) == false)
		    {
		        initialHeight = Math.Min(this.DropDownHeight, this.MaxDropDownHeight);
		    }

		    if (this.scrollViewer.DesiredSize.Height > initialHeight)
		    {
		        this.scrollViewer.Height = initialHeight;
		    }
		    else
		    {
		        initialHeight = this.scrollViewer.DesiredSize.Height;
		    }

		    var monitor = RibbonControl.GetControlMonitor(this);
			var delta = monitor.Bottom - this.PointToScreen(new Point()).Y - this.ActualHeight - initialHeight - heightDelta;

		    if (delta >= 0)
		    {
		        this.ShowPopupOnTop = false;
		    }
		    else
		    {
		        var deltaTop = this.PointToScreen(new Point()).Y - initialHeight - heightDelta - monitor.Top;

		        if (deltaTop > delta)
		        {
		            this.ShowPopupOnTop = true;
		        }
		        else
		        {
		            this.ShowPopupOnTop = false;
		        }

		        if (deltaTop < 0)
		        {
		            delta = Math.Max(Math.Abs(delta), Math.Abs(deltaTop));

		            if (delta > this.galleryPanel.GetItemSize().Height)
		            {
		                this.scrollViewer.Height = delta;
		            }
		            else
		            {
		                this.canSizeY = false;
		                this.scrollViewer.Height = this.galleryPanel.GetItemSize().Height;
		            }
		        }
		    }

		    popupChild.UpdateLayout();
		}

		/// <summary>
		///     Reports when a combo box's popup closes.
		/// </summary>
		/// <param name="e">The event data for the <see cref="E:System.Windows.Controls.ComboBox.DropDownClosed" /> event.</param>
		protected override void OnDropDownClosed(EventArgs e)
		{
			base.OnDropDownClosed(e);
			if (Mouse.Captured == this) Mouse.Capture(null);
			if (this.focusedElement != null) this.focusedElement.LostKeyboardFocus -= this.OnFocusedElementLostKeyboardFocus;
			this.focusedElement = null;
			this.ShowPopupOnTop = false;
			this.galleryPanel.Width = double.NaN;
			this.scrollViewer.Height = double.NaN;
		}

		private void OnFocusedElementLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (this.focusedElement != null) this.focusedElement.LostKeyboardFocus -= this.OnFocusedElementLostKeyboardFocus;
			this.focusedElement = Keyboard.FocusedElement;
			if (this.focusedElement != null)
			{
				this.focusedElement.LostKeyboardFocus += this.OnFocusedElementLostKeyboardFocus;
				if ((this.IsEditable) &&
				    (this.Items.Contains(this.ItemContainerGenerator.ItemFromContainer(Keyboard.FocusedElement as DependencyObject))))
				{
					this.SelectedItem = this.ItemContainerGenerator.ItemFromContainer(Keyboard.FocusedElement as DependencyObject);
				}
			}
		}

		/// <summary>
		///     Invoked when a <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown" /> attached routed event occurs.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if ((this.IsEditable) && ((e.Key == Key.Down) || (e.Key == Key.Up)) && (!this.IsDropDownOpen))
			{
				this.IsDropDownOpen = true;
				e.Handled = true;
				return;
			}

			base.OnPreviewKeyDown(e);
		}

		/// <summary>
		///     Invoked when a <see cref="E:System.Windows.Input.Keyboard.KeyDown" /> attached routed event occurs.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Down)
			{
				Debug.WriteLine("Down pressed. FocusedElement - " + Keyboard.FocusedElement);
				if ((this.Menu != null) && this.Menu.Items.Contains(this.Menu.ItemContainerGenerator.ItemFromContainer(Keyboard.FocusedElement as DependencyObject)))
				{
					var indexOfMSelectedItem = this.Menu.ItemContainerGenerator.IndexFromContainer(Keyboard.FocusedElement as DependencyObject);
					if (indexOfMSelectedItem != this.Menu.Items.Count - 1)
					{
						Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerFromIndex(indexOfMSelectedItem + 1) as IInputElement);
					}
					else
					{
						if ((this.Items.Count > 0) && (!this.IsEditable))
						{
							Keyboard.Focus(this.ItemContainerGenerator.ContainerFromIndex(0) as IInputElement);
						}
						else Keyboard.Focus(this.Menu.Items[0] as IInputElement);
					}
				}
				else if (this.Items.Contains(this.ItemContainerGenerator.ItemFromContainer(Keyboard.FocusedElement as DependencyObject)))
				{
					var indexOfSelectedItem = this.ItemContainerGenerator.IndexFromContainer(Keyboard.FocusedElement as DependencyObject);
					if (indexOfSelectedItem != this.Items.Count - 1)
					{
						Keyboard.Focus(this.ItemContainerGenerator.ContainerFromIndex(indexOfSelectedItem + 1) as IInputElement);
					}
					else
					{
						if ((this.Menu != null) && (this.Menu.Items.Count > 0) && (!this.IsEditable)) Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerFromIndex(0) as IInputElement);
						else
						{
							Keyboard.Focus(this.ItemContainerGenerator.ContainerFromIndex(0) as IInputElement);
						}
					}
				}
				else if (this.SelectedItem != null) Keyboard.Focus(this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as IInputElement);
				e.Handled = true;
				Debug.WriteLine("FocusedElement - " + Keyboard.FocusedElement);
				return;
			}
			else if (e.Key == Key.Up)
			{
				Debug.WriteLine("Up pressed. FocusedElement - " + Keyboard.FocusedElement);
				if ((this.Menu != null) && this.Menu.Items.Contains(this.Menu.ItemContainerGenerator.ItemFromContainer(Keyboard.FocusedElement as DependencyObject)))
				{
					var indexOfMSelectedItem = this.Menu.ItemContainerGenerator.IndexFromContainer(Keyboard.FocusedElement as DependencyObject);
					if (indexOfMSelectedItem != 0)
					{
						Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerFromIndex(indexOfMSelectedItem - 1) as IInputElement);
					}
					else
					{
						if ((this.Items.Count > 0) && (!this.IsEditable))
						{
							Keyboard.Focus(this.ItemContainerGenerator.ContainerFromIndex(this.Items.Count - 1) as IInputElement);
						}
						else Keyboard.Focus(this.Menu.Items[this.Menu.Items.Count - 1] as IInputElement);
					}
				}
				else if (this.Items.Contains(this.ItemContainerGenerator.ItemFromContainer(Keyboard.FocusedElement as DependencyObject)))
				{
					var indexOfSelectedItem = this.ItemContainerGenerator.IndexFromContainer(Keyboard.FocusedElement as DependencyObject);
					if (indexOfSelectedItem != 0)
					{
						Keyboard.Focus(this.ItemContainerGenerator.ContainerFromIndex(indexOfSelectedItem - 1) as IInputElement);
					}
					else
					{
						if ((this.Menu != null) && (this.Menu.Items.Count > 0) && (!this.IsEditable)) Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerFromIndex(this.Menu.Items.Count - 1) as IInputElement);
						else
						{
							Keyboard.Focus(this.ItemContainerGenerator.ContainerFromIndex(this.Items.Count - 1) as IInputElement);
						}
					}
				}
				else if (this.SelectedItem != null) Keyboard.Focus(this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as IInputElement);
				Debug.WriteLine("FocusedElement - " + Keyboard.FocusedElement);
				e.Handled = true;
				return;
			}
			else if ((e.Key == Key.Return) && (!this.IsEditable) && this.IsDropDownOpen)
			{
				var element = Keyboard.FocusedElement as DependencyObject;

				// only try to select if we got a focusedElement
				if (element != null)
				{
					var newSelectedIndex = this.ItemContainerGenerator.IndexFromContainer(element);

					// only set the selected index if the focused element was in a container in this combobox
					if (newSelectedIndex > -1)
					{
						this.SelectedIndex = newSelectedIndex;
					}
				}
			}
			base.OnKeyDown(e);
		}

		#endregion

		#region Methods

		/// <summary>
		///     Handles key tip pressed
		/// </summary>
		public virtual void OnKeyTipPressed()
		{
			this.Dispatcher.BeginInvoke(
				DispatcherPriority.Normal,
				(DispatcherOperationCallback)delegate(object arg)
				{
					var ctrl = (ComboBox)arg;

					// Edge case: Whole dropdown content is disabled
					if (ctrl.IsKeyboardFocusWithin == false)
					{
						Keyboard.Focus(ctrl);
					}
					return null;
				},
				this);

			if (!this.IsEditable)
			{
				this.IsDropDownOpen = true;
			}
		}

		/// <summary>
		///     Handles back navigation with KeyTips
		/// </summary>
		public void OnKeyTipBack()
		{
		}

		#endregion

		#region Private methods

		// Prevent reopenning of the dropdown menu (popup)
		private void OnContentBorderPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.IsDropDownOpen)
			{
				this.IsDropDownOpen = false;
				e.Handled = true;
			}
		}

		// Handles resize both drag
		private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
		{
			// Set height
			this.SetDragHeight(e);

			// Set width
			this.menuPanel.Width = double.NaN;
			if (double.IsNaN(this.galleryPanel.Width))
			{
				this.galleryPanel.Width = this.galleryPanel.ActualWidth;
			}

			var monitorRight = RibbonControl.GetControlMonitor(this).Right;
			var popupChild = this.DropDownPopup.Child as FrameworkElement;
			var delta = monitorRight - this.PointToScreen(new Point()).X - popupChild.ActualWidth - e.HorizontalChange;
			var deltaX = popupChild.ActualWidth - this.galleryPanel.ActualWidth;
			var deltaBorders = this.dropDownBorder.ActualWidth - this.galleryPanel.ActualWidth;

			if (delta > 0)
			{
				this.galleryPanel.Width = Math.Max(0, Math.Max(this.galleryPanel.Width + e.HorizontalChange, this.ActualWidth - deltaBorders));
			}
			else
			{
				this.galleryPanel.Width = Math.Max(0, Math.Max(monitorRight - this.PointToScreen(new Point()).X - deltaX, this.ActualWidth - deltaBorders));
			}
		}

		// Handles resize vertical drag
		private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
		{
			this.SetDragHeight(e);
		}

		private void SetDragHeight(DragDeltaEventArgs e)
		{
			if (!this.canSizeY)
			{
				return;
			}

			if (double.IsNaN(this.scrollViewer.Height))
			{
				this.scrollViewer.Height = this.scrollViewer.ActualHeight;
			}

			if (this.ShowPopupOnTop)
			{
				var monitorTop = RibbonControl.GetControlMonitor(this).Top;

				// Calc shadow height
				var delta = this.PointToScreen(new Point()).Y - this.dropDownBorder.ActualHeight - e.VerticalChange - monitorTop;
				if (delta > 0)
				{
					this.scrollViewer.Height = Math.Max(0,
						Math.Min(Math.Max(this.galleryPanel.GetItemSize().Height, this.scrollViewer.Height + e.VerticalChange), this.MaxDropDownHeight));
				}
				else
				{
					delta = this.PointToScreen(new Point()).Y - this.dropDownBorder.ActualHeight - monitorTop;
					this.scrollViewer.Height = Math.Max(0,
						Math.Min(Math.Max(this.galleryPanel.GetItemSize().Height, this.scrollViewer.Height + delta), this.MaxDropDownHeight));
				}
			}
			else
			{
				var monitorBottom = RibbonControl.GetControlMonitor(this).Bottom;
				var popupChild = this.DropDownPopup.Child as FrameworkElement;
				var delta = monitorBottom - this.PointToScreen(new Point()).Y - this.ActualHeight - popupChild.ActualHeight - e.VerticalChange;
				if (delta > 0)
				{
					this.scrollViewer.Height = Math.Max(0,
						Math.Min(Math.Max(this.galleryPanel.GetItemSize().Height, this.scrollViewer.Height + e.VerticalChange), this.MaxDropDownHeight));
				}
				else
				{
					delta = monitorBottom - this.PointToScreen(new Point()).Y - this.ActualHeight - popupChild.ActualHeight;
					this.scrollViewer.Height = Math.Max(0,
						Math.Min(Math.Max(this.galleryPanel.GetItemSize().Height, this.scrollViewer.Height + delta), this.MaxDropDownHeight));
				}
			}
		}

		#endregion
	}
}