﻿using protoextractor.IR;
using System;
using System.Collections.Generic;
using System.IO;

namespace protoextractor.compiler
{
	abstract class DefaultProtoCompiler
	{
		// IR of the input data.
		protected IRProgram _program;
		// Location where the compiled output has to be written.
		protected string _path;

		// The name of the file which will contain the whole dumped IR program.
		protected static string _dumpFileName = "dump.proto";

		// If TRUE, this object will dump the whole program to a single file.
		public bool DumpMode
		{
			get;
			set;
		}

		// If TRUE, a package-style folder structure will be generated.
		// If FALSE, all proto files will be written directly under '_path'.
		public bool PackageStructured
		{
			get;
			set;
		}

		// Process the incoming parameters and return a string that can be used as the value
		// for the specified option type.
		public delegate string OptionValueString(IRNamespace ns, string fileName);

		// Set of options that need to be defined at file level.
		private Dictionary<string, OptionValueString> _fileOptions;

		// Forces accepting an IR program.
		public DefaultProtoCompiler(IRProgram program)
		{
			_program = program;
			DumpMode = false;
			PackageStructured = true;
			_fileOptions = new Dictionary<string, OptionValueString>();
		}

		// The directory where all files will be written to.
		public DefaultProtoCompiler SetOutputPath(string path)
		{
			_path = path;

			return this;
		}

		// Add an option that must be added to the top of each compiled proto file.
		public void SetFileOption(string option, OptionValueString value)
		{
			if (option == null || value == null)
			{
				throw new Exception("Parameters are not allowed to be null!");
			}

			_fileOptions[option] = value;
		}

		// Write out all set options onto the given textstream.
		protected void WriteFileOptions(IRNamespace ns, string fileName, TextWriter w)
		{
			// Loop each option and write to the TextWriter.
			foreach (var kv in _fileOptions)
			{
				var optValue = kv.Value(ns, fileName);
				var option = string.Format("option {0} = \"{1}\";", kv.Key, optValue);

				w.WriteLine(option);
			}
		}

		// Write one or more lines of comments to the given file.
		// Comments can occur almost everywhere and are allowed to be suffix a statement.
		// Each comment is affixed with // (forward slashes) and suffixed with a newline.
		protected void WriteComments(TextWriter w, string prefix = "", params string[] commentList)
		{
			foreach (string comment in commentList)
			{
				w.WriteLine(prefix + "// " + comment);
			}
		}

		// Compiles the IR program to the target format.
		abstract public void Compile();
	}
}
