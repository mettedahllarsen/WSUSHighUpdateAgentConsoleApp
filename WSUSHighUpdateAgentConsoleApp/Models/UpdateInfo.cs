using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSUSHighUpdateAgentConsoleApp.Models
{
	public class UpdateInfo
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string SupportUrl { get; set; }
		public string KBArticleId { get; set; }
		public string OsUpgrade { get; set; }
		//public File[] Files { get; set; }
		//public Category[] Categories { get; set; }
	}
}
