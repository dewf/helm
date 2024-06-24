using Org.Whatever.QtTesting;

namespace AppRunner;

internal static class Program
{
    class WidgetHandler : Widget.SignalHandler
    {
        public void CustomContextMenuRequested(Common.Point pos)
        {
            throw new NotImplementedException();
        }

        public void WindowIconChanged(Icon.Handle icon)
        {
            throw new NotImplementedException();
        }

        public void WindowTitleChanged(string title)
        {
            throw new NotImplementedException();
        }
    }
    
    class MyStringModel(AbstractListModel.Interior interior) : AbstractListModel.MethodDelegate
    {
        // private AbstractListModel.Interior _interior = interior;
        private List<string> _items = ["one", "two", "three", "four", "five"];

        public ModelIndex.Deferred Parent(ModelIndex.Handle child)
        {
            return new ModelIndex.Deferred.Empty();
        }

        public int RowCount(ModelIndex.Handle parent)
        {
            return _items.Count;
        }

        public int ColumnCount(ModelIndex.Handle parent)
        {
            return 1;
        }

        public Variant.Deferred Data(ModelIndex.Handle index, Enums.ItemDataRole role)
        {
            if (index.IsValid() && index.Column() == 0 && index.Row() < _items.Count)
            {
                switch (role)
                {
                    case Enums.ItemDataRole.DisplayRole:
                    {
                        return new Variant.Deferred.FromString(_items[index.Row()]);
                    }
                    case Enums.ItemDataRole.DecorationRole:
                    {
                        var which = index.Row() % 17; // count in enum (minus transparent)
                        var color = new PaintResources.Color.Deferred.FromConstant((PaintResources.Color.Constant)which);
                        return new Variant.Deferred.FromColor(color);
                        // // var which = (Icon.ThemeIcon)(index.Row() % (int)Icon.ThemeIcon.NThemeIcons);
                        // // var icon = new Icon.Deferred.FromThemeIcon(which);
                        // return new Variant.Deferred.FromIcon(icon);
                    }
                }
            }
            return new Variant.Deferred.Empty();
        }

        public Variant.Deferred HeaderData(int section, Enums.Orientation orientation, Enums.ItemDataRole role)
        {
            return new Variant.Deferred.Empty();
        }

        public void AddOne()
        {
            var lastIndex = _items.Count;
            interior.BeginInsertRows(new ModelIndex.Deferred.Empty(), lastIndex, lastIndex);
            _items.Add($"Item {_items.Count + 1}");
            interior.EndInsertRows();
        }
    }

    delegate void ClickHandler();

    class ButtonHandler(ClickHandler onClick) : PushButton.SignalHandler
    {
        public void Clicked(bool checkState)
        {
            onClick();
        }

        public void Pressed()
        {
            throw new NotImplementedException();
        }

        public void Released()
        {
            throw new NotImplementedException();
        }

        public void Toggled(bool checkState)
        {
            throw new NotImplementedException();
        }
    }
    
    [STAThread]
    private static void Main(string[] args)
    {
        Library.Init();

        MyStringModel? md = null;
        
        using(var model = AbstractListModel.CreateSubclassed(interior =>
              {
                  md = new MyStringModel(interior);
                  return md;
              }, 0))
        using (var app = Application.Create(args))
        {
            using (var widget = Widget.Create(new WidgetHandler()))
            {
                widget.Resize(400, 400);

                var vbox = BoxLayout.Create(BoxLayout.Direction.TopToBottom);
                widget.SetLayout(vbox);

                var listView = ListView.Create();
                listView.SetModel(model);
                vbox.AddWidget(listView);

                var button = PushButton.Create(new ButtonHandler(() =>
                {
                    md!.AddOne();
                }));
                button.SetSignalMask(PushButton.SignalMask.Clicked);
                button.SetText("add one item");
                vbox.AddWidget(button);
                
                widget.Show();
                Application.Exec();
            }
        }
        
        Library.DumpTables(); // check to make sure we're not leaking anything
        Library.Shutdown();
    }
}
