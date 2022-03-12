#region Copyright & License

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

using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Logging;

namespace Be.Stateless.Azure.WebJobs.Host.Bindings;

public class FromQueryBindingProvider : IBindingProvider
{
	public FromQueryBindingProvider(ILogger logger)
	{
		_logger = logger;
	}

	#region IBindingProvider Members

	public Task<IBinding> TryCreateAsync(BindingProviderContext context)
	{
		var binding = CreateBinding(_logger, context.Parameter.ParameterType);
		return Task.FromResult(binding);
	}

	#endregion

	private IBinding CreateBinding(ILogger log, Type T)
	{
		var type = typeof(FromQueryBinding<>).MakeGenericType(T);
		var context = Activator.CreateInstance(type, log);
		return (IBinding) context;
	}

	private readonly ILogger _logger;
}
