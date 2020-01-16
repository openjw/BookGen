﻿//-----------------------------------------------------------------------------
// (c) 2019-2020 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
//-----------------------------------------------------------------------------

namespace BookGen.Framework.Server
{
    public interface IRequestHandler
    {
        bool CanServe(string AbsoluteUri);
    }
}
