﻿//-----------------------------------------------------------------------------
// (c) 2019 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
//-----------------------------------------------------------------------------

using BookGen.Domain;
using BookGen.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BookGen
{
    internal class WebsiteBuilder
    {
        private FsPath _outdir;
        private FsPath _indir;
        private FsPath _imgdir;
        private FsPath _toc;
        private Config _currentConfig;
        private List<string> _files;

        private static void CopyImages(FsPath outdir, FsPath imgdir)
        {
            Console.WriteLine("Copy images to output...");
            var targetdir = outdir.Combine(imgdir.GetName());
            imgdir.CopyDirectory(targetdir);
            targetdir.ProtectDirectory();
        }

        private void CopyAssets(FsPath assets, FsPath outdir)
        {
            if (assets.IsExisting)
            {
                Console.WriteLine("Copy template assets to output...");
                var targetdir = outdir.Combine(assets.GetName());
                assets.CopyDirectory(targetdir);
                targetdir.ProtectDirectory();
            }
        }

        private static void CreateOutputDirectory(FsPath outdir)
        {
            Console.WriteLine("Creating output directory...");
            outdir.CreateDir();
        }

        public WebsiteBuilder(Config currentConfig)
        {
            MarkdownModifier.Styles = currentConfig.StyleClasses;

            _currentConfig = currentConfig;
            _outdir = currentConfig.OutputDir.ToPath();
            _indir = new FsPath(Environment.CurrentDirectory);
            _imgdir = currentConfig.ImageDir.ToPath();
            _toc = currentConfig.TOCFile.ToPath();
            _files = MarkdownUtils.GetFilesToProcess(_toc.ReadFile());
        }

        private void GenerateIndex(GeneratorContent content, Template template)
        {
            Console.WriteLine("Generating Index file...");
            var input = _indir.Combine(_currentConfig.Index);
            var output = _outdir.Combine("index.html");

            content.Content = MarkdownUtils.Markdown2HTML(input.ReadFile());
            var html = template.ProcessTemplate(content);
            output.WriteFile(html);

        }

        private void GenerateTOCcontent(GeneratorContent content)
        {
            var tocContent = MarkdownUtils.Markdown2HTML(_toc.ReadFile());
            foreach (var file in _files)
            {
                tocContent = tocContent.Replace(file, _currentConfig.HostName + Path.ChangeExtension(file, ".html"));
            }
            content.TableOfContents = tocContent;
        }

        private void GeneratePagesJs(List<string> files)
        {
            Console.WriteLine("Generating pages.js...");
            List<string> pages = new List<string>(files.Count);
            foreach (var file in files)
            {
                pages.Add(_currentConfig.HostName + Path.ChangeExtension(file, ".html"));
            }
            FsPath target = _outdir.Combine("pages.js");
            target.WriteFile("var pages=" + JsonConvert.SerializeObject(pages) + ";");
        }

        private void GenerateSubPageIndexes(GeneratorContent content, Template template)
        {
            Console.WriteLine("Generating index files for sub content folders...");
            foreach (var file in _files)
            {
                var dir = Path.GetDirectoryName(file);
                var output = _outdir.Combine(dir).Combine("index.html");
                if (!output.IsExisting)
                {
                    content.Title = dir;
                    content.Content = "";
                    var html = template.ProcessTemplate(content);
                    output.WriteFile(html);
                }
            }
        }

        private void GenerateSubPages(GeneratorContent content, Template template)
        {
            Console.WriteLine("Generating Sub Markdown Files...");
            foreach (var file in _files)
            {
                var input = _indir.Combine(file);
                var output = _outdir.Combine(Path.ChangeExtension(file, ".html"));

                var inputContent = input.ReadFile();

                content.Title = MarkdownUtils.GetTitle(inputContent);
                content.Content = MarkdownUtils.Markdown2HTML(inputContent);
                var html = template.ProcessTemplate(content);
                output.WriteFile(html);
            }
        }

        public void Run()
        {
            CreateOutputDirectory(_outdir);
            CopyAssets(_indir.Combine(_currentConfig.AssetsDir), _outdir);
            CopyImages(_outdir, _imgdir);

            var content = new GeneratorContent(_currentConfig);
            GenerateTOCcontent(content);

            Template template = new Template(_currentConfig.Template.ToPath());
            GenerateIndex(content, template);
            GeneratePagesJs(_files);
            GenerateSubPages(content, template);
            GenerateSubPageIndexes(content, template);
        }
    }
}