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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Be.Stateless.Extensions;
using Be.Stateless.IO;
using Microsoft.AspNetCore.Http;

namespace Be.Stateless.Azure.AspNetCore.Http.Extensions;

public static class QueryCollectionExtensions
{
	public static Stream AsJsonStream(this IQueryCollection queryCollection)
	{
		if (queryCollection == null) throw new ArgumentNullException(nameof(queryCollection));
		return new EnumerableStream(queryCollection.GetJsonEnumerator());
	}

	private static IEnumerable<byte[]> GetJsonEnumerator(this IQueryCollection queryCollection)
	{
		yield return _leftCurlyBrace;
		using var enumerator = queryCollection.GetEnumerator();
		var hasNext = enumerator.MoveNext();
		while (hasNext)
		{
			yield return _quote;
			yield return Encoding.UTF8.GetBytes(enumerator.Current.Key);
			yield return _quote;
			yield return _colon;
			var value = enumerator.Current.Value.ToString();
			if (value.IsNullOrEmpty())
			{
				yield return _null;
			}
			else
			{
				yield return _quote;
				yield return Encoding.UTF8.GetBytes(value);
				yield return _quote;
			}
			hasNext = enumerator.MoveNext();
			if (hasNext) yield return _comma;
		}
		yield return _rightCurlyBrace;
	}

	private static readonly byte[] _colon = Encoding.UTF8.GetBytes(":");
	private static readonly byte[] _comma = Encoding.UTF8.GetBytes(",");
	private static readonly byte[] _leftCurlyBrace = Encoding.UTF8.GetBytes("{");
	private static readonly byte[] _null = Encoding.UTF8.GetBytes("null");
	private static readonly byte[] _quote = Encoding.UTF8.GetBytes("\"");
	private static readonly byte[] _rightCurlyBrace = Encoding.UTF8.GetBytes("}");
}
