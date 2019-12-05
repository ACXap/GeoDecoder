// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace GeoCoding
{
    public class PersistGroupExpandedStateBehavior : Behavior<Expander>
    {
        #region Static Fields

        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(
            "GroupName",
            typeof(object),
            typeof(PersistGroupExpandedStateBehavior),
            new PropertyMetadata(default(object)));

        private static readonly DependencyProperty ExpandedStateStoreProperty =
            DependencyProperty.RegisterAttached(
                "ExpandedStateStore",
                typeof(IDictionary<object, bool>),
                typeof(PersistGroupExpandedStateBehavior),
                new PropertyMetadata(default(IDictionary<object, bool>)));

        #endregion

        #region Public Properties

        public object GroupName
        {
            get
            {
                return GetValue(GroupNameProperty);
            }

            set
            {
                SetValue(GroupNameProperty, value);
            }
        }

        #endregion

        #region Methods

        protected override void OnAttached()
        {
            base.OnAttached();

            bool? expanded = this.GetExpandedState();

            if (expanded != null)
            {
                this.AssociatedObject.IsExpanded = expanded.Value;
            }

            this.AssociatedObject.Expanded += this.OnExpanded;
            this.AssociatedObject.Collapsed += this.OnCollapsed;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.Expanded -= this.OnExpanded;
            this.AssociatedObject.Collapsed -= this.OnCollapsed;

            base.OnDetaching();
        }

        private ItemsControl FindItemsControl()
        {
            DependencyObject current = AssociatedObject;

            while (current != null && !(current is ItemsControl))
            {
                current = VisualTreeHelper.GetParent(current);
            }

            if (current == null)
            {
                return null;
            }

            return current as ItemsControl;
        }

        private bool? GetExpandedState()
        {
            var dict = GetExpandedStateStore();

            if (!dict.ContainsKey(GroupName))
            {
                return null;
            }

            return dict[GroupName];
        }

        private IDictionary<object, bool> GetExpandedStateStore()
        {
            ItemsControl itemsControl = FindItemsControl();

            if (itemsControl == null)
            {
                throw new Exception(
                    "Behavior needs to be attached to an Expander that is contained inside an ItemsControl");
            }

            var dict = (IDictionary<object, bool>)itemsControl.GetValue(ExpandedStateStoreProperty);

            if (dict == null)
            {
                dict = new Dictionary<object, bool>();
                itemsControl.SetValue(ExpandedStateStoreProperty, dict);
            }

            return dict;
        }

        private void OnCollapsed(object sender, RoutedEventArgs e)
        {
            this.SetExpanded(false);
        }

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            this.SetExpanded(true);
        }

        private void SetExpanded(bool expanded)
        {
            var dict = GetExpandedStateStore();

            dict[GroupName] = expanded;
        }

        #endregion
    }
}