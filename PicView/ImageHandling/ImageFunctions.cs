﻿using ImageMagick;
using PicView.ChangeImage;
using PicView.UILogic;
using PicView.UILogic.Sizing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicView.ImageHandling
{
    internal static class ImageFunctions
    {
        internal static async Task<bool> SetRating(ushort rating) => await Task.Run(() =>
        {
            if (Error_Handling.CheckOutOfRange())
            {
                return false;
            }

            try
            {
                using (MagickImage image = new MagickImage(Navigation.Pics[Navigation.FolderIndex]))
                {
                    var profile = new ExifProfile();
                    profile.SetValue(ExifTag.Rating, rating);

                    image.SetProfile(profile);

                    image.Write(Navigation.Pics[Navigation.FolderIndex]);
                }
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        });


        internal static async Task OptimizeImageAsyncWithErrorChecking()
        {
            if (Error_Handling.CheckOutOfRange())
            {
                return;
            }
            Tooltip.ShowTooltipMessage(Application.Current.Resources["Applying"] as string, true);
            var success = await OptimizeImageAsync(Navigation.Pics[Navigation.FolderIndex]).ConfigureAwait(false);
            if (success)
            {
                await ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, () =>
                {
                    var width = ConfigureWindows.GetMainWindow.MainImage.Source.Width;
                    var height = ConfigureWindows.GetMainWindow.MainImage.Source.Height;

                    SetTitle.SetTitleString((int)width, (int)height, ChangeImage.Navigation.FolderIndex, null);
                    Tooltip.CloseToolTipMessage();
                });
            }
            else
            {
                Tooltip.ShowTooltipMessage(Application.Current.Resources["UnexpectedError"] as string, true);
            }
        }

        internal static async Task<bool> OptimizeImageAsync(string file) => await Task.Run(() =>
        {
            switch (Path.GetExtension(file).ToUpperInvariant())
            {
                case ".JPG":
                case ".JPEG":
                case ".PNG":
                case ".ICO":
                    break;
                default: return false;
            }
            ImageOptimizer imageOptimizer = new()
            {
                OptimalCompression = true
            };
            return imageOptimizer.LosslessCompress(file);
        });

        internal static RenderTargetBitmap ImageErrorMessage()
        {
            var w = ScaleImage.XWidth != 0 ? ScaleImage.XWidth : 300 * WindowSizing.MonitorInfo.DpiScaling;
            var h = ScaleImage.XHeight != 0 ? ScaleImage.XHeight : 300 * WindowSizing.MonitorInfo.DpiScaling;
            var rect = new Rect(new Size(w, h));
            var visual = new DrawingVisual();
            using (var ctx = visual.RenderOpen())
            {
                var typeface = new Typeface("/PicView;component/Themes/Resources/fonts/#Tex Gyre Heros");
                //text
                var text = new FormattedText("Unable to render image", CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, 16, (Brush)Application.Current.Resources["MainColorBrush"], WindowSizing.MonitorInfo.DpiScaling)
                {
                    TextAlignment = System.Windows.TextAlignment.Center
                };

                ctx.DrawText(text, new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2));
            }
            RenderTargetBitmap rtv = new((int)w, (int)h, 96.0, 96.0, PixelFormats.Default);
            rtv.Render(visual);
            rtv.Freeze();
            return rtv;
        }
    }
}
