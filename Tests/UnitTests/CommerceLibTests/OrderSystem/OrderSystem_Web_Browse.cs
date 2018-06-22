// Web browse test through order system

namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    using UnitTests.Common;


    public class OrderSystem_Web_Browse : AdminWebTestBase
    {

        public OrderSystem_Web_Browse() : base()
        {
        }

        public override WebTestRequest CreateRequest(int index)
        {
            WebTestRequest request = new WebTestRequest(CommerceManagerUrl + "/Apps/Shell/Pages/ContentFrame.aspx");
            switch (index)
            {
                case 0:
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "OrderSearch-List", false, false);
                    request.QueryStringParameters.Add("class", "PurchaseOrder", false, false);
                    break;
                case 1:
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "Orders-List", false, false);
                    request.QueryStringParameters.Add("filter", "all", false, false);
                    request.QueryStringParameters.Add("class", "PurchaseOrder", false, false);
                    break;
                    /*
                case 2:
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "PurchaseOrder-View", false, false);
                    request.QueryStringParameters.Add("class", "PurchaseOrder", false, false);
                    request.QueryStringParameters.Add("id", "550", false, false);
                    request.QueryStringParameters.Add("customerid", "3b70f937-d8b9-43a8-9b94-884f9f832bfe", false, false);
                    ExtractHiddenFields extractionRule = new ExtractHiddenFields();
                    extractionRule.Required = true;
                    extractionRule.HtmlDecode = true;
                    extractionRule.ContextParameterName = "1";
                    request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule.Extract);
                    break;
                     * */
                    /*
                case 3:
                    request.Method = "POST";
                    request.Headers.Add(new WebTestRequestHeader("x-microsoftajax", "Delta=true"));
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "PurchaseOrder-View", false, false);
                    request.QueryStringParameters.Add("class", "PurchaseOrder", false, false);
                    request.QueryStringParameters.Add("id", "550", false, false);
                    request.QueryStringParameters.Add("customerid", "3b70f937-d8b9-43a8-9b94-884f9f832bfe", false, false);
                    FormPostHttpBody requestBody = new FormPostHttpBody();
                    requestBody.FormPostParameters.Add("ScriptManager1", "ScriptManager1|ctl17$ViewControl$DefaultTabContainer$ctl00$ctl00$LineItemAddresse" +
                            "sDialog$ctl01$DialogTrigger");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl03$ctl00$ShipmentViewDialog$ctl01$Dialog" +
                            "Trigger", this.Context["$HIDDEN1.ctl17$ViewControl$DefaultTabContainer$ctl03$ctl00$ShipmentViewDialog$ctl" +
                            "01$DialogTrigger"].ToString());
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl03$ctl00$ShipmentViewDialog$ctl01$Select" +
                            "edAddressField", this.Context["$HIDDEN1.ctl17$ViewControl$DefaultTabContainer$ctl03$ctl00$ShipmentViewDialog$ctl" +
                            "01$SelectedAddressField"].ToString());
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl03$ctl00$ShipmentViewDialog$ctl01$Shippi" +
                            "ngMethodList", "17995798-a2cc-43ad-81e8-bb932f6827e4");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl03$ctl00$ShipmentViewDialog$ctl01$Method" +
                            "Name", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl03$ctl00$ShipmentViewDialog$ctl01$Tracki" +
                            "ngNumber", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl03$ctl00$ShipmentViewDialog$ctl01$Shipme" +
                            "ntTotal", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl03$ctl00$ShipmentViewDialog$ctl01$Discou" +
                            "ntAmount", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl03$ctl00$ShipmentViewDialog$ctl01$Status" +
                            "", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$DialogT" +
                            "rigger", this.Context["$HIDDEN1.ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl0" +
                            "1$DialogTrigger"].ToString());
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$Name", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$FirstNa" +
                            "me", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$LastNam" +
                            "e", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$Organiz" +
                            "ation", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$Line1", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$Line2", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$City", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$State", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$Country" +
                            "Code", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$Country" +
                            "Name", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$PostalC" +
                            "ode", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$RegionC" +
                            "ode", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$RegionN" +
                            "ame", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$DayTime" +
                            "Phone", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$Evening" +
                            "Phone", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$FaxNumb" +
                            "er", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl02$ctl00$AddressViewDialog$ctl01$Email", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl01$DialogT" +
                            "rigger", this.Context["$HIDDEN1.ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl0" +
                            "1$DialogTrigger"].ToString());
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl01$Selecte" +
                            "dPaymentStatusField", this.Context["$HIDDEN1.ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl0" +
                            "1$SelectedPaymentStatusField"].ToString());
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl01$Payment" +
                            "Type", "59");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl01$Payment" +
                            "MethodList", "7601ad82-52dd-4ce0-b4e6-1ae78d821861");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl01$Name", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl01$Amount", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl01$Payment" +
                            "Status", "Pending");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl01$MetaDat" +
                            "aTab$ctl00$MetaValueCtrl", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl01$MetaDat" +
                            "aTab$ctl01$MetaValueCtrl", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl01$MetaDat" +
                            "aTab$ctl02$MetaValueCtrl", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl01$MetaDat" +
                            "aTab$ctl03$MetaValueCtrl", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl01$ctl00$PaymentViewDialog$ctl01$MetaDat" +
                            "aTab$ctl04$MetaValueCtrl", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl00$ctl00$LineItemAddressesDialog$ctl01$D" +
                            "ialogTrigger", "1553");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl00$ctl00$LineItemAddressesDialog$ctl01$S" +
                            "electedAddressField", this.Context["$HIDDEN1.ctl17$ViewControl$DefaultTabContainer$ctl00$ctl00$LineItemAddressesDialo" +
                            "g$ctl01$SelectedAddressField"].ToString());
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl00$ctl00$LineItemAddressesDialog$ctl01$D" +
                            "isplayName", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl00$ctl00$LineItemAddressesDialog$ctl01$L" +
                            "istPrice", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl00$ctl00$LineItemAddressesDialog$ctl01$D" +
                            "iscountAmount", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl00$ctl00$LineItemAddressesDialog$ctl01$Q" +
                            "uantity", "");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl00$ctl00$LineItemAddressesDialog$ctl01$L" +
                            "ineItemAddressesFilter", "614");
                    requestBody.FormPostParameters.Add("ctl17$ViewControl$DefaultTabContainer$ctl00$ctl00$LineItemAddressesDialog$ctl01$S" +
                            "hippingMethodList", "17995798-a2cc-43ad-81e8-bb932f6827e4");
                    requestBody.FormPostParameters.Add("ctl17_ViewControl_DefaultTabContainer_ClientState", "{\"ActiveTabIndex\":0,\"TabState\":[true,true,true,true]}");
                    requestBody.FormPostParameters.Add("__EVENTTARGET", "ctl17$ViewControl$DefaultTabContainer$ctl00$ctl00$LineItemAddressesDialog$ctl01$D" +
                            "ialogTrigger");
                    requestBody.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
                    requestBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                    requestBody.FormPostParameters.Add("_action", this.Context["$HIDDEN1._action"].ToString());
                    requestBody.FormPostParameters.Add("_params", this.Context["$HIDDEN1._params"].ToString());
                    requestBody.FormPostParameters.Add("ctl17$OrderGroupEdit$OrderStatusList", "NewOrder");
                    requestBody.FormPostParameters.Add("ctl17$OrderGroupEdit$OrderCurrencyList", "USD");
                    requestBody.FormPostParameters.Add("ctl17_OrderGroupEdit_CustomerName_Input", "1 1");
                    requestBody.FormPostParameters.Add("ctl17_OrderGroupEdit_CustomerName_SelectedIndex", this.Context["$HIDDEN1.ctl17_OrderGroupEdit_CustomerName_SelectedIndex"].ToString());
                    requestBody.FormPostParameters.Add("ctl17_OrderGroupEdit_CustomerName_Data", "%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3Etest%20account%3C%" +
                            "2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ed2bc6c6f-ad" +
                            "a4-452c-9f62-a65efdac3b6f%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%" +
                            "3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3Etest%20account%3C%2Fc%3E%3C%2" +
                            "Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ef893cd67-eda3-479b-8e7" +
                            "c-eaea60473ccb%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3" +
                            "Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3Etest%20account%3C%2Fc%3E%3C%2Fr%3E%3C%2F" +
                            "c%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Eb83b6bae-5842-4416-92fa-6d7631f2e" +
                            "41d%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3" +
                            "E%3Cc%3EText%3C%2Fc%3E%3Cc%3Etestme3%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E" +
                            "%3Cc%3EValue%3C%2Fc%3E%3Cc%3E9468b319-9e1d-4b77-a89e-75ee1aa0416f%3C%2Fc%3E%3C%2" +
                            "Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2F" +
                            "c%3E%3Cc%3Esasha1%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc" +
                            "%3E%3Cc%3Eca9ec324-bbb4-4eea-991b-1ee08b0e3350%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%" +
                            "2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3Esasha10%" +
                            "3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E984f16c3" +
                            "-7fd4-478a-b918-d0267e613a4b%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3" +
                            "Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3Esasha11%3C%2Fc%3E%3C%2Fr%3" +
                            "E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E0122ba51-1cb0-4154-80e7-fc" +
                            "109c681689%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3" +
                            "E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3Eadmin%20user%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3" +
                            "Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E00000000-0000-0000-0000-000000000000%3C" +
                            "%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%" +
                            "3EText%3C%2Fc%3E%3Cc%3Eadmin%20user%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%" +
                            "3Cc%3EValue%3C%2Fc%3E%3Cc%3E00000000-0000-0000-0000-000000000000%3C%2Fc%3E%3C%2F" +
                            "r%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc" +
                            "%3E%3Cc%3EAlexandre%20Siniouguine%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3C" +
                            "c%3EValue%3C%2Fc%3E%3Cc%3Ee5ed4dfa-e8ee-4f71-8ab6-e80cd0317fec%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3" +
                            "E%3Cc%3Epunkouter%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc" +
                            "%3E%3Cc%3E0491bd1e-a56c-46d8-8f2a-09a10b60dbf4%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%" +
                            "2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3Etestuser" +
                            "%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Eeeb4591" +
                            "a-2342-4964-8915-a93331199c3b%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%" +
                            "3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EChris%20Lutz%3C%2Fc%3E%3C" +
                            "%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E6bd347f7-4868-4063-9" +
                            "f64-f1111c27ae9b%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E" +
                            "%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3Esasha%20test%3C%2Fc%3E%3C%2Fr%3E%3C%2F" +
                            "c%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ef61899c8-8d4c-4771-b991-9129f650a" +
                            "b90%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3" +
                            "E%3Cc%3EText%3C%2Fc%3E%3Cc%3EBrian%20Brennan%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3" +
                            "E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E66c3b4a2-6370-45f9-99cb-85595c3f0c72%3C%2Fc" +
                            "%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3ETe" +
                            "xt%3C%2Fc%3E%3Cc%3EAnne%20Burgess%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3C" +
                            "c%3EValue%3C%2Fc%3E%3Cc%3E35b1eebc-7fc9-4ac8-8adc-baff1392b42c%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3" +
                            "E%3Cc%3EPeter%20Yeung%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C" +
                            "%2Fc%3E%3Cc%3E18bf7ce3-061c-4f15-b6de-9cfa423bcc2d%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E" +
                            "%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EJose" +
                            "ph%20Grause%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3C" +
                            "c%3E050c66e4-2f81-4e85-a216-56412f949b0e%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E" +
                            "%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EMark%20Hall%3C" +
                            "%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E98807686-9" +
                            "39a-45cd-854c-5e5cd8dc9d8d%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc" +
                            "%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EB2B%20Supplier%20User%20One%" +
                            "3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ecd9cac0d" +
                            "-d25a-4687-86cf-b0b4855bf6ec%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3" +
                            "Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3E1%201%3C%2Fc%3E%3C%2Fr%3E%" +
                            "3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E3b70f937-d8b9-43a8-9b94-884f" +
                            "9f832bfe%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E");
                    requestBody.FormPostParameters.Add("ctl17_OrderGroupEdit_AddressesList_Input", "");
                    requestBody.FormPostParameters.Add("ctl17_OrderGroupEdit_AddressesList_SelectedIndex", this.Context["$HIDDEN1.ctl17_OrderGroupEdit_AddressesList_SelectedIndex"].ToString());
                    requestBody.FormPostParameters.Add("ctl17_OrderGroupEdit_AddressesList_Data", "%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EShipping%20Address" +
                            "%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3EShippin" +
                            "g%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%" +
                            "3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%" +
                            "2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3" +
                            "EText%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr" +
                            "%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%" +
                            "3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3C%2Fr%3E%3" +
                            "C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2F" +
                            "c%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EBilling%20Address%3C" +
                            "%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3EBilling%20" +
                            "Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3" +
                            "Cr%3E%3Cc%3EText%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%" +
                            "3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3ETex" +
                            "t%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%" +
                            "3Cc%3EValue%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2" +
                            "Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3C%2Fr%3E%3C%2F" +
                            "c%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E" +
                            "%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr" +
                            "%3E%3Cc%3EValue%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%" +
                            "3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3" +
                            "C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3" +
                            "C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C" +
                            "%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%" +
                            "3EValue%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3" +
                            "E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EBilling%20Add" +
                            "ress%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3EBil" +
                            "ling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E");
                    requestBody.FormPostParameters.Add("ctl17$OrderGroupEdit$AffiliateList", "");
                    requestBody.FormPostParameters.Add("ctl17$OrderGroupEdit$MetaDataTab$ctl00$MetaValueCtrl", "PO543318");
                    requestBody.FormPostParameters.Add("ctl17$OrderGroupEdit$MetaDataTab$ctl01$DTClientControl1$Date", "8/4/2008");
                    requestBody.FormPostParameters.Add("ctl17$OrderGroupEdit$MetaDataTab$ctl01$DTClientControl1$vceDate1_ClientState", this.Context["$HIDDEN1.ctl17$OrderGroupEdit$MetaDataTab$ctl01$DTClientControl1$vceDate1_ClientS" +
                            "tate"].ToString());
                    requestBody.FormPostParameters.Add("ctl17$OrderGroupEdit$MetaDataTab$ctl01$DTClientControl1$cveDate2_ClientState", this.Context["$HIDDEN1.ctl17$OrderGroupEdit$MetaDataTab$ctl01$DTClientControl1$cveDate2_ClientS" +
                            "tate"].ToString());
                    requestBody.FormPostParameters.Add("ctl17$OrderGroupEdit$MetaDataTab$ctl01$DTClientControl1$cveDate3_ClientState", this.Context["$HIDDEN1.ctl17$OrderGroupEdit$MetaDataTab$ctl01$DTClientControl1$cveDate3_ClientS" +
                            "tate"].ToString());
                    requestBody.FormPostParameters.Add("ctl17$OrderGroupEdit$MetaDataTab$ctl01$NameRequiredE_ClientState", this.Context["$HIDDEN1.ctl17$OrderGroupEdit$MetaDataTab$ctl01$NameRequiredE_ClientState"].ToString());
                    requestBody.FormPostParameters.Add("ctl17$OrderGroupEdit$MetaDataTab$ctl02$MetaValueCtrl", "0");
                    requestBody.FormPostParameters.Add("ctl17_ViewControl_DefaultTabContainer_ctl00_ctl00_LineItemsFilter_Input", "");
                    requestBody.FormPostParameters.Add("ctl17_ViewControl_DefaultTabContainer_ctl00_ctl00_LineItemsFilter_SelectedIndex", this.Context["$HIDDEN1.ctl17_ViewControl_DefaultTabContainer_ctl00_ctl00_LineItemsFilter_Select" +
                            "edIndex"].ToString());
                    requestBody.FormPostParameters.Add("ctl17_ViewControl_DefaultTabContainer_ctl00_ctl00_LineItemsFilter_Data", "%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20CH%20Power" +
                            "Shot%20SD850%20IS%208.0%20MP%20Digital%20Elph%20Camera%20with%204x%20Optical%20I" +
                            "mage%20Stabilized%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue" +
                            "%3C%2Fc%3E%3Cc%3E56a522d9-704d-4516-a09c-c1492a5b5d8a%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc" +
                            "%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EC" +
                            "anon%20PowerShot%20A590IS%208MP%20Digital%20Camera%20with%204x%20Optical%20Image" +
                            "%20Stabilized%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%" +
                            "2Fc%3E%3Cc%3Eee2e4654-dddd-4243-a907-b668ca78576c%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%" +
                            "3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon" +
                            "%20PowerShot%20SD870IS%208MP%20Digital%20Camera%20with%203.8x%20Wide%20Angle%20O" +
                            "ptical%20Image%20Stabilized%20Zoom%20(Silve%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E" +
                            "%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E0e85d363-57b8-4802-bf5b-e9feea69c07a%3C%2Fc%" +
                            "3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3ETex" +
                            "t%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20SD1000%207.1MP%20Digital%20Elph%20Camera%2" +
                            "0with%203x%20Optical%20Zoom%20(Silver)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%" +
                            "3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Efa59f93e-51e7-4709-9b7d-a0899f5e7a44%3C%2Fc%3E%3C" +
                            "%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%" +
                            "2Fc%3E%3Cc%3ECanon%20PowerShot%20SD750%207.1MP%20Digital%20Elph%20Camera%20with%" +
                            "203x%20Optical%20Zoom%20(Silver)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc" +
                            "%3EValue%3C%2Fc%3E%3Cc%3E424d4854-ac9a-4a47-92c1-5b25d5225a32%3C%2Fc%3E%3C%2Fr%3" +
                            "E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E" +
                            "%3Cc%3ECanon%20PowerShot%20SD1100IS%208MP%20Digital%20Camera%20with%203x%20Optic" +
                            "al%20Image%20Stabilized%20Zoom%20(Blue)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr" +
                            "%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E0218d9cd-d040-4117-994e-4a9f9c27a780%3C%2Fc%3E%3" +
                            "C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C" +
                            "%2Fc%3E%3Cc%3ECanon%20EOS%2040D%2010.1MP%20Digital%20SLR%20Camera%20with%20EF%20" +
                            "28-135mm%20f%2F3.5-5.6%20IS%20USM%20Standard%20Zoom%20Lens%3C%2Fc%3E%3C%2Fr%3E%3" +
                            "C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Eca97909f-0120-409d-a21c-76d9d" +
                            "31db68a%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3" +
                            "Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Digital%20Rebel%20XSi%2012MP%20Digital%" +
                            "20SLR%20Camera%20with%20EF-S%2018-55mm%20f%2F3.5-5.6%20IS%20Lens%20(Black)%3C%2F" +
                            "c%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E95754463-87d7" +
                            "-4fdc-b031-fbc5f1defb17%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E" +
                            "%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20Pro%20Serie" +
                            "s%20S5%20IS%208.0MP%20Digital%20Camera%20with%2012x%20Optical%20Image%20Stabiliz" +
                            "ed%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%" +
                            "3E7c81284e-59ab-47ec-a7c9-b9b9af2013df%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3" +
                            "C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerSho" +
                            "t%20G9%2012.1MP%20Digital%20Camera%20with%206x%20Optical%20Image%20Stabilized%20" +
                            "Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ed10" +
                            "3543f-1f4d-400d-a784-08814cd5cf63%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc" +
                            "%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20S" +
                            "D950IS%2012.1MP%20Digital%20Camera%20with%203.7x%20Optical%20Image%20Stabilized%" +
                            "20Zoom%20(Titanium)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2" +
                            "Fc%3E%3Cc%3Ea6846806-bf40-4bd0-91d9-bef367b74b6f%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3" +
                            "C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%" +
                            "20PowerShot%20SD1100IS%208MP%20Digital%20Camera%20with%203x%20Optical%20Image%20" +
                            "Stabilized%20Zoom%20(Silver)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EV" +
                            "alue%3C%2Fc%3E%3Cc%3Eca3df52e-5a76-49b7-8010-553a84801563%3C%2Fc%3E%3C%2Fr%3E%3C" +
                            "%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc" +
                            "%3ECanon%20PowerShot%20SX100IS%208MP%20Digital%20Camera%20with%2010x%20Optical%2" +
                            "0Image%20Stabilized%20Zoom%20(Black)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E" +
                            "%3Cc%3EValue%3C%2Fc%3E%3Cc%3E4e098297-52d8-4b24-b80a-a6bfc1b2646c%3C%2Fc%3E%3C%2" +
                            "Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2F" +
                            "c%3E%3Cc%3ECanon%20PowerShot%20A720IS%208MP%20Digital%20Camera%20with%206x%20Opt" +
                            "ical%20Image%20Stabilized%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc" +
                            "%3EValue%3C%2Fc%3E%3Cc%3E915aba8b-7215-4e8d-9f3e-bd85424832f2%3C%2Fc%3E%3C%2Fr%3" +
                            "E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E" +
                            "%3Cc%3ECanon%20PowerShot%20SD1100IS%208MP%20Digital%20Camera%20with%203x%20Optic" +
                            "al%20Image%20Stabilized%20Zoom%20(Brown)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3C" +
                            "r%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E97f0c42f-5996-4dad-8911-878c2980b992%3C%2Fc%3E%" +
                            "3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3" +
                            "C%2Fc%3E%3Cc%3ECanon%20PowerShot%20SD750%207.1MP%20Digital%20Elph%20Camera%20wit" +
                            "h%203x%20Optical%20Zoom%20(Black)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3C" +
                            "c%3EValue%3C%2Fc%3E%3Cc%3E0e738e10-c384-4aaf-b1b8-9578bfb9fbeb%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3" +
                            "E%3Cc%3ECanon%20PowerShot%20SD870IS%208MP%20Digital%20Camera%20with%203.8x%20Wid" +
                            "e%20Angle%20Optical%20Image%20Stabilized%20Zoom%20(Black%3C%2Fc%3E%3C%2Fr%3E%3C%" +
                            "2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E405394ae-40e1-4e88-852e-c436576" +
                            "49a74%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr" +
                            "%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A650IS%2012.1MP%20Digital%20C" +
                            "amera%20with%206x%20Optical%20Image%20Stabilized%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2" +
                            "Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E177f93d1-ff34-4294-a1e5-c21a3f6b" +
                            "b542%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%" +
                            "3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20SD1100IS%208MP%20Digital%20Cam" +
                            "era%20with%203x%20Optical%20Image%20Stabilized%20Zoom%20(Pink)%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E4ad3635e-1f34-46c4-922e-b" +
                            "1e51c385d81%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%" +
                            "3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A580%208MP%20Digital%20" +
                            "Camera%20with%204x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%" +
                            "3Cc%3EValue%3C%2Fc%3E%3Cc%3E98d71a1d-2bdb-4090-9606-fe7957df3452%3C%2Fc%3E%3C%2F" +
                            "r%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc" +
                            "%3E%3Cc%3ECanon%20PowerShot%20A470%207MP%20Digital%20Camera%20with%203.4x%20Opti" +
                            "cal%20Zoom%20(Gray)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2" +
                            "Fc%3E%3Cc%3Ebe591230-75cd-4914-ac7a-0f0350068f23%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3" +
                            "C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%" +
                            "20PowerShot%20SD1000%207.1MP%20Digital%20Elph%20Camera%20with%203x%20Optical%20Z" +
                            "oom%20(Black)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%" +
                            "3Cc%3Ed20e75b4-8b0c-410e-a9fb-172e9fbd7568%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Digi" +
                            "tal%20Rebel%20XTi%2010.1MP%20Digital%20SLR%20Camera%20with%20EF-S%2018-55mm%20f%" +
                            "2F3.5-5.6%20Lens%20(Black)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EVal" +
                            "ue%3C%2Fc%3E%3Cc%3E9cc5391d-1613-45ab-a332-62ed9306e236%3C%2Fc%3E%3C%2Fr%3E%3C%2" +
                            "Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3" +
                            "ECanon%20PowerShot%20A570IS%207.1MP%20Digital%20Camera%20with%204x%20Optical%20I" +
                            "mage%20Stabilized%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue" +
                            "%3C%2Fc%3E%3Cc%3E2b2d5ae8-cce2-42ec-8d40-49403b44dd3b%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc" +
                            "%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EC" +
                            "anon%20Digital%20Rebel%20XSI%2012MP%20Digital%20SLR%20Camera%20(Black%20Body%20O" +
                            "nly)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ed67" +
                            "61119-2f49-4c16-a503-ecb429e90e01%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc" +
                            "%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20S" +
                            "D1100IS%208MP%20Digital%20Camera%20with%203x%20Optical%20Image%20Stabilized%20Zo" +
                            "om%20(Gold)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3C" +
                            "c%3E350c5766-87eb-43bb-96c9-a1a15e064a70%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E" +
                            "%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerS" +
                            "hot%20A460%205.0MP%20Digital%20Camera%20with%204x%20Optical%20Zoom%20(Silver)%3C" +
                            "%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Efc4a08fc-3" +
                            "f4d-4fcf-a7ac-d3c14ac2c554%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc" +
                            "%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20EOS%205D%2012.8%20MP" +
                            "%20Digital%20SLR%20Camera%20(Body%20Only)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3" +
                            "Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E9eaeb1f6-e426-48e5-ba0f-aa54d893a937%3C%2Fc%3E" +
                            "%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%" +
                            "3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A470%207MP%20Digital%20Camera%20with%203.4x%" +
                            "20Optical%20Zoom%20(Blue)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValu" +
                            "e%3C%2Fc%3E%3Cc%3Edc518243-d9ed-486d-aafa-2f28b11c8163%3C%2Fc%3E%3C%2Fr%3E%3C%2F" +
                            "c%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3E" +
                            "Canon%20Digital%20Rebel%20XT%208MP%20Digital%20SLR%20Camera%20with%20EF-S%2018-5" +
                            "5mm%20f3.5-5.6%20Lens%20(Black)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%" +
                            "3EValue%3C%2Fc%3E%3Cc%3E85e4b46b-939b-4aae-8b55-1d4281c73f27%3C%2Fc%3E%3C%2Fr%3E" +
                            "%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%" +
                            "3Cc%3ECanon%20PowerShot%20SX100IS%208MP%20Digital%20Camera%20with%2010x%20Optica" +
                            "l%20Image%20Stabilized%20Zoom%20(Silver)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3C" +
                            "r%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E75b99498-1db7-412c-9186-311b129a0481%3C%2Fc%3E%" +
                            "3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3" +
                            "C%2Fc%3E%3Cc%3ECanon%20Digital%20Rebel%20XTi%2010.1MP%20Digital%20SLR%20Camera%2" +
                            "0(Black%20Body%20Only)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3" +
                            "C%2Fc%3E%3Cc%3E3c915247-3314-4337-b854-455244358c2a%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3" +
                            "E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECan" +
                            "on%20EOS%205D%2012.8%20MP%20Digital%20SLR%20Camera%20with%20EF%2024-105mm%20f%2F" +
                            "4%20L%20IS%20USM%20Lens%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%" +
                            "3C%2Fc%3E%3Cc%3E9c64daac-187b-4ade-b467-b7ad916bcbd5%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%" +
                            "3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECa" +
                            "non%20Digital%20Rebel%20XTi%2010.1MP%20Digital%20SLR%20Camera%20(Silver%20Body%2" +
                            "0Only)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ec" +
                            "16d62da-1a43-4aa6-8bd9-711559d5ed66%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2" +
                            "Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%2" +
                            "0A470%207MP%20Digital%20Camera%20with%203.4x%20Optical%20Zoom%20(Red)%3C%2Fc%3E%" +
                            "3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E8e8c4ac7-8b75-496a" +
                            "-a271-d5caa9e02abd%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%" +
                            "3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Digital%20Rebel%20XT%208MP%2" +
                            "0Digital%20SLR%20Camera%20(Body%20Only%20-%20Black)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3" +
                            "E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ec9bb365c-9f0d-4856-bb6f-c64c742e114b" +
                            "%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3" +
                            "Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A560%207.1MP%20Digital%20Camera%20" +
                            "with%204x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EVal" +
                            "ue%3C%2Fc%3E%3Cc%3E990f7917-3938-476d-b892-fe8dd36b876c%3C%2Fc%3E%3C%2Fr%3E%3C%2" +
                            "Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3" +
                            "ECanon%20Digital%20Rebel%20XSI%2012MP%20Digital%20SLR%20Camera%20with%20EF-S%201" +
                            "8-55mm%20f%2F3.5-5.6%20IS%20Lens%20(Silver)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E" +
                            "%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E162fb77f-5c69-4eb8-a793-0f2e3a9262a5%3C%2Fc%" +
                            "3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3ETex" +
                            "t%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A470%207.1%20MP%20Digital%20Camera%20with%" +
                            "203.4x%20Optical%20Zoom%20(Orange)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3" +
                            "Cc%3EValue%3C%2Fc%3E%3Cc%3Eb8aea984-d9fb-49d2-bc5f-a8d4935b4cad%3C%2Fc%3E%3C%2Fr" +
                            "%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%" +
                            "3E%3Cc%3ECanon%20Digital%20Rebel%20XTi%2010.1MP%20Digital%20SLR%20Camera%20with%" +
                            "20EF-S%2018-55mm%20f%2F3.5-5.6%20Lens%20(Silver)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3" +
                            "Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Eb1a77f1f-b3b2-4bda-a320-d774c7a902b0%3C" +
                            "%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%" +
                            "3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A630%208MP%20Digital%20Camera%20with%" +
                            "204x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C" +
                            "%2Fc%3E%3Cc%3Ea6c39eb4-54fe-4166-9636-c33d6515fde6%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E" +
                            "%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECano" +
                            "n%20PowerShot%20A540%206MP%20Digital%20Camera%20with%204x%20Optical%20Zoom%3C%2F" +
                            "c%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E27492329-9df5" +
                            "-4653-9504-8211fb787d51%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E" +
                            "%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20EOS%201D%20Mark%20III%2" +
                            "010.1MP%20Digital%20SLR%20Camera%20(Body%20Only)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3" +
                            "Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Eba1d1dd7-0a0f-40be-979c-04957a693d77%3C" +
                            "%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%" +
                            "3EText%3C%2Fc%3E%3Cc%3ECanon%20Digital%20Rebel%20XT%208MP%20Digital%20SLR%20Came" +
                            "ra%20with%20EF-S%2018-55mm%20f3.5-5.6%20Lens%20(Silver)%3C%2Fc%3E%3C%2Fr%3E%3C%2" +
                            "Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E8d640f74-ac6f-447e-811f-fcc62a23" +
                            "eaab%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%" +
                            "3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20SD40%207.1MP%20Digital%20Elph%" +
                            "20Camera%20with%202.4x%20Optical%20Zoom%20(Precious%20Rose)%3C%2Fc%3E%3C%2Fr%3E%" +
                            "3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ea2366b00-2b16-4039-b733-082a" +
                            "859113b8%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%" +
                            "3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20TX1%207.1MP%20Digital%20Ca" +
                            "mera%20with%2010x%20Optical%20Image%20Stabilized%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2" +
                            "Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E0e0b7a4d-ea1f-4037-b787-cf77895c" +
                            "f4ec%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%" +
                            "3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20SD40%207.1MP%20Digital%20Elph%" +
                            "20Camera%20with%202.4x%20Optical%20Zoom%20(Twilight%20Sepia)%3C%2Fc%3E%3C%2Fr%3E" +
                            "%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E4d71302c-df64-43f4-918c-40c" +
                            "8db861d29%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E" +
                            "%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20SD800%20IS%207.1MP%20Digi" +
                            "tal%20Elph%20Camera%20with%203.8x%20Wide%20Angle%20Image-Stabilized%20Optical%20" +
                            "Zoo%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ed385" +
                            "82e8-6032-4161-9be2-2a045c937bb3%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%" +
                            "3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20Pr" +
                            "o%20Series%20S3%20IS%206MP%20with%2012x%20Image%20Stabilized%20Zoom%3C%2Fc%3E%3C" +
                            "%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ed1269447-4e86-4bdd-9" +
                            "d0a-1674bc3f9b6c%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E" +
                            "%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20EOS%201Ds%20Mark%20III%2021.1M" +
                            "P%20Digital%20SLR%20Camera%20(Body%20Only)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%" +
                            "3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ee8631c5b-7e38-480b-9fa7-7b04b0734f21%3C%2Fc%3" +
                            "E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText" +
                            "%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A550%207.1MP%20Digital%20Camera%20with%204x" +
                            "%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc" +
                            "%3E%3Cc%3Ed5bd6ee6-3a87-4967-9d08-abb992047dc0%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%" +
                            "2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20" +
                            "Digital%20Rebel%20XSI%2012MP%20Digital%20SLR%20Camera%20(Silver%20Body%20Only)%3" +
                            "C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E66647f2f-" +
                            "9fcd-41db-a1c7-7e13863df80c%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C" +
                            "c%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20SD600%2" +
                            "06MP%20Digital%20Elph%20Camera%20with%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%" +
                            "3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Eba43cc65-16dd-4ddd-9be6-b651" +
                            "4136f763%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%" +
                            "3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20SD630%206MP%20Digital%20El" +
                            "ph%20Camera%20with%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3C" +
                            "r%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E679a68ab-bd2e-4c0f-b1ab-6046fe577ff9%3C%2Fc%3E%" +
                            "3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3" +
                            "C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A530%205MP%20Digital%20Camera%20with%204x%20O" +
                            "ptical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%" +
                            "3Cc%3Ec82d94df-f609-44eb-ac78-e1928f7eda41%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powe" +
                            "rshot%20A520%204MP%20Digital%20Camera%20with%204x%20Optical%20Zoom%3C%2Fc%3E%3C%" +
                            "2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ed9e38dc4-c0d2-4727-b4" +
                            "6e-763710b049f5%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%" +
                            "3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A95%205MP%20Digital" +
                            "%20Camera%20with%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%" +
                            "3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E1bc9e355-4548-4d64-bd36-6e0d8a5c9eb9%3C%2Fc%3E%3C" +
                            "%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%" +
                            "2Fc%3E%3Cc%3ECanon%20Powershot%20SD450%205MP%20Digital%20Elph%20Camera%20with%20" +
                            "3x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2" +
                            "Fc%3E%3Cc%3E7fcf2569-57ce-4209-9c07-b1fb69934236%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3" +
                            "C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%" +
                            "20Powershot%20SD430%205MP%20Digital%20Camera%20with%203x%20Optical%20Zoom%20(Wi-" +
                            "Fi%20Capable)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%" +
                            "3Cc%3E45c66328-e398-4299-9c8c-055cf1ee11e0%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powe" +
                            "rshot%20S2%20IS%205MP%20Digital%20Camera%20with%2012x%20Optical%20Image%20Stabil" +
                            "ized%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3C" +
                            "c%3E18299df7-474c-4bd5-8bd9-8fe5e59512e2%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E" +
                            "%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerS" +
                            "hot%20A710%20IS%207.1MP%20Digital%20Camera%20with%206x%20Image-Stabilized%20Opti" +
                            "cal%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc" +
                            "%3Ed40f65e7-bc7d-467b-b497-b7c83801451d%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%" +
                            "3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerSh" +
                            "ot%20A640%2010MP%20Digital%20Camera%20with%204x%20Optical%20Zoom%3C%2Fc%3E%3C%2F" +
                            "r%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E350ee06d-2eca-4262-b276" +
                            "-099c238dab38%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3C" +
                            "c%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD500%207.1MP%20Digit" +
                            "al%20Elph%20Camera%20with%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C" +
                            "c%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E76d0b6ab-bc97-4001-95a8-6cd0e963b8a5%3C%" +
                            "2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3" +
                            "EText%3C%2Fc%3E%3Cc%3ECanon%20EOS-10D%206.3MP%20Digital%20SLR%20Camera%20(Body%2" +
                            "0Only)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ef" +
                            "2d2d62b-548e-4a38-ba23-7b44715b8a84%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2" +
                            "Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%2" +
                            "0A620%207.1MP%20Digital%20Camera%20with%204x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3" +
                            "E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E07ce756a-e5e8-4099-99a0-04" +
                            "f4b7362c8a%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3" +
                            "E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD400%205MP%20Digital%20" +
                            "Elph%20Camera%20with%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%" +
                            "3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E83b16bdd-e704-4a55-9936-12f7f360e754%3C%2Fc%3" +
                            "E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText" +
                            "%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A430%204MP%20Digital%20Camera%20with%204x%2" +
                            "0Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3" +
                            "E%3Cc%3E808e6717-915d-47cc-9070-762253dbdbfc%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2F" +
                            "r%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Po" +
                            "werShot%20S410%204MP%20Digital%20Elph%20with%203x%20Optical%20Zoom%3C%2Fc%3E%3C%" +
                            "2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E6c117a9e-0a8f-437a-90" +
                            "aa-848cc415cb33%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%" +
                            "3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A70%203.2MP%20Digit" +
                            "al%20Camera%20w%2F%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3C" +
                            "r%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Eaa36d7b9-c1d0-458f-bb88-24e13ac5a3bb%3C%2Fc%3E%" +
                            "3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3" +
                            "C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD550%207.1MP%20Digital%20Elph%20Camera%20wit" +
                            "h%203x%20Optical%20Zoom%20(Beige)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3C" +
                            "c%3EValue%3C%2Fc%3E%3Cc%3Eb7206adc-21ae-4e51-8482-f5ba6608818a%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3" +
                            "E%3Cc%3ECanon%20PowerShot%20A80%204MP%20Digital%20Camera%20w%2F%203x%20Optical%2" +
                            "0Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E94" +
                            "6d1147-d92e-4379-8d1d-9204fbc53bf6%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2F" +
                            "c%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20" +
                            "SD900%20Titanium%2010MP%20Digital%20Elph%20Camera%20with%203x%20Optical%20Zoom%3" +
                            "C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E11701942-" +
                            "1650-4182-aa5e-ea87f44d3344%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C" +
                            "c%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD30%20" +
                            "5MP%20Digital%20Elph%20Camera%20with%202.4x%20Optical%20Zoom%20(Tuxedo%20Black)%" +
                            "3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ea2d56434" +
                            "-e6f9-4ced-839e-f13d3b14c0e1%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3" +
                            "Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20A410%2" +
                            "03.2MP%20Digital%20Camera%20with%203.2x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%" +
                            "2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ebeef18e9-3c3a-4a4c-be6c-106aa80" +
                            "b3373%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr" +
                            "%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD20%205MP%20Ultra%20Compact%" +
                            "20Digital%20Camera%20(Silver)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E" +
                            "Value%3C%2Fc%3E%3Cc%3E4629708c-a172-46d2-95d3-bf40f65a39f2%3C%2Fc%3E%3C%2Fr%3E%3" +
                            "C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3C" +
                            "c%3ECanon%20PowerShot%20S500%205MP%20Digital%20Elph%20with%203x%20Optical%20Zoom" +
                            "%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ed7d9ac0" +
                            "b-d3a5-4ea0-8351-08fc8b302e88%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%" +
                            "3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20A610%" +
                            "205MP%20Digital%20Camera%20with%204x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc" +
                            "%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ec5bf5374-e270-4b17-b1f9-7797ecb4f8" +
                            "23%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E" +
                            "%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Digital%20Rebel%20XT%208MP%20Digital%20SLR%2" +
                            "0Camera%20(Body%20Only%20-%20Silver)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E" +
                            "%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ec21072cf-9033-4ea9-a610-07bc0239aea7%3C%2Fc%3E%3C%2" +
                            "Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2F" +
                            "c%3E%3Cc%3ECanon%20PowerShot%20A510%203.2MP%20Digital%20Camera%20with%204x%20Opt" +
                            "ical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3C" +
                            "c%3E1c0cf90e-c241-492e-b26e-8505ab1506c6%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E" +
                            "%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerS" +
                            "hot%20A60%202MP%20Digital%20Camera%20with%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr" +
                            "%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E72079f38-9c3b-43b3-8b4d-" +
                            "ff331520f9d6%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc" +
                            "%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20SD110%203MP%20Digital%" +
                            "20Elph%20with%202x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%" +
                            "3Cc%3EValue%3C%2Fc%3E%3Cc%3Ef3998474-1ef4-4b99-8e04-1fcd22b86802%3C%2Fc%3E%3C%2F" +
                            "r%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc" +
                            "%3E%3Cc%3ECanon%20PowerShot%20A700%206MP%20Digital%20Camera%20with%206x%20Optica" +
                            "l%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3" +
                            "E55284ca6-af41-40f1-8607-31eb264e8d5e%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C" +
                            "%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot" +
                            "%20S70%207.1MP%20Digital%20Camera%20with%203.6x%20Optical%20Zoom%3C%2Fc%3E%3C%2F" +
                            "r%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E55eb9953-178b-4f6a-bd58" +
                            "-be65b5642f2c%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3C" +
                            "c%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20Pro%201%208MP%20Digit" +
                            "al%20Camera%20with%207x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3C" +
                            "r%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ec55bb9b6-0efd-4170-8ca3-d317179a314e%3C%2Fc%3E%" +
                            "3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3" +
                            "C%2Fc%3E%3Cc%3ECanon%20PowerShot%20S40%204MP%20Digital%20Camera%20w%2F%203x%20Op" +
                            "tical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3" +
                            "Cc%3E0940ed50-e9be-448c-90d4-fa94d3d63795%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3" +
                            "E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Power" +
                            "Shot%20SD100%203.2MP%20Digital%20ELPH%20Camera%20w%2F%202x%20Optical%20Zoom%3C%2" +
                            "Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E6c76548c-c57" +
                            "7-4a66-a85e-11648ab7328f%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3" +
                            "E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20EOS%2020D%208.2MP%20Di" +
                            "gital%20SLR%20Camera%20(Body%20Only)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E" +
                            "%3Cc%3EValue%3C%2Fc%3E%3Cc%3E0486b509-e78b-41a1-847e-458a68838a14%3C%2Fc%3E%3C%2" +
                            "Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2F" +
                            "c%3E%3Cc%3ECanon%20Powershot%20S80%208MP%20Digital%20Camera%20with%203.6x%20Wide" +
                            "%20Angle%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValu" +
                            "e%3C%2Fc%3E%3Cc%3Eb56e7489-f2b3-48c5-9560-6f8d62dc5b11%3C%2Fc%3E%3C%2Fr%3E%3C%2F" +
                            "c%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3E" +
                            "Canon%20EOS%206.3MP%20Digital%20Rebel%20Camera%20(Body%20Only)%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E703a6746-fd3b-4aa4-9ee5-5" +
                            "40e0de23177%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%" +
                            "3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A400%203.2MP%20Digital%" +
                            "20Camera%20with%202.2x%20Optical%20Zoom%20(Silver)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E" +
                            "%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ecbb86ff8-396f-4856-a9dd-81c7ce73aa8f%" +
                            "3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3C" +
                            "c%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20SD700%20IS%206MP%20Digital%20Elph%2" +
                            "0Camera%20with%204x%20Image%20Stabilized%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C" +
                            "c%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Efa4fc60b-e4a8-494b-a7d1-0c99c7f70ca0%3C%" +
                            "2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3" +
                            "EText%3C%2Fc%3E%3Cc%3ECanon%20EOS%2020D%208.2MP%20Digital%20SLR%20Camera%20with%" +
                            "20EF-S%2018-55mm%20f%2F3.5-5.6%20Lens%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3" +
                            "E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E8992ba11-1d4e-465c-970f-857e750b2751%3C%2Fc%3E%3C%" +
                            "2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2" +
                            "Fc%3E%3Cc%3ECanon%20Powershot%20G1%203MP%20Digital%20Camera%20w%2F%203x%20Optica" +
                            "l%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3" +
                            "Eb5d61f37-1026-4db6-b996-b92855a327b7%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C" +
                            "%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot" +
                            "%20SD200%203.2MP%20Digital%20Elph%20Camera%20with%203x%20Optical%20Zoom%3C%2Fc%3" +
                            "E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ecfb6e9e9-6d97-40" +
                            "15-9717-7fef1fae68ba%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3C" +
                            "r%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A85%204MP%20Di" +
                            "gital%20Camera%20with%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E" +
                            "%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E7ca55696-1246-4529-9863-2bd639dae219%3C%2Fc%" +
                            "3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3ETex" +
                            "t%3C%2Fc%3E%3Cc%3ECanon%20EOS-1D%20Mark%20II%208.2MP%20Digital%20SLR%20Camera%20" +
                            "(Body%20Only)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%" +
                            "3Cc%3Eb49fc13b-da34-4dc3-9ec7-af92b40430ac%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20EOS%" +
                            "2030D%208.2MP%20Digital%20SLR%20Camera%20Kit%20with%20EF-S%2018-55mm%20f%2F3.5-5" +
                            ".6%20Lens%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%" +
                            "3E2e9968b9-c4cf-4bb3-8fd9-1fa359273f7f%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3" +
                            "C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20EOS%206." +
                            "3MP%20Digital%20Rebel%20Camera%20with%2018-55mm%20Lens%3C%2Fc%3E%3C%2Fr%3E%3C%2F" +
                            "c%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E134936bc-e20b-4d86-8816-5e684acf2" +
                            "8e0%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3" +
                            "E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20G5%205MP%20Digital%20Camera%20w" +
                            "%2F%204x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValu" +
                            "e%3C%2Fc%3E%3Cc%3E23849ad3-67fc-4aae-9df3-ceead0c6dd44%3C%2Fc%3E%3C%2Fr%3E%3C%2F" +
                            "c%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3E" +
                            "Canon%20PowerShot%20G6%207.1MP%20Digital%20Camera%20with%204x%20Optical%20Zoom%3" +
                            "C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E56a410e1-" +
                            "b337-4061-9531-f4ef4fa5299e%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C" +
                            "c%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20S30%203" +
                            "MP%20Digital%20Camera%20with%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E" +
                            "%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E77264a47-0fa0-4448-a048-31293e6affe3%" +
                            "3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3C" +
                            "c%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20S400%204MP%20Digital%20Camera%20w%2" +
                            "F%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%" +
                            "3C%2Fc%3E%3Cc%3E2c12fb8d-556b-4869-acb2-40ec2e7c629f%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%" +
                            "3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECa" +
                            "non%20PowerShot%20A75%203.2MP%20Digital%20Camera%20with%203X%20Optical%20Zoom%3C" +
                            "%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E9c9faa40-2" +
                            "0a5-4c38-9b42-c5a7938b5b78%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc" +
                            "%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD30%205" +
                            "MP%20Digital%20Elph%20Camera%20with%202.4x%20Optical%20Zoom%20(Rockstar%20Red)%3" +
                            "C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E2118cddd-" +
                            "8d72-49ca-bddc-b45d31aeb0ec%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C" +
                            "c%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20Pro90%2" +
                            "02.6%20MP%20IS%20Camera%20Kit%20w%2F%2010x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%" +
                            "3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ed007471a-b516-420d-a62c-f526" +
                            "6d52a6e8%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%" +
                            "3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20G7%2010MP%20Digital%20Came" +
                            "ra%20with%206x%20Image-Stabilized%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E" +
                            "%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ebc9066cc-33ab-4528-8750-b0c86dda5c9b%" +
                            "3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3C" +
                            "c%3EText%3C%2Fc%3E%3Cc%3ECanon%20EOS%2030D%208.2MP%20Digital%20SLR%20Camera%20wi" +
                            "th%20EF%2028-135mm%20f%2F3.5-5.6%20IS%20USM%20Standard%20Zoom%20Lens%3C%2Fc%3E%3" +
                            "C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E448a9f01-663b-49b4-" +
                            "bf98-2c1abaade7b5%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3" +
                            "E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Rebel%20XTi%2010.1%20MP%20Dig" +
                            "ital%20SLR%20Camera%20with%20EF-S%2017-85mm%20Zoom%20Lens%3C%2Fc%3E%3C%2Fr%3E%3C" +
                            "%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ee0252c68-aa4c-4948-a5eb-412df6" +
                            "446e8c%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C" +
                            "r%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Digital%20Rebel%20XT%208MP%20Digital%20S" +
                            "LR%20Camera%20with%20%20EF-S%2017-85mm%20f4%2F5.6%20USM%20Image%20Stabilized%20L" +
                            "ens%20(B%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3" +
                            "Ee21e09d6-0050-4307-8af5-1233d5ae6bdd%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C" +
                            "%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot" +
                            "%20A300%203.2MP%20Digital%20Camera%20with%205.1x%20Digital%20Zoom%3C%2Fc%3E%3C%2" +
                            "Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E8d710aef-f6ec-4a3a-af1" +
                            "1-0b795c9a90c7%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3" +
                            "Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD300%204MP%20Digita" +
                            "l%20Elph%20Camera%20with%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc" +
                            "%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E7096595a-336f-4003-bcd3-9f3b44302efb%3C%2" +
                            "Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3E" +
                            "Text%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20S200%202MP%20Digital%20ELPH%20Camera%20" +
                            "w%2F%20%202x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E" +
                            "Value%3C%2Fc%3E%3Cc%3E2afe42ae-b9bc-4244-951b-1086f1ae46ba%3C%2Fc%3E%3C%2Fr%3E%3" +
                            "C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3C" +
                            "c%3ECanon%20PowerShot%20SD40%207.1MP%20Digital%20Elph%20Camera%20with%202.4x%20O" +
                            "ptical%20Zoom%20(Olive%20Gray)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3" +
                            "EValue%3C%2Fc%3E%3Cc%3E2a98f2a2-2d76-46e7-a53d-4454a8f46483%3C%2Fc%3E%3C%2Fr%3E%" +
                            "3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3" +
                            "Cc%3ECanon%20PowerShot%20G3%20%204MP%20Digital%20Camera%20w%2F%204x%20Optical%20" +
                            "Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ede2" +
                            "4c28d-7758-4c19-8e70-c78a7934241f%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc" +
                            "%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A" +
                            "310%203.2MP%20Digital%20Camera%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3" +
                            "EValue%3C%2Fc%3E%3Cc%3Ee8a1d39e-4bb6-4557-8c20-260dc5023ba4%3C%2Fc%3E%3C%2Fr%3E%" +
                            "3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3" +
                            "Cc%3ECanon%20Powershot%20SD20%205MP%20Ultra%20Compact%20Digital%20Camera%20(Garn" +
                            "et)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E5c12" +
                            "2391-88f7-4316-a1bd-db99a0e40830%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%" +
                            "3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20S3" +
                            "30%202MP%20Digital%20ELPH%20Camera%20w%2F%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr" +
                            "%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E39dbf873-e74b-4975-8848-" +
                            "2f6ca6aef7f1%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc" +
                            "%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20EOS-1Ds%2011.1MP%20Digital%20SLR%2" +
                            "0Camera%20(Body%20Only)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%" +
                            "3C%2Fc%3E%3Cc%3Ecb8e0780-189b-4a7b-9e0b-629d664d3ff2%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%" +
                            "3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECa" +
                            "non%20Sure%20Shot%2090u%2035mm%20Date%20Camera%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc" +
                            "%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E6a7b02dd-86b0-4727-9653-3fac267876d5%3C%2" +
                            "Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3E" +
                            "Text%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A400%203.2MP%20Digital%20Camera%20with%" +
                            "202.2x%20Optical%20Zoom%20(Blue)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc" +
                            "%3EValue%3C%2Fc%3E%3Cc%3E81d327d0-5a47-4426-be55-86c4a1245b88%3C%2Fc%3E%3C%2Fr%3" +
                            "E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E" +
                            "%3Cc%3ECanon%20PowerShot%20S1%20IS%203.2%20MP%20Digital%20Camera%20with%2010x%20" +
                            "Image%20Stabilized%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%" +
                            "3Cc%3EValue%3C%2Fc%3E%3Cc%3E2497a3b9-3c6b-4fe0-a1d9-a291ac17ab38%3C%2Fc%3E%3C%2F" +
                            "r%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc" +
                            "%3E%3Cc%3ECanon%20Powershot%20SD10%204MP%20Digital%20Camera%20(Silver)%3C%2Fc%3E" +
                            "%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E9107910e-4233-4e2" +
                            "b-9848-d100f56519c3%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr" +
                            "%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20S410%204MP%20Di" +
                            "gital%20Elph%20with%203x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3" +
                            "Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E25a466b0-9d1e-4b3b-8b61-46a346456185%3C%2Fc%3E" +
                            "%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%" +
                            "3C%2Fc%3E%3Cc%3ECanon%20Powershot%20A410%203.2MP%20Digital%20Camera%20with%203.2" +
                            "x%20Optical%20Zoom%20%26%20Canon%20PIXMA%20iP4200%20Photo%20Printer%3C%2Fc%3E%3C" +
                            "%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Efb66a331-924f-44e0-8" +
                            "c68-5e616e3de14f%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E" +
                            "%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD20%205MP%20Ultra" +
                            "%20Compact%20Digital%20Camera%20(Zen%20Grey)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3" +
                            "E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Edf3be773-08b9-4b25-a533-196f0b9fe2b0%3C%2Fc" +
                            "%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3ETe" +
                            "xt%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20S45%204MP%20Digital%20Camera%20w%2F%203x%" +
                            "20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%" +
                            "3E%3Cc%3E515fd0aa-893b-450f-9473-d9ca374d65df%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2" +
                            "Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20P" +
                            "ro70%201.6MP%20Digital%20Camera%20with%2028-70mm%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2" +
                            "Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E08096839-34ab-4da9-b375-1738ae01" +
                            "dcb8%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%" +
                            "3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A400%203.2MP%20Digital%20Camer" +
                            "a%20with%202.2x%20Optical%20Zoom%20(Orange)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E" +
                            "%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E5d9e44a6-d297-4c8b-8e86-2d043678dbc1%3C%2Fc%" +
                            "3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3ETex" +
                            "t%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20S500%205MP%20Digital%20Elph%20with%203x%20" +
                            "Optical%20Zoom%20(Coach%20Edition)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3" +
                            "Cc%3EValue%3C%2Fc%3E%3Cc%3E57a12aa2-1e69-43b9-b299-a34d87a1a58a%3C%2Fc%3E%3C%2Fr" +
                            "%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%" +
                            "3E%3Cc%3ECanon%20PowerShot%20S20%203.2MP%20Digital%20Camera%20w%2F%202x%20Optica" +
                            "l%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3" +
                            "Edfe37536-0e66-46a8-bb99-f09091e7c65a%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C" +
                            "%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20EOS%2020D" +
                            "%208.2MP%20Digital%20SLR%20Camera%20with%20EF-S%2017-85mm%20f%2F4-5.6%20IS%20USM" +
                            "%20Lens%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E" +
                            "04d7f2b9-c761-4788-ba47-5e7efa0ae134%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%" +
                            "2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20EOS%201Ds%" +
                            "20Mark%20II%2016.7MP%20Digital%20SLR%20Camera%20(Body%20Only)%3C%2Fc%3E%3C%2Fr%3" +
                            "E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E7a76ad52-3dec-481c-9cfa-84" +
                            "ef4d1c2bbd%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3" +
                            "E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD10%204MP%20Digital%20C" +
                            "amera%20(White)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3" +
                            "E%3Cc%3E0f5d59eb-5f9c-4737-80a0-5ea735ed56e5%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2F" +
                            "r%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Po" +
                            "wershot%20SD30%205MP%20Digital%20Elph%20Camera%20with%202.4x%20Optical%20Zoom%20" +
                            "(Glamour%20Gold)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%" +
                            "3E%3Cc%3Eb41b5ac6-d1c4-4868-8034-d823ec16182b%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2" +
                            "Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20P" +
                            "owershot%20S60%205MP%20Digital%20Camera%20with%203.6x%20Optical%20Zoom%3C%2Fc%3E" +
                            "%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E5e6bb423-c4fb-435" +
                            "6-b919-a71d27238544%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr" +
                            "%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD10%204MP%20Di" +
                            "gital%20Camera%20(Black)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue" +
                            "%3C%2Fc%3E%3Cc%3E844ce720-9d7c-4f24-9f91-bf0e561cf5d6%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc" +
                            "%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EC" +
                            "anon%20PowerShot%20A400%203.2MP%20Digital%20Camera%20with%202.2x%20Optical%20Zoo" +
                            "m%20(Green)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3C" +
                            "c%3Ee130fa92-c5d0-421f-b268-54e9e87dd493%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E" +
                            "%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerS" +
                            "hot%20SD110%203MP%20Digital%20Elph%20with%202x%20Optical%20Zoom%20(Coach%20Editi" +
                            "on)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Eacad" +
                            "806c-b9e0-456f-bf5e-db5cf6925602%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%" +
                            "3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20S1" +
                            "0%202MP%20Digital%20Camera%20w%2F%202x%20Optical%20Zoom%3C%2Fc%3E%3C%2Fr%3E%3C%2" +
                            "Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E4c85dece-6454-4818-af4c-d762fffd" +
                            "9897%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%" +
                            "3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20SD500%207.1%20MP%20Digital%20E" +
                            "LPH%20Camera%20with%203x%20Optical%20Zoom%20(Coach%20Edition)%3C%2Fc%3E%3C%2Fr%3" +
                            "E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E72cbcf8b-eef2-49be-9020-e3" +
                            "f22f36cc62%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3" +
                            "E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD20%205MP%20Ultra%20Com" +
                            "pact%20Digital%20Camera%20(Midnight%20Blue)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E" +
                            "%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Eea13917e-5962-4ac2-bba3-25b475554161%3C%2Fc%" +
                            "3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3ETex" +
                            "t%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20A10%201.3MP%20Digital%20Camera%20w%2F%203x" +
                            "%20Optical%20Zoom%20and%20CP-10%20Photo%20Printer%20Value%20Package%3C%2Fc%3E%3C" +
                            "%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E9e28eeec-f4a5-429a-a" +
                            "1bb-4dba0ccea18c%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E" +
                            "%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD30%205MP%20Digit" +
                            "al%20Elph%20Camera%20with%202.4x%20Optical%20Zoom%20(Vivacious%20Violet)%3C%2Fc%" +
                            "3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Eba24fda6-cd64-4" +
                            "34f-81e2-d10be9111a91%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3" +
                            "Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3ECanon%20Powershot%20SD10%204MP%20" +
                            "Digital%20Camera%20(Bronze)%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EVa" +
                            "lue%3C%2Fc%3E%3Cc%3Ebdb8acaf-5609-4681-8c44-681e1df3f553%3C%2Fc%3E%3C%2Fr%3E%3C%" +
                            "2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%" +
                            "3EProduct%201%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%" +
                            "3Cc%3Eproduct1%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3" +
                            "Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EProduct%202%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3" +
                            "E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Eproduct2%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%" +
                            "3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EVa" +
                            "riation%201%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3C" +
                            "c%3Evariation1%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3" +
                            "Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EVariation%202%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc" +
                            "%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Evariation2%3C%2Fc%3E%3C%2Fr%3E%3C%" +
                            "2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%" +
                            "3EPackage%201%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%" +
                            "3Cc%3Epackage1%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3" +
                            "Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EPackage%202%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3" +
                            "E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Epackage2%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%" +
                            "3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EBu" +
                            "ndle1%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ebu" +
                            "ndle1%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr" +
                            "%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EBundle%202%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3" +
                            "Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3Ebundle-2%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EDynamic%20Pa" +
                            "ckage%201%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%" +
                            "3Edynamic1%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3" +
                            "E%3Cr%3E%3Cc%3EText%3C%2Fc%3E%3Cc%3EVariation%201%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%" +
                            "3Cc%3E%3Cr%3E%3Cc%3EValue%3C%2Fc%3E%3Cc%3E9f431b5c-8b53-4e16-8f20-e0e8207c84d8%3" +
                            "C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E");
                    requestBody.FormPostParameters.Add("ctl17_ViewControl_DefaultTabContainer_ctl00_ctl00_LineItemsGrid_EventList", this.Context["$HIDDEN1.ctl17_ViewControl_DefaultTabContainer_ctl00_ctl00_LineItemsGrid_EventLis" +
                            "t"].ToString());
                    requestBody.FormPostParameters.Add("ctl17_ViewControl_DefaultTabContainer_ctl00_ctl00_LineItemsGrid_Data", @"%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E1553%3C%2Fc%3E%3Cc%3E56a522d9-704d-4516-a09c-c1492a5b5d8a%3C%2Fc%3E%3Cc%3ECanon%20CH%20PowerShot%20SD850%20IS%208.0%20MP%20Digital%20Elph%20Camera%20with%204x%20Optical%20Image%20Stabilized%20Zoom%3C%2Fc%3E%3Cc%3E10%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E299.99%3C%2Fc%3E%3Cc%3E299.99%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E2426.8625%3C%2Fc%3E%3Cc%3E2426.86%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E1554%3C%2Fc%3E%3Cc%3E0e85d363-57b8-4802-bf5b-e9feea69c07a%3C%2Fc%3E%3Cc%3ECanon%20PowerShot%20SD870IS%208MP%20Digital%20Camera%20with%203.8x%20Wide%20Angle%20Optical%20Image%20Stabilized%20Zoom%20(Silve%3C%2Fc%3E%3Cc%3E10%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E464.06%3C%2Fc%3E%3Cc%3E464.06%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E4067.5625%3C%2Fc%3E%3Cc%3E4067.56%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E");
                    requestBody.FormPostParameters.Add("ctl17_ViewControl_DefaultTabContainer_ctl01_ctl00_PaymentOptionList_EventList", this.Context["$HIDDEN1.ctl17_ViewControl_DefaultTabContainer_ctl01_ctl00_PaymentOptionList_Even" +
                            "tList"].ToString());
                    requestBody.FormPostParameters.Add("ctl17_ViewControl_DefaultTabContainer_ctl01_ctl00_PaymentOptionList_Data", "%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E539%3C%2Fc%3E%3Cc%3EPay%20By%20Phone" +
                            "%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E5358.35%3C%2Fc%3E%3Cc%3E5358.35%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3Cc%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E");
                    requestBody.FormPostParameters.Add("ctl17_ViewControl_DefaultTabContainer_ctl02_ctl00_AddressList_EventList", this.Context["$HIDDEN1.ctl17_ViewControl_DefaultTabContainer_ctl02_ctl00_AddressList_EventList"].ToString());
                    requestBody.FormPostParameters.Add("ctl17_ViewControl_DefaultTabContainer_ctl02_ctl00_AddressList_Data", @"%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E614%3C%2Fc%3E%3Cc%3EShipping%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E615%3C%2Fc%3E%3Cc%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E616%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E617%3C%2Fc%3E%3Cc%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E618%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E619%3C%2Fc%3E%3Cc%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E620%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E621%3C%2Fc%3E%3Cc%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E622%3C%2Fc%3E%3Cc%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E623%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E624%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E625%3C%2Fc%3E%3Cc%3EBilling%20Address%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E");
                    requestBody.FormPostParameters.Add("ctl17_ViewControl_DefaultTabContainer_ctl03_ctl00_ShipmentList_EventList", this.Context["$HIDDEN1.ctl17_ViewControl_DefaultTabContainer_ctl03_ctl00_ShipmentList_EventList" +
                            ""].ToString());
                    requestBody.FormPostParameters.Add("ctl17_ViewControl_DefaultTabContainer_ctl03_ctl00_ShipmentList_Data", "%3Cr%3E%3Cc%3E%3Cr%3E%3Cc%3E%3C%2Fc%3E%3Cc%3E337%3C%2Fc%3E%3Cc%3EFixed%20Shipping" +
                            "%20Rate%3C%2Fc%3E%3Cc%3E%3Cr%3E%3Cc%3E10%3C%2Fc%3E%3Cc%3E10.00%3C%2Fc%3E%3C%2Fr%" +
                            "3E%3C%2Fc%3E%3Cc%3E%3C%2Fc%3E%3C%2Fr%3E%3C%2Fc%3E%3C%2Fr%3E");
                    request.Body = requestBody; 
                    break;
                     * */
                case 2:
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "Orders-List", false, false);
                    request.QueryStringParameters.Add("filter", "all", false, false);
                    request.QueryStringParameters.Add("class", "PurchaseOrder", false, false);
                    break;
                    /*
                case 5:
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "PurchaseOrder-View", false, false);
                    request.QueryStringParameters.Add("class", "PurchaseOrder", false, false);
                    request.QueryStringParameters.Add("id", "548", false, false);
                    request.QueryStringParameters.Add("customerid", "3b70f937-d8b9-43a8-9b94-884f9f832bfe", false, false); 
                    break;
                     * */
                case 3:
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "Orders-List", false, false);
                    request.QueryStringParameters.Add("filter", "all", false, false);
                    request.QueryStringParameters.Add("class", "PurchaseOrder", false, false);
                    break;
                case 4:
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "Orders-List", false, false);
                    request.QueryStringParameters.Add("filter", "all", false, false);
                    request.QueryStringParameters.Add("class", "ShoppingCart", false, false);
                    break;
                    /*
                case 6:
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "ShoppingCart-View", false, false);
                    request.QueryStringParameters.Add("class", "ShoppingCart", false, false);
                    request.QueryStringParameters.Add("id", "539", false, false);
                    request.QueryStringParameters.Add("customerid", "f70c7453-b7ef-4ea5-8426-5fb01bdf5050", false, false);
                    break;
                     * */
                case 5:
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "Orders-List", false, false);
                    request.QueryStringParameters.Add("filter", "all", false, false);
                    request.QueryStringParameters.Add("class", "ShoppingCart", false, false);
                    break;
                case 6:
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "Orders-List", false, false);
                    request.QueryStringParameters.Add("filter", "all", false, false);
                    request.QueryStringParameters.Add("class", "PaymentPlan", false, false);
                    break;
                    /*
                case 11:
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "PaymentPlan-View", false, false);
                    request.QueryStringParameters.Add("class", "PaymentPlan", false, false);
                    request.QueryStringParameters.Add("id", "212", false, false);
                    request.QueryStringParameters.Add("customerid", "13486a80-e8c4-4d03-96aa-516a49b2c2c5", false, false);
                    break;
                     * */
                case 7:
                    request.QueryStringParameters.Add("_a", "Order", false, false);
                    request.QueryStringParameters.Add("_v", "Orders-List", false, false);
                    request.QueryStringParameters.Add("filter", "thismonth", false, false);
                    request.QueryStringParameters.Add("class", "PaymentPlan", false, false);
                    break;
                default:
                    return null;
            }
            return request;
        }
    }
}
