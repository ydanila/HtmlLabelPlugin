﻿using System;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

// ReSharper disable once CheckNamespace
namespace HtmlLabel.Forms.Plugin.Abstractions
{
	public class LabelHtml : Label
	{
		/// <summary>
	    /// MaxLines property for the label
	    /// </summary>
	    public static readonly BindableProperty MaxLinesProperty =
		    BindableProperty.CreateAttached("MaxLines", typeof(int), typeof(LabelHtml), default(int));

		/// <summary>
		/// ExtendedHorizontalTextAlignment property for the label
		/// </summary>
		public static readonly BindableProperty ExtendedHorizontalTextAlignmentProperty =
			BindableProperty.CreateAttached("ExtendedHorizontalTextAlignment", typeof(ExtendedHorizontalTextAlignment), typeof(LabelHtml), default(ExtendedHorizontalTextAlignment));

		public static int GetMaxLines(BindableObject view)
	    {
		    if (view == null) return default(int);
		    return (int)view.GetValue(MaxLinesProperty);
	    }

		public static void SetMaxLines(BindableObject view, int value)
		{
			view?.SetValue(MaxLinesProperty, value);
		}

		public static ExtendedHorizontalTextAlignment GetExtendedHorizontalTextAlignment(BindableObject view)
	    {
		    if (view == null) return default(ExtendedHorizontalTextAlignment);
		    return (ExtendedHorizontalTextAlignment)view.GetValue(ExtendedHorizontalTextAlignmentProperty);
	    }

		public static void SetExtendedHorizontalTextAlignment(BindableObject view, ExtendedHorizontalTextAlignment value)
		{
			view?.SetValue(ExtendedHorizontalTextAlignmentProperty, value);
		}

		/// <summary>
		/// Send the Navigating event
		/// </summary>
		/// <param name="args"></param>
		internal void SendNavigating(WebNavigatingEventArgs args)
		{
			Navigating?.Invoke(this, args);
		}

		/// <summary>
		/// Send the Navigated event
		/// </summary>
		/// <param name="args"></param>
		internal void SendNavigated(WebNavigatingEventArgs args)
	    {
		    Navigated?.Invoke(this, args);
	    }

		public ExtendedHorizontalTextAlignment ExtendedHorizontalTextAlignment
		{
			get { return GetExtendedHorizontalTextAlignment(this); }
			set { SetExtendedHorizontalTextAlignment(this, value); }
		}

		/// <summary>
		/// Fires before the open URL request is done.
		/// </summary>
		public event EventHandler<WebNavigatingEventArgs> Navigating;

		/// <summary>
		/// Fires when the open URL request is done.
		/// </summary>
		public event EventHandler<WebNavigatingEventArgs> Navigated;
	}

	internal class LabelRendererHelper
	{
		private readonly Label _label;
		private readonly string _text;
		private readonly StringBuilder _builder;

		public LabelRendererHelper(Label label, string text)
		{
			_label = label;
			_text = text;
			_builder = new StringBuilder();
		}

		private void SetFontAttributes()
		{
			if (_label.FontAttributes == FontAttributes.None) return;
			switch (_label.FontAttributes)
			{
				case FontAttributes.Bold:
					_builder.Append("font-weight: bold; ");
					break;
				case FontAttributes.Italic:
					_builder.Append("font-style: italic; ");
					break;
			}
		}
		private void SetFontFamily()
		{
			if (_label.FontFamily == null) return;
			_builder.Append($"font-family: '{_label.FontFamily}'; ");
		}

		private void SetFontSize()
		{
			if (Math.Abs(_label.FontSize - 14d) < 0.000000001) return;
			_builder.Append($"font-size: {_label.FontSize}px; ");
		}

		private void SetTextColor()
		{
			if (_label.TextColor.IsDefault) return;
			var color = _label.TextColor;
			var red = (int)(color.R * 255);
			var green = (int)(color.G * 255);
			var blue = (int)(color.B * 255);
			var alpha = (int)(color.A * 255);
			var hex = $"#{red:X2}{green:X2}{blue:X2}{alpha:X2}";
			_builder.Append($"color: {hex}; ");
		}

		private void SetHorizontalTextAlign()
		{
			var labelHtml = ((LabelHtml)_label);
			if (labelHtml.ExtendedHorizontalTextAlignment == ExtendedHorizontalTextAlignment.Start) return;
			switch (labelHtml.ExtendedHorizontalTextAlignment)
			{
				case ExtendedHorizontalTextAlignment.Center:
					_builder.Append("text-align: center; ");
					break;
				case ExtendedHorizontalTextAlignment.End:
					_builder.Append("text-align: right; ");
					break;
				case ExtendedHorizontalTextAlignment.Justify:
					_builder.Append("text-align: justify; ");
					break;
			}
		}

		public override string ToString()
		{
			if (string.IsNullOrWhiteSpace(_label.Text))
				return string.Empty;

			_builder.Append("<div style=\"");
			SetFontAttributes();
			SetFontFamily();
			SetFontSize();
			SetTextColor();
			SetHorizontalTextAlign();
			_builder.Append($"\">{_text}</div>");
			var text = _builder.ToString();
			return text;
		}
	}
}