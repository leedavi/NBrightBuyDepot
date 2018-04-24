using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using NBrightCore.common;
using NBrightDNN;
using Nevoweb.DNN.NBrightBuy.Components;

namespace Nevoweb.DNN.NBrightBuy.Providers.NBrightBuyDepot
{
    public class Events : Components.Interfaces.EventInterface
    {
        public override NBrightInfo ValidateCartBefore(NBrightInfo cartInfo)
        {
            return cartInfo;
        }

        public override NBrightInfo ValidateCartAfter(NBrightInfo cartInfo)
        {
            var userid = cartInfo.UserId;
            var uInfo = UserController.Instance.GetUser(PortalSettings.Current.PortalId, userid);
            if (uInfo != null)
            {
                var defaultdepot = GetDefaultDepot();
                AssignDepot(uInfo, cartInfo.PortalId, cartInfo.UserId, defaultdepot);

                var settings = PortalController.Instance.GetCurrentPortalSettings();
                var role = RoleController.Instance.GetRole(settings.PortalId, r => r.RoleName == "hasaccount");
                if (role != null)
                {
                    var c = new ClientData(cartInfo.PortalId, cartInfo.UserId);
                    if (c.Exists)
                    {
                        if (c.DataRecord.GetXmlPropertyBool("genxml/depotassigned"))
                        {
                            if ((!c.DataRecord.GetXmlPropertyBool("genxml/checkbox/hasaccount") || c.DataRecord.GetXmlPropertyRaw("genxml/checkbox/hasaccount") == "") && !UserController.Instance.GetCurrentUserInfo().IsInRole("hasaccount"))
                            {
                                //Assign to user
                                var oDnnRoleController = new RoleController();
                                oDnnRoleController.AddUserRole(cartInfo.PortalId, cartInfo.UserId, role.RoleID, System.DateTime.Now.AddDays(-1), DotNetNuke.Common.Utilities.Null.NullDate);
                                c.DataRecord.SetXmlProperty("genxml/checkbox/hasaccount", "True");
                                c.Save();
                            }
                        }
                        if (!c.DataRecord.GetXmlPropertyBool("genxml/checkbox/hasaccount") && UserController.Instance.GetCurrentUserInfo().IsInRole("hasaccount"))
                        {
                            RoleController.DeleteUserRole(UserController.Instance.GetCurrentUserInfo(), role, settings, false);
                        }
                    }
                }
            }
            return cartInfo;
        }

        public override NBrightInfo ValidateCartItemBefore(NBrightInfo cartItemInfo)
        {
            return cartItemInfo;
        }

        public override NBrightInfo ValidateCartItemAfter(NBrightInfo cartItemInfo)
        {
            return cartItemInfo;
        }

        public override NBrightInfo AfterCartSave(NBrightInfo nbrightInfo)
        {
            return nbrightInfo;
        }

        public override NBrightInfo AfterCategorySave(NBrightInfo nbrightInfo)
        {
            return nbrightInfo;
        }

        public override NBrightInfo AfterProductSave(NBrightInfo nbrightInfo)
        {
            return nbrightInfo;
        }

        public override NBrightInfo AfterSavePurchaseData(NBrightInfo nbrightInfo)
        {
            return nbrightInfo;
        }

        public override NBrightInfo BeforeOrderStatusChange(NBrightInfo nbrightInfo)
        {
            return nbrightInfo;
        }

        public override NBrightInfo AfterOrderStatusChange(NBrightInfo nbrightInfo)
        {
            return nbrightInfo;
        }

        public override NBrightInfo BeforePaymentOK(NBrightInfo nbrightInfo)
        {
            //see if we have an imported client
            var userid = nbrightInfo.UserId;
            var uInfo = UserController.Instance.GetUser(PortalSettings.Current.PortalId, userid);
            if (uInfo != null)
            {
                var defaultdepot = GetDefaultDepot();
                AssignDepot(uInfo, nbrightInfo.PortalId, nbrightInfo.UserId, defaultdepot);
            }
            return nbrightInfo;
        }

