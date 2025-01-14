﻿using PicView.ChangeImage;
using PicView.FileHandling;
using PicView.UILogic;
using PicView.Views.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static PicView.UILogic.UC;

namespace PicView.PicGallery
{
    internal static class GalleryFunctions
    {
        internal static bool IsHorizontalOpen { get; set; }
        internal static bool IsVerticalFullscreenOpen { get; set; }
        internal static bool IsHorizontalFullscreenOpen { get; set; }

        public class tempPics
        {
            internal BitmapSource pic;
            public string name;

            public tempPics(BitmapSource pic, string name)
            {
                this.pic = pic;
                this.name = name;
            }
        }

        private static IEnumerable<T> OrderBySequence<T, TId>(this IEnumerable<T> source, IEnumerable<TId> order, Func<T, TId> idSelector)
        {
            var lookup = source?.ToDictionary(idSelector, t => t);
            foreach (var id in order)
            {
                yield return lookup[id];
            }
        }

        internal static async Task SortGallery()
        {
            var pics = new System.Collections.Generic.List<tempPics>();

            await ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                for (int i = 0; i < GetPicGallery.Container.Children.Count; i++)
                {
                    var x = GetPicGallery.Container.Children[i] as PicGalleryItem;
                    pics.Add(new tempPics(x?.img?.Source as BitmapSource, Navigation.Pics[i]));
                }

                Clear();
            }));

            Navigation.Pics = FileLists.FileList();

            try
            {
                pics = pics.OrderBySequence(Navigation.Pics, pic => pic.name).ToList();
            }
            catch (Exception)
            {
                await GalleryLoad.Load().ConfigureAwait(false);
                pics.Clear();
                pics = null;
                return;
            }

            for (int i = 0; i < pics.Count; i++)
            {
                GalleryLoad.Add(pics[i].pic, i);
            }
            pics.Clear();
            pics = null;
        }

        internal static void Clear()
        {
            ConfigureWindows.GetMainWindow.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (GetPicGallery == null)
                {
                    return;
                }

                GetPicGallery.Container.Children.Clear();

#if DEBUG
                Trace.WriteLine("Cleared Gallery children");
#endif
            }));

        }
    }
}