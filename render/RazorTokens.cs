using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using DotNetNuke.Collections;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;
using NBrightCore.common;
using NBrightCore.providers;
using NBrightCore.render;
using NBrightDNN;
using NBrightDNN.render;
using Nevoweb.DNN.NBrightBuy.Components;
using RazorEngine.Templating;
using RazorEngine.Text;
using NBrightCore.images;
using System.IO;
using DotNetNuke.Entities.Users;
using NBrightBuy.render;
using Nevoweb.DNN.NBrightBuy;
using Nevoweb.DNN.NBrightBuy.Components.Interfaces;

namespace NBrightBuy.NBrightBuyDepot.render
{
    public class RazorTokens<T> : NBrightBuyRazorTokens<T>
    {

        public IEncodedString TestName(NBrightInfo info)
        {
            return new RawString("MY PLUGIN TOKEN");
        }

        public IEncodedString DropDownListDepot(NBrightInfo info, String xpath, String attributes = "")
        {
            var objCtrl = new NBrightBuyController();
            var rtnList = objCtrl.GetList(PortalSettings.Current.PortalId, -1, "DEPOT", ""," order by [XMLData].value('(genxml/textbox/ref)[1]','nvarchar(50)')", 0, 0, 0, 0, Utils.GetCurrentCulture());

            if (attributes.StartsWith("ResourceKey:")) attributes = ResourceKey(attributes.Replace("ResourceKey:", "")).ToString();

            var strOut = "";

            var upd = getUpdateAttr(xpath, attributes);
            var id = getIdFromXpath(xpath);
            strOut = "<select id='" + id + "' " + upd + " " + attributes + ">";
            var s = "";
            strOut += "    <option value=''></option>";
            foreach (var tItem in rtnList)
            {
                if (info.GetXmlProperty(xpath) == tItem.GetXmlProperty("genxml/textbox/ref"))
                    s = "selected";
                else
                    s = "";
                strOut += "    <option value='" + tItem.GetXmlProperty("genxml/textbox/ref") + "' " + s + ">" + tItem.GetXmlProperty("genxml/textbox/name") + "</option>";
            }
            strOut += "</select>";

            return new RawString(strOut);
        }

    }
}
