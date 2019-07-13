﻿//-----------------------------------------------------------------------------
// (c) 2019 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
//-----------------------------------------------------------------------------

using BookGen.Contracts;
using BookGen.Core;
using BookGen.Core.Configuration;
using BookGen.Core.Contracts;
using BookGen.Core.Markdown;
using BookGen.Domain;
using BookGen.Framework;
using BookGen.Utilities;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BookGen.GeneratorSteps
{
    internal class GenerateSearchPage : ITemplatedStep
    {
        public Template Template { get; set; }
        public GeneratorContent Content { get; set; }

        private readonly StringBuilder _buffer;
        private readonly Regex _spaces;

        public GenerateSearchPage()
        {
            _buffer = new StringBuilder();
            _spaces = new Regex(@"\s+", RegexOptions.Compiled);
        }

        public void RunStep(RuntimeSettings settings, ILog log)
        {
            Content.Metadata = FillMeta(settings.Configruation);

            log.Info("Generating search page...");
            GenerateSearchContents(settings, log);
            GenerateSearchForm(settings);

            var output = settings.OutputDirectory.Combine("search.html");
            Content.Title = settings.Configruation.SearchOptions.SearchPageTitle;
            Content.Content = _buffer.ToString();

            var html = Template.ProcessTemplate(Content);
            output.WriteFile(html);
        }

        private string FillMeta(Config configruation)
        {
            var meta = new MetaTag().FillWithConfigDefaults(configruation);
            meta.Title = $"{configruation.Metadata.Title} - {configruation.SearchOptions.SearchPageTitle}";
            meta.Description = configruation.SearchOptions.SearchPageTitle;
            meta.Url = $"{configruation.HostName}search.html";
            return meta.GetHtmlMeta();
        }

        private void GenerateSearchForm(RuntimeSettings settings)
        {
            var options = settings.Configruation.SearchOptions;
            var replacements = new string[]
            {
                options.SearchPageTitle,
                options.SearchTextBoxText,
                options.SearchButtonText,
                options.SearchResults,
                options.NoResults
            };

            var result = Properties.Resources.searchform.ReplaceTags(replacements);
            _buffer.Append(result);
        }

        private string RenderAndCompressForSearch(string filecontent)
        {
            var rendered = MarkdownRenderers.Markdown2Plain(filecontent);
            return _spaces.Replace(rendered, " ");
        }

        private void GenerateSearchContents(RuntimeSettings settings, ILog log)
        {
            _buffer.Append("<div id=\"searchcontents\" style=\"display:none;\">\n");
            foreach (var chapter in settings.TocContents.Chapters)
            {
                foreach (var link in settings.TocContents.GetLinksForChapter(chapter))
                {
                    log.Detail("Processing file for search index: {0}", link.Link);
                    var fileContent = settings.SourceDirectory.Combine(link.Link).ReadFile();
                    var rendered = RenderAndCompressForSearch(fileContent);

                    var file = Path.ChangeExtension(link.Link, ".html");
                    var fullpath = $"{settings.Configruation.HostName}{file}";

                    _buffer.AppendFormat("<div title=\"{0}\" data-link=\"{1}\">", link.DisplayString, fullpath);
                    _buffer.Append(rendered);
                    _buffer.Append("</div>\n");
                }
            }
            _buffer.Append("</div>");
        }
    }
}
