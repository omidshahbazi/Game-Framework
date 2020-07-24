// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Extensions;
using System.Drawing;
using System.Text;

namespace GameFramework.Common.Utilities
{
	public static class HTMLGenerator
	{
		public class HTMLStyle
		{
			public Color Color = Color.Black;
			public Font Font;
		}

		public static HTMLStyle Style
		{
			get;
			set;
		}

		public static void BeginHTML(StringBuilder Builder)
		{
			BeginMarkup(Builder, "html");
		}

		public static void EndHTML(StringBuilder Builder)
		{
			Builder.Append("</html>");
		}

		public static void BeginBody(StringBuilder Builder)
		{
			BeginMarkup(Builder, "body");
		}

		public static void EndBody(StringBuilder Builder)
		{
			Builder.Append("</body>");
		}

		public static void WriteContent(StringBuilder Builder, object Content)
		{
			Builder.Append(Content);
		}

		public static void WriteContent(StringBuilder Builder, string Content, params object[] Args)
		{
			Builder.Append(string.Format(Content, Args));
		}

		public static void WriteSpace(StringBuilder Builder)
		{
			Builder.Append("&nbsp;");
		}

		public static void BeginHeader2(StringBuilder Builder)
		{
			BeginMarkup(Builder, "h2");
		}

		public static void EndHeader2(StringBuilder Builder)
		{
			Builder.Append("</h2>");
		}

		public static void BeginTable(StringBuilder Builder)
		{
			BeginMarkup(Builder, "table");
		}

		public static void EndTable(StringBuilder Builder)
		{
			Builder.Append("</table>");
		}

		public static void BeginTableHeader(StringBuilder Builder)
		{
			BeginMarkup(Builder, "thead");
		}

		public static void EndTableHeader(StringBuilder Builder)
		{
			Builder.Append("</thead>");
		}

		public static void BeginTableRow(StringBuilder Builder)
		{
			BeginMarkup(Builder, "tr");
		}

		public static void EndTableRow(StringBuilder Builder)
		{
			Builder.Append("</tr>");
		}

		public static void BeginTableData(StringBuilder Builder)
		{
			BeginMarkup(Builder, "td");
		}

		public static void EndTableData(StringBuilder Builder)
		{
			Builder.Append("</td>");
		}

		private static void BeginMarkup(StringBuilder Builder, string Markup)
		{
			Builder.Append('<');
			Builder.Append(Markup);
			Builder.Append(' ');

			WriterStyle(Builder);

			Builder.Append('>');
		}

		private static void WriterStyle(StringBuilder Builder)
		{
			if (Style == null)
				return;

			Builder.Append("style =\"");

			Builder.Append("color:");
			Builder.Append(Style.Color.ToHex());
			Builder.Append("; ");

			if (Style.Font != null)
			{
				Builder.Append("font-family:");
				Builder.Append(Style.Font.FontFamily.Name);
				Builder.Append("; ");

				Builder.Append("font-size:");
				Builder.Append(Style.Font.Size);
				Builder.Append("px; ");
			}

			Builder.Append('\"');
		}
	}
}