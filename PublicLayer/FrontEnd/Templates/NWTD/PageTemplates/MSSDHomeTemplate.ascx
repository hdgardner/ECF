<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MSSDHomeTemplate.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.NWTD.PageTemplates.MSSDHomeTemplate" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Catalog/SearchByPublisher.ascx" TagName="SearchBar" TagPrefix="NWTD" %>
<%@ Register Src="~/Templates/NWTD/MSSDHomeHeader.ascx" TagName="HomeHeader" TagPrefix="NWTD" %>
<%@ Register Src="~/Templates/NWTD/MSSDFooterModule.ascx" TagName="Footer" TagPrefix="NWTD" %>
<%@ Register Src="~/Structure/Base/Controls/Menu/Breadcrumb.ascx" TagPrefix="cms" TagName="Breadcrumb" %>
<%@ Register Src="~/Structure/Base/Controls/Common/ThemedControlModule.ascx" TagName="ThemedControlModule"
    TagPrefix="cms" %>
<%@ Register src="../../../Structure/User/NWTDControls/Controls/User/Login.ascx" tagname="Login" tagprefix="uc1" %>
<%@ Register assembly="OakTree.Web.UI" namespace="OakTree.Web.UI.WebControls" tagprefix="OakTree" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Cart/NewCart.ascx" TagName="NewCart" TagPrefix="NWTD" %>

<div id="center">
	
	<div id="nwtd-page" class="nwtd-homePage">
		<div id="nwtd-pageHeader">
			<NWTD:HomeHeader runat="server" ID="nwtdheader" />
			<%--<div id="nwtd-submenu" style="clear:both;">
				<div class="nwtd-loginMenu">
					<cms:ThemedControlModule ID="ThemedControlModule1" EnableViewState="false" ThemePath="UserStatusModule.ascx" runat="server" />
				</div>
			</div>--%>
		</div>
		<div id="nwtd-pageContent" class="nwtd-homePageContent">
			<div id="print-logo"><img id="Img1" src="~/Images/print-logo-mssd.png" runat="server" /></div>
			<asp:Image ID="Image1" runat="server" ImageUrl="~/App_Themes/NWTD/images-template/home-page-banner-placeholder.png" Width="975" Height="253" Visible="false" />

		<%-- Old flash slideshow control that Heath replaced 10/10/17 --%>	
        <%--    <OakTree:SWFObject 
				    runat="server" 
				    ID="soHeader" Visible="true"
				    Src="~/Templates/NWTD/Swf/nwtd_flash.swf" SWFHeight="254" SWFWidth="974">
				    <Parameters>
					    <OakTree:SWFParam Name="wmode" Value="opaque" />
				    </Parameters>
				    <FlashVars>
					    <OakTree:SWFFlashVar Name="sitevar" Value="mssd" />
				    </FlashVars>
				    <AlternateContent>
					    Viewing this content requires Adobe Flash Player
				    </AlternateContent>
			    </OakTree:SWFObject>    --%> 
				
		<%-- The following is just a flash example from Oak Tree. It has always been commented out and was NOT part of the original flash slideshow that has been replaced --%>	
	    <%--    <object width="585" height="253" title="sample">
				<param name="movie" value="/Templates/NTWD/Swf/nwtd_flash.swf" />
				<param name="flashvars" value="sitevar=NV" />
				<embed 
					src="/Templates/NTWD/Swf/nwtd_flash.swf" 
					quality="high" pluginspage="http://www.adobe.com/shockwave/download/download.cgi?P1_Prod_Version=ShockwaveFlash" 
					type="application/x-shockwave-flash" 
					width="585" 
					height="253" 
					flashvars="sitevar=NV" >
				</embed>
			</object>   --%>

<%-- NON-FLASH SLIDESHOW BEGIN (HDG 10/10/17) ************************************************************************************** --%>

<%-- On 10/10/17, Heath implemented the following javascript/html slideshow, in combination with additional logic in home_v2.css 
        to replace the outdated flash control above --%>  
    <%-- Flash Replacement Part #1 - slideshow javascript (hdg 10/10/17) --%>  
	        <script type="text/javascript">
	            $(document).ready(function () {
	                $("#slideshow > div:gt(0)").hide();

	                setInterval(function () {
	                    $('#slideshow > div:first')
                    .fadeOut(4000)
                    .next()
                    .fadeIn(4000)
                    .end()
                    .appendTo('#slideshow');
	                }, 10000);
	            });
            </script>    
   
   <%-- Flash Replacement Part #2 - place images (hdg 10/10/17) --%>         
            <div class="main_block">
                <div class="static_pic">
                    <img id="StaticHomePageImage" runat="server" width="409" height="254">
                </div>
                <div class="slide_pics">
                    <div id="slideshow">
                        <div>
                            <img id="SlideshowImage1" runat="server" width="561" height="254">
                        </div>
                        <div>
                            <img id="SlideshowImage2" runat="server" width="561" height="254">
                        </div>
                        <div>
                            <img id="SlideshowImage3" runat="server" width="561" height="254">
                        </div>
                        <div>
                            <img id="SlideshowImage4" runat="server" width="561" height="254">
                        </div>
                        <div>
                            <img id="SlideshowImage5" runat="server" width="561" height="254">
                         </div> 
                    <!-- 
                        <div>
                            <img id="SlideshowImage6" runat="server" width="561" height="254">
                        </div>
                        <div>
                            <img id="SlideshowImage7" runat="server" width="561" height="254">
                        </div>  
                    -->
                    </div>
                </div>
            </div>  

