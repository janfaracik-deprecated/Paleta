using Paleta.Converters;
using Paleta.Helpers;
using Paleta.Models;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI;

namespace Paleta.ViewModels
{
    public class ExportViewModel : BindableBase
    {

        public ObservableCollection<ExportItem> Exports { get; set; } = new ObservableCollection<ExportItem>();
        public ObservableCollection<ExportItem> SearchItems { get; set; } = new ObservableCollection<ExportItem>();

        public string Name { get; set; }
        public ObservableCollection<ColorItem> Colors { get; set; } = new ObservableCollection<ColorItem>();

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                Search(value);
                OnPropertyChanged();
            }
        }

        public ExportViewModel(ColorCollectionItem colorCollectionItem)
        {

            Name = colorCollectionItem.Name;
            Colors = new ObservableCollection<ColorItem>(colorCollectionItem.Colors);

            Debug.WriteLine(Colors.Count);

            if (Colors.Count == 1)
            {
                ColorItem colorItem = Colors[0];
                Color color = colorItem.Color;
                Exports.Add(new ExportItem { Name = "HEX", Value = ColorConverters.ColorToHex(color), Tags = "Web" });
                Exports.Add(new ExportItem { Name = "RGB", Value = ColorConverters.ColorToRGB(color), Tags = "Web" });
                Exports.Add(new ExportItem { Name = "ARGB", Value = ColorConverters.ColorToARGB(color), Tags = "Web" });
                Exports.Add(new ExportItem { Name = "RGBA", Value = ColorConverters.ColorToRGBA(color), Tags = "Web" });
                Exports.Add(new ExportItem { Name = "HSL", Value = ColorConverters.ColorToHSL(color), Tags = "Web" });
                Exports.Add(new ExportItem { Name = "Windows.UI.Color", Value = ColorConverters.ColorToWindowsUI(color), Tags = "Web" });
                Exports.Add(new ExportItem { Name = "Xamarin (C#)", Value = ColorConverters.ColorToXamarin(color), Tags = "Microsoft Windows 10 Visual Studio" });
                Exports.Add(new ExportItem { Name = "Swift", Value = ColorConverters.ColorToSwift(color), Tags = "Apple Xcode macOS iOS tvOS watchOS" });
                Exports.Add(new ExportItem { Name = "Objective-C", Value = ColorConverters.ColorToObjectiveC(color), Tags = "Apple Xcode macOS iOS tvOS watchOS" });
            }
            else
            {
                Exports.Add(new ExportItem { Name = "Linear Gradient", Value = ColorConverters.ColorsToLinearGradient(colorCollectionItem), Tags = "Web" });
                Exports.Add(new ExportItem { Name = "Radial Gradient", Value = ColorConverters.ColorsToRadialGradient(colorCollectionItem), Tags = "Web" });
            }

        }

        public static int LevenshteinDistance(string src, string dest)
        {
            if (dest.ToLower().Contains(src.ToLower()))
            {
                return 1;
            }

            int[,] d = new int[src.Length + 1, dest.Length + 1];
            int i, j, cost;
            char[] str1 = src.ToCharArray();
            char[] str2 = dest.ToCharArray();

            for (i = 0; i <= str1.Length; i++)
            {
                d[i, 0] = i;
            }
            for (j = 0; j <= str2.Length; j++)
            {
                d[0, j] = j;
            }
            for (i = 1; i <= str1.Length; i++)
            {
                for (j = 1; j <= str2.Length; j++)
                {

                    if (str1[i - 1] == str2[j - 1])
                        cost = 0;
                    else
                        cost = 1;

                    d[i, j] =
                        Math.Min(
                            d[i - 1, j] + 1,              // Deletion
                            Math.Min(
                                d[i, j - 1] + 1,          // Insertion
                                d[i - 1, j - 1] + cost)); // Substitution

                    if ((i > 1) && (j > 1) && (str1[i - 1] ==
                        str2[j - 2]) && (str1[i - 2] == str2[j - 1]))
                    {
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                    }
                }
            }

            return d[str1.Length, str2.Length];
        }

        public static List<ExportItem> aSearch(string value, List<ExportItem> wordList, double fuzzyness)
        {
            List<ExportItem> foundWords = (from s in wordList
                                      let levenshteinDistance = LevenshteinDistance(value, s.Name)
                                      let length = Math.Max(s.Name.Length, value.Length)
                                      let score = 1.0 - (double)levenshteinDistance / length
                                      where score > fuzzyness
                                      select s
                                      ).ToList();

            return foundWords;
        }

        private void Search(string value)
        {

            if (string.IsNullOrWhiteSpace(value))
            {
                SearchItems.Clear();
                return;
            }

            //SearchItems.Clear();

            //var list = new List<ExportItem>(Exports);

            //Debug.WriteLine("initial size is: " + list.Count);

            //List<ExportItem> help = aSearch(value, list, 0.75);

            //Debug.WriteLine("post size is " + help.Count);

            //foreach (var item in help)
            //{
            //    SearchItems.Add(item);
            //}

            foreach (ExportItem exportItem in Exports.Where(exportItem => exportItem.ToString().ToLower().Contains(value)))
            {
                if (!SearchItems.Contains(exportItem))
                {
                    SearchItems.Add(exportItem);
                }
            }

            List<ExportItem> listToRemove = new List<ExportItem>();

            for (int i = 0; i < SearchItems.Count; i++)
            {
                if (!SearchItems[i].ToString().ToLower().Contains(value))
                {
                    listToRemove.Add(SearchItems[i]);
                }
            }

            foreach (ExportItem exportItem in listToRemove)
            {
                SearchItems.Remove(exportItem);
            }

        }

    }
}