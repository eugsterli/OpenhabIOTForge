﻿<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="forgeViewerTest._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, minimum-scale=1.0, initial-scale=1, user-scalable=no" />
    <meta charset="utf-8" />
    <link rel="stylesheet" href="https://developer.api.autodesk.com/modelderivative/v2/viewers/style.min.css" type="text/css" />
    <title>Forge Viewer</title>
    <script src="https://developer.api.autodesk.com/modelderivative/v2/viewers/three.min.js"></script>
    <script src="https://developer.api.autodesk.com/modelderivative/v2/viewers/viewer3D.min.js"></script>
    
    <script src="/Scripts/ForgeViewer.js"></script>
    <script src="/Scripts/ForgeApplication.js"></script>
    <script src="/Scripts/SmokeDetectorsExtension.js"></script>

    <style>
        body {
            margin: 0;
        }

        #MyViewerDiv {
            width: 100%;
            height: 100%;
            margin: 0;
            background-color: #F0F8FF;
        }

         .adsk-viewing-viewer notouch{
             height:80%;
             width:60%;
         }
    </style>


    

    <%--Bootstrap--%>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>

</head>
<body>


    <form id="form1" runat="server">
        <div>
            <div id="Header" style="height: 70px; background-color: dimgray; color: white;">
                <div style="display: table-cell; margin-left: 40px;">
                    <h1 style="margin-left: 40px;">Forge Viewer</h1>
                </div>

                <div style="display: table-cell; vertical-align: middle;">
                    <asp:FileUpload ID="FileUpload1" runat="server" Style="margin-left: 40px; float: left;" />
                </div>
                <div style="display: table-cell; vertical-align: middle;">
                    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Upload and translate" Style="margin-left: 40px; color: black;" />
                </div>
            </div>
        </div>
    </form>

     <button id="MyLockButton" onclick='viewer.loadExtension('SmokeDetectorsExtension');' style="z-index:10">Lock it!</button>
    <button id="MyUnlockButton" onclick='viewer.addEventListener(Autodesk.Viewing.OBJECT_TREE_CREATED_EVENT);' style="z-index:10">Unlock it!</button>
    <div id="MyViewerDiv"></div>
   

    <%-- <div id="forgeViewer" />--%>
</body>
</html>
