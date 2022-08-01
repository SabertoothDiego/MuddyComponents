// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace SabertoothDiego.MuddyComponents;
public interface IMuddySelect
{
    void CheckGenericTypeMatch(object select_item);
    bool MultiSelection { get; set; }
}

public interface IMuddyShadowSelect
{
}