﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Visitors.TypeScript;
using Reinforced.Typings.Visitors.Typings;

namespace Reinforced.Typings.Tests.Core
{
    public class MockFileOperations : IFilesOperations
    {
        public bool DeployCalled { get; private set; }
        public bool TempRegistryCleared { get; private set; }

        public ExportContext Context { get; set; }

        public Dictionary<string, string> ExportedFiles { get; private set; }

        public MockFileOperations()
        {
            ExportedFiles = new Dictionary<string, string>();
        }

        public void DeployTempFiles()
        {
            DeployCalled = true;
        }

        public void ClearTempRegistry()
        {
            TempRegistryCleared = true;
        }

        public void Export(string fileName, ExportedFile file)
        {
            StringBuilder sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                ExportCore(sw, file);
            }
            ExportedFiles[fileName] = sb.ToString();
        }

        protected virtual void ExportCore(TextWriter tw, ExportedFile file)
        {
            var visitor = Context.Global.ExportPureTypings ? new TypingsExportVisitor(tw, Context.Global.TabSymbol) : new TypeScriptExportVisitor(tw, Context.Global.TabSymbol);
            WriteWarning(tw);
            foreach (var fileGlobalReference in file.GlobalReferences.References)
            {
                visitor.Visit(fileGlobalReference);
            }

            foreach (var referencesReference in file.References.References)
            {
                visitor.Visit(referencesReference);
            }

            foreach (var globalReferencesImport in file.GlobalReferences.Imports)
            {
                visitor.Visit(globalReferencesImport);
            }

            foreach (var referencesImport in file.References.Imports)
            {
                visitor.Visit(referencesImport);
            }

            foreach (var fileNamespace in file.Namespaces)
            {
                visitor.Visit(fileNamespace);
            }
        }

        private void WriteWarning(TextWriter tw)
        {
            if (Context.Global.WriteWarningComment)
            {
                tw.WriteLine("//     This code was generated by a Reinforced.Typings tool. ");
                tw.WriteLine("//     Changes to this file may cause incorrect behavior and will be lost if");
                tw.WriteLine("//     the code is regenerated.");
                tw.WriteLine();
            }
        }
    }
}
