﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static AltTool.ClothData;

namespace AltTool
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static TextBlock statusTextBlock = null;
        private static ProgressBar statusProgress = null;
        public static ObservableCollection<ClothData> clothes;
        private static ClothData selectedCloth = null;
        public static ProjectBuild projectBuildWindow = null;
        private static bool hasChanged = false;

        public MainWindow()
        {
            InitializeComponent();

            statusTextBlock = ((TextBlock)FindName("currentStatusBar"));
            statusProgress = ((ProgressBar)FindName("currentProgress"));

            clothes = new ObservableCollection<ClothData>();
            clothesListBox.ItemsSource = clothes;

        }

        public static void SetStatus(string status)
        {
            statusTextBlock.Text = status;
        }

        public static void SetProgress(double progress)
        {
            if (progress > 1)
                progress = 1;
            if (progress < 0)
                progress = 0;

            statusProgress.Value = statusProgress.Maximum * progress;
        }

        private void AddMaleClothes_Click(object sender, RoutedEventArgs e)
        {
            ProjectController.Instance().AddFiles(Sex.Male);
            hasChanged = true;
        }

        private void AddFemaleClothes_Click(object sender, RoutedEventArgs e)
        {
            ProjectController.Instance().AddFiles(Sex.Female);
            hasChanged = true;
        }

        private void RemoveUnderCursor_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
            {
                var _clothes = clothes.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();

                clothes.Clear();

                foreach(var cloth in _clothes)
                {
                    if(cloth != selectedCloth)
                    {
                        clothes.Add(cloth);
                    }
                }

                selectedCloth = null;
                editGroupBox.Visibility = Visibility.Hidden;
                hasChanged = true;
            }
        }

        private void ClothesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                hasChanged = true;
                selectedCloth = (ClothData)e.AddedItems[0];

                if (selectedCloth != null)
                {
                    clothEditWindow.Visibility = Visibility.Collapsed;
                    pedPropEditWindow.Visibility = Visibility.Collapsed;

                    if (selectedCloth.IsComponent())
                    {
                        clothEditWindow.Visibility = Visibility.Visible;
                        drawableName.Text = selectedCloth.Name;

                        texturesList.ItemsSource = selectedCloth.textures;
                        fpModelPath.Text = selectedCloth.fpModelPath != "" ? selectedCloth.fpModelPath : "Not selected...";

                        unkFlag1Check.IsChecked = selectedCloth.componentFlags.unkFlag1;
                        unkFlag2Check.IsChecked = selectedCloth.componentFlags.unkFlag2;
                        unkFlag3Check.IsChecked = selectedCloth.componentFlags.unkFlag3;
                        unkFlag4Check.IsChecked = selectedCloth.componentFlags.unkFlag4;
                        isHighHeelsCheck.IsChecked = selectedCloth.componentFlags.isHighHeels;
                    }
                    else
                    {
                        pedPropEditWindow.Visibility = Visibility.Visible;
                        drawableName.Text = selectedCloth.Name;
                        pedPropName.Text = selectedCloth.Name;

                        pedPropTexturesList.ItemsSource = selectedCloth.textures;

                        pedPropFlag1.IsChecked = selectedCloth.pedPropFlags.unkFlag1;
                        pedPropFlag2.IsChecked = selectedCloth.pedPropFlags.unkFlag2;
                        pedPropFlag3.IsChecked = selectedCloth.pedPropFlags.unkFlag3;
                        pedPropFlag4.IsChecked = selectedCloth.pedPropFlags.unkFlag4;
                        pedPropFlag5.IsChecked = selectedCloth.pedPropFlags.unkFlag5;
                    }
                }
            }
        }

        private void DrawableName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(selectedCloth != null)
            {
                selectedCloth.Name = drawableName.Text;
            }
            hasChanged = true;
        }

        private void NewProjectButton_Click(object sender, RoutedEventArgs e)
        {
            clothes.Clear();
        }

        private void OpenProjectButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "altV cloth JSON (*.altv-cloth.json)|*.altv-cloth.json";
            openFileDialog.FilterIndex = 1;
            openFileDialog.DefaultExt = "altv-cloth.json";

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    ProjectBuilder.LoadProject(filename);
                }
            }
        }

        private void SaveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog  = new SaveFileDialog();
            saveFileDialog.Filter          = "altV cloth JSON (*.altv-cloth.json)|*.altv-cloth.json";
            saveFileDialog.FilterIndex     = 1;
            saveFileDialog.DefaultExt      = "altv-cloth.json";

            if (saveFileDialog.ShowDialog() == true)
            {
                foreach (string filename in saveFileDialog.FileNames)
                {
                    ProjectBuilder.BuildProject(filename);
                }
                // We saved, so reset this.
                hasChanged = false;
            }
        }

        private void AddTexture_Click(object sender, RoutedEventArgs e)
        {
            if(selectedCloth != null)
                ProjectController.Instance().AddTexture(selectedCloth);
        }

        private void RemoveTexture_Click(object sender, RoutedEventArgs e)
        {
            if(texturesList.SelectedItem != null)
            {
                ((ObservableCollection<string>)texturesList.ItemsSource).Remove((string)texturesList.SelectedItem);
            }
        }

        private void BuildProjectButton_Click(object sender, RoutedEventArgs e)
        {
            projectBuildWindow = new ProjectBuild();
            projectBuildWindow.Show();
        }

        private void UnkFlag1Check_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.componentFlags.unkFlag1 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void UnkFlag2Check_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.componentFlags.unkFlag2 = unkFlag2Check.IsChecked.GetValueOrDefault(false);
        }

        private void UnkFlag3Check_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.componentFlags.unkFlag3 = unkFlag3Check.IsChecked.GetValueOrDefault(false);
        }

        private void UnkFlag4Check_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.componentFlags.unkFlag4 = unkFlag4Check.IsChecked.GetValueOrDefault(false);
        }

        private void IsHighHeelsCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.componentFlags.isHighHeels = isHighHeelsCheck.IsChecked.GetValueOrDefault(false);
        }

        private void ClearFPModel_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.fpModelPath = "";
            fpModelPath.Text = "Not selected...";
        }

        private void SelectFPModel_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                ProjectController.Instance().SetFPModel(selectedCloth);
            fpModelPath.Text = selectedCloth.fpModelPath != "" ? selectedCloth.fpModelPath : "Not selected...";
        }

        private void PedPropName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (selectedCloth != null)
            {
                selectedCloth.Name = drawableName.Text;
            }
        }

        private void PedPropFlag1_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.pedPropFlags.unkFlag1 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void PedPropFlag2_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.pedPropFlags.unkFlag2 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void PedPropFlag3_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.pedPropFlags.unkFlag3 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void PedPropFlag4_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.pedPropFlags.unkFlag4 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void PedPropFlag5_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedCloth != null)
                selectedCloth.pedPropFlags.unkFlag5 = unkFlag1Check.IsChecked.GetValueOrDefault(false);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (hasChanged)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to save before exiting?", "Exit without saving?", MessageBoxButton.YesNoCancel);
                switch(messageBoxResult)
                {
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Yes:
                        SaveProjectButton_Click(sender, null);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
