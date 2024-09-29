// ReSharper disable once CheckNamespace
namespace Fluent;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Fluent.Internal.KnownBoxes;

/// <summary>
/// Enables transitons for content changes.
/// </summary>
[TemplatePart(Name = PreviousContentPartName, Type = typeof(ContentPresenter))]
[TemplatePart(Name = CurrentContentPartName, Type = typeof(ContentPresenter))]
public class TransitioningControl : Control
{
    private ContentPresenter? previousContentPresenter;
    private ContentPresenter? currentContentPresenter;
    private Storyboard? currentStoryBoard;

    /// <summary>
    /// The part name for the content presenter represeting the previous/old content.
    /// </summary>
    public const string PreviousContentPartName = "PART_PreviousContent";

    /// <summary>
    /// The part name for the content presenter represeting the new/current content.
    /// </summary>
    public const string CurrentContentPartName = "PART_CurrentContent";

    static TransitioningControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitioningControl), new FrameworkPropertyMetadata(typeof(TransitioningControl)));

        IsTabStopProperty.OverrideMetadata(typeof(TransitioningControl), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
        FocusableProperty.OverrideMetadata(typeof(TransitioningControl), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
    }

    /// <summary>Identifies the <see cref="NextContent"/> dependency property.</summary>
    public static readonly DependencyProperty NextContentProperty = DependencyProperty.Register(nameof(NextContent), typeof(object), typeof(TransitioningControl), new PropertyMetadata(default, OnNextContentChanged));

    /// <summary>
    /// The next content, which will be the current content.
    /// </summary>
    public object? NextContent
    {
        get => this.GetValue(NextContentProperty);
        set => this.SetValue(NextContentProperty, value);
    }

    private static void OnNextContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TransitioningControl)d;

        if (ReferenceEquals(e.OldValue, e.NewValue) is false)
        {
            control.StartTransition(e.OldValue, e.NewValue);
        }
    }

    /// <summary>Identifies the <see cref="TransitionStoryboard"/> dependency property.</summary>
    public static readonly DependencyProperty TransitionStoryboardProperty = DependencyProperty.Register(nameof(TransitionStoryboard), typeof(Storyboard), typeof(TransitioningControl), new PropertyMetadata(default(Storyboard), OnTransitionStoryboardChanged));

    /// <summary>
    /// The <see cref="Storyboard"/> being used to animate transitions.
    /// </summary>
    public Storyboard? TransitionStoryboard
    {
        get => (Storyboard?)this.GetValue(TransitionStoryboardProperty);
        set => this.SetValue(TransitionStoryboardProperty, value);
    }

    private static void OnTransitionStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (TransitioningControl)d;

        if (control.currentStoryBoard?.IsFrozen is false)
        {
            control.currentStoryBoard.Completed -= control.TransitionStoryboardCompleted;
        }

        if (e.NewValue is Storyboard newValue)
        {
            var wasFrozen = newValue.IsFrozen;
            control.currentStoryBoard = newValue.IsFrozen ? newValue.Clone() : newValue;
            control.currentStoryBoard.Completed += control.TransitionStoryboardCompleted;

            if (wasFrozen
                && control.currentStoryBoard.CanFreeze)
            {
                control.currentStoryBoard.Freeze();
            }
        }
        else
        {
            control.currentStoryBoard = null;
            control.previousContentPresenter?.SetCurrentValue(ContentPresenter.ContentProperty, null);
        }
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this.previousContentPresenter = this.GetTemplateChild(PreviousContentPartName) as ContentPresenter;
        this.currentContentPresenter = this.GetTemplateChild(CurrentContentPartName) as ContentPresenter;

        if (this.NextContent is not null
            && this.currentContentPresenter is not null)
        {
            this.currentContentPresenter.SetCurrentValue(ContentPresenter.ContentProperty, this.NextContent);
        }
    }

    /// <summary>
    /// Starts the transtion from old to new content.
    /// </summary>
#pragma warning disable WPF0005
    protected virtual void StartTransition(object? oldContent, object? newContent)
#pragma warning restore WPF0005
    {
        if (this.currentContentPresenter is null)
        {
            return;
        }

        this.StopTransition();

        if (this.currentStoryBoard is not null)
        {
            this.previousContentPresenter?.SetCurrentValue(ContentPresenter.ContentProperty, oldContent);
        }

        this.currentContentPresenter.SetCurrentValue(ContentPresenter.ContentProperty, newContent);

        if (oldContent is not null)
        {
            this.currentStoryBoard?.Begin(this, this.Template);
        }
    }

    /// <summary>
    /// Stops the current transition and finally removes the old content.
    /// </summary>
    protected virtual void StopTransition()
    {
        this.previousContentPresenter?.SetCurrentValue(ContentPresenter.ContentProperty, null);
        //this.currentStoryBoard?.Stop(this);
    }

    private void TransitionStoryboardCompleted(object? sender, EventArgs e)
    {
        this.StopTransition();
    }
}