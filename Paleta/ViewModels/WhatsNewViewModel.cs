using Paleta.Models;
using Shared.Helpers;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace Paleta.ViewModels
{
    public class WhatsNewViewModel : BindableBase
    {
        public ObservableCollection<WhatsNewItem> Items { get; set; } = new ObservableCollection<WhatsNewItem>();

        public WhatsNewViewModel()
        {
            Items.Add(new WhatsNewItem { Title = "New User Interface", Description = "Fluent throughout", Icon = Symbol.FontColor });
            Items.Add(new WhatsNewItem { Title = "Gradients", Description = "You can now save gradients as well as standard colors", Icon = Symbol.FontColor });
            Items.Add(new WhatsNewItem { Title = "Versions", Description = "Versions backs up all your colors so you can restore them later", Icon = Symbol.FontColor });
            Items.Add(new WhatsNewItem { Title = "Clipboard Integration", Description = "Have a color on your clipboard? Paleta will now offer the option to import it", Icon = Symbol.Paste });
            Items.Add(new WhatsNewItem { Title = "Open in New Window", Description = "Open colors in seperate windows", Icon = Symbol.NewWindow });
            Items.Add(new WhatsNewItem { Title = "Import and Export", Description = "Easier to share than ever", Icon = Symbol.Import });
            Items.Add(new WhatsNewItem { Title = "Sort", Description = "Quickly sort your colors by alphabetical order", Icon = Symbol.Sort });
            Items.Add(new WhatsNewItem { Title = "Color Wheel", Description = "Great for picking colors quickly", Icon = Symbol.FontColor });
            Items.Add(new WhatsNewItem { Title = "Drag and Drop", Description = "Drag to reorder, duplicate or move to a different palette", Icon = Symbol.FontColor });
        }
    }
}