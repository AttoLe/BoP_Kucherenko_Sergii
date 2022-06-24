using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace coursework.Addons;

public static class GridExtensions
{
    public static void CreateRows(this Grid grid, List<double> rows)
    {
        foreach (double r in rows)
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(r, GridUnitType.Star);
            grid.RowDefinitions.Add(row);
        }
    }

    public static void CreateColumns(this Grid grid, List<double> columns)
    {
        foreach (double c in columns)
        {
            ColumnDefinition column = new ColumnDefinition();
            column.Width = new GridLength(c, GridUnitType.Star);
            grid.ColumnDefinitions.Add(column);
        }
    }

    public static void LocateOn(this Grid grid, UIElement o, (int, int) location)
    {
        Grid.SetRow(o, location.Item1);
        Grid.SetColumn(o, location.Item2);
        grid.Children.Add(o);       
    }
}
