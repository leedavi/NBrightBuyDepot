using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
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
    }
}
