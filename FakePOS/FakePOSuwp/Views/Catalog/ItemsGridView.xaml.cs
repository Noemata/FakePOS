using System;
using System.Linq;
using System.Threading.Tasks;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
#endif

using Microsoft.Toolkit.Mvvm.DependencyInjection;
using FakePOS.ViewModels;

namespace FakePOS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemsGridView : UserControl
    {
        private ExpressionAnimation _expression = null;

        public ItemsGridView()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
            Loaded += OnLoaded;
        }

        internal ItemsGridViewModel ViewModel = Ioc.Default.GetService<ItemsGridViewModel>();

        private ExpressionAnimation Expression => _expression ?? (_expression = CreateExpression());

        public void Initialize()
        {
            ViewModel.ItemsControl = gridView;
            ViewModel.BarItemsControl = target;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("ItemSelectedBack");
            if (animation != null)
            {
                int id = ViewModel.State.SelectedItemId;
                if (id > 0)
                {
                    if (ViewModel.Items != null)
                    {
                        var item = ViewModel.Items.Where(r => r.Id == id).FirstOrDefault();
                        if (item != null)
                        {
                            gridView.ScrollIntoView(item);
                            await Task.Delay(10);
                            await gridView.TryStartConnectedAnimationAsync(animation, item, "container");
                            return;
                        }
                        ViewModel.State.SelectedItemId = 0;
                    }
                }
                animation.Cancel();
            }
        }

        private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (!args.InRecycleQueue)
            {
                var container = args.ItemContainer;
                var containerVisual = ElementCompositionPreview.GetElementVisual(container);

                var item = container.GetChildOfType<Border>();
                var itemVisual = ElementCompositionPreview.GetElementVisual(item);

                Expression.SetReferenceParameter("container", containerVisual);
                itemVisual.StartAnimation("Offset", Expression);
            }
        }

        private ExpressionAnimation CreateExpression()
        {
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            var scrollViewer = gridView.GetChildOfType<ScrollViewer>();
            var scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

            var _expression = compositor.CreateExpressionAnimation("Vector3(0, (-scroller.Translation.Y - container.Offset.Y + 300) * 0.05, 0)");
            _expression.SetReferenceParameter("scroller", scrollerPropertySet);
            return _expression;
        }
    }
}
