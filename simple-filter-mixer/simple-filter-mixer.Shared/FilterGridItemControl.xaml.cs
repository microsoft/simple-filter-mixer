using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace simple_filter_mixer
{
    public partial class FilterGridItemControl : UserControl, INotifyPropertyChanged
    {
        private static readonly double DefaultGridItemWidth = 200d;

        public Image FilterPreviewImage
        {
            get
            {
                return ItemImage;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static readonly DependencyProperty GridItemWidthProperty =
            DependencyProperty.Register(
                "GridItemWidth",
                typeof(double),
                typeof(FilterGridItemControl),
                new PropertyMetadata(DefaultGridItemWidth, new PropertyChangedCallback(OnGridItemWidthChanged)));

        public double GridItemWidth
        {
            get
            {
                return (double)GetValue(GridItemWidthProperty);
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Width must be greater than 0");
                }

                SetValue(GridItemWidthProperty, value);
            }
        }

        private static void OnGridItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("OnGridItemWidthChanged: " + e.NewValue);
            FilterGridItemControl control = d as FilterGridItemControl;
            control.OnPropertyChanged("GridItemWidth");
            control.LayoutRoot.Width = (double)e.NewValue;
        }

        public FilterGridItemControl()
        {
            this.InitializeComponent();

            if (App.DisplayRatio > 1.7)
            {
                GridItemWidth = 245;
            }
            else
            {
                GridItemWidth = 190;
            }
        }
    }
}
