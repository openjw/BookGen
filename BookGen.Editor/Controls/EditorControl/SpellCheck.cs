﻿//-----------------------------------------------------------------------------
// (c) 2019 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
//-----------------------------------------------------------------------------

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BookGen.Editor.Controls
{
    internal sealed class SpellCheck
    {
        private const string _blockpatterns = "^[`]{3,}(.*)[`]{3,}|";
        private readonly Hunspell _hunspell;
        private readonly TextView _renderTarget;
        private readonly SpellingColorizer _spellingColorizer;
        private readonly Regex _codeBlockRegex;
        private readonly Regex _uriRegex;
        private readonly Regex _wordRegex;

        public SpellCheck(TextView target, Hunspell hunspell)
        {
            _renderTarget = target;
            _spellingColorizer = new SpellingColorizer();
            _codeBlockRegex = new Regex(_blockpatterns, RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
            _uriRegex = new Regex("(http|ftp|https|mailto):\\/\\/[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])?", RegexOptions.Compiled);
            _wordRegex = new Regex("-[^\\w]+|^'[^\\w]+|[^\\w]+'[^\\w]+|[^\\w]+-[^\\w]+|[^\\w]+'$|[^\\w]+-$|^-$|^'$|[^\\w'-]", RegexOptions.Compiled);

            _hunspell = hunspell;
            _renderTarget.BackgroundRenderers.Add(_spellingColorizer);
        }

        internal void Invalidate()
        {
            _spellingColorizer.Errors.Clear();
            _renderTarget.Redraw();
            _renderTarget.InvalidateLayer(_spellingColorizer.Layer);
        }

        public int DoSpellCheck()
        {
            if (_hunspell == null) return 0;
            if (!_renderTarget.VisualLinesValid) return _spellingColorizer.Errors.Count;

            _spellingColorizer.Errors.Clear();

            foreach (VisualLine current in _renderTarget.VisualLines.AsParallel<VisualLine>())
            {
                int num = 0;
                string text = _renderTarget.Document.GetText(current.FirstDocumentLine.Offset, current.LastDocumentLine.EndOffset - current.FirstDocumentLine.Offset);
                if (!string.IsNullOrEmpty(text))
                {
                    text = Regex.Replace(text, "[\\u2018\\u2019\\u201A\\u201B\\u2032\\u2035]", "'");
                    string input = text;

                    input = _codeBlockRegex.Replace(input, string.Empty);
                    input = _uriRegex.Replace(input, string.Empty);
                    var words = _wordRegex.Split(input).Where(w => !string.IsNullOrEmpty(w));
                    foreach (string word in words)
                    {
                        string trimmed = word.Trim(new char[] { '\'', '_', '-' });
                        int trimcount = text.IndexOf(trimmed, num, System.StringComparison.InvariantCultureIgnoreCase);
                        if (trimcount > -1)
                        {
                            int start = current.FirstDocumentLine.Offset + trimcount;
                            if (!_hunspell.IsSpelledCorrectly(trimmed))
                            {
                                TextSegment item = new TextSegment
                                {
                                    StartOffset = start,
                                    Length = word.Length
                                };
                                _spellingColorizer.Errors.Add(item);
                            }
                            num = text.IndexOf(word, num, StringComparison.InvariantCultureIgnoreCase) + word.Length;
                        }
                    }
                }
            }
            _renderTarget.InvalidateLayer(_spellingColorizer.Layer);
            return _spellingColorizer.Errors.Count;
        }

        public bool Spell(string word)
        {
            return _hunspell.IsSpelledCorrectly(word);
        }

        public List<string> Suggest(string word)
        {
            return _hunspell.GetSuggestions(word);
        }
    }
}
