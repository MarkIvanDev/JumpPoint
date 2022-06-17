using NittyGritty.Uwp.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using MUXC = Microsoft.UI.Xaml.Controls;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace JumpPoint.Uwp.Controls
{
    public sealed class PanedTabView2 : Control
    {
        public PanedTabView2()
        {
            this.DefaultStyleKey = typeof(PanedTabView2);
        }



        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(PanedTabView2), new PropertyMetadata(null));


        public object LeftHeader
        {
            get { return (object)GetValue(LeftHeaderProperty); }
            set { SetValue(LeftHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftHeaderProperty =
            DependencyProperty.Register("LeftHeader", typeof(object), typeof(PanedTabView2), new PropertyMetadata(null));



        public DataTemplate LeftHeaderTemplate
        {
            get { return (DataTemplate)GetValue(LeftHeaderTemplateProperty); }
            set { SetValue(LeftHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftHeaderTemplateProperty =
            DependencyProperty.Register("LeftHeaderTemplate", typeof(DataTemplate), typeof(PanedTabView2), new PropertyMetadata(null));



        public object RightHeader
        {
            get { return (object)GetValue(RightHeaderProperty); }
            set { SetValue(RightHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightHeaderProperty =
            DependencyProperty.Register("RightHeader", typeof(object), typeof(PanedTabView2), new PropertyMetadata(null));



        public DataTemplate RightHeaderTemplate
        {
            get { return (DataTemplate)GetValue(RightHeaderTemplateProperty); }
            set { SetValue(RightHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightHeaderTemplateProperty =
            DependencyProperty.Register("RightHeaderTemplate", typeof(DataTemplate), typeof(PanedTabView2), new PropertyMetadata(null));



        public object TabStripContent
        {
            get { return (object)GetValue(TabStripContentProperty); }
            set { SetValue(TabStripContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabStripContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabStripContentProperty =
            DependencyProperty.Register("TabStripContent", typeof(object), typeof(PanedTabView2), new PropertyMetadata(null));



        public DataTemplate TabStripContentTemplate
        {
            get { return (DataTemplate)GetValue(TabStripContentTemplateProperty); }
            set { SetValue(TabStripContentTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabStripContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabStripContentTemplateProperty =
            DependencyProperty.Register("TabStripContentTemplate", typeof(DataTemplate), typeof(PanedTabView2), new PropertyMetadata(null));



        public object TabItemsSource
        {
            get { return (object)GetValue(TabItemsSourceProperty); }
            set { SetValue(TabItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabItemsSourceProperty =
            DependencyProperty.Register("TabItemsSource", typeof(object), typeof(PanedTabView2), new PropertyMetadata(null, OnTabItemsSourceChanged));

        private static void OnTabItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PanedTabView2 panedTabView)
            {
                panedTabView.UpdateTabItemsSource(e.OldValue as INotifyCollectionChanged, e.NewValue as INotifyCollectionChanged);
            }
        }

        private void UpdateTabItemsSource(INotifyCollectionChanged oldItems, INotifyCollectionChanged newItems)
        {
            if (oldItems != null)
            {
                oldItems.CollectionChanged -= TabItems_CollectionChanged;
            }

            if (newItems != null)
            {
                newItems.CollectionChanged += TabItems_CollectionChanged;
            }
        }

        private void TabItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateTabColumnWidth();
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(PanedTabView2), new PropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PanedTabView2 control && control.TabHeaderList != null)
            {
                control.TabHeaderList.SelectedItem = control.SelectedItem;
            }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(PanedTabView2), new PropertyMetadata(-1, OnSelectedIndexChanged));

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PanedTabView2 control && control.TabHeaderList != null)
            {
                var oldValue = (int)e.OldValue;
                var newValue = (int)e.NewValue;
                var count = control.TabHeaderList.Items.Count;
                if (oldValue != newValue && newValue == -1 && count > 0)
                {
                    if (oldValue >= 0 && oldValue < count)
                    {
                        // If old index is still available, select that
                        control.TabHeaderList.SelectedItem =  control.TabHeaderList.Items.ElementAt(oldValue);
                    }
                    else
                    {
                        // If old index is not available, select the last tab
                        control.TabHeaderList.SelectedItem = control.TabHeaderList.Items.ElementAt(count - 1);
                    }
                }
            }
        }

        public DataTemplate TabHeaderTemplate
        {
            get { return (DataTemplate)GetValue(TabHeaderTemplateProperty); }
            set { SetValue(TabHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabHeaderTemplateProperty =
            DependencyProperty.Register("TabHeaderTemplate", typeof(DataTemplate), typeof(PanedTabView2), new PropertyMetadata(null));



        public DataTemplate TabItemTemplate
        {
            get { return (DataTemplate)GetValue(TabItemTemplateProperty); }
            set { SetValue(TabItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TabItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabItemTemplateProperty =
            DependencyProperty.Register("TabItemTemplate", typeof(DataTemplate), typeof(PanedTabView2), new PropertyMetadata(null));



        public bool IsPaneOpen
        {
            get { return (bool)GetValue(IsPaneOpenProperty); }
            set { SetValue(IsPaneOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPaneOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPaneOpenProperty =
            DependencyProperty.Register("IsPaneOpen", typeof(bool), typeof(PanedTabView2), new PropertyMetadata(false));



        public object MenuItemsSource
        {
            get { return (object)GetValue(MenuItemsSourceProperty); }
            set { SetValue(MenuItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MenuItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuItemsSourceProperty =
            DependencyProperty.Register("MenuItemsSource", typeof(object), typeof(PanedTabView2), new PropertyMetadata(null));



        public DataTemplateSelector MenuItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(MenuItemTemplateSelectorProperty); }
            set { SetValue(MenuItemTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MenuItemTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuItemTemplateSelectorProperty =
            DependencyProperty.Register("MenuItemTemplateSelector", typeof(DataTemplateSelector), typeof(PanedTabView2), new PropertyMetadata(null));



        public object FooterMenuItemsSource
        {
            get { return (object)GetValue(FooterMenuItemsSourceProperty); }
            set { SetValue(FooterMenuItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FooterMenuItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FooterMenuItemsSourceProperty =
            DependencyProperty.Register("FooterMenuItemsSource", typeof(object), typeof(PanedTabView2), new PropertyMetadata(null));



        public event TypedEventHandler<MUXC.NavigationView, MUXC.NavigationViewItemInvokedEventArgs> ItemInvoked;
        private ListView TabHeaderList;
        private Grid TabHeaderGrid;
        private ColumnDefinition LeftHeaderColumn;
        private ColumnDefinition TabHeaderColumn;
        private ColumnDefinition RightHeaderColumn;
        private ContentPresenter RightHeaderPresenter;
        private ItemsPresenter TabsItemsPresenter;
        private ScrollViewer TabsScrollViewer;
        private Size previousAvailableSize;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("navView") is MUXC.NavigationView navView)
            {
                navView.ItemInvoked += (s, e) => ItemInvoked?.Invoke(s, e);
            }

            if (GetTemplateChild(nameof(TabHeaderList)) is ListView tabHeaderList)
            {
                TabHeaderList = tabHeaderList;
                TabHeaderList.Loaded += TabHeaderList_Loaded;
                TabHeaderList.SelectionChanged += TabHeaderList_SelectionChanged;
            }

            TabHeaderGrid = GetTemplateChild(nameof(TabHeaderGrid)) as Grid;
            if (TabHeaderGrid != null)
            {
                TabHeaderGrid.SizeChanged += TabHeaderGrid_SizeChanged;
            }

            LeftHeaderColumn = GetTemplateChild(nameof(LeftHeaderColumn)) as ColumnDefinition;
            TabHeaderColumn = GetTemplateChild(nameof(TabHeaderColumn)) as ColumnDefinition;
            RightHeaderColumn = GetTemplateChild(nameof(RightHeaderColumn)) as ColumnDefinition;
            RightHeaderPresenter = GetTemplateChild(nameof(RightHeaderPresenter)) as ContentPresenter;
        }

        private void TabHeaderGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTabColumnWidth();
        }

        private void TabHeaderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabHeaderList != null)
            {
                SelectedItem = TabHeaderList.SelectedItem;
                SelectedIndex = TabHeaderList.SelectedIndex;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (previousAvailableSize.Width != availableSize.Width)
            {
                previousAvailableSize = availableSize;
                UpdateTabColumnWidth();
            }

            return base.MeasureOverride(availableSize);
        }

        public void UpdateTabColumnWidth()
        {
            if (TabHeaderGrid != null)
            {
                var widthTaken = 0.0;
                if (LeftHeaderColumn != null)
                {
                    widthTaken += LeftHeaderColumn.ActualWidth;
                }

                if (RightHeaderColumn != null)
                {
                    if (RightHeaderPresenter != null)
                    {
                        var rightContentSize = RightHeaderPresenter.DesiredSize;
                        RightHeaderColumn.MinWidth = rightContentSize.Width;
                        widthTaken += rightContentSize.Width;
                    }
                }

                if (TabHeaderColumn != null)
                {
                    var availableWidth = previousAvailableSize.Width - widthTaken;
                    if (availableWidth > 0)
                    {
                        var requiredWidth = TabHeaderList.Items.Count * 180;
                        if (requiredWidth > availableWidth)
                        {
                            TabHeaderColumn.Width = new GridLength(availableWidth);
                            if (TabsScrollViewer != null)
                            {
                                TabsScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                            }
                        }
                        else
                        {
                            TabHeaderColumn.Width = GridLength.Auto;
                            if (TabsScrollViewer != null)
                            {
                                TabsScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                            }
                        }

                        if (TabHeaderList != null && TabHeaderList.SelectedItem != null && TabsScrollViewer != null)
                        {
                            var itemContainer = TabHeaderList.ContainerFromItem(TabHeaderList.SelectedItem) as FrameworkElement;
                            if (itemContainer == null || !XamlHelper.Contains(TabsScrollViewer, itemContainer))
                            {
                                TabHeaderList.ScrollIntoView(TabHeaderList.SelectedItem, ScrollIntoViewAlignment.Leading);
                            }
                        }
                    }
                }
            }
        }

        private void TabHeaderList_Loaded(object sender, RoutedEventArgs e)
        {
            TabHeaderList.SelectedItem = SelectedItem;

            TabsItemsPresenter = XamlHelper.FindChildren<ItemsPresenter>(TabHeaderList).FirstOrDefault();
            if (TabsItemsPresenter != null)
            {
                TabsItemsPresenter.SizeChanged += TabsItemsPresenter_SizeChanged;
            }

            TabsScrollViewer = XamlHelper.FindChildren<ScrollViewer>(TabHeaderList).FirstOrDefault();
            if (TabsScrollViewer != null)
            {
                TabsScrollViewer.Loaded += TabsScrollViewer_Loaded;
            }

            UpdateTabColumnWidth();
        }

        private void TabsScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTabColumnWidth();
        }

        private void TabsItemsPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTabColumnWidth();
        }
    }
}
