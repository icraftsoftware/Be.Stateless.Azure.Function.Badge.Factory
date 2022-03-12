﻿#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.Azure.Function;
using Be.Stateless.Azure.WebJobs.Description;
using Be.Stateless.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;

namespace Be.Stateless.Azure.WebJobs.Host.Config;

/// <summary>
/// Custom Binding for Azure Functions.
/// </summary>
/// <seealso href="https://www.red-gate.com/simple-talk/blogs/custom-binding-azure-functions/"/>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class FromQueryBindingExtensionProvider : IExtensionConfigProvider
{
	public FromQueryBindingExtensionProvider(ILogger<Startup> logger)
	{
		_logger = logger;
	}

	#region IExtensionConfigProvider Members

	public void Initialize(ExtensionConfigContext context)
	{
		context.AddBindingRule<FromQueryStringAttribute>().Bind(new FromQueryBindingProvider(_logger));
	}

	#endregion

	private readonly ILogger<Startup> _logger;
}
