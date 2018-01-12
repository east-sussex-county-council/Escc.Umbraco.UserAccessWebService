# Umbraco User Access Web Service

Setup a User Type for web authors, if necessary, in Umbraco admin.

Add / alter the following keys in the `appSettings` section:

	<add key="apiuser" value="" />
	<add key="apikey" value="" />
	<add key="defaultUserPermissions" value="ACFKU" />
	<add key="WebAuthorUserType" value="WebAuthor" />
	<add key="HomeContentTypeAlias" value="Home" />
	<add key="AdminAccountName" value="" />
    <add key="AdminAccountEmail" value="" />

`apiuser` and `apikey` should be complex keys that match the values entered in the Umbraco User Access manager `web.config`.

`HomeContentTypeAlias` should match the alias of the page you want to set as the root node for web authors.

Add / alter the following key to enable Http Authentication:

	<system.webServer>
	    <modules>
	        <add name="BasicAuthHttpModule" type="Escc.Umbraco.UserAccessWebService.Services.Authorisation, Escc.Umbraco.UserAccessWebService" />
	    </modules>
	</system.webServer>

