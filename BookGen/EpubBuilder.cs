﻿//-----------------------------------------------------------------------------
// (c) 2019 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
//-----------------------------------------------------------------------------

using Bookgen.Template;
using BookGen.Core;
using BookGen.Core.Configuration;
using BookGen.Core.Contracts;
using BookGen.Framework;

namespace BookGen
{
    internal class EpubBuilder : Builder
    {
        public EpubBuilder(string workdir, Config configuration, ILog log) : base(workdir, configuration, log, configuration.TargetEpub)
        {
            var session = new GeneratorSteps.Epub.EpubSession();

            AddStep(new GeneratorSteps.CreateOutputDirectory());
            AddStep(new GeneratorSteps.CopyAssets(configuration.TargetEpub));
            AddStep(new GeneratorSteps.CopyImagesDirectory(true, true));
            AddStep(new GeneratorSteps.Epub.CreateEpubStructure());
            AddStep(new GeneratorSteps.Epub.CreateEpubPages(session));
            AddStep(new GeneratorSteps.Epub.CreateEpubToc());
            AddStep(new GeneratorSteps.Epub.CreatePackageOpf(session));
            AddStep(new GeneratorSteps.Epub.CreateEpubPack());
        }

        protected override FsPath ConfigureOutputDirectory(FsPath workingDirectory)
        {
            return workingDirectory.Combine(Settings.Configuration.TargetEpub.OutPutDirectory);
        }

        protected override string ConfigureTemplate()
        {
            return BuiltInTemplates.Epub;
        }
    }
}
