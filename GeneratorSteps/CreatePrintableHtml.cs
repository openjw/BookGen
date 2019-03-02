﻿//-----------------------------------------------------------------------------
// (c) 2019 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
//-----------------------------------------------------------------------------

using BookGen.Domain;
using BookGen.Utilities;
using System;
using System.Text;

namespace BookGen.GeneratorSteps
{
    public class CreatePrintableHtml : IGeneratorStep
    {
        private StringBuilder _content;

        public CreatePrintableHtml()
        {
            _content = new StringBuilder();
        }

        public void RunStep(GeneratorSettings settings)
        {
            Console.WriteLine("Generating Printable html...");

            var output = settings.OutputDirectory.Combine("print.html");

            CreateHeader();

            foreach (var file in settings.TocFiles)
            {
                var input = settings.SourceDirectory.Combine(file);

                var inputContent = input.ReadFile();

                var md = MarkdownUtils.Markdown2HTML(inputContent);

                _content.AppendLine(md);
                _content.AppendLine("<!-- Pagebreak -->");
            }

            CreateFooter();

            output.WriteFile(_content.ToString());
        }

        private void CreateFooter()
        {
            _content.Append("</body></html>");
        }

        private void CreateHeader()
        {
            _content.AppendLine("<!DOCTYPE html>");
            _content.AppendLine("<html lang=\"en\">");
            _content.AppendLine("<head></head><body>");
        }
    }
}