﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reinforced.Typings.Ast;

namespace Reinforced.Typings.Visitors.TypeScript
{
    partial class TypeScriptExportVisitor
    {
        public override void Visit(RtClass node)
        {
            if (node == null) return;
            Visit(node.Documentation);
            var prev = Context;
            Context = WriterContext.Class;
            AppendTabs();
            Decorators(node);
            if (prev == WriterContext.Module) Write("export ");
            Write("class ");
            Visit(node.Name);
            if (node.Extendee != null)
            {
                Write(" extends ");
                Visit(node.Extendee);
            }
            if (node.Implementees.Count > 0)
            {
                Write(" implements ");
                SequentialVisit(node.Implementees, ", ");
            }
            Br(); AppendTabs();
            Write("{"); Br();
            Tab();
            var members = node.Members.OrderBy(c => c is RtConstructor ? 0 : 1);
            foreach (var rtMember in members)
            {
                Visit(rtMember);
            }
            UnTab();
            AppendTabs(); WriteLine("}");
            Context = prev;
        }

    }
}
