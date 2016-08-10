<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Elektronisk signering</h1>
        <p class="lead">Med JavaScript-API fra Signere.no kan du enkelt integrere elektronisk signering på nettsider.</p>
        <button class="btn btn-primary btn-lg" onclick="createSignature()">Signer et testdokument! &raquo;</button>
    </div>
    

    <script src="https://signerejs.azureedge.net/e103992bb614413680f2a65e00756068.js"></script>
	
    <script>
        var rootPath = "https://" + window.location.hostname + window.location.pathname.replace("index.html", "");
        var sigObj =
        {
            ExternalDocumentRef: "0123456789", //ekstern referanse
            DocumentTitle: "JsSign testdokument", //tittel for signering
            DocumentDescription: "Test document to sign... Lorem ipsum dolor sit amet, " +
                "consectetur adipiscing elit. Sed id tristique diam. Nunc vel dui porttitor, " +
                "gravida ligula eu, sagittis turpis. Phasellus eu aliquam mi. In convallis " +
                "tellus at nulla sollicitudin euismod. Duis quis nunc dictum erat ornare aliquam. " +
                "Aenean mattis tincidunt dictum. Class aptent taciti sociosqu ad litora torquent " +
                "per conubia nostra, per inceptos himenaeos. Fusce viverra hendrerit dolor et " +
                "faucibus. Mauris iaculis quam justo.", //beskrivelse av dokument
            DocumentUrl: rootPath+"Resources/UnitTestDoc.pdf", //url hvor dokument kan lastes ned
            DocumentItemDescription: "Dokumenttittel", //tittel på dokument
            Domain: "signerejsexampleapp.azurewebsites.net",
            ErrorUrl: rootPath+"index.html#error?errorcode=[2]", //url man blir redirect-et til ved feil 
            CancelUrl: rootPath+"index.html#cancel", //url man blir redirect-et til hvis bruker avbryter
            SuccessUrl: rootPath+"index.html#success?jssignjwt=[jssignjwt]", //url man blir redirect-et til ved suksess
            SigneeRefs: []
        };
		
		function createSignature(){
		    try{
			    signere.CreateSignature(sigObj);
		    }
		    catch (e) {
		        console.log(e);
		    }
		}

		if(window.location.hash.indexOf('jssignjwt')!==-1 || window.location.search.indexOf('jssignjwt')!==-1 && signere){
			document.getElementById('result').style.display = 'block'; 
			signere.ParseSignatureResult(function(result){
			document.getElementById('result').innerHTML=JSON.stringify(result, null, 2);
			});
			
		}
    </script>

    <%--<div class="row">
        <div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301948">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Get more libraries</h2>
            <p>
                NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Web Hosting</h2>
            <p>
                You can easily find a web hosting company that offers the right mix of features and price for your applications.
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301950">Learn more &raquo;</a>
            </p>
        </div>
    </div>--%>
</asp:Content>
