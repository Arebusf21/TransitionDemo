using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Timers;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TransitionDemo
{
    /// <summary>
    /// Based on an empty page that can be used on its own or navigated to within a Frame.
    /// There appears to be a bug in the ScrollViewer control, the ZoomFactor property is not been updated ater a call to ChangeView()
    /// To work around this, we are keeping track of the zoomFactor in a local field. At least this is the case when an image is enclosed in 
    /// a ScrollViewer. The ScrollViewer control wants to display the image based on the width. It tries to fit the image width to the viewport and 
    /// scrolls the length if needed, it also sets the ScrollFactor to 1. While this is OK for an image where the width is greater than the length, it 
    /// doesn't work for a long image, (where the length is longer than the width).
    /// Using the code below, when trying to show the image full size, it will still scroll the width of the image even though there is room 
    /// to fit int in. Frustrating....
    /// Also, when zooming an image, right now you will have no scrolling unless its turned on for zooming. We need to be able to turn zooming on right
    /// after launch. We have provided a fix of sorts but its very much a hack. It looks as though we have a working solution anyway.
    /// </summary>
    public sealed partial class DetailView : Page
    {

    #region Field & Object Definitions
        private Uri imageUri;   // Image to view.
        private float zoomFactor;
        private bool scrollingTurnedOn;
    #endregion

    #region Property Definitions
    #endregion

    #region Methods

        public DetailView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Called by the event handler for the NavigationView.BackRequested event.
        /// </summary>
        /// <returns></returns>
        private bool On_BackRequested()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (!rootFrame.CanGoBack)
                return false;

            rootFrame.GoBack(new SuppressNavigationTransitionInfo());     // Go back to the MainPage.

            return true;
        }

        /// <summary>
        /// Sets the ScrollViewer mode and visibility.
        /// Not used at present, for future use.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="visibility"></param>
        private void SetScrollViewerMode(ScrollMode mode, ScrollBarVisibility visibility)
        {
            if (this.scrollViewer != null)
            {
                this.scrollViewer.HorizontalScrollMode = mode;
                this.scrollViewer.HorizontalScrollBarVisibility = visibility;
                this.scrollViewer.VerticalScrollMode = mode;
                this.scrollViewer.VerticalScrollBarVisibility = visibility;
            }
            return;
        }

        /// <summary>
        /// Sets the ScrollViewer ScrollBar visibility.
        /// </summary>
        /// <param name="visibility"></param>
        private void SetScrollViewerVisibility(ScrollBarVisibility visibility)
        {
            if (this.scrollViewer != null)
            {
                this.scrollViewer.HorizontalScrollBarVisibility = visibility;   
                this.scrollViewer.VerticalScrollBarVisibility = visibility;
            }
            return;
        }

        /// <summary>
        /// Determine the ZoomFactor to fit the image to the ScrollViewer viewport.
        /// not used at present.
        /// </summary>
        /// <returns></returns>
        private float GetZoomFactorToFit()
        {
            return (float)Math.Min(this.scrollViewer.ActualWidth / this.imageToShow.ActualWidth,
                         this.scrollViewer.ActualHeight / this.imageToShow.ActualHeight);
        }

        /// <summary>
        /// Determine the Zoom factor to show the image full size in the ScrollViewer.
        /// </summary>
        /// <returns></returns>
        private float GetZoomFactorForFullSize()
        {
            // Get the Zoom factor to show the image full size.
            return (float)Math.Max(this.bitmapImage.PixelWidth / this.imageToShow.ActualWidth,
                    this.bitmapImage.PixelHeight / this.imageToShow.ActualHeight);
        }


        /// <summary>
        /// Fit the image to the screen size, (Viewport).
        /// </summary>
        public void FitToScreen()
        {
            // Get & save vthe zoom factor to fit the image to the screen and update the view.           
            this.ZoomSlider.Value = this.zoomFactor = GetZoomFactorToFit();
            this.scrollViewer.ChangeView(null, null, this.zoomFactor);

            return;
        }

        /// <summary>
        /// Show the image in the ScrollViewer at its actual size.
        /// </summary>
        private void ShowActualSize()
        {
            // Enable scrolling.
            if (!this.scrollingTurnedOn)
                SetScrollViewerVisibility(ScrollBarVisibility.Auto);

            // Set the Zoom factor to show full size and update the view.
            // Note that changing the scrollbar visibility will regenerate the image and show it full size even without
            // calling ChangeView. I am not sure this is the correct behaviour but for now we will work with it.
            // Anyway, this seems to work ok.
            this.ZoomSlider.Value = this.zoomFactor = GetZoomFactorForFullSize();
            this.scrollViewer.ChangeView(null, null, this.zoomFactor);

            return;
        }

    #endregion

    #region Event Handlers

        /// <summary>
        /// Handles the BackButton click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        /// <summary>
        /// Overrides the OnNavigatedTo event of the DetailView Page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);  // Not sure if this should be called first or last?

            this.imageUri = (Uri)e.Parameter;    // Uri from the selected image in the GridView of the MainPage.

            // Start the animation
            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("forwardAnimation");
            if (animation != null)
            {
                animation.TryStart(this.imageToShow);
            }
        }

        /// <summary>
        /// Overrides the OnNavigatingFrom event of the DetailView Page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // initialize the back navigation.
            if (e.NavigationMode == NavigationMode.Back)
            {
                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("backAnimation", this.imageToShow);

                // Use the recommended configuration for back animation.
                animation.Configuration = new DirectConnectedAnimationConfiguration();
            }
        }

        /// <summary>
        /// Handles the ZoomSlider ValueChanged event. Some of the code below is in response to the behaviour of the Zoom control.
        /// Clicking on the Zoom control, (AppBar Button) causes the Slider ValueChanged event to be fired. this not desirable
        /// straight after launch as we have just shown an image to fit inside the viewport. Turning Scrolling on will resize the image
        /// before we are ready for it. We compare the stored zoom factor to the chaned event parameter to see if its changed. 
        /// It will only be changed if we have moved the slider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (this.scrollViewer != null)
            {
                if (e.NewValue != this.zoomFactor)
                {
                    this.scrollViewer.ChangeView(null, null, (float)e.NewValue);
                    this.zoomFactor = (float)e.NewValue;    //Save the new zoom factor.   
                    if (!this.scrollingTurnedOn)
                    {
                        SetScrollViewerVisibility(ScrollBarVisibility.Auto);
                        this.scrollingTurnedOn = true;
                    }
                }
                
            }
        }
        
        /// <summary>
        /// Handles the ScrollViewer loaded event.
        /// Not used at present.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the initial value for the zoomFactor, will be set to 1, although in my view it should not be.
            this.zoomFactor = GetZoomFactorToFit();
            
            return;
        }

        /// <summary>
        /// Handles the Slider Loaded event. 
        /// Set the initial value for the slider.
        /// Every time the Zoom button is selected, the Slider loaded event is raised.
        /// So we need to initialize it to the current zoom factor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomSlider_Loaded(object sender, RoutedEventArgs e)
        {
            Slider slider = sender as Slider;
            slider.Value = this.zoomFactor;
        }

        #endregion
    }
}
