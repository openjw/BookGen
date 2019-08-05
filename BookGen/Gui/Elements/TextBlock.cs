﻿//-----------------------------------------------------------------------------
// (c) 2019 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
//-----------------------------------------------------------------------------

using BookGen.Gui.Renderering;

namespace BookGen.Gui.Elements
{
    internal class TextBlock : ConsoleUiElement, IHaveContent
    {
        public string Content { get; set; }

        public override void Render(ITerminalRenderer target)
        {
            target.Text(Content, Foreground, Background, TextFormat.Default);
        }
    }
}
