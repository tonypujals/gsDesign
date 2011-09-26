﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Subfuzion.Silverlight.Tcp
{
	// This class does not completely represent all possible policy file options
	// see: http://msdn.microsoft.com/en-us/library/cc645032(v=vs.95).aspx
	public class Policy
	{
		private string _policyFileName;
		private byte[] _policy;
		private List<string> _allowedFromUris;
		private List<SocketResource> _grantedToSocketResources;

		public Policy()
		{
		}

		public Policy(byte[] policy)
		{
			_policy = policy;
		}

		public Policy(string fileName)
		{
			LoadPolicyFile(fileName);
		}

		public Policy(FileStream fileStream)
		{
			LoadPolicyFile(fileStream);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			if (_policyFileName != null)
			{
				sb.Append("\n  file: '").Append(Path.GetFullPath(_policyFileName)).Append("'");
			}

			if (_allowedFromUris != null && _allowedFromUris.Count > 0)
			{
				sb.Append("\n  allow connections from:");

				foreach (string uri in _allowedFromUris)
				{
					sb.Append("\n    - ");
					sb.Append(uri);
				}
			}

			if (_grantedToSocketResources != null && _grantedToSocketResources.Count > 0)
			{
				sb.Append("\n  allow connections to:");

				foreach (var socketResource in _grantedToSocketResources)
				{
					sb.Append("\n    - ")
						.Append("port=").Append(socketResource.Port)
						.Append(", protocol=").Append(socketResource.Protocol);
				}
			}

			return sb.ToString();
		}

		public byte[] Bytes
		{
			get
			{
				return _policy;
			}
			
			set
			{
				_policy = value;

				var stream = new MemoryStream(_policy);
				var xdoc = XElement.Load(stream);

				var uriResults =
					xdoc.Descendants("allow-from").Select(
						allowFromElement => (allowFromElement.Descendants("domain").Select(domain => domain.Attribute("uri"))));

				_allowedFromUris = new List<string>();

				foreach (var uriResult in uriResults)
				{
					foreach (var uriAttribute in uriResult)
					{
						_allowedFromUris.Add(uriAttribute.Value);
					}
				}

				var grantToResults = from grantToElement in xdoc.Descendants("grant-to")
				                    select (from socketResource in grantToElement.Descendants("socket-resource")
				                            let xAttribute = socketResource.Attribute("port")
				                            where xAttribute != null
				                            let attribute = socketResource.Attribute("protocol")
				                            where attribute != null
				                            select new SocketResource
				                                   	{
				                                   		Port = xAttribute.Value,
				                                   		Protocol = attribute.Value
				                                   	});

				_grantedToSocketResources = new List<SocketResource>();
				foreach (var grantToResult in grantToResults)
				{
					foreach (var socketResource in grantToResult)
					{
						_grantedToSocketResources.Add(socketResource);
					}
				}
			}
		}

		public int Length { get { return Bytes != null ? Bytes.Length : 0; } }

		// caller should catch file exceptions
		private void LoadPolicyFile(string policyFileName)
		{
			_policyFileName = policyFileName;
			LoadPolicyFile(new FileStream(policyFileName, FileMode.Open));
		}

		private void LoadPolicyFile(FileStream fileStream)
		{
			var policy = new byte[fileStream.Length];
			fileStream.Read(policy, 0, policy.Length);
			fileStream.Close();

			// successfully loaded new policy, so save information
			Bytes = policy;
		}
	}
}