        public override NBrightInfo AfterPaymentOK(NBrightInfo nbrightInfo)
        {
            if (nbrightInfo.UserId > 0)
            {
                var c = new ClientData(nbrightInfo.PortalId, nbrightInfo.UserId);
                if (c.Exists)
                {
                    var objCtrl = new NBrightBuyController();
                    var depotnum = c.DataRecord.GetXmlProperty("genxml/dropdownlist/depot");
                    var depot = objCtrl.GetByGuidKey(nbrightInfo.PortalId, -1, "DEPOT", depotnum);
                    if (depot != null)
                    {
                        SendOrderEmail("OrderCreatedClient", nbrightInfo.ItemID, "ordercreatedemailsubject", StoreSettings.Current.AdminEmail, "Depot", depot.GetXmlProperty("genxml/textbox/email"), nbrightInfo);
                    }
                }
            }
            return nbrightInfo;
        }
        public NBrightInfo SendOrderEmail(string emailtype, int orderId, string emailsubjectresxkey, string fromEmail, string emailmsg, string emailList, NBrightInfo objInfo)
        {
            var ordData = new OrderData(orderId);
            var lang = StoreSettings.Current.EditLanguage;
            if (lang == "") lang = Utils.GetCurrentCulture();

            var emailBody = "";
            emailBody = NBrightBuyUtils.RazorTemplRender("OrderHtmlOutput.cshtml", 0, "", ordData, "/DesktopModules/NBright/NBrightBuy", StoreSettings.Current.Get("themefolder"), lang, StoreSettings.Current.Settings());

            NBrightBuyUtils.SendEmail(emailBody, emailList, emailtype, objInfo, emailsubjectresxkey, fromEmail, lang);

            return objInfo;
        }

        public override NBrightInfo BeforePaymentFail(NBrightInfo nbrightInfo)
        {
            return nbrightInfo;
        }


        public override NBrightInfo AfterPaymentFail(NBrightInfo nbrightInfo)
        {
            return nbrightInfo;
        }

        public override NBrightInfo BeforeSendEmail(NBrightInfo nbrightInfo, string emailsubjectrexkey)
        {
            return nbrightInfo;
        }

        public override NBrightInfo AfterSendEmail(NBrightInfo nbrightInfo, string emailsubjectrexkey)
        {
            return nbrightInfo;
        }


        private DefaultDepot GetDefaultDepot()
        {
            var objCtrl = new NBrightBuyController();
            var defaultdepotnum = "";
            var defaultdepotemail = "";
            var l = objCtrl.GetList(PortalSettings.Current.PortalId, -1, "DEPOT", "", " order by [XMLData].value('(genxml/textbox/ref)[1]','nvarchar(50)')", 0, 0, 0, 0, Utils.GetCurrentCulture());
            if (l.Any())
            {

                defaultdepotnum = l.First().GetXmlProperty("genxml/textbox/ref");
                defaultdepotemail = l.First().GetXmlProperty("genxml/textbox/email");
                foreach (var i in l)
                {
                    if (i.GetXmlPropertyBool("genxml/checkbox/default"))
                    {
                        defaultdepotnum = i.GetXmlProperty("genxml/textbox/ref");
                        defaultdepotemail = i.GetXmlProperty("genxml/textbox/email");
                        break;
                    }
                }
            }
            var d = new DefaultDepot();
            d.Ref = defaultdepotnum;
            d.Email = defaultdepotemail;
            return d;
        }

        private void AssignDepot(UserInfo uInfo, int portalId, int userId, DefaultDepot defaultdepot)
        {
            var objCtrl = new NBrightBuyController();

            var c = new ClientData(portalId, userId);
            if (c.Exists)
            {
                if (!c.DataRecord.GetXmlPropertyBool("genxml/depotassigned"))
                {
                    // send email that client needs assignment
                    var emailBody = "<div>" + DnnUtils.GetResourceString("/DesktopModules/NBright/NBrightBuyDepot/App_LocalResources/", "Admin.assignedemail") + " " + uInfo.Email + "</div>";
                    NBrightBuyUtils.SendEmail(emailBody, defaultdepot.Email, "", c.DataRecord, "DEPOT", StoreSettings.Current.AdminEmail, StoreSettings.Current.EditLanguage);
                }

                var depotnum = c.DataRecord.GetXmlProperty("genxml/dropdownlist/depot");
                var nbi = objCtrl.GetByGuidKey(PortalSettings.Current.PortalId, -1, "DEPOTUSER", uInfo.Email);
                if (depotnum == "" && nbi != null)
                {
                    depotnum = nbi.GetXmlProperty("genxml/dropdownlist/depot");
                    var depot = objCtrl.GetByGuidKey(portalId, -1, "DEPOT", depotnum);
                    if (depot != null)
                    {
                        c.DataRecord.SetXmlProperty("genxml/dropdownlist/depot", depotnum);
                        c.DataRecord.SetXmlProperty("genxml/depotassigned", "True");
                        c.Save();
                    }
                    objCtrl.Delete(nbi.ItemID);
                }
            }

        }

    }

    public class DefaultDepot
    {
        public string Ref { get; set; }
        public string Email { get; set; }
    }

}
