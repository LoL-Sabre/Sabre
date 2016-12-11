﻿using System;
using System.Collections.Generic;
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
using MahApps.Metro.Controls;
using MahApps.Metro;
using System.IO;

namespace Sabre
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Logger log;
        private Config cfg;
        private string ecdsa;
        private StreamReader wadHashReader;
        private WADFile wad;
        public List<string> WADHashes = new List<string>();
        public MainWindow()
        {
            log = new Logger(DateTime.Now.ToString("HH-mm-ss"));
            log.Write("LOGGER INITIALIZED", Logger.WriterType.WriteMessage);
            InitializeComponent();
            log.Write("SABRE INITIALIZED", Logger.WriterType.WriteMessage);
            cfg = new Config("config.ini", log);
            Functions.LoadSettings(cfg, this);
            wadHashReader = new StreamReader(File.OpenRead("WAD.txt"));
            do
            {
                WADHashes.Add(wadHashReader.ReadLine());
            } while(wadHashReader.BaseStream.Position != wadHashReader.BaseStream.Length);
        }
        
        private void buttonGit(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/LoL-Sabre/Sabre");
            }
            catch (Exception) { }
        }

        private void deleteLogs(object sender, RoutedEventArgs e)
        {
            try
            {
                log.DeleteLogs();
            } catch(Exception) { }
        
        }

        private void openSettings(object sender, RoutedEventArgs e)
        {
            Functions.SwitchGrids(main, gridSettings);
        }

        private void openSkinCollection(object sender, RoutedEventArgs e)
        {
            Functions.SwitchGrids(main, gridSkinCollection);
        }

        private void openSkinCreation(object sender, RoutedEventArgs e)
        {
            Functions.SwitchGrids(main, gridSkinCreation);
        }
        private void buttonHome(object sender, RoutedEventArgs e)
        {
            foreach(Grid g in sabre.Children)
            {
                g.Visibility = Visibility.Hidden;
            }
            main.Visibility = Visibility.Visible;
        }
         
        private void changeAppearance(object sender, SelectionChangedEventArgs e)
        {
            if(comboAccents.SelectedItem != null && comboThemes.SelectedItem != null)
            {
                try
                {
                    Functions.ChangeAppearance(comboAccents.SelectedItem.ToString(), comboThemes.SelectedItem.ToString());
                    Config.Write(comboThemes.SelectedItem.ToString(), comboAccents.SelectedItem.ToString(), textLoLPath.Text);
                    log.Write("APPEARANCE CHANGED TO " + comboAccents.SelectedItem.ToString() + " " + comboThemes.SelectedItem.ToString(), Logger.WriterType.WriteMessage);
                }
                catch (Exception) { log.Write("ERROR APPEARANCE TO " + comboAccents.SelectedItem.ToString() + " " + comboThemes.SelectedItem.ToString(), Logger.WriterType.WriteError); }
            }
        }

        private void changeLoLPath(object sender, RoutedEventArgs e)
        {
            textLoLPath.Text = Functions.SelectFolder("Select your League of Legends folder");
            Config.Write(comboThemes.SelectedItem.ToString(), comboAccents.SelectedItem.ToString(), textLoLPath.Text);
        }

        private void tileWADExtractor_Click(object sender, RoutedEventArgs e)
        {
            Functions.SwitchGrids(gridSkinCreation, gridWADExtractor);
        }

        private void btnWADExtractorPath_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            if(ofd.ShowDialog() == true)
            {
                wad = new WADFile(ofd.FileName, log, WADHashes);
                ecdsa = "";
                foreach(byte b in wad.header.ECDSA)
                {
                    ecdsa += b.ToString("X2");
                }
                dataWADExtractor.ItemsSource = wad.Entries;
                textWADExtractorPath.Text = ofd.FileName;
            }
        }

        private void btnWADExtractAll_Click(object sender, RoutedEventArgs e)
        {
            Functions.ExtractWAD(dataWADExtractor.SelectedItems, wad.Entries, Functions.ExtractionType.All, WADHashes);
        }

        private void btnWADExtractSelected_Click(object sender, RoutedEventArgs e)
        {
            Functions.ExtractWAD(dataWADExtractor.SelectedItems, wad.Entries, Functions.ExtractionType.Selected, WADHashes);
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(gridWADExtractor.Visibility == Visibility.Visible && e.Key == Key.E)
            {
                MessageBox.Show(ecdsa);
            }
        }
    }
}
