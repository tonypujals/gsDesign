﻿using System.ServiceModel;

namespace gsDesign.LauncherGUI.Services
{
	[ServiceContract]
	public interface IRService
	{
		[OperationContract]
		string GetUserDirectory();
		
		/// <summary>
		/// Returns the file path name of the script
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		[OperationContract]
		string SaveScript(string script);
	}
}
