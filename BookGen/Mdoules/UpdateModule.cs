﻿//-----------------------------------------------------------------------------
// (c) 2020 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
//-----------------------------------------------------------------------------

using BookGen.Api;
using BookGen.Core;
using BookGen.Domain.ArgumentParsing;

namespace BookGen.Mdoules
{
    internal class UpdateModule : ModuleBase
    {
        public UpdateModule(ProgramState currentState) : base(currentState)
        {
        }

        public override string ModuleCommand => "Update";

        private UpdateParameters GetUpdateParameters(ArgumentParser arguments)
        {
            return new UpdateParameters
            {
                Prerelease = arguments.GetSwitch("p", "prerelease"),
                SearchOnly = arguments.GetSwitch("s", "searchonly")
            };
        }

        public override bool Execute(ArgumentParser tokenizedArguments)
        {
            var parameters = GetUpdateParameters(tokenizedArguments);

            var log = new ConsoleLog(LogLevel.Info);
            var updater = new Updater(log);
            if (parameters.SearchOnly)
            {
                updater.FindNewerRelease(parameters.Prerelease);
            }
            else
            {
                updater.UpdateProgram(parameters.Prerelease, "BookGen.exe").Wait();
            }

            return true;
        }
    }
}
