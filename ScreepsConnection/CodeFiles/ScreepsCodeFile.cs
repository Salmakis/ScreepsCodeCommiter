using System;
using System.Collections.Generic;
using System.Text;

namespace ScreepsConnection
{
	public class ScreepsCodeFile
	{
		public string FileName;
		public string FileContent;

		public ScreepsCodeFile(string fileName, string fileContent)
		{
			this.FileName = fileName;
			this.FileContent = fileContent;
		}
	}
}
