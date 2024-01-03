﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp1
{
    /// <summary>
    /// MyDocumentViewer.xaml 的互動邏輯
    /// </summary>
    public partial class MyDocumentViewer : Window
    {
        // 預設字體顏色和背景色
        Color fontColor = Colors.Blue;
        Color backgroundColor = Colors.LightYellow;
        public MyDocumentViewer()
        {
            InitializeComponent();
            // 初始化字體顏色選擇器和背景色選擇器
            fontColorPicker.SelectedColor = fontColor;
            backgroundColorPicker.SelectedColor = backgroundColor;

            // 初始化字體家族組合框
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                fontFamilyComboBox.Items.Add(fontFamily);
            }
            fontFamilyComboBox.SelectedIndex = 8;
            // 初始化字體大小組合框
            fontSizeComboBox.ItemsSource = new List<Double>() { 8, 9, 10, 12, 20, 24, 32, 40, 50, 60, 80, 90 };
            fontSizeComboBox.SelectedIndex = 3;
        }
        // 新建文件的命令處理
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // 在這裡實現「新建」的操作，例如打開一個新文件、清空文檔等
            MyDocumentViewer myDocumentViewer = new MyDocumentViewer();
            myDocumentViewer.Show();
        }
        // 打開文件的命令處理
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // 使用 OpenFileDialog 選擇文件
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "RTF 檔案|*.rtf|HTML 檔案|*.html|所有檔案|*.*";

            if (fileDialog.ShowDialog() == true)
            {
                string fileName = fileDialog.FileName;
                string fileExtension = Path.GetExtension(fileName);

                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

                // 根據文件格式載入內容
                if (fileExtension.Equals(".rtf", StringComparison.OrdinalIgnoreCase))
                {
                    // RTF 格式
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                    {
                        range.Load(fileStream, DataFormats.Rtf);
                    }
                }
                else if (fileExtension.Equals(".html", StringComparison.OrdinalIgnoreCase))
                {
                    // HTML 格式
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
                    {
                        range.Load(fileStream, DataFormats.Html);
                    }
                }
                else
                {
                    MessageBox.Show("不支援的檔案格式");
                }
            }
        }
        // 儲存文件的命令處理
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog(); // 使用 SaveFileDialog 選擇儲存位置和文件名稱
            fileDialog.Filter = "RTF 檔案|*.rtf|HTML 檔案|*.html|所有檔案|*.*";

            if (fileDialog.ShowDialog() == true)
            {
                string fileName = fileDialog.FileName;
                string fileExtension = Path.GetExtension(fileName);

                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

                if (fileExtension.Equals(".rtf", StringComparison.OrdinalIgnoreCase))
                {
                    // RTF 格式
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        range.Save(fileStream, DataFormats.Rtf);
                    }
                }
                else if (fileExtension.Equals(".html", StringComparison.OrdinalIgnoreCase))
                {
                    // HTML 格式
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        range.Save(fileStream, DataFormats.Html);
                    }
                }
                else
                {
                    MessageBox.Show("不支援的檔案格式");
                }
            }
        }

        // 編輯區域的選擇變更事件
        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //判斷選中的文字是否為粗體，並同步更新boldButton的狀態
            object property = rtbEditor.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            boldButton.IsChecked = (property is FontWeight && (FontWeight)property == FontWeights.Bold);

            //判斷選中的文字是否為斜體，並同步更新italicButton的狀態
            property = rtbEditor.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            italicButton.IsChecked = (property is FontStyle && (FontStyle)property == FontStyles.Italic);

            //判斷選中的文字是否有底線，並同步更新underlineButton的狀態
            property = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            underlineButton.IsChecked = (property != DependencyProperty.UnsetValue && property == TextDecorations.Underline);

            //判斷選中的文字的字體，同步更新fontFamilyComboBox的狀態
            property = rtbEditor.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            fontFamilyComboBox.SelectedItem = property;

            //判斷所選中的文字的字體大小，同步更新fontSizeComboBox的狀態
            property = rtbEditor.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            fontSizeComboBox.SelectedItem = property;

            //判斷所選中的文字的字體色彩，同步更新fontColorPicker的狀態
            SolidColorBrush? foregroundProperty = rtbEditor.Selection.GetPropertyValue(TextElement.ForegroundProperty) as SolidColorBrush;
            if (foregroundProperty != null)
            {
                fontColorPicker.SelectedColor = foregroundProperty.Color;
            }
        }
        // 字體色彩選擇器的選擇變更事件
        private void fontColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fontColor = (Color)e.NewValue;
            SolidColorBrush fontBrush = new SolidColorBrush(fontColor);
            rtbEditor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, fontBrush);
        }

        private void fontFamilyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (fontFamilyComboBox.SelectedItem != null)
            {
                rtbEditor.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, fontFamilyComboBox.SelectedItem);
            }
        }

        private void fontSizeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (fontSizeComboBox.SelectedItem != null)
            {
                rtbEditor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSizeComboBox.SelectedItem);
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            rtbEditor.Document.Blocks.Clear();
        }
        // 背景色選擇器的選擇變更事件
        private void BackgroundColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (backgroundColorPicker.SelectedColor.HasValue)
            {
                SolidColorBrush backgroundBrush = new SolidColorBrush(backgroundColorPicker.SelectedColor.Value);
                rtbEditor.Background = backgroundBrush;
            }
        }

    }
}