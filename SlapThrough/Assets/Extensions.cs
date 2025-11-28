using System;
using System.Collections.Generic;
using System.Linq;

public static class Extensions
{
	public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
	{
		Random rnd = new Random();
		return source.OrderBy((item) => rnd.Next());
	}

	private static readonly Random _rng = new Random();

	public static T[] Shuffle<T>(this T[] array)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));

		// Fisher–Yates shuffle
		for (int i = array.Length - 1; i > 0; i--)
		{
			int j = _rng.Next(0, i + 1);

			// swap
			(array[i], array[j]) = (array[j], array[i]);
		}

		return array; // return for fluent usage
	}
}