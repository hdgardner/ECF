using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.ComponentModel;

namespace OakTree.Web.UI.WebControls{

public enum TagType : short {
	JavaScript,
	Link
}

[ToolboxData("<{0}:PathControl runat=\"server\" />"),DefaultProperty("Path")]
public class PathControl : Control, IAttributeAccessor {

	public PathControl() { }

	public string Path {
		get { return ViewState["Path"] as string; }
		set { ViewState["Path"] = value; }
	}

	public TagType TagType {
		get { return (ViewState["TagType"] == null) ? TagType.Link : (TagType)ViewState["TagType"]; }
		set { ViewState["TagType"] = value; }
	}


	Hashtable attribs = new Hashtable();


	protected override void Render(HtmlTextWriter writer) {
		base.Render(writer);

		RenderStartTag(writer);

		foreach (string key in attribs.Keys) {
			writer.WriteAttribute(key, (string)attribs[key]);
		}

		RenderEndTag(writer);
		writer.Write(Environment.NewLine);

	}


	void RenderStartTag(HtmlTextWriter writer) {

		if (TagType == TagType.JavaScript) {
			writer.Write("<script type=\"text/javascript\" src=\"" + ResolveClientUrl(Path) + "\"");
		} else {
			writer.Write("<link href=\"" + ResolveClientUrl(Path) + "\"");
		}
	}

	void RenderEndTag(HtmlTextWriter writer) {

		if (TagType == TagType.JavaScript) {
			writer.Write("></script>");
		} else {
			writer.Write(" />");
		}
	}



	public string GetAttribute(string key) {
		return attribs[key] as string;
	}

	public void SetAttribute(string key, string value) {
		attribs[key] = value;
	}
}}