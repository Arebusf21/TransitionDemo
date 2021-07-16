using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

/// <summary>
/// Used to demo a technique for displaying a photo or other image from a GridView to a image view.
/// Demonstrates Connected Animation from a GridView to an image and back.
/// Design aims:
/// Initial view of image in detail view to be scaled to the ScrollViewer viewport size.
/// Images where the height is greater than the width to be scaled to the ScrollViewer without scroll bars.
/// Inages to be resized when the Page size is changed by the user.
/// Showing the image at full size to show scroll bars if bigger than the ScrollViewer viewport.
/// Zoom control to change size of image, updated to show correct scaling when changing image size to full size and back.
/// Initial size of image in the ScrollViewer to be set at Zoom factor 1.
/// The solution appears to be the set the initial state of the ScrollViewer scrollbars as disabled and invisible.
/// 
/// Based on: https://docs.microsoft.com/en-us/windows/apps/design/motion/connected-animation
/// 
/// </summary>
namespace TransitionDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        #region Field & Object Definitions
        GridViewItem gridViewItem = null;

        #endregion

        #region Property Definitions
        #endregion

        #region Methods
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new GridViewModel();
        }
        #endregion

        #region Event Handlers  

        /// <summary>
        /// Overrides the OnNavigatedTo Page event. Not used.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Handles the GridView loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GridViewDemo_Loaded(object sender, RoutedEventArgs e)
        {

            // handle the back animation if returning from the detail view.
            GridViewItem item = this.gridViewItem; // Get persisted item
            if (item != null)
            {
                GridViewDemo.ScrollIntoView(item);
                ConnectedAnimation animation =
                    ConnectedAnimationService.GetForCurrentView().GetAnimation("backAnimation");
                if (animation != null)
                {
                    await GridViewDemo.TryStartConnectedAnimationAsync(animation, item, "imageView");
                }
            }
            return;
        }

        /// <summary>
        /// Handles the GridView DoubleTapped event. We will use the double tap to simulate a double click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewDemo_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Save the selected item.
            this.gridViewItem = (GridViewItem)GridViewDemo.SelectedItem;

            // Prepare the animation
            GridViewDemo.PrepareConnectedAnimation("forwardAnimation", this.gridViewItem, "imageView");


            // Pass the image uri to the DetailView page.
            Uri item = this.gridViewItem.ImageUri;
            this.Frame.Navigate(typeof(DetailView), item, new SuppressNavigationTransitionInfo());
            return;
        }

        #endregion

    }

    /// <summary>
    /// GridViewItem class. Represents an Item in the GridView.
    /// </summary>
    class GridViewItem
    {
        public Uri ImageUri { get; set; }       // Uri if the image.
        public string Txt { get; set; }         // Text id.

        /// <summary>
        /// Create a GridViewItem based on a Uri and id.
        /// </summary>
        /// <param name="imageUri"></param>
        /// <param name="str"></param>
        public GridViewItem(Uri imageUri, string str)
        {
            ImageUri = imageUri;
            Txt = str;
        }

        /// <summary>
        /// Default creator.
        /// </summary>
        public GridViewItem() { return; }
    }

    /// <summary>
    /// GridViewModel class. Represents the collection of items in the GridView 
    /// </summary>
    class GridViewModel
    {
        public List<GridViewItem> Items { get; set; }

        /// <summary>
        /// Note that this will only work as long as the images are added to the project. Build action set to content.
        /// </summary>
        public GridViewModel()
        {
            //Dummy Data
            Items = new List<GridViewItem>();
            Items.Add(new GridViewItem() { ImageUri = new Uri("ms-appx:///Assets/19.JPG"), Txt = "1" });
            Items.Add(new GridViewItem() { ImageUri = new Uri("ms-appx:///Assets/21.JPG"), Txt = "2" });
        }
    }
}
