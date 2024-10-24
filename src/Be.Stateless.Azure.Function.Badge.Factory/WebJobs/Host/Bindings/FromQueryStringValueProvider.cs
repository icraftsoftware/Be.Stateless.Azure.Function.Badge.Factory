#region Copyright & License

// Copyright © 2012 - 2024 François Chabot
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
using System.Text.Json;
using System.Threading.Tasks;
using Be.Stateless.Azure.AspNetCore.Http.Extensions;
using Be.Stateless.IO.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Logging;

namespace Be.Stateless.Azure.WebJobs.Host.Bindings;

public class FromQueryStringValueProvider<T> : IValueProvider where T : new()
{
	public FromQueryStringValueProvider(IQueryCollection query, ILogger logger)
	{
		_query = query;
		_logger = logger;
	}

	#region IValueProvider Members

	public Task<object> GetValueAsync()
	{
		try
		{
			if (_logger.IsEnabled(LogLevel.Debug))
				_logger.LogDebug(
					"Deserializing type {type} from query string's json equivalent {json}.",
					typeof(T).Name,
					ToInvokeString());
			var result = JsonSerializer.Deserialize<T>(_query.AsJsonStream(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
			return Task.FromResult<object>(result);
		}
		catch (Exception exception)
		{
			_logger.LogCritical(exception, "Could not deserialize type {type} from query string.", typeof(T).Name);
			throw;
		}
	}

	public Type Type => typeof(T);

	public string ToInvokeString()
	{
		return _query.AsJsonStream().ReadToEnd();
	}

	#endregion

	private readonly ILogger _logger;
	private readonly IQueryCollection _query;
}
