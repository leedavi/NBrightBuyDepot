// --- Copyright (c) notice NevoWeb ---
//  Copyright (c) 2014 SARL NevoWeb.  www.nevoweb.com. The MIT License (MIT).
// Author: D.C.Lee
// ------------------------------------------------------------------------
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// ------------------------------------------------------------------------
// This copyright notice may NOT be removed, obscured or modified without written consent from the author.
// --- End copyright notice --- 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using NBrightCore.common;
using NBrightCore.render;
using NBrightDNN;

using Nevoweb.DNN.NBrightBuy.Base;
using Nevoweb.DNN.NBrightBuy.Components;
using DataProvider = DotNetNuke.Data.DataProvider;

namespace Nevoweb.DNN.NBrightBuy.Providers.NBrightBuyDepot
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ViewNBrightGen class displays the content
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Admin : NBrightBuyAdminBase
    {

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);
                if (Page.IsPostBack == false)
                {
                    ImportDepots();
                    // do razor code
                    RazorPageLoad();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                //display the error on the template (don;t want to log it here, prefer to deal with errors directly.)
                var l = new Literal();
                l.Text = exc.ToString();
                phData.Controls.Add(l);
            }
        }

        private void RazorPageLoad()
        {
            var strOut = NBrightBuyUtils.RazorTemplRender("Admin.cshtml", 0, "", null, ControlPath, "config", Utils.GetCurrentCulture(), StoreSettings.Current.Settings());
            var lit = new Literal();
            lit.Text = strOut;
            phData.Controls.Add(lit);
        }

        private void ImportDepots()
        {
            var fullfilename = PortalSettings.Current.HomeDirectoryMapPath + @"\Themes\config\Default\depotimport.csv";
            if (File.Exists(fullfilename))
            {
                var objCtrl = new NBrightBuyController();
                string line;
                // Read the file and display it line by line.
                System.IO.StreamReader file = new System.IO.StreamReader(fullfilename);
                while ((line = file.ReadLine()) != null)
                {
                    var s = line.Split(',');
                    if (s.Count() == 2)
                    {
                        var userInfo = UserController.GetUserByEmail(PortalSettings.Current.PortalId, s[0]);
                        if (userInfo != null)
                        {
                            var nbi = new NBrightInfo();
                            nbi.GUIDKey = userInfo.Email;
                            nbi.UserId = userInfo.UserID;
                            nbi.SetXmlProperty("genxml/depot",s[1]);
                            objCtrl.Update(nbi);

                            var nbi2 = objCtrl.GetByType(PortalId, -1, "CLIENT", userInfo.UserID.ToString());
                            if (nbi2 != null)
                            {
                                nbi2.SetXmlProperty("genxml/dropdownlist/depot",s[1]);
                                objCtrl.Update(nbi2);
                            }
                            else
                            {
                                var c = new ClientData(PortalId,userInfo.UserID);
                                c.DataRecord.SetXmlProperty("genxml/dropdownlist/depot", s[1]);
                                c.Save();
                            }
                        }
                    }
                }
                file.Close();
                System.Threading.Thread.Sleep(1000);
                File.Delete(fullfilename);
            }
        }

        #endregion



        }

    }
