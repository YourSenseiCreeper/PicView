﻿using PicView.ChangeImage;
using PicView.ImageHandling;
using PicView.UILogic;
using PicView.UILogic.Sizing;
using PicView.Views.UserControls;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static PicView.UILogic.HideInterfaceLogic;

namespace PicView.PicGallery
{
    internal static class GalleryLoad
    {
        internal static void PicGallery_Loaded(object sender, RoutedEventArgs e)
        {
            // Add events and set fields, when it's loaded.
            UC.GetPicGallery.Scroller.PreviewMouseWheel += GalleryNavigation.ScrollTo;
            UC.GetPicGallery.Scroller.ScrollChanged += (s, x) => ConfigureWindows.GetMainWindow.Focus(); // Maintain window focus when scrolling manually
            UC.GetPicGallery.grid.MouseLeftButtonDown += (s, x) => ConfigureWindows.GetMainWindow.Focus();
            UC.GetPicGallery.x2.MouseLeftButtonDown += delegate { GalleryToggle.CloseHorizontalGallery(); };
        }

        internal static void LoadLayout(bool fullscreen)
        {
            if (UC.GetPicGallery == null)
            {
                UC.GetPicGallery = new Views.UserControls.PicGallery
                {
                    Opacity = 0
                };

                ConfigureWindows.GetMainWindow.ParentContainer.Children.Add(UC.GetPicGallery);
                Panel.SetZIndex(UC.GetPicGallery, 999);
            }

            if (fullscreen)
            {
                if (Properties.Settings.Default.FullscreenGalleryHorizontal)
                {
                    // Set size
                    GalleryNavigation.SetSize(22);
                    UC.GetPicGallery.Width = WindowSizing.MonitorInfo.WorkArea.Width;
                    UC.GetPicGallery.Height = (GalleryNavigation.PicGalleryItem_Size + 25) * WindowSizing.MonitorInfo.DpiScaling;

                    // Set alignment
                    UC.GetPicGallery.HorizontalAlignment = HorizontalAlignment.Center;
                    UC.GetPicGallery.VerticalAlignment = VerticalAlignment.Bottom;

                    // Set scrollbar visibility and orientation
                    UC.GetPicGallery.Scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    UC.GetPicGallery.Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    UC.GetPicGallery.Container.Orientation = Orientation.Horizontal;

                    // Set style
                    UC.GetPicGallery.Margin = new Thickness(0, 0, 0, 10);
                    UC.GetPicGallery.border.BorderThickness = new Thickness(0, 0, 0, 0);
                    UC.GetPicGallery.border.Background = new SolidColorBrush(Colors.Transparent);
                    UC.GetPicGallery.Container.Margin = new Thickness(0, 0, 0, 5);

                    // Make sure bools are correct
                    GalleryFunctions.IsHorizontalOpen = false;
                    GalleryFunctions.IsHorizontalFullscreenOpen = true;
                    GalleryFunctions.IsVerticalFullscreenOpen = false;
                }
                else
                {
                    // Set size
                    GalleryNavigation.SetSize(18);
                    UC.GetPicGallery.Width = (GalleryNavigation.PicGalleryItem_Size + 25) * WindowSizing.MonitorInfo.DpiScaling;
                    UC.GetPicGallery.Height = WindowSizing.MonitorInfo.WorkArea.Height;

                    // Set alignment
                    UC.GetPicGallery.HorizontalAlignment = HorizontalAlignment.Right;
                    UC.GetPicGallery.VerticalAlignment = VerticalAlignment.Stretch;

                    // Set scrollbar visibility and orientation
                    UC.GetPicGallery.Scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    UC.GetPicGallery.Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    UC.GetPicGallery.Container.Orientation = Orientation.Vertical;

                    // Set style
                    UC.GetPicGallery.Margin = new Thickness(0, 0, 0, 0);
                    UC.GetPicGallery.border.BorderThickness = new Thickness(1, 0, 0, 0);
                    if (Properties.Settings.Default.DarkTheme)
                    {
                        UC.GetPicGallery.border.Background = (SolidColorBrush)Application.Current.Resources["BackgroundColorBrushFade"];
                    }
                    else
                    {
                        UC.GetPicGallery.border.Background = new SolidColorBrush(Colors.Transparent);
                    }

                    UC.GetPicGallery.Container.Margin = new Thickness(0, 0, 0, 0);

                    // Make sure bools are correct
                    GalleryFunctions.IsHorizontalOpen = false;
                    GalleryFunctions.IsHorizontalFullscreenOpen = false;
                    GalleryFunctions.IsVerticalFullscreenOpen = true;
                }

                ConfigureWindows.GetMainWindow.SizeToContent = SizeToContent.WidthAndHeight;
                ConfigureWindows.GetMainWindow.ResizeMode = ResizeMode.CanMinimize;


                UC.GetPicGallery.x2.Visibility = Visibility.Collapsed;
                UC.GetPicGallery.Container.Margin = new Thickness(0, 0, 0, 0);

                ShowNavigation(false);
                ShowTopandBottom(false);
                ConfigureSettings.ConfigColors.UpdateColor(true);
                ConfigureWindows.GetMainWindow.Focus();
            }
            else
            {
                GalleryNavigation.SetSize(17);

                if (UC.GetPicGallery.Container.Children.Count > 0)
                {
                    for (int i = 0; i < UC.GetPicGallery.Container.Children.Count; i++)
                    {
                        var item = (PicGalleryItem)UC.GetPicGallery.Container.Children[i];
                        item.outterborder.Width = item.outterborder.Height = GalleryNavigation.PicGalleryItem_Size;
                        item.innerborder.Width = item.innerborder.Height = GalleryNavigation.PicGalleryItem_Size_s;
                    }
                }

                // Set size
                if (Properties.Settings.Default.Fullscreen)
                {
                    UC.GetPicGallery.Width = WindowSizing.MonitorInfo.Width;
                    UC.GetPicGallery.Height = WindowSizing.MonitorInfo.Height;
                }
                else
                {
                    UC.GetPicGallery.Width = ConfigureWindows.GetMainWindow.ParentContainer.ActualWidth;
                    UC.GetPicGallery.Height = ConfigureWindows.GetMainWindow.ParentContainer.ActualHeight;
                }

                // Set alignment
                UC.GetPicGallery.HorizontalAlignment = HorizontalAlignment.Stretch;
                UC.GetPicGallery.VerticalAlignment = VerticalAlignment.Stretch;

                // Set scrollbar visibility and orientation
                UC.GetPicGallery.Scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                UC.GetPicGallery.Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                UC.GetPicGallery.Container.Orientation = Orientation.Vertical;

                // Set style
                UC.GetPicGallery.x2.Visibility = Visibility.Visible;
                UC.GetPicGallery.Container.Margin = new Thickness(0, 65 * WindowSizing.MonitorInfo.DpiScaling, 0, 0);
                UC.GetPicGallery.border.BorderThickness = new Thickness(1, 0, 0, 1);
                UC.GetPicGallery.border.Background = (SolidColorBrush)Application.Current.Resources["BackgroundColorBrushFade"];


                // Make sure bools are correct
                GalleryFunctions.IsHorizontalOpen = true;
                GalleryFunctions.IsHorizontalFullscreenOpen = false;
                GalleryFunctions.IsVerticalFullscreenOpen = false;
            }


            if (UC.GetPicGallery.Container.Children.Count > 0)
            {
                var tempItem = (PicGalleryItem)UC.GetPicGallery.Container.Children[0];
                if (tempItem.innerborder.Width != GalleryNavigation.PicGalleryItem_Size)
                {
                    for (int i = 0; i < UC.GetPicGallery.Container.Children.Count; i++)
                    {
                        var item = (PicGalleryItem)UC.GetPicGallery.Container.Children[i];
                        item.outterborder.Width = item.outterborder.Height = GalleryNavigation.PicGalleryItem_Size;
                        item.innerborder.Width = item.innerborder.Height = GalleryNavigation.PicGalleryItem_Size_s;
                    }
                }
            }
        }

