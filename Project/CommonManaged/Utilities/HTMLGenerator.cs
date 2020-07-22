// Copyright 2019. All Rights Reserved.
using GameFramework.Common.Extensions;
using System.Drawing;
using System.Text;

namespace GameFramework.Common.Utilities
{
	public static class HTMLGenerator
	{
		public static void BeginHTML(StringBuilder Builder)
		{
			Builder.Append("<html>");
		}

		public static void EndHTML(StringBuilder Builder)
		{
			Builder.Append("</html>");
		}

		public static void BeginBody(StringBuilder Builder)
		{
			Builder.Append("<body>");
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

		public static void BeginHeader2(StringBuilder Builder, Color Color = new Color())
		{
			Builder.Append("<h2 ");
			Builder.Append("style =\"color:");
			Builder.Append(Color.ToHex());
			Builder.Append("\">");
		}

		public static void EndHeader2(StringBuilder Builder)
		{
			Builder.Append("</h2>");
		}

		public static void BeginTable(StringBuilder Builder)
		{
			Builder.Append("<table>");
		}

		public static void EndTable(StringBuilder Builder)
		{
			Builder.Append("</table>");
		}

		public static void BeginTableHeader(StringBuilder Builder)
		{
			Builder.Append("<thead>");
		}

		public static void EndTableHeader(StringBuilder Builder)
		{
			Builder.Append("</thead>");
		}

		public static void BeginTableRow(StringBuilder Builder)
		{
			Builder.Append("<tr>");
		}

		public static void EndTableRow(StringBuilder Builder)
		{
			Builder.Append("</tr>");
		}

		public static void BeginTableData(StringBuilder Builder)
		{
			Builder.Append("<td>");
		}

		public static void EndTableData(StringBuilder Builder)
		{
			Builder.Append("</td>");
		}
	}
}