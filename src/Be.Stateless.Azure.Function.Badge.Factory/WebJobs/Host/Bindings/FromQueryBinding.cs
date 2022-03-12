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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Extensions.Logging;

namespace Be.Stateless.Azure.WebJobs.Host.Bindings;

public class FromQueryBinding<T> : IBinding where T : new()
{
	public FromQueryBinding(ILogger logger)
	{
		_logger = logger;
	}

	#region IBinding Members

	public Task<IValueProvider> BindAsync(BindingContext context)
	{
		var request = context.BindingData.Select(kv => kv.Value).OfType<HttpRequest>().First();
		return Task.FromResult<IValueProvider>(new FromQueryValueProvider<T>(request.Query, _logger));
	}

	public bool FromAttribute => true;

	public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
	{
		return null;
	}

	public ParameterDescriptor ToParameterDescriptor() => new();

	#endregion

	private readonly ILogger _logger;
}
