﻿using Oc.Em;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TestUtility
{
    /// <summary>
    /// クラスを列挙してクラス図用出力生成
    /// </summary>
    internal abstract class CreateClassDiagramCommand : Command
    {
        protected CreateClassDiagramCommand(Type baseType) : base($"{baseType.Name}列挙") {
            this.BaseType = baseType;
        }

        private Type BaseType { get; }

        public override void Execute() {
            var baseType = this.BaseType;

            var a = Program.AssemblyCSharp;

            List<Type> types = new List<Type>();
            //色々検索
            //  例外が発生する場合、関連ファイルの読み込みができていないため、
            //  Craftopia\Craftopia_Data\Managed のファイルを実行ディレクトリにコピーする必要があります。
            foreach (var t in a.GetTypes()) {
                //WriteLine($"\t{t.Name}");
                if (t.BaseType == null) { continue; }
                //UnityEngine.Debug.Log($"\t\tBaseType:{t.BaseType.Name}");

                //BaseType派生取得
                //  指定の基本クラスとして扱えるかで判断します
                if (!baseType.IsAssignableFrom(t)) {
                    continue;
                }
                types.Add(t);
            }
            if (types.Any()) {
                WriteLine("");
                WriteLine($"{baseType.Name}派生");
                WriteLine("");

                OutputTable(types.ToArray());

                WriteLine("");

                OutputClassDiagram(types.ToArray());

                WriteLine("");
            }
        }

        /// <summary>
        /// 表としての出力
        /// </summary>
        /// <param name="types"></param>
        protected void OutputTable(Type[] types) {
            WriteLine($"| クラス     | 基本クラス   |       |  ");
            WriteLine($"|------------|--------------|-------|  ");

            foreach (var t in types) {
                string opt = t.IsAbstract ? "abstract" : "";
                WriteLine($"| {t.Name}  | {((t.BaseType != this.BaseType) ? t.BaseType?.Name : "")}    | {opt}  |  ");
            }
        }

        /// <summary>
        /// クラス図としての出力
        /// </summary>
        /// <param name="types"></param>
        /// <remarks>
        /// Visual Studio Code で Markdown Preview Enhanced を使用しています。
        /// それ以外でも mermaid.js を使用可能な環境であれば表示可能と思われます。
        /// </remarks>
        protected void OutputClassDiagram(Type[] types) {
            WriteLine($"```mermaid");
            WriteLine($"classDiagram");

            //基本クラス
            WriteClassDiagram(this.BaseType);

            foreach (var t in types) {
                WriteClassDiagram(t);
                //TODO インターフェース？
            }
            WriteLine($"```");
        }
        private void WriteClassDiagram(Type t) {
            WriteLine("");
            if(t.BaseType != this.BaseType && t != this.BaseType) {
                //継承関係
                WriteLine($"\t{t.BaseType.Name} <|-- {t.Name}");
            }
            WriteLine($"\tclass {t.Name}" + "{");
            WriteLine("\t}");
            WriteLine("");
        }
    }
}