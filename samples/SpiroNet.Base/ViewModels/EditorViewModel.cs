/*
SpiroNet.ViewModels
Copyright (C) 2019 Wiesław Šoltés

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
using SpiroNet.Editor;
using SpiroNet.Json;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace SpiroNet.ViewModels;

public class EditorViewModel
{
    public SpiroEditor Editor { get; set; }

    public ICommand InvalidateCommand { get; set; }

    public ICommand NewCommand { get; set; }

    public ICommand OpenCommand { get; set; }

    public ICommand SaveAsCommand { get; set; }

    public ICommand ExportCommand { get; set; }

    public ICommand ExitCommand { get; set; }

    public ICommand DeleteCommand { get; set; }

    public ICommand IsStrokedCommand { get; set; }

    public ICommand IsFilledCommand { get; set; }

    public ICommand IsClosedCommand { get; set; }

    public ICommand IsTaggedCommand { get; set; }

    public ICommand ToolCommand { get; set; }

    public ICommand PointTypeCommand { get; set; }

    public ICommand ExecuteScriptCommand { get; set; }

    public void OpenDrawing(string path)
    {
        try
        {
            using (var f = System.IO.File.OpenText(path))
            {
                string json = f.ReadToEnd();
                var drawing = JsonSerializer.Deserialize<SpiroDrawing>(json);
                if (drawing != null)
                {
                    Editor.LoadDrawing(drawing);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(ex.StackTrace);
        }
    }

    public void OpenPlate(string path)
    {
        try
        {
            using (var f = System.IO.File.OpenText(path))
            {
                string plate = f.ReadToEnd();
                var drawing = Editor.FromPlateString(plate);
                if (drawing != null)
                {
                    Editor.LoadDrawing(drawing);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(ex.StackTrace);
        }
    }

    public void SaveAsDrawing(string path)
    {
        try
        {
            using (var f = System.IO.File.CreateText(path))
            {
                var json = JsonSerializer.Serialize(Editor.Drawing);
                f.Write(json);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(ex.StackTrace);
        }
    }

    public void SaveAsPlate(string path)
    {
        try
        {
            using (var f = System.IO.File.CreateText(path))
            {
                string plate = Editor.ToPlateString();
                f.Write(plate);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(ex.StackTrace);
        }
    }

    public void ExportAsSvg(string path)
    {
        try
        {
            using (var f = System.IO.File.CreateText(path))
            {
                string svg = Editor.ToSvgString();
                f.Write(svg);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(ex.StackTrace);
        }
    }

    public void ExportAsPs(string path)
    {
        try
        {
            using (var f = System.IO.File.CreateText(path))
            {
                string ps = Editor.ToPsString();
                f.Write(ps);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(ex.StackTrace);
        }
    }
}