<%-- NON-FLASH SLIDESHOW END (HDG 10/10/17) **************************************************************************************** --%>   

			<!-- <h3 class="home">We are here for you!  Call us Monday through Friday 8:00am – 4:30pm (Mountain time) at 800.995.1444 or 801.773.3200 </h3> -->
			<asp:Panel runat="server" ID="pnlAnonymousMainContent">
				<cms:CmsPlaceHolder runat="server" ID="MainContentArea" />
			</asp:Panel>
			<div class="nwtd-homeBoxes">
					
				<%--Column 1--%>			
				<asp:LoginView runat="server" ID="lvhomeBox1">

					<%--Anonyous--%>
					<AnonymousTemplate>
						<asp:Panel runat="server" ID="pnlLogin" CssClass="homeBox1">
							<uc1:Login ID="Login1" runat="server" />
						</asp:Panel>
					</AnonymousTemplate>

					<%--Level A and B--%>
					<LoggedInTemplate>
						<asp:panel runat="server" ID="pnlSearch"  CssClass="homeBox1">
							<h3>Search our Catalog</h3>
							<NWTD:SearchBar ButtonText="Search" ShowLabel="false" ShowAdvancedSearch="false" runat="server" ID="searchBar" />
							<p class="search-txt"><a href="/Catalog/searchresults.aspx">How to Search Our Catalog</a></p>
						</asp:panel>
					</LoggedInTemplate>
				</asp:LoginView>
									
				<%--Column 2--%>
				<div class="homeBoxCol2">
					
					<%--Anonymous Panel--%>
					<asp:Panel runat="server" ID="pnlAnonymousFeature1" CssClass="homeBox2">
						<cms:CmsPlaceHolder runat="server" ID="AnonymousFeatureContent1" />
						<asp:Literal ID="litAnon1" runat="server" >Anonymous 1</asp:Literal>
					</asp:Panel>

					<%--Level A Panel--%>
					<asp:Panel runat="server" ID="pnlLevelAFeature1" CssClass="homeBox2">
						<cms:CmsPlaceHolder runat="server" ID="FeatureContent1" />
						<NWTD:NewCart runat="server" LinkImageUrl="~/Repository/create-new-cart-btn.png" />
						<p><a href="/Cart/Manage.aspx"><img src="/Repository/manage-carts-btn.png" alt="" border="0" height="21" width="178" /></a></p>
						<p style="font-size: 10px; margin-top: 10px; line-height: 10px;"><strong>Note:</strong> Please contact us if you wish to place an online order. You will need authorization from your school or school district. </p>
						<asp:Literal ID="litLevelA1"  runat="server" >Level A 1</asp:Literal>
					</asp:Panel>

					<%--Level B Panel--%>
					<asp:Panel runat="server" ID="pnlLevelBFeature1" CssClass="homeBox2">
						<cms:CmsPlaceHolder runat="server" ID="LevelBFeatureContent1" />
						<NWTD:NewCart runat="server" LinkImageUrl="~/Repository/create-new-cart-btn.png" />
						<p><a href="/Cart/Manage.aspx"><img src="/Repository/manage-carts-btn.png" alt="" border="0" height="21" width="178" /></a></p>
						<p style="font-size: 10px; margin-top: 10px; line-height: 10px;"><strong>Note:</strong> Please contact us if you wish to place an online order. You will need authorization from your school or school district. </p>
						<asp:Literal ID="litLevelB1"  runat="server"  >Level B 1</asp:Literal>
					</asp:Panel>
				</div>

				<%--Column 3--%>
				<div class="homeBoxCol3">

					<%--Anonymous Panel--%>
					<asp:Panel runat="server" ID="pnlAnonymousFeature2" CssClass="homeBox3">
						<cms:CmsPlaceHolder runat="server" ID="AnonymousFeatureContent2" />
						<asp:Literal ID="litAnon2" runat="server"  >Anonymous 2</asp:Literal>
					</asp:Panel>
					
					<%--Level A Panel--%>
					<asp:Panel runat="server" ID="pnlLevelAFeature2" CssClass="homeBox3">
						<cms:CmsPlaceHolder runat="server" ID="FeatureContent2" />	
						<asp:Literal ID="litLevelA2"  runat="server" >Level A 2</asp:Literal>
					</asp:Panel>
		
					<%--Level B Panel--%>
					<asp:Panel runat="server" ID="pnlLevelBFeature2" CssClass="homeBox3">
						<cms:CmsPlaceHolder runat="server" ID="LevelBFeatureContent2" />	
						<asp:Literal ID="litLevelB2"  runat="server">Level B 2</asp:Literal>
					</asp:Panel>
				</div>
				<br style="clear:both;" />
			<NWTD:Footer runat="server" ID="nwtdfooter" />
		</div>
	</div>
</div>