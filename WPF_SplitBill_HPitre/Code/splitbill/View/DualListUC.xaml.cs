using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections;

namespace splitbill.View
{
    public partial class DualListUC : UserControl
    {
        public DualListUC()
        {
            InitializeComponent();
        }

        #region Columns

        public static readonly DependencyProperty LeftColumnsProperty =
            DependencyProperty.Register(nameof(LeftColumns), typeof(GridViewColumnCollection), typeof(DualListUC),
                new PropertyMetadata(null, OnColumnsChanged));

        public GridViewColumnCollection LeftColumns
        {
            get => (GridViewColumnCollection)GetValue(LeftColumnsProperty);
            set => SetValue(LeftColumnsProperty, value);
        }



        public static readonly DependencyProperty RightColumnsProperty =
            DependencyProperty.Register(nameof(RightColumns), typeof(GridViewColumnCollection), typeof(DualListUC),
                new PropertyMetadata(null, OnColumnsChanged));

        public GridViewColumnCollection RightColumns
        {
            get => (GridViewColumnCollection)GetValue(RightColumnsProperty);
            set => SetValue(RightColumnsProperty, value);
        }
        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DualListUC control = (DualListUC)d;

            if (e.Property == LeftColumnsProperty && control.LeftColumns != null)
            {
                GridView gvLeft = new GridView();

                foreach (var col in control.LeftColumns)
                    gvLeft.Columns.Add(CloneColumn(col));

                control.AvailableListView.View = gvLeft;
            }

            if (e.Property == RightColumnsProperty && control.RightColumns != null)
            {
                GridView gvRight = new GridView();

                foreach (var col in control.RightColumns) 
                    gvRight.Columns.Add(CloneColumn(col));

                control.AssignedListView.View = gvRight;
            }
        }

        private static GridViewColumn CloneColumn(GridViewColumn source)
        {
            return new GridViewColumn
            {
                Header = source.Header,
                DisplayMemberBinding = source.DisplayMemberBinding,
                CellTemplate = source.CellTemplate,
                Width = source.Width
            };
        }

        #endregion
        public static readonly DependencyProperty LeftTitleProperty =
DependencyProperty.Register(nameof(LeftTitle), typeof(string), typeof(DualListUC),
new PropertyMetadata("PARTICIPANTS"));

        public string LeftTitle
        {
            get => (string)GetValue(LeftTitleProperty);
            set => SetValue(LeftTitleProperty, value);
        }


        public static readonly DependencyProperty RightTitleProperty =
            DependencyProperty.Register(nameof(RightTitle), typeof(string), typeof(DualListUC),
                new PropertyMetadata("ASSIGNÉS"));

        public string RightTitle
        {
            get => (string)GetValue(RightTitleProperty);
            set => SetValue(RightTitleProperty, value);
        }

        public static readonly DependencyProperty HintTextProperty =
            DependencyProperty.Register(nameof(HintText), typeof(string), typeof(DualListUC),
                new PropertyMetadata("Ctrl+clic pour sélection multiple"));

        public string HintText
        {
            get => (string)GetValue(HintTextProperty);
            set => SetValue(HintTextProperty, value);
        }

        public static readonly DependencyProperty IsEditableProperty =
    DependencyProperty.Register(nameof(IsEditable), typeof(bool), typeof(DualListUC),
        new PropertyMetadata(true));

        public bool IsEditable
        {
            get => (bool)GetValue(IsEditableProperty);
            set => SetValue(IsEditableProperty, value);
        }

        public static readonly DependencyProperty ConfirmButtonTextProperty =
            DependencyProperty.Register(nameof(ConfirmButtonText), typeof(string), typeof(DualListUC),
                new PropertyMetadata("✓ Confirmer"));

        public string ConfirmButtonText
        {
            get => (string)GetValue(ConfirmButtonTextProperty);
            set => SetValue(ConfirmButtonTextProperty, value);
        }

        public static readonly DependencyProperty AvailableItemsProperty =
            DependencyProperty.Register(nameof(AvailableItems), typeof(IEnumerable), typeof(DualListUC));

        public IEnumerable AvailableItems
        {
            get => (IEnumerable)GetValue(AvailableItemsProperty);
            set => SetValue(AvailableItemsProperty, value);
        }

        public static readonly DependencyProperty AssignedItemsProperty =
            DependencyProperty.Register(nameof(AssignedItems), typeof(IEnumerable), typeof(DualListUC));

        public IEnumerable AssignedItems
        {
            get => (IEnumerable)GetValue(AssignedItemsProperty);
            set => SetValue(AssignedItemsProperty, value);
        }

        public static readonly DependencyProperty AssignedCountProperty =
            DependencyProperty.Register(nameof(AssignedCount), typeof(int), typeof(DualListUC),
                new PropertyMetadata(0));

        public int AssignedCount
        {
            get => (int)GetValue(AssignedCountProperty);
            set => SetValue(AssignedCountProperty, value);
        }

        public static readonly DependencyProperty AssignSelectedCommandProperty =
            DependencyProperty.Register(nameof(AssignSelectedCommand), typeof(ICommand), typeof(DualListUC));

        public ICommand AssignSelectedCommand
        {
            get => (ICommand)GetValue(AssignSelectedCommandProperty);
            set => SetValue(AssignSelectedCommandProperty, value);
        }

        public static readonly DependencyProperty AssignAllCommandProperty =
            DependencyProperty.Register(nameof(AssignAllCommand), typeof(ICommand), typeof(DualListUC));

        public ICommand AssignAllCommand
        {
            get => (ICommand)GetValue(AssignAllCommandProperty);
            set => SetValue(AssignAllCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveSelectedCommandProperty =
            DependencyProperty.Register(nameof(RemoveSelectedCommand), typeof(ICommand), typeof(DualListUC));

        public ICommand RemoveSelectedCommand
        {
            get => (ICommand)GetValue(RemoveSelectedCommandProperty);
            set => SetValue(RemoveSelectedCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveAllCommandProperty =
            DependencyProperty.Register(nameof(RemoveAllCommand), typeof(ICommand), typeof(DualListUC));

        public ICommand RemoveAllCommand
        {
            get => (ICommand)GetValue(RemoveAllCommandProperty);
            set => SetValue(RemoveAllCommandProperty, value);
        }

        public static readonly DependencyProperty ConfirmCommandProperty =
            DependencyProperty.Register(nameof(ConfirmCommand), typeof(ICommand), typeof(DualListUC));

        public ICommand ConfirmCommand
        {
            get => (ICommand)GetValue(ConfirmCommandProperty);
            set => SetValue(ConfirmCommandProperty, value);
        }
    }
}