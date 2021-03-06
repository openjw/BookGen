﻿//-----------------------------------------------------------------------------
// (c) 2019-2020 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace BookGen.Api
{
    /// <summary>
    /// Represents a link in the Markdown ToC
    /// </summary>
    public sealed class Link : IEquatable<Link>
    {
        /// <summary>
        /// Link text
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Link url
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Creates a new link
        /// </summary>
        /// <param name="text">link text</param>
        /// <param name="url">link url</param>
        public Link(string text, string url)
        {
            Text = text;
            Url = url;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as Link);
        }

        /// <inheritdoc/>
        public bool Equals(Link? other)
        {
            return
                Text == other?.Text &&
                Url == other?.Url;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Text, Url);
        }

        /// <inheritdoc/>
        public static bool operator ==(Link left, Link right)
        {
            return EqualityComparer<Link>.Default.Equals(left, right);
        }

        /// <inheritdoc/>
        public static bool operator !=(Link left, Link right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Converts link contents to a HTML tag
        /// </summary>
        /// <returns>link as a HTML tag</returns>
        public string GetHtml()
        {
            return $"<a href=\"{Url}\">{Text}</a>";
        }

        /// <summary>
        /// Convert the link extension to .html file that can be referenced
        /// </summary>
        /// <param name="host">Host link</param>
        /// <returns>A link on host</returns>
        public string ConvertToLinkOnHost(string host)
        {
            var file = System.IO.Path.ChangeExtension(this.Url, ".html");
            return $"{host}{file}";
        }

        /// <summary>
        /// Deconstruct to a tuple
        /// </summary>
        /// <param name="url">link url</param>
        /// <param name="text">link text</param>
        public void Deconstruct(out string url, out string text)
        {
            url = Url;
            text = Text;
        }
    }
}
