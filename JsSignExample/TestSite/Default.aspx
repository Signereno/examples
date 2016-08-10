<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Elektronisk signering</h1>
        <p class="lead">Med JavaScript-API fra Signere.no kan du enkelt integrere elektronisk signering på nettsider.</p>
        <button class="btn btn-primary btn-lg" onclick="createSignature()">Signer et testdokument! &raquo;</button>
    </div>
    

    <script src="enter the URL of your unique JS client here, you can get it from support@signere.no"></script>
	
    <script>
        var rootPath = "https://" + window.location.hostname + window.location.pathname.replace("index.html", "");
        var sigObj =
        {
            ExternalDocumentRef: "0123456789",
            DocumentTitle: "JsSign testdokument",
            DocumentDescription: "Test document to sign... Lorem ipsum dolor sit amet, " +
                "consectetur adipiscing elit. Sed id tristique diam. Nunc vel dui porttitor, " +
                "gravida ligula eu, sagittis turpis. Phasellus eu aliquam mi. In convallis " +
                "tellus at nulla sollicitudin euismod. Duis quis nunc dictum erat ornare aliquam. " +
                "Aenean mattis tincidunt dictum. Class aptent taciti sociosqu ad litora torquent " +
                "per conubia nostra, per inceptos himenaeos. Fusce viverra hendrerit dolor et " +
                "faucibus. Mauris iaculis quam justo.", //beskrivelse av dokument
            DocumentUrl: rootPath+"Resources/UnitTestDoc.pdf",
            DocumentItemDescription: "Dokumenttittel",
            Domain: "enter the domain where your application runs",
            ErrorUrl: rootPath+"index.html#error?errorcode=[2]",
            CancelUrl: rootPath+"index.html#cancel",
            SuccessUrl: rootPath+"index.html#success?jssignjwt=[jssignjwt]",
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

  
</asp:Content>
