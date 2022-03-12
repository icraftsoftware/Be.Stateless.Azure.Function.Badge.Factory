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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Be.Stateless.Azure.WebJobs.Host.Bindings;

public class FromQueryValueProvider<T> : IValueProvider where T : new()
{
	public FromQueryValueProvider(IQueryCollection query, ILogger logger)
	{
		_query = query;
		_logger = logger;
	}

	#region IValueProvider Members

	public Task<object> GetValueAsync()
	{
		try
		{
			var properties = _query.ToDictionary(kv => kv.Key, kv => kv.Value.Single());
			var json = JsonConvert.SerializeObject(properties, Formatting.None);
			var result = JsonConvert.DeserializeObject<T>(json);
			return Task.FromResult<object>(result);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Could not deserialize{typeof(T)} from request Query.");
			throw;
		}
	}

	public Type Type => typeof(object);

	public string ToInvokeString() => string.Empty;

	#endregion

	private readonly ILogger _logger;
	private readonly IQueryCollection _query;
}
