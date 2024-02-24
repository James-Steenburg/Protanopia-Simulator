﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace ProtanopiaSimulator.ViewModels.Pages
{
    public partial class ImagesDisplayViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isSaveButtonEnabled = false;

        [ObservableProperty]
        private string _selectedImagePath = "pack://application:,,,/Assets/pexels-johannes-plenio-1103970.jpg";

        [ObservableProperty]
        private ImageSource _transformedImage;

        [ObservableProperty]
        private double _q1 = 1.05118294;

        [ObservableProperty]
        private double _q2 = -0.05116099;

        [ObservableProperty]
        private Visibility _secondImageVisibility = Visibility.Collapsed;

        [RelayCommand]
        private void RunSimulation()
        {
            Bitmap bitmapModified = new Bitmap(SelectedImagePath);
            int x, y;
            for (x = 0; x < bitmapModified.Width; x++)
            {
                for (y = 0; y < bitmapModified.Height; y++)
                {
                    Color pixelColor = bitmapModified.GetPixel(x, y);
                    Color newColor = ColorBlindessTransform(pixelColor, Q1, Q2);
                    bitmapModified.SetPixel(x, y, newColor);
                }
            }
            TransformedImage = ImageSourceFromBitmap(bitmapModified);
            SecondImageVisibility = Visibility.Visible;
            IsSaveButtonEnabled = true;
        }

        [RelayCommand]
        private void OpenNewImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            //string SelectedFileName;
            
            if (dlg.ShowDialog() == true)
            {
                SelectedImagePath = dlg.FileName;
                //SelectedFileName = dlg.FileName;
                //FileNameLabel.Content = SelectedFileName;
                //BitmapImage bitmap = new BitmapImage();
                //bitmap.BeginInit();
                //bitmap.UriSource = new Uri(SelectedImagePath);
                //bitmap.EndInit();
                //ImageViewer1.Source = bitmap;

                SecondImageVisibility = Visibility.Collapsed;
                IsSaveButtonEnabled = false;

            }
            
        }

        [RelayCommand]
        private void SaveSimulatedImage()
        {
            //ounter++;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public double[,] T =
        {
            {0.31399022, 0.63951294, 0.04649755 },
            {0.15537241, 0.75789446, 0.08670142 },
            {0.01775239, 0.10944209, 0.87256922 }
        };

        public double[,] invT =
        {
            {5.47221206, -4.6419601, 0.16963708 },
            {-1.1252419, 2.29317094, -0.1678952 },
            {0.02980165, -0.19318073, 1.16364789 }
        };

        public Color ColorBlindessTransform(Color originalColor, double q1, double q2)
        {
            Color ColorBlindColor = Color.FromArgb(255, 255, 255);
            double L, M, S;
            L = T[0, 0] * originalColor.R + T[0, 1] * originalColor.G + T[0, 2] * originalColor.B;
            M = T[1, 0] * originalColor.R + T[1, 1] * originalColor.G + T[1, 2] * originalColor.B;
            S = T[2, 0] * originalColor.R + T[2, 1] * originalColor.G + T[2, 2] * originalColor.B;

            double[,] simulateProtanopiaMatrix =
            {
                {0, q1, q2 },
                {0, 1, 0 },
                {0, 0, 1 }
            };

            double L2, M2, S2;
            //Placeholders
            L2 = simulateProtanopiaMatrix[0, 0] * L + simulateProtanopiaMatrix[0, 1] * M + simulateProtanopiaMatrix[0, 2] * S;
            M2 = simulateProtanopiaMatrix[1, 0] * L + simulateProtanopiaMatrix[1, 1] * M + simulateProtanopiaMatrix[1, 2] * S;
            S2 = simulateProtanopiaMatrix[2, 0] * L + simulateProtanopiaMatrix[2, 1] * M + simulateProtanopiaMatrix[2, 2] * S;

            double nR = invT[0, 0] * L2 + invT[0, 1] * M2 + invT[0, 2] * S2;
            double nG = invT[1, 0] * L2 + invT[1, 1] * M2 + invT[1, 2] * S2;
            double nB = invT[2, 0] * L2 + invT[2, 1] * M2 + invT[2, 2] * S2;

            ColorBlindColor = Color.FromArgb((int)nR, (int)nG, (int)nB);
            return ColorBlindColor;
        }
    }
}
