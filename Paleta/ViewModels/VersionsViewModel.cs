using Palette.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Palette.ViewModels
{
    public class VersionsViewModel
    {

        public ObservableCollection<VersionItem> Versions { get; set; } = new ObservableCollection<VersionItem>();

        public VersionsViewModel()
        {

            {
                ColorItem color = new ColorItem { Color = Color.FromArgb(255,42,245,152), Offset = 0 };
                ColorItem color2 = new ColorItem { Color = Colors.Teal, Offset = 1 };
                VersionItem version2 = new VersionItem { Name = "Accent Color", Rotation = 45 };
                version2.Colors.Add(color);
                version2.Colors.Add(color2);
                Versions.Add(version2);
            }
            {
                ColorItem color = new ColorItem { Color = Color.FromArgb(255, 42, 245, 152), Offset = 0 };
                ColorItem color2 = new ColorItem { Color = Colors.Aqua, Offset = 1 };
                VersionItem version2 = new VersionItem { Name = "Accent Color", Rotation = 135 };
                version2.Colors.Add(color);
                version2.Colors.Add(color2);
                Versions.Add(version2);
            }
            {
                ColorItem color = new ColorItem { Color = Colors.DarkOrange, Offset = 1 };
                VersionItem version2 = new VersionItem { Name = "Accent Color", Rotation = 20 };
                version2.Colors.Add(color);
                Versions.Add(version2);
            }
            {
                ColorItem color = new ColorItem { Color = Colors.PeachPuff, Offset = 1 };
                VersionItem version2 = new VersionItem { Name = "Accent Color", Rotation = 20 };
                version2.Colors.Add(color);
                Versions.Add(version2);
            }
            {
                ColorItem color = new ColorItem { Color = Colors.PeachPuff, Offset = 1 };
                VersionItem version2 = new VersionItem { Name = "Accent Color", Rotation = 60 };
                version2.Colors.Add(color);
                Versions.Add(version2);
            }

        }

    }
}
