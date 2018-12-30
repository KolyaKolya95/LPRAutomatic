[33mcommit 8dae2c9392697bd5a725ed6639c7c49f58d151c1[m[33m ([m[1;36mHEAD -> [m[1;32mmaster[m[33m)[m
Author: Kolya Dudla <kolya.kolya1495@gmail.com>
Date:   Sun Nov 4 23:46:42 2018 +0200

    init
    
    Init commit

[1mdiff --git a/LPRAutomatic/MainWindow.xaml b/LPRAutomatic/MainWindow.xaml[m
[1mindex e474ff5..15652d6 100644[m
[1m--- a/LPRAutomatic/MainWindow.xaml[m
[1m+++ b/LPRAutomatic/MainWindow.xaml[m
[36m@@ -6,7 +6,21 @@[m
         xmlns:local="clr-namespace:LPRAutomatic"[m
         mc:Ignorable="d"[m
         Title="MainWindow" Height="450" Width="800">[m
[31m-    <Grid>[m
[31m-        [m
[32m+[m[32m    <Grid x:Name="MainGrid">[m
[32m+[m
[32m+[m[32m        <Grid x:Name="FileInfoGrid" Height="50" Margin="10,10,9.6,0" VerticalAlignment="Top" Background="{DynamicResource {x:Static SystemColors.AppWorkspaceBrushKey}}">[m
[32m+[m[32m            <TextBox x:Name="FileNameTextBox" HorizontalAlignment="Left" Height="30" Margin="10,10,0,0" TextWrapping="Wrap" Text="File name" VerticalAlignment="Top" Width="650"/>[m
[32m+[m[32m            <Button x:Name="FindButton" Content="Load Image" Margin="665,10,10,0" VerticalAlignment="Top" Height="30" Click="FindButton_Click"/>[m
[32m+[m[32m        </Grid>[m
[32m+[m[32m        <Grid x:Name="InfoGrid" HorizontalAlignment="Left" Height="345" VerticalAlignment="Top" Width="200" Margin="10,65,0,0" Background="{DynamicResource {x:Static SystemColors.ActiveBorderBrushKey}}">[m
[32m+[m[32m            <ListBox x:Name="InfoNumberListBox" HorizontalAlignment="Left" Height="325" Margin="10,10,0,0" VerticalAlignment="Top" Width="180"/>[m
[32m+[m[32m        </Grid>[m
[32m+[m[32m        <Grid x:Name="CarImageGrid" Height="250" Margin="215,65,9.6,0" VerticalAlignment="Top" Background="{DynamicResource {x:Static SystemColors.ActiveBorderBrushKey}}">[m
[32m+[m[32m            <Image x:Name="CarImage" Height="230" Margin="10,10,10,0" VerticalAlignment="Top" OpacityMask="{DynamicResource {x:Static SystemColors.ControlLightLightBrushKey}}"/>[m
[32m+[m[32m        </Grid>[m
[32m+[m[32m        <Grid x:Name="NumberImagesGrid" Height="90" Margin="215,320,9.6,0" VerticalAlignment="Top" Background="{DynamicResource {x:Static SystemColors.ActiveBorderBrushKey}}">[m
[32m+[m[32m            <Image HorizontalAlignment="Left" Height="70" Margin="10,10,0,0" VerticalAlignment="Top" Width="260"/>[m
[32m+[m[32m            <Image Height="70" Margin="299,10,10,0" VerticalAlignment="Top"/>[m
[32m+[m[32m        </Grid>[m
     </Grid>[m
 </Window>[m
[1mdiff --git a/LPRAutomatic/MainWindow.xaml.cs b/LPRAutomatic/MainWindow.xaml.cs[m
[1mindex a14bf5c..62376fe 100644[m
[1m--- a/LPRAutomatic/MainWindow.xaml.cs[m
[1m+++ b/LPRAutomatic/MainWindow.xaml.cs[m
[36m@@ -1,17 +1,7 @@[m
[31m-﻿using System;[m
[31m-using System.Collections.Generic;[m
[31m-using System.Linq;[m
[31m-using System.Text;[m
[31m-using System.Threading.Tasks;[m
[32m+[m[32m﻿using Microsoft.Win32;[m
[32m+[m[32musing System;[m
 using System.Windows;[m
[31m-using System.Windows.Controls;[m
[31m-using System.Windows.Data;[m
[31m-using System.Windows.Documents;[m
[31m-using System.Windows.Input;[m
[31m-using System.Windows.Media;[m
 using System.Windows.Media.Imaging;[m
[31m-using System.Windows.Navigation;[m
[31m-using System.Windows.Shapes;[m
 [m
 namespace LPRAutomatic[m
 {[m
[36m@@ -24,5 +14,24 @@[m [mnamespace LPRAutomatic[m
         {[m
             InitializeComponent();[m
         }[m
[32m+[m
[32m+[m[32m        /// <summary>[m
[32m+[m[32m        /// FIndButton button for load image in to image form[m
[32m+[m[32m        /// </summary>[m
[32m+[m[32m        /// <param name="sender"></param>[m
[32m+[m[32m        /// <param name="e"></param>[m
[32m+[m[32m        private void FindButton_Click(object sender, RoutedEventArgs e)[m
[32m+[m[32m        {[m
[32m+[m[32m            OpenFileDialog openFileDialog = new OpenFileDialog();[m
[32m+[m[32m            openFileDialog.Title = "Select a picture";[m
[32m+[m[32m            openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +[m
[32m+[m[32m              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +[m
[32m+[m[32m              "Portable Network Graphic (*.png)|*.png";[m
[32m+[m[32m            if (openFileDialog.ShowDialog() == true)[m
[32m+[m[32m            {[m
[32m+[m[32m                FileNameTextBox.Text = openFileDialog.FileName;[m
[32m+[m[32m                CarImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));[m
[32m+[m[32m            }[m
[32m+[m[32m        }[m
     }[m
 }[m
