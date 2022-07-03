﻿using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Telegram.Extensions;

public static class EnumerableExtensions
{
	public static IList<IList<TSource>> ChunkBy<TSource>(this IEnumerable<TSource> source, int columnsCount)
	{
		return source.Select((element, index) => new
		{
			Index = index,
			Value = element
		})
		.GroupBy(element => element.Index / columnsCount)
		.Select
		(
			element => element.Select(x => x.Value).ToList() as IList<TSource>
		)
		.ToList();
	}

	public static IReplyMarkup ToReplyKeyboard<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> keySelector, int columnsCount, bool resizeKeyboard = false, bool oneTimeKeyboard = false)
	{
		ArgumentNullException.ThrowIfNull(keySelector);
		
		var keyboardButtons = source.Select
		(
			element => new KeyboardButton
			(
				keySelector(element).ToString()
			)
		)
		.ToList();

		var chunk = keyboardButtons.ChunkBy(columnsCount);
		return new ReplyKeyboardMarkup(chunk)
		{
			ResizeKeyboard = resizeKeyboard,
			OneTimeKeyboard = oneTimeKeyboard
		};
	}

	public static IReplyMarkup ToInlineKeyboard<TSource, TValue>(this IEnumerable<TSource> source, 
		Func<TSource, TValue> keySelector, int columnsCount)
	{
		var keyboardButtons = source.Select
		(
			element => InlineKeyboardButton.WithCallbackData
			(
				keySelector(element).ToString()
			)
		)
		.ToList();

		return new InlineKeyboardMarkup
		(
			keyboardButtons.ChunkBy(columnsCount)
		);
	}
}