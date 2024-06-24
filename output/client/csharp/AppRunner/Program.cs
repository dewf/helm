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

    record ModelItem(string Text, PaintResources.Color.Constant Color)
    {
        public readonly string Text = Text;
        public readonly PaintResources.Color.Constant Color = Color;
    }
    
    class MyStringModel(AbstractListModel.Interior interior) : AbstractListModel.MethodDelegate
    {
        private readonly List<ModelItem> _items = [];
        private int _nextInsertVal = 1;

        public int RowCount(ModelIndex.Handle parent)
        {
            return _items.Count;
        }

        public Variant.Deferred Data(ModelIndex.Handle index, Enums.ItemDataRole role)
        {
            if (index.IsValid() && index.Column() == 0 && index.Row() < _items.Count)
            {
                switch (role)
                {
                    case Enums.ItemDataRole.DisplayRole:
                    {
                        return new Variant.Deferred.FromString(_items[index.Row()].Text);
                    }
                    case Enums.ItemDataRole.DecorationRole:
                    {
                        var color = new PaintResources.Color.Deferred.FromConstant(_items[index.Row()].Color);
                        return new Variant.Deferred.FromColor(color);
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
            var item = new ModelItem($"Item {_nextInsertVal}", (PaintResources.Color.Constant)(_nextInsertVal % 17));
            _nextInsertVal += 1;
            //
            var lastIndex = _items.Count;
            interior.BeginInsertRows(new ModelIndex.Deferred.Empty(), lastIndex, lastIndex);
            _items.Add(item);
            interior.EndInsertRows();
        }

        public void RemoveFirst()
        {
            interior.BeginRemoveRows(new ModelIndex.Deferred.Empty(), 0, 0);
            _items.RemoveAt(0);
            interior.EndRemoveRows();
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
        
        using(var model = AbstractListModel.CreateSubclassed(interior => {
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

                var addButton = PushButton.Create(new ButtonHandler(() =>
                {
                    md!.AddOne();
                }));
                addButton.SetSignalMask(PushButton.SignalMask.Clicked);
                addButton.SetText("add one item");
                vbox.AddWidget(addButton);

                var removeButton = PushButton.Create(new ButtonHandler(() =>
                {
                    md!.RemoveFirst();
                }));
                removeButton.SetSignalMask(PushButton.SignalMask.Clicked);
                removeButton.SetText("remove first");
                vbox.AddWidget(removeButton);
                
                widget.Show();
                Application.Exec();
            }
        }
        
        Library.DumpTables(); // check to make sure we're not leaking anything
        Library.Shutdown();
    }
}