        internal static Task Load() => Task.Run(async () =>
        {
            /// TODO Maybe make this start at at folder index
            /// and get it work with a real sorting method?
            /// 

            if (UC.GetPicGallery is null) { return; }

            var count = Navigation.Pics.Count;
            var index = Navigation.FolderIndex;

            if (count >= 250)
            {
                for (int i = 0; i < count; i++)
                {
                    if (count != Navigation.Pics.Count)
                    {
                        await ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                        {
                            UC.GetPicGallery.Container.Children.Clear();
                            await Load().ConfigureAwait(false); // restart when changing directory
                        }));
                        return;
                    }

                    ConfigureWindows.GetMainWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        bool selected = i == index;
                        UC.GetPicGallery.Container.Children.Add(new PicGalleryItem(null, i, selected));
                        if (selected)
                        {
                            GalleryNavigation.SelectedGalleryItem = i;
                            Timers.PicGalleryTimerHack();
                        }
                    }));
                }
                Parallel.For(0, count, i =>
                {
                    UpdatePic(i);
                });
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (count != Navigation.Pics.Count)
                    {
                        await ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                        {
                            UC.GetPicGallery.Container.Children.Clear();
                            await Load().ConfigureAwait(false);
                        }));
                        return;
                    }

                    Add(i);
                }
            }
        });

        internal static void Add(int i)
        {
            var pic = Thumbnails.GetBitmapSourceThumb(new System.IO.FileInfo(Navigation.Pics[i]));

            if (pic == null)
            {
                pic = ImageFunctions.ImageErrorMessage();
            }

            AddUI(pic, i);
        }

        internal static void Add(BitmapSource? pic, int i)
        {
            pic = Thumbnails.GetBitmapSourceThumb(new System.IO.FileInfo(Navigation.Pics[i]));

            if (pic == null)
            {
                pic = ImageFunctions.ImageErrorMessage();
            }

            AddUI(pic, i);
        }

        internal static void AddUI(BitmapSource? pic, int i)
        {
            pic = Thumbnails.GetBitmapSourceThumb(new System.IO.FileInfo(Navigation.Pics[i]));

            if (pic == null)
            {
                pic = ImageFunctions.ImageErrorMessage();
            }

            ConfigureWindows.GetMainWindow.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                var selected = i == Navigation.FolderIndex;
                var item = new PicGalleryItem(pic, i, selected);
                item.MouseLeftButtonDown += async delegate
                {
                    await GalleryClick.ClickAsync(i).ConfigureAwait(false);
                };
                UC.GetPicGallery.Container.Children.Add(item);
            }));
        }

        internal static void UpdatePic(int i)
        {
            if (ChangeImage.Navigation.Pics?.Count < ChangeImage.Navigation.FolderIndex || ChangeImage.Navigation.Pics?.Count < 1)
            {
                GalleryFunctions.Clear();
                _ = Load().ConfigureAwait(false); // restart when changing directory
                return;
            }

            var pic = Thumbnails.GetBitmapSourceThumb(new System.IO.FileInfo(Navigation.Pics[i]));
            if (pic == null)
            {
                pic = ImageFunctions.ImageErrorMessage();
            }
            ConfigureWindows.GetMainWindow.Dispatcher.Invoke(DispatcherPriority.Background, new Action(async () =>
            {
                if (ChangeImage.Navigation.Pics?.Count < ChangeImage.Navigation.FolderIndex || ChangeImage.Navigation.Pics?.Count < 1 || i >= UC.GetPicGallery.Container.Children.Count)
                {
                    GalleryFunctions.Clear();
                    await Load().ConfigureAwait(false); // restart when changing directory
                    return;
                }
                var item = (PicGalleryItem)UC.GetPicGallery.Container.Children[i];
                item.img.Source = pic;
                item.MouseLeftButtonDown += async delegate
                {
                    await GalleryClick.ClickAsync(i).ConfigureAwait(false);
                };
            }));
        }
    }
}