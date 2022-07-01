using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Items;
using JumpPoint.Platform;
using JumpPoint.ViewModels;
using NittyGritty.Utilities;
using JumpPoint.ViewModels.Helpers;
using JumpPoint.Platform.Items.Arrangers.Sorters;
using System.Collections.Specialized;
using JumpPoint.Platform.Items.Arrangers.Groupers;
using System.Linq;
using NittyGritty.Uwp.Helpers;
using Windows.UI.Xaml.Controls.Primitives;
using System.ComponentModel;
using NittyGritty.Collections;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace JumpPoint.Uwp.Controls
{
    public sealed partial class JumpPointViewer : UserControl, INotifyPropertyChanged
    {
        private readonly object syncObject;

        public event PropertyChangedEventHandler PropertyChanged;

        public JumpPointViewer()
        {
            this.InitializeComponent();

            syncObject = new object();
        }


        private void OnIconLoaded(object sender, RoutedEventArgs args)
        {
            if(sender is UIElement element)
            {
                var elementVisual = ElementCompositionPreview.GetElementVisual(element);
                var compositor = elementVisual.Compositor;

                // Create animation to scale up the rectangle
                var pointerEnteredAnimation = compositor.CreateVector3KeyFrameAnimation();
                pointerEnteredAnimation.InsertKeyFrame(1.0f, new Vector3(1.2f));

                // Create animation to scale the rectangle back down
                var pointerExitedAnimation = compositor.CreateVector3KeyFrameAnimation();
                pointerExitedAnimation.InsertKeyFrame(1.0f, new Vector3(1.0f));

                var root = VisualTreeHelper.GetParent(element) as UIElement;
                EventUtilities.RegisterEvent<UIElement, PointerEventHandler, PointerRoutedEventArgs>(
                    root,
                    h => root.PointerEntered += h,
                    h => root.PointerEntered -= h,
                    h => (o, e) => h(o, e),
                    (subscriber, s, e) =>
                    {
                        elementVisual.CenterPoint = new Vector3(elementVisual.Size / 2, 0);
                        elementVisual.StartAnimation("Scale", pointerEnteredAnimation);
                    });

                EventUtilities.RegisterEvent<UIElement, PointerEventHandler, PointerRoutedEventArgs>(
                    root,
                    h => root.PointerExited += h,
                    h => root.PointerExited -= h,
                    h => (o, e) => h(o, e),
                    (subscriber, s, e) =>
                    {
                        elementVisual.StartAnimation("Scale", pointerExitedAnimation);
                    });
            }
        }


        private void SelectionHandler(NotificationMessage message)
        {
            if (message.Notification == nameof(CommandHelper.SelectAllCommand))
            {
                allView.SelectAll();
            }
            else if (message.Notification == nameof(CommandHelper.InvertSelectionCommand))
            {
                var children = XamlHelper.FindChildren<SelectorItem>(allView);
                foreach (var item in children)
                {
                    item.IsSelected = !item.IsSelected;
                }
            }
        }

        private void ManageCommands(NotificationMessage message)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Context)));
        }


        public ShellContextViewModelBase Context
        {
            get { return (ShellContextViewModelBase)GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Context.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.Register("Context", typeof(ShellContextViewModelBase), typeof(JumpPointViewer), new PropertyMetadata(null, OnContextChanged));

        private static void OnContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = (JumpPointViewer)d;
            if (e.OldValue is ShellContextViewModelBase oldContext)
            {
                Messenger.Default.Unregister<NotificationMessage>(viewer, $"{MessengerTokens.JumpPointViewerSelection}_{oldContext.TabKey}", viewer.SelectionHandler);
                Messenger.Default.Unregister<NotificationMessage>(viewer, MessengerTokens.CommandManagement, viewer.ManageCommands);
            }

            if (e.NewValue is ShellContextViewModelBase newContext)
            {
                Messenger.Default.Register<NotificationMessage>(viewer, $"{MessengerTokens.JumpPointViewerSelection}_{newContext.TabKey}", viewer.SelectionHandler);
                Messenger.Default.Register<NotificationMessage>(viewer, MessengerTokens.CommandManagement, viewer.ManageCommands);
            }
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(JumpPointViewer), new PropertyMetadata(null));



        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(JumpPointViewer), new PropertyMetadata(null));



        public DynamicCollection<JumpPointItem> ItemsSource
        {
            get { return (DynamicCollection<JumpPointItem>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(DynamicCollection<JumpPointItem>), typeof(JumpPointViewer), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is JumpPointViewer viewer)
            {
                viewer.UpdateItemsSource(e.OldValue as DynamicCollection<JumpPointItem>, e.NewValue as DynamicCollection<JumpPointItem>);
            }
        }

        private void UpdateItemsSource(DynamicCollection<JumpPointItem> oldCollection, DynamicCollection<JumpPointItem> newCollection)
        {
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= ItemsSource_CollectionChanged;
            }

            if (newCollection != null)
            {
                ArrangeItems();
                newCollection.CollectionChanged += ItemsSource_CollectionChanged;
            }
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ArrangeItems();
        }

        private void ArrangeItems()
        {
            lock (syncObject)
            {
                if (ItemsSource is null) return;

                var sorter = Sorter.GetSorter(SortBy);
                var sortedItems = sorter.Sort(ItemsSource, IsSortAscending);
                var grouper = Grouper.GetGrouper(GroupBy, CustomGrouper);
                if (grouper != null)
                {
                    var groupedItems = grouper.Group(sortedItems, IsGroupAscending);
                    ItemsViewSource.IsSourceGrouped = true;
                    ItemsViewSource.ItemsPath = new PropertyPath("Items");
                    ItemsViewSource.Source = groupedItems.ToList();
                    semanticZoom.IsZoomedInViewActive = true;
                    semanticZoom.CanChangeViews = true;
                }
                else
                {
                    ItemsViewSource.IsSourceGrouped = false;
                    ItemsViewSource.ItemsPath = new PropertyPath(string.Empty);
                    ItemsViewSource.Source = sortedItems.ToList();
                    semanticZoom.IsZoomedInViewActive = true;
                    semanticZoom.CanChangeViews = false;
                }
            }
        }



        public SortBy SortBy
        {
            get { return (SortBy)GetValue(SortByProperty); }
            set { SetValue(SortByProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortBy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortByProperty =
            DependencyProperty.Register("SortBy", typeof(SortBy), typeof(JumpPointViewer), new PropertyMetadata(SortBy.Name, OnSortByChanged));

        private static void OnSortByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is JumpPointViewer viewer && (SortBy)e.OldValue != (SortBy)e.NewValue)
            {
                viewer.ArrangeItems();
            }
        }



        public bool IsSortAscending
        {
            get { return (bool)GetValue(IsSortAscendingProperty); }
            set { SetValue(IsSortAscendingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSortAscending.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSortAscendingProperty =
            DependencyProperty.Register("IsSortAscending", typeof(bool), typeof(JumpPointViewer), new PropertyMetadata(true, OnIsSortAscendingChanged));

        private static void OnIsSortAscendingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is JumpPointViewer viewer && (bool)e.OldValue != (bool)e.NewValue)
            {
                viewer.ArrangeItems();
            }
        }



        public GroupBy GroupBy
        {
            get { return (GroupBy)GetValue(GroupByProperty); }
            set { SetValue(GroupByProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupBy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupByProperty =
            DependencyProperty.Register("GroupBy", typeof(GroupBy), typeof(JumpPointViewer), new PropertyMetadata(GroupBy.None, OnGroupByChanged));

        private static void OnGroupByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is JumpPointViewer viewer && (GroupBy)e.OldValue != (GroupBy)e.NewValue)
            {
                viewer.ArrangeItems();
            }
        }



        public bool IsGroupAscending
        {
            get { return (bool)GetValue(IsGroupAscendingProperty); }
            set { SetValue(IsGroupAscendingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsGroupAscending.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsGroupAscendingProperty =
            DependencyProperty.Register("IsGroupAscending", typeof(bool), typeof(JumpPointViewer), new PropertyMetadata(true, OnIsGroupAscendingChanged));

        private static void OnIsGroupAscendingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is JumpPointViewer viewer && (bool)e.OldValue != (bool)e.NewValue)
            {
                viewer.ArrangeItems();
            }
        }



        public Grouper CustomGrouper
        {
            get { return (Grouper)GetValue(CustomGrouperProperty); }
            set { SetValue(CustomGrouperProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomGrouper.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomGrouperProperty =
            DependencyProperty.Register("CustomGrouper", typeof(Grouper), typeof(JumpPointViewer), new PropertyMetadata(null));



        public ObservableCollection<JumpPointItem> SelectedItems
        {
            get { return (ObservableCollection<JumpPointItem>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<JumpPointItem>), typeof(JumpPointViewer), new PropertyMetadata(null));

        private Visibility GetDetailsHeaderVisibility(string layout)
        {
            return layout == LayoutModes.Details ?
                Visibility.Visible : Visibility.Collapsed;
        }

        private void OnCheckboxUnchecked(object sender, RoutedEventArgs e)
        {
            if (App.Current.Resources.TryGetValue("SelectNoneCommand", out var resource) && resource is XamlUICommand command)
            {
                command.Command.Execute(null);
            }
        }

        private void OnCheckboxChecked(object sender, RoutedEventArgs e)
        {
            if (App.Current.Resources.TryGetValue("SelectAllCommand", out var resource) && resource is XamlUICommand command)
            {
                command.Command.Execute(null);
            }
        }

        private void OnItemOpened(object sender, RoutedEventArgs e)
        {
            if (App.Current.Resources.TryGetValue("OpenItemCommand", out var resource) &&
                resource is XamlUICommand command &&
                sender is FrameworkElement element &&
                element.DataContext is JumpPointItem item)
            {
                command.Command.Execute(new OpenItemParameter(ViewModelLocator.Instance.GetNavigationHelper(Context.TabKey), item));
            }
        }
    }

    public class EqualityTrigger : StateTriggerBase
    {

        public object LeftValue
        {
            get { return (object)GetValue(LeftValueProperty); }
            set { SetValue(LeftValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftValueProperty =
            DependencyProperty.Register("LeftValue", typeof(object), typeof(EqualityTrigger), new PropertyMetadata(null, OnValueChanged));



        public object RightValue
        {
            get { return (object)GetValue(RightValueProperty); }
            set { SetValue(RightValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightValueProperty =
            DependencyProperty.Register("RightValue", typeof(object), typeof(EqualityTrigger), new PropertyMetadata(null, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EqualityTrigger trigger)
            {
                trigger.UpdateTrigger();
            }
        }

        private void UpdateTrigger()
        {
            SetActive(EqualityTrigger.AreValuesEqual(LeftValue, RightValue, true));
        }

        internal static bool AreValuesEqual(object value1, object value2, bool convertType)
        {
            if (value1 == value2)
            {
                return true;
            }

            if (value1 != null && value2 != null && convertType)
            {
                // Try the conversion in both ways:
                return ConvertTypeEquals(value1, value2) || ConvertTypeEquals(value2, value1);
            }

            return false;
        }

        private static bool ConvertTypeEquals(object value1, object value2)
        {
            // Let's see if we can convert:
            if (value2 is Enum)
            {
                value1 = ConvertToEnum(value2.GetType(), value1);
            }
            else
            {
                value1 = Convert.ChangeType(value1, value2.GetType(), CultureInfo.InvariantCulture);
            }

            return value2.Equals(value1);
        }

        private static object ConvertToEnum(Type enumType, object value)
        {
            try
            {
                return Enum.IsDefined(enumType, value) ? Enum.ToObject(enumType, value) : null;
            }
            catch
            {
                return null;
            }
        }
    }

}
