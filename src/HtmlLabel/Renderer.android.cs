using System.ComponentModel;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Widget;
using HtmlLabel.Forms.Plugin.Abstractions;
using HtmlLabel.Forms.Plugin.Droid;
using Org.Xml.Sax;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Context = Android.Content.Context;

[assembly: ExportRenderer(typeof(LabelHtml), typeof(HtmlLabelRenderer))]
// ReSharper disable once CheckNamespace
namespace HtmlLabel.Forms.Plugin.Droid
{
    /// <summary>
    /// HtmlLable Implementation
    /// </summary>
    [Preserve(AllMembers = true)]
    public class HtmlLabelRenderer : LabelRenderer
    {
	    public HtmlLabelRenderer(Context context) : base(context) { }

		public static void Initialize() { }

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (Control == null) return;

			UpdateText();
			UpdateMaxLines();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == LabelHtml.MaxLinesProperty.PropertyName)
				UpdateMaxLines();
			else if (e.PropertyName == Label.TextProperty.PropertyName ||
					 e.PropertyName == Label.FontAttributesProperty.PropertyName ||
					 e.PropertyName == Label.FontFamilyProperty.PropertyName ||
					 e.PropertyName == Label.FontSizeProperty.PropertyName ||
					 e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName ||
					 e.PropertyName == Label.TextColorProperty.PropertyName ||
					 e.PropertyName == LabelHtml.ExtendedHorizontalTextAlignmentProperty.PropertyName)
				UpdateText();
		}

		private void UpdateMaxLines()
		{
			var maxLines = LabelHtml.GetMaxLines(Element);
			if (maxLines == default(int)) return;
			Control.SetMaxLines(maxLines);
		}

		private void UpdateText()
		{
			if (Control == null || Element == null) return;
			if (string.IsNullOrEmpty(Control.Text)) return;

			var customHtml = new LabelRendererHelper(Element, Control.Text).ToString();
			customHtml = customHtml.Replace("ul>", "ulc>").Replace("ol>", "olc>").Replace("li>", "lic>");

			Control.SetIncludeFontPadding(false);

			SetTextViewHtml(Control, customHtml);
		}

		protected void SetTextViewHtml(TextView text, string html)
		{
			var sequence = Build.VERSION.SdkInt >= BuildVersionCodes.N ?
				Html.FromHtml(html, FromHtmlOptions.ModeCompact, null, new ListTagHandler()) :
#pragma warning disable 618
				Html.FromHtml(html, null, new ListTagHandler());
#pragma warning restore 618

			// Makes clickable links
			SpannableStringBuilder strBuilder = new SpannableStringBuilder(sequence);
			var urls = strBuilder.GetSpans(0, sequence.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
			foreach (var span in urls)
				MakeLinkClickable(strBuilder, (URLSpan)span);

			var value = RemoveLastChar(strBuilder);
			text.SetText(value, TextView.BufferType.Spannable);
			text.MovementMethod = LinkMovementMethod.Instance;
		}

		protected void MakeLinkClickable(SpannableStringBuilder strBuilder, URLSpan span)
		{
			var start = strBuilder.GetSpanStart(span);
			var end = strBuilder.GetSpanEnd(span);
			var flags = strBuilder.GetSpanFlags(span);
			var clickable = new MyClickableSpan((LabelHtml)Element, span);
			strBuilder.SetSpan(clickable, start, end, flags);
			strBuilder.RemoveSpan(span);
		}

		protected ISpanned RemoveLastChar(ISpanned text)
		{
			var builder = new SpannableStringBuilder(text);
			builder.Delete(text.Length() - 1, text.Length());
			return builder;
		}

		private class MyClickableSpan : ClickableSpan
		{
			private readonly LabelHtml _label;
			private readonly URLSpan _span;

			public MyClickableSpan(LabelHtml label, URLSpan span)
			{
				_label = label;
				_span = span;
			}

			public override void OnClick(global::Android.Views.View widget)
			{
				var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = _span.URL }, _span.URL);
				_label.SendNavigating(args);

				if (args.Cancel)
					return;

				Device.OpenUri(new System.Uri(_span.URL));
				_label.SendNavigated(args);
			}
		}
	}

	internal class ListTagHandler : Java.Lang.Object, Html.ITagHandler
	{
		private bool _first = true;
		private string _parent;
		private int _index = 1;

		public void HandleTag(bool opening, string tag, IEditable output, IXMLReader xmlReader)
		{
			if (tag.Equals("ulc"))
			{
				_parent = "ulc";
				_index = 1;
			}
			else if (tag.Equals("olc"))
			{
				_parent = "olc";
				_index = 1;
			}

			if (!tag.Equals("lic")) return;

			var lastChar = (char)0;
			if (output.Length() > 0)
			{
				lastChar = output.CharAt(output.Length() - 1);
			}
			if (_parent.Equals("ulc"))
			{
				if (_first)
				{
					output.Append(lastChar == '\n' ? "\t�  " : "\n\t�  ");
					_first = false;
				}
				else
					_first = true;
			}
			else
			{
				if (_first)
				{
					if (lastChar == '\n')
						output.Append("\t" + _index + ". ");
					else
						output.Append("\n\t" + _index + ". ");
					_first = false;
					_index++;
				}
				else
					_first = true;
			}
		}
	}
}