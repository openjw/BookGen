﻿//-----------------------------------------------------------------------------
// (c) 2019 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
//-----------------------------------------------------------------------------

using BookGen.Domain;
using BookGen.Utilities;
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
            imgdir.CopyDirectory(outdir.Combine(imgdir.GetName()));
        }

        private void CopyAssets(FsPath assets, FsPath outdir)
        {
            if (assets.IsExisting)
            {
                Console.WriteLine("Copy template assets to output...");
                assets.CopyDirectory(outdir.Combine(assets.GetName()));
            }
        }

        private static void CreateOutputDirectory(FsPath outdir)
        {
            Console.WriteLine("Creating output directory...");
            outdir.CreateDir();
        }

        private void GenerateTOCcontent(Dictionary<string, string> content)
        {
            var tocContent = MarkdownUtils.Markdown2HTML(_toc.ReadFile());
            foreach (var file in _files)
            {
                tocContent = tocContent.Replace(file, _currentConfig.HostName + Path.ChangeExtension(file, ".html"));
            }
            content["toc"] = tocContent;
        }

        public WebsiteBuilder(Config currentConfig)
        {
            _outdir = currentConfig.OutputDir.ToPath();
            _indir = new FsPath(Environment.CurrentDirectory);
            _imgdir = currentConfig.ImageDir.ToPath();
            _toc = currentConfig.TOCFile.ToPath();
            _files = MarkdownUtils.GetFilesToProcess(_toc.ReadFile());
        }

        public void Run()
        {
            CreateOutputDirectory(_outdir);
            CopyAssets(_indir.Combine(_currentConfig.AssetsDir), _outdir);
            CopyImages(_outdir, _imgdir);

            var content = new Dictionary<string, string>();
            content.Add("toc", "");
            content.Add("content", "");
            GenerateTOCcontent(content);

            Template template = new Template(_currentConfig.Template.ToPath());
            GenerateIndex();
            

            Console.WriteLine("Generating Sub Markdown Files...");

            foreach (var file in _files)
            {
                var input = _indir.Combine(file);
                var output = _outdir.Combine(Path.ChangeExtension(file, ".html"));

                content["content"] = MarkdownUtils.Markdown2HTML(input.ReadFile());
                var html = template.ProcessTemplate(content);
                output.WriteFile(html);
            }
        }

        private void GenerateIndex()
        {
            Console.WriteLine("Generating Index file...");
            var input = _indir.Combine(_currentConfig.Index);
            var output = _outdir.Combine("index.html");
        }
    }
}
