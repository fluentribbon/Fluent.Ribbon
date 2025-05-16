namespace FluentTest.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fluent;

    public class MenuViewModel
    {
        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

        public MenuViewModel()
        {
            this.MenuItems = new ObservableCollection<MenuItemViewModel>
            {
            new MenuItemViewModel("Item 1")
            {
                Children =
                {
                    new MenuItemViewModel("Item 1")
                    {
                        ContextMenuItems = { new MenuItemViewModel("Context for sub Item 1") }
                    },
                    new MenuItemViewModel("Item 2")
                    {
                        ContextMenuItems = { new MenuItemViewModel("Context for sub Item 2") }
                    },
                    new MenuItemViewModel("Item 3")
                },
                ContextMenuItems = { new MenuItemViewModel("Context for Item 1") }
            },
            new MenuItemViewModel("Item 2")
            {
                Children =
                {
                    new MenuItemViewModel("Item 1"),
                    new MenuItemViewModel("Item 2"),
                    new MenuItemViewModel("Item 3"),
                },
                ContextMenuItems = { new MenuItemViewModel("Context for Item 2") }
            },
            new MenuItemViewModel("Item 10", true, true),
            new MenuItemViewModel("Item 11", true, true),
            new MenuItemViewModel("Item 12", true, true),
            new MenuItemViewModel("Item 13", true, true),
            new MenuItemViewModel("Item 14", true, true),
            new MenuItemViewModel("Item 15", true, true),
            new MenuItemViewModel("Item 16"),
            new MenuItemViewModel("Item 17"),
            new MenuItemViewModel("Item 18"),
            new MenuItemViewModel("Item 19"),
            new MenuItemViewModel("Item 20"),
            new MenuItemViewModel("Item 21"),
            new MenuItemViewModel("Item 22"),
            new MenuItemViewModel("Item 23"),
            new MenuItemViewModel("Item 24"),
            new MenuItemViewModel("Item 25"),
            };
        }
    }

    public class MenuItemViewModel
    {
        public string Header { get; set; }

        public bool IsCheckable { get; set; }

        public bool IsChecked { get; set; }

        public ObservableCollection<MenuItemViewModel> Children { get; set; }

        public ObservableCollection<MenuItemViewModel> ContextMenuItems { get; set; }

        public MenuItemViewModel(string header, bool isCheckable = false, bool isChecked = false)
        {
            this.Header = header;
            this.IsCheckable = isCheckable;
            this.IsChecked = isChecked;
            this.Children = new();
            this.ContextMenuItems = new ObservableCollection<MenuItemViewModel>();
        }
    }
}