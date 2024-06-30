using Org.Whatever.QtTesting;

namespace AppRunner;

internal static class Program
{
    // class WidgetHandler : Widget.SignalHandler
    // {
    //     public void CustomContextMenuRequested(Common.Point pos)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void WindowIconChanged(Icon.Handle icon)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void WindowTitleChanged(string title)
    //     {
    //         throw new NotImplementedException();
    //     }
    // }
    //
    // record ModelItem(string Text, PaintResources.Color.Constant Color)
    // {
    //     public string Text = Text;
    //     public readonly PaintResources.Color.Constant Color = Color;
    // }
    //
    // class MyStringModel : AbstractListModel.MethodDelegate
    // {
    //     private readonly AbstractListModel.Interior _interior;
    //     private List<ModelItem> _items = [];
    //     private int _nextInsertVal = 1;
    //
    //     public MyStringModel()
    //     {
    //         _interior = AbstractListModel
    //             .CreateSubclassed(this, AbstractListModel.MethodMask.Flags | AbstractListModel.MethodMask.SetData)
    //             .GetInteriorHandle();
    //     }
    //
    //     public void Dispose()
    //     {
    //         Console.WriteLine("disposing interior");
    //         _interior.Dispose();
    //         Console.WriteLine("done");
    //     }
    //
    //     public AbstractListModel.Handle QtModel => _interior;
    //
    //     public int RowCount(ModelIndex.Handle parent)
    //     {
    //         return _items.Count;
    //     }
    //
    //     public Variant.Deferred Data(ModelIndex.Handle index, Enums.ItemDataRole role)
    //     {
    //         if (index.IsValid() && index.Column() == 0 && index.Row() < _items.Count)
    //         {
    //             switch (role)
    //             {
    //                 case Enums.ItemDataRole.DisplayRole:
    //                 {
    //                     return new Variant.Deferred.FromString(_items[index.Row()].Text);
    //                 }
    //                 case Enums.ItemDataRole.DecorationRole:
    //                 {
    //                     var color = new PaintResources.Color.Deferred.FromConstant(_items[index.Row()].Color);
    //                     return new Variant.Deferred.FromColor(color);
    //                 }
    //             }
    //         }
    //         return new Variant.Deferred.Empty();
    //     }
    //
    //     public Variant.Deferred HeaderData(int section, Enums.Orientation orientation, Enums.ItemDataRole role)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public AbstractListModel.ItemFlags GetFlags(ModelIndex.Handle index, AbstractListModel.ItemFlags baseFlags)
    //     {
    //         if (index.IsValid() && index.Column() == 0 && index.Row() < _items.Count)
    //         {
    //             return baseFlags | AbstractListModel.ItemFlags.ItemIsEditable;
    //         }
    //         return baseFlags;
    //     }
    //
    //     public bool SetData(ModelIndex.Handle index, Variant.Handle value, Enums.ItemDataRole role)
    //     {
    //         if (index.IsValid() && index.Column() == 0 && index.Row() < _items.Count && role == Enums.ItemDataRole.EditRole)
    //         {
    //             _items[index.Row()].Text = value.ToString2();
    //             var deferred = new ModelIndex.Deferred.FromHandle(index);
    //             _interior.EmitDataChanged(deferred, deferred, []);
    //             return true;
    //         }
    //         return false;
    //     }
    //
    //     public int ColumnCount(ModelIndex.Handle parent)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void AddOne()
    //     {
    //         var item = new ModelItem($"Item {_nextInsertVal}", (PaintResources.Color.Constant)(_nextInsertVal % 17));
    //         _nextInsertVal += 1;
    //         //
    //         var lastIndex = _items.Count;
    //         _interior.BeginInsertRows(new ModelIndex.Deferred.Empty(), lastIndex, lastIndex);
    //         _items.Add(item);
    //         _interior.EndInsertRows();
    //     }
    //
    //     public void RemoveFirst()
    //     {
    //         _interior.BeginRemoveRows(new ModelIndex.Deferred.Empty(), 0, 0);
    //         _items.RemoveAt(0);
    //         _interior.EndRemoveRows();
    //     }
    //
    //     private readonly PaintResources.Color.Constant[] _replacementColors =
    //     [
    //         PaintResources.Color.Constant.Blue,
    //         PaintResources.Color.Constant.Magenta,
    //         PaintResources.Color.Constant.Red,
    //         PaintResources.Color.Constant.Yellow,
    //         PaintResources.Color.Constant.Green,
    //     ];
    //
    //     public void ReplaceAll()
    //     {
    //         _interior.BeginResetModel();
    //         _items =
    //             Enumerable
    //                 .Range(0, 100)
    //                 .Select(i => new ModelItem($"NEW item {i + 1}", _replacementColors[i % _replacementColors.Length]))
    //                 .ToList();
    //         _interior.EndResetModel();
    //     }
    // }
    //
    // delegate void ClickHandler();
    //
    // class ButtonHandler(ClickHandler onClick) : PushButton.SignalHandler
    // {
    //     public void Clicked(bool checkState)
    //     {
    //         onClick();
    //     }
    //
    //     public void Pressed()
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void Released()
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void Toggled(bool checkState)
    //     {
    //         throw new NotImplementedException();
    //     }
    // }
    //
    // class ListViewHandler : ListView.SignalHandler
    // {
    //     public void CustomContextMenuRequested(Common.Point pos)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void Activated(ModelIndex.Handle index)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void Clicked(ModelIndex.Handle index)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void DoubleClicked(ModelIndex.Handle index)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void Entered(ModelIndex.Handle index)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void IconSizeChanged(Common.Size size)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void Pressed(ModelIndex.Handle index)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void ViewportEntered()
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public void IndexesMoved(ModelIndex.Handle[] indexes)
    //     {
    //         throw new NotImplementedException();
    //     }
    // }
    
    [STAThread]
    private static void Main(string[] args)
    {
        Library.Init();

        // using(var model = new MyStringModel())
        // using (var app = Application.Create(args))
        // {
        //     using (var widget = Widget.Create(new WidgetHandler()))
        //     {
        //         widget.Resize(400, 400);
        //
        //         var vbox = BoxLayout.Create(BoxLayout.Direction.TopToBottom);
        //         widget.SetLayout(vbox);
        //
        //         var listView = ListView.Create(new ListViewHandler());
        //         listView.SetModel(model.QtModel);
        //         vbox.AddWidget(listView);
        //
        //         var addButton = PushButton.Create(new ButtonHandler(() =>
        //         {
        //             model.AddOne();
        //         }));
        //         addButton.SetSignalMask(PushButton.SignalMask.Clicked);
        //         addButton.SetText("add one item");
        //         vbox.AddWidget(addButton);
        //
        //         var removeButton = PushButton.Create(new ButtonHandler(() =>
        //         {
        //             model.RemoveFirst();
        //         }));
        //         removeButton.SetSignalMask(PushButton.SignalMask.Clicked);
        //         removeButton.SetText("remove first");
        //         vbox.AddWidget(removeButton);
        //
        //         var replaceButton = PushButton.Create(new ButtonHandler(() =>
        //         {
        //             model.ReplaceAll();
        //         }));
        //         replaceButton.SetSignalMask(PushButton.SignalMask.Clicked);
        //         replaceButton.SetText("replace all");
        //         vbox.AddWidget(replaceButton);
        //         
        //         widget.Show();
        //         Application.Exec();
        //     }
        // }
        
        Library.DumpTables(); // check to make sure we're not leaking anything
        Library.Shutdown();
    }
}
