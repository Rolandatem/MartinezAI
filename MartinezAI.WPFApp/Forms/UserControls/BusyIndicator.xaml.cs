using System.Windows;
using System.Windows.Controls;

namespace MartinezAI.WPFApp.Forms.UserControls;

/// <summary>
/// Interaction logic for BusyIndicator.xaml
/// </summary>
public partial class BusyIndicator : UserControl
{
	#region "Member Variables"
	CancellationTokenSource? _delayCancellationTokenSource = null;
    #endregion

    #region "Constructor"
    public BusyIndicator()
    {
        InitializeComponent();
    }
    #endregion

    #region "Dependency Properties"
    public static readonly DependencyProperty IsBusyProperty =
    DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(BusyIndicator),
        new PropertyMetadata(false, OnIsBusyChanged));

    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public static readonly DependencyProperty IsOverlayVisibleProperty =
        DependencyProperty.Register(nameof(IsOverlayVisible), typeof(bool), typeof(BusyIndicator),
            new PropertyMetadata(false));

    public bool IsOverlayVisible
    {
        get => (bool)GetValue(IsOverlayVisibleProperty);
        set => SetValue(IsOverlayVisibleProperty, value);
    }
    #endregion

    #region "Events"
    private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((BusyIndicator)d).OnIsBusyChangedAsync((bool)e.NewValue);
    }
    #endregion

    #region "Private Methods"
    private async void OnIsBusyChangedAsync(bool isBusy)
    {
        _delayCancellationTokenSource?.Cancel();
        _delayCancellationTokenSource = null;

        if (isBusy)
        {
            _delayCancellationTokenSource = new CancellationTokenSource();
            try
            {
                await Task.Delay(500, _delayCancellationTokenSource.Token);
                if (_delayCancellationTokenSource.IsCancellationRequested == false &&
                    this.IsBusy)
                {
                    this.IsOverlayVisible = true;
                }
            }
            catch (TaskCanceledException) { }
        }
        else
        {
            this.IsOverlayVisible = false;
        }
    }
    #endregion
}
