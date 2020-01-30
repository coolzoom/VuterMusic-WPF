﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

/*
           Lemon App 通用主题模板 
     Twilight./Lemon QQ:2728578956

   需要用到的几个部分:
       1.Background:在此Page上自由绘制   或 
          在 GetBrush方法内以Brush类型返回
       2.ThemeColor 主题颜色
       3.Font 字体颜色 White or Black
   可以随意更改的部分:
       1.命名空间 namespace
       2.作者,版权,程序集信息
    不可更改的:
       1.此类名
       2.基本方法 GetPage GetThemeColor GetFont
     */
namespace LemonApp.Theme.TheFirstSnow
{
    /// <summary>
    /// Drawe.xaml 的交互逻辑
    /// </summary>
    public partial class Drawer : ThemeBase
    {
        public Drawer()
        {
            ThemeName = "第一场雪";
            ThemeColor = Color.FromArgb(255, 158, 194, 248);
            FontColor = "White";
            InitializeComponent();
            border.Source = Properties.Resources.motionelement.ToBitmapImage();
            Resources["snow"] = new ImageBrush(Properties.Resources.a.ToBitmapImage());
            canvas.Background = new ImageBrush(Properties.Resources.thefirstsnow.ToBitmapImage());
            Draw();
            (Resources["Ani"] as Storyboard).Begin();
        }
        public override ThemeBase GetPage()
        {
            return new Drawer();
        }
        public static new string NameSpace = "LemonApp.Theme.TheFirstSnow.Drawer";
        public async void Draw()
        {
            await Task.Delay(1000);
            for (int i = 0; i < 20; i++)
            {
                await Task.Delay(i * 100);
                var b = new Image();
                Random rd = new Random();
                int width = rd.Next(10, 20);
                b.Width = width;
                b.Height = width;
                b.Source = (Resources["snow"] as ImageBrush).ImageSource;
                b.Opacity = 0.8;
                RenderOptions.SetBitmapScalingMode(b, BitmapScalingMode.LowQuality);
                int left = rd.Next(0, (int)canvas.ActualWidth);
                Canvas.SetLeft(b, left);
                int speed = rd.Next(15, 25);
                DoubleAnimationUsingPath d = new DoubleAnimationUsingPath()
                {
                    Duration = TimeSpan.FromSeconds(speed),
                    RepeatBehavior = RepeatBehavior.Forever,
                    PathGeometry = new PathGeometry(new List<PathFigure>() {
                    new PathFigure(new Point(left, 0-width),
                    new List<PathSegment>() {
                        new LineSegment( new Point(left, canvas.ActualHeight), false) }
                    , false) }),
                    Source = PathAnimationSource.Y
                };
                canvas.Children.Add(b);
                b.BeginAnimation(Canvas.TopProperty, d);
            }
        }
    }

    public static class h
    {
        public static BitmapImage ToBitmapImage(this byte[] array, int[] DecodePixel = null)
        {
            using (var ms = new MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                if (DecodePixel != null)
                {
                    image.DecodePixelHeight = DecodePixel[0];
                    image.DecodePixelWidth = DecodePixel[1];
                }
                image.EndInit();
                image.Freeze();
                return image;
            }
        }

    }
}
