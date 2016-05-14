/*
SpiroNet.Avalonia
Copyright (C) 2015 Wiesław Šoltés

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 3
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA
02110-1301, USA.

*/
using Avalonia.Markup;
using SpiroNet.Editor;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SpiroNet.Avalonia
{
    internal class ShapeToDataConverter : IMultiValueConverter
    {
        public static ShapeToDataConverter Instance = new ShapeToDataConverter();

        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Count != 2)
                return null;

            var shape = values[0] as SpiroShape;
            var dict = values[1] as IDictionary<SpiroShape, string>;
            if (shape == null || dict == null)
                return null;

            string data;
            if (!dict.TryGetValue(shape, out data))
                return null;

            return data;
        }
    }
}
