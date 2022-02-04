﻿/*
SpiroNet.Json
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
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace SpiroNet.Json;

internal class ObservableCollectionContractResolver : DefaultContractResolver
{
    public override JsonContract ResolveContract(Type type)
    {
        if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
        {
            return base.ResolveContract(typeof(ObservableCollection<>).MakeGenericType(type.GenericTypeArguments[0]));
        }
        else
        {
            return base.ResolveContract(type);
        }
    }
}