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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailView : Page
    {

    #region Field & Object Definitions

        Uri imageUri;   // Image to view.

    #endregion

    #region Property Definitions
        float imageActualHeight { get; set; }
        float imageActualWidth { get; set; }
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
        /// Fit the image to the screen size, (Viewport).
        /// </summary>
        public void FitToScreen()
        {
            //float zoomFactor = (float)Math.Min(this.scrollViewer.ActualWidth / this.imageToShow.ActualWidth,
            //    this.scrollViewer.ActualHeight / this.imageToShow.ActualHeight);
            //scrollViewer.ChangeView(null, null, zoomFactor);

            // May not need to do this once the image is initially loaded.....
            SetScrollViewerMode(ScrollMode.Disabled, ScrollBarVisibility.Disabled);

            return;
        }

        /// <summary>
        /// Sow the image in the ScrollViewer at actual size.
        /// </summary>
        private void ShowActualSize()
        {
            //scrollViewer.ChangeView(null, null, 1);

            // Set the Zoom factor to show full size and update the view.
            float zoomFactor = (float)Math.Max(this.bitmapImage.PixelWidth / this.imageToShow.ActualWidth,
                    this.bitmapImage.PixelHeight / this.imageToShow.ActualHeight);
            scrollViewer.ChangeView(null, null, zoomFactor);

            // Set the Zoom slider to the new value.
            ZoomSlider.Value = zoomFactor;

            // Enable scrolling, (May turn scrolling on once the image has been loaded and initially shown).
            SetScrollViewerMode(ScrollMode.Enabled, ScrollBarVisibility.Auto);

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
            base.OnNavigatedTo(e);

            imageUri = (Uri)e.Parameter;    // Uri from the selected image in the GridView of the MainPage.

            ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("forwardAnimation");
            if (animation != null)
            {
                animation.TryStart(imageToShow);
            }
        }

        /// <summary>
        /// Overrides the OnNavigatingFrom event of the DetailView Page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("backAnimation", imageToShow);

                // Use the recommended configuration for back animation.
                animation.Configuration = new DirectConnectedAnimationConfiguration();
            }
        }

        /// <summary>
        /// Handles the ZoomSlider ValueChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (scrollViewer != null)
            {
                scrollViewer.ChangeView(null, null, (float)e.NewValue);
            }
        }
        
        /// <summary>
        /// Handles the ScrollViewer loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            return;
        }

        /// <summary>
        /// Handles the Slider Loaded event. 
        /// Set the initial vale for the slider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomSlider_Loaded(object sender, RoutedEventArgs e)
        {
            Slider slider = sender as Slider;
            slider.Value = 1;
        }

    #endregion

    }
}
