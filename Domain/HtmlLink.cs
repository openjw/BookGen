﻿//-----------------------------------------------------------------------------
// (c) 2019 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace BookGen.Domain
{
    public class HtmlLink : IEquatable<HtmlLink>
    {
        public string DisplayString { get; }
        public string Link { get; }

        public HtmlLink(string display, string link)
        {
            DisplayString = display;
            Link = link;
        }

        public override string ToString()
        {
            return $"<a href=\"{Link}\">{DisplayString}</a>";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HtmlLink);
        }

        public bool Equals(HtmlLink other)
        {
            return other != null &&
                   DisplayString == other.DisplayString &&
                   Link == other.Link;
        }

        public string GetLinkOnHost(string host)
        {
            var file = System.IO.Path.ChangeExtension(this.Link, ".html");
            return $"{host}{file}";
        }

        public override int GetHashCode()
        {
            var hashCode = -2003123855;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DisplayString);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Link);
            return hashCode;
        }
    }
}
