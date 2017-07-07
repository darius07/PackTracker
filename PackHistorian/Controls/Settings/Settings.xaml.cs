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
using System.Windows.Shapes;
using PackTracker.Update;

namespace PackTracker.Controls.Settings {
  /// <summary>
  /// Interaktionslogik für Settings.xaml
  /// </summary>
  public partial class Settings {
    PackTracker.Settings _settings;

    public Settings(PackTracker.Settings Settings) {
      InitializeComponent();
      lb_tabs.ItemsSource = new List<ITitledElement>() {
        new General(Settings),
        new Update(Settings, new Updater()),
        new Credits(),
      };
      lb_tabs.SelectedIndex = 0;

      _settings = Settings;
    }
  }
}
