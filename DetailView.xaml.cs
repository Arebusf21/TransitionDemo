using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using muxc=Microsoft.UI.Xaml.Controls;
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
        /// Fit the image to the screen size, (Viewport).
        /// </summary>
        public void FitToScreen()
        {
            float zoomFactor = (float)Math.Min(this.scrollViewer.ActualWidth / this.imageToShow.ActualWidth,
                this.scrollViewer.ActualHeight / this.imageToShow.ActualHeight);
            scrollViewer.ChangeView(null, null, zoomFactor);
            return;
        }

        /// <summary>
        /// Sow the image in the ScrollViewer at actual size.
        /// </summary>
        private void ShowActualSize()
        {
            scrollViewer.ChangeView(null, null, 1);
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
            FitToScreen();
            return;
        }

        #endregion
    }
}
